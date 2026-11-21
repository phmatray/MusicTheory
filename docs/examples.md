---
title: Examples
description: Short programs for common tasks, from a chord chart to an interval quiz.
section: Help
order: 1
---

## A chord chart for every key

```csharp
foreach (var tonic in new[] { "C", "G", "D", "F", "Bb" })
{
    var chords = new ChordProgression(KeySignature.Parse(tonic)).GetDiatonicChords();
    Console.WriteLine($"{tonic,-3} {string.Join("  ", chords)}");
}
```

## Naming the notes of a chord symbol

```csharp
string Spell(string symbol) => string.Join(" ", ChordSymbol.Parse(symbol).ToChord().GetNotes().Select(n => n.GetName()));

Console.WriteLine(Spell("F#m7b5"));    // F# A C E
Console.WriteLine(Spell("Abmaj9"));    // Ab C Eb G Bb
Console.WriteLine(Spell("E7#9"));      // E G# B D F##
```

## Spelling notes from a MIDI keyboard in a key

```csharp
string Spell(int[] midi, string key) =>
    string.Join(" ", midi.Select(m => Note.FromMidiNumber(m).TransposeBySemitones(0, KeySignature.Parse(key)).GetName()));

Console.WriteLine(Spell(new[] { 61, 65, 68 }, "Db"));   // Db F Ab
Console.WriteLine(Spell(new[] { 61, 65, 68 }, "C#"));   // C# E# G#
```

## An interval quiz

A reproducible quiz: the same seed always asks the same questions.

```csharp
var random = new Random(7);
var names = new[] { "m2", "M2", "m3", "M3", "P4", "A4", "P5", "m6", "M6", "m7", "M7" };
for (int i = 0; i < 3; i++)
{
    var root = Note.FromMidiNumber(random.Next(55, 72));
    var interval = Interval.Parse(names[random.Next(names.Length)]);
    var answer = root.Transpose(interval);
    Console.WriteLine($"What is a {interval} above {root}? {answer}");
}
```

## Transposing a song for a capo

With a capo on the 3rd fret, a song in E♭ is played with the shapes of C:

```csharp
var song = new[] { "Eb", "Cm", "Ab", "Bb7" };
var shapes = song.Select(c => ChordTransposer.Transpose(c, -3, KeySignature.Parse("C")));
Console.WriteLine(string.Join(" ", shapes));   // C Am F G7
```

## Frequencies for a tuner

```csharp
foreach (var name in new[] { "E2", "A2", "D3", "G3", "B3", "E4" })
    Console.WriteLine($"{name}: {Note.Parse(name).Frequency:0.00} Hz");
```
