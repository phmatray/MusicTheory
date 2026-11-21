---
title: "Tutorial: ear-training drills"
description: Run seeded ear-training drills, check answers, reproduce a session from a seed and score partial answers.
section: Tutorials
order: 4
---

The `MusicTheory.Training` namespace turns the theory library into quiz questions. Every drill has the same
shape: build it with options and a seed, call `Next()` for a question, then `Check(question, answer)` for a
score. This tutorial builds a short session with the interval drill, then shows the other four drills.
Each block runs as written, and none of them prints a random question: they print relations and scores.

## A first question

A drill takes options and a seed. `IntervalDrillOptions.For` picks the interval pool for a difficulty, and
the seed fixes every random choice.

```csharp
using MusicTheory.Training;

var drill = new IntervalDrill(IntervalDrillOptions.For(Difficulty.Beginner), seed: 42);
var question = drill.Next();

Console.WriteLine(question.Prompt);                                  // Name the interval
Console.WriteLine(question.Choices.Count);                           // 5
Console.WriteLine(question.Choices.Contains(question.Answer));       // True
Console.WriteLine(question.Sounds.Count is 1 or 2);                  // True
```

`Prompt` is the text to show. `Sounds` is what to play: a list of steps, each step a list of notes sounded
together, so a melodic interval is two steps of one note and a harmonic one is a single step of two notes.
`Choices` is the answer pool, and `Answer` is the expected value. Playing `Sounds` is up to your app.

## Checking an answer

`Check` returns an `AnswerResult` with `IsCorrect`, a `Score` between 0 and 1 and the `Expected` answer.
The interval drill scores 1 for a right answer and 0 for a wrong one. It compares by ear: the answer is
reduced to its simple form and matched on semitones, so an augmented fourth and a diminished fifth are the same answer.

```csharp
var right = drill.Check(question, question.Answer);
var wrongChoice = question.Choices.First(c => c.Semitones != question.Answer.Semitones);
var wrong = drill.Check(question, wrongChoice);

Console.WriteLine($"{right.IsCorrect} {right.Score}");               // True 1
Console.WriteLine($"{wrong.IsCorrect} {wrong.Score}");               // False 0
Console.WriteLine(wrong.Expected == question.Answer);                // True
```

## Reproducible sessions

Two drills built with the same seed ask the same questions in the same order, and a different seed asks
different ones. The random source is `SeededRandom`, a small SplitMix64 generator, so a session is the same
on every machine and every run. That makes a failing exercise easy to replay and a lesson easy to share.

```csharp
string Signature(Question<Interval> q) =>
    q.Answer.Name + ":" + string.Join("|", q.Sounds.Select(step => string.Join(",", step.Select(n => n.MidiNumber))));

List<string> Session(ulong seed, int count)
{
    var d = new IntervalDrill(IntervalDrillOptions.For(Difficulty.Intermediate), seed);
    return Enumerable.Range(0, count).Select(_ => Signature(d.Next())).ToList();
}

Console.WriteLine(Session(7, 5).SequenceEqual(Session(7, 5)));       // True
Console.WriteLine(Session(7, 5).SequenceEqual(Session(8, 5)));       // False
```

`SeededRandom` is public, so you can use it for your own choices, such as picking the next drill type.

```csharp
var a = new SeededRandom(99);
var b = new SeededRandom(99);
Console.WriteLine(a.NextUInt64() == b.NextUInt64());                 // True
Console.WriteLine(a.Next(12) is >= 0 and < 12);                      // True
```

## Other drills

The chord, scale, progression and melody drills follow the same `Next()` and `Check` shape. The first two
return a `Question<ChordType>` and a `Question<ScaleType>` with a pool of choices and an all-or-nothing score.

```csharp
var chords = new ChordDrill(ChordDrillOptions.For(Difficulty.Beginner), seed: 1);
var chordQuestion = chords.Next();
Console.WriteLine(chordQuestion.Choices.Count);                      // 2
Console.WriteLine(chords.Check(chordQuestion, chordQuestion.Answer).Score);   // 1

var scales = new ScaleDrill(ScaleDrillOptions.For(Difficulty.Beginner), seed: 1);
var scaleQuestion = scales.Next();
Console.WriteLine(scales.Check(scaleQuestion, scaleQuestion.Answer).Score);   // 1
```

The progression drill asks for the scale degrees of a short chord progression, and the melody drill asks for
the notes of a melody. Their questions have no choices (`Choices` is empty) and carry the `Key`. Their score
is partial: the fraction of positions that are right.

```csharp
var progressions = new ProgressionDrill(ProgressionDrillOptions.For(Difficulty.Beginner), seed: 3);
var p = progressions.Next();
Console.WriteLine(p.Answer.Count);                                   // 4
Console.WriteLine(p.Key is not null);                                // True

var guess = p.Answer.ToList();
guess[3] = guess[3] == 1 ? 5 : 1;                                    // one position wrong
var partial = progressions.Check(p, guess);
Console.WriteLine(partial.IsCorrect);                                // False
Console.WriteLine(partial.Score is > 0 and < 1);                     // True
Console.WriteLine(partial.Score);                                    // 0.75

var melodies = new MelodyDrill(MelodyDrillOptions.For(Difficulty.Beginner), seed: 3);
var m = melodies.Next();
var played = m.Answer.ToList();
played[1] = played[1].Transpose(Interval.Parse("m2"));               // one note off
var melodyResult = melodies.Check(m, played);
Console.WriteLine(melodyResult.Score);                               // 0.75
Console.WriteLine(melodies.Check(m, m.Answer).IsCorrect);            // True
```

## Difficulty

`Difficulty` has three levels, `Beginner`, `Intermediate` and `Advanced`. Every options type has a
`For(Difficulty)` factory that widens its pool: the interval drill grows from five intervals to twelve, adds descending questions at Intermediate and
harmonic ones at Advanced, and the melody drill grows from 4 to 8 notes.

```csharp
string Row(Difficulty level)
{
    var o = IntervalDrillOptions.For(level);
    return $"{level} {o.Pool.Count} {o.Descending} {o.Harmonic} {MelodyDrillOptions.For(level).Length}";
}

Console.WriteLine(Row(Difficulty.Beginner));       // Beginner 5 False False 4
Console.WriteLine(Row(Difficulty.Intermediate));   // Intermediate 9 True False 6
Console.WriteLine(Row(Difficulty.Advanced));       // Advanced 12 True True 8
```

The options are plain records, so you can also build your own pool, for example thirds only.

```csharp
var thirds = new IntervalDrillOptions([Interval.Parse("m3"), Interval.Parse("M3")], Descending: false, Harmonic: false);
var thirdsDrill = new IntervalDrill(thirds, seed: 5);
Console.WriteLine(thirdsDrill.Next().Choices.Count);                 // 2
```
