---
title: Getting started
description: Install MusicTheory and take a first tour of notes, intervals, scales, chords and keys.
section: Start
order: 2
---

MusicTheory targets .NET 11. Add the package and import the namespace:

```bash
dotnet add package MusicTheory
```

```csharp
using MusicTheory;
```

Every type in the library is an immutable value: two notes with the same letter, accidental and octave are
equal, and every operation returns a new object.

## Notes

```csharp
var c4 = new Note(NoteName.C);                         // octave 4 and natural by default
var fSharp5 = new Note(NoteName.F, Alteration.Sharp, 5);
var bFlat3 = Note.Parse("Bb3");
var middleC = Note.FromMidiNumber(60);

Console.WriteLine(middleC == c4);                      // True
Console.WriteLine(fSharp5.MidiNumber);                 // 78
Console.WriteLine(Note.Parse("A4").Frequency);         // 440
```

## Intervals

An interval is a quality, a number and a direction. Build one, parse it from its short name, or measure it
between two notes:

```csharp
var majorThird = new Interval(IntervalQuality.Major, 3);
var perfectFifthDown = Interval.Parse("P-5");

Console.WriteLine(Interval.Between(Note.Parse("C4"), Note.Parse("G4")));  // P5
Console.WriteLine(Interval.Between(Note.Parse("E4"), Note.Parse("C4")));  // M-3

Console.WriteLine(c4.Transpose(majorThird));        // E4
Console.WriteLine(c4.Transpose(perfectFifthDown));  // F3
```

## Scales

```csharp
var cMajor = new Scale(c4, ScaleType.Major);
Console.WriteLine(string.Join(" ", cMajor.GetNotes()));        // C4 D4 E4 F4 G4 A4 B4 C5

var dDorian = new Scale(Note.Parse("D4"), ScaleType.Dorian);
Console.WriteLine(dDorian.Contains(Note.Parse("B")));          // True
Console.WriteLine(cMajor.GetDegree(Note.Parse("G")));          // 5
```

## Chords

```csharp
var g7 = new Chord(Note.Parse("G3"), ChordType.Dominant7);
Console.WriteLine(string.Join(" ", g7.GetNotes()));            // G3 B3 D4 F4
Console.WriteLine(g7.GetSymbol());                             // G7

var firstInversion = g7.WithInversion(ChordInversion.First);
Console.WriteLine(firstInversion.GetSlashChordNotation());     // G/B

var symbol = ChordSymbol.Parse("C7b9#11");
Console.WriteLine(string.Join(" ", symbol.ToChord().GetNotes())); // C4 E4 G4 Bb4 Db5 F#5
```

## Keys and harmony

```csharp
var key = KeySignature.Parse("Eb");
Console.WriteLine(key.AccidentalCount);                        // -3
Console.WriteLine(string.Join(" ", key.AlteredNotes));         // B E A
Console.WriteLine(key.GetRelative());                          // Cm

var progression = new ChordProgression(KeySignature.Parse("C"));
Console.WriteLine(string.Join(" ", progression.ParseProgression("ii7 - V7 - Imaj7"))); // Dm7 G7 Cmaj7

var analysis = HarmonicAnalyzer.Analyze(new[] { "Am", "D7", "G" }.Select(ChordSymbol.Parse), KeySignature.Parse("G"));
Console.WriteLine(string.Join(" ", analysis));                 // ii V7 I
```

## Errors

Invalid input fails at once, with a message that says what is wrong:

```csharp
try
{
    new Interval(IntervalQuality.Perfect, 3);
}
catch (ArgumentOutOfRangeException e)
{
    Console.WriteLine(e.Message.Split(" (")[0]); // A 3rd is major, minor, diminished or augmented, never perfect.
}

// H is the German name of B: English parsing refuses it, German parsing reads it.
Console.WriteLine(Note.TryParse("H4", out _));                   // False
Console.WriteLine(Note.Parse("H4", NoteNotation.German));        // B4
```

## Next steps

- [Notes](notes.md), [Intervals](intervals.md), [Chords](chords.md), [Scales](scales.md), [Keys](key-signatures.md)
- [The API reference](../api/index.html), generated from the XML documentation
