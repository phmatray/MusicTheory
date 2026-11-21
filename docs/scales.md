---
title: Scales
description: A Scale is a set of intervals above a tonic, spelled from the tonic so every degree has the right letter.
section: Concepts
order: 4
---

A `Scale` is a tonic and a `ScaleType`. Its notes are the type's intervals applied to the tonic, so each
degree gets the right letter: F♯ major has E♯, G♭ major has C♭. Every scale runs from the tonic to the
tonic an octave above, both included.

## Scale types

```csharp
void Show(string tonic, ScaleType type) =>
    Console.WriteLine(string.Join(" ", new Scale(Note.Parse(tonic), type).GetNotes()));

Show("C4", ScaleType.Major);           // C4 D4 E4 F4 G4 A4 B4 C5
Show("A4", ScaleType.NaturalMinor);    // A4 B4 C5 D5 E5 F5 G5 A5
Show("A4", ScaleType.HarmonicMinor);   // A4 B4 C5 D5 E5 F5 G#5 A5
Show("A4", ScaleType.MelodicMinor);    // A4 B4 C5 D5 E5 F#5 G#5 A5
```

The seven modes:

```csharp
Show("C4", ScaleType.Ionian);      // C4 D4 E4 F4 G4 A4 B4 C5
Show("D4", ScaleType.Dorian);      // D4 E4 F4 G4 A4 B4 C5 D5
Show("E4", ScaleType.Phrygian);    // E4 F4 G4 A4 B4 C5 D5 E5
Show("F4", ScaleType.Lydian);      // F4 G4 A4 B4 C5 D5 E5 F5
Show("G4", ScaleType.Mixolydian);  // G4 A4 B4 C5 D5 E5 F5 G5
Show("A4", ScaleType.Aeolian);     // A4 B4 C5 D5 E5 F5 G5 A5
Show("B4", ScaleType.Locrian);     // B4 C5 D5 E5 F5 G5 A5 B5
```

And the others:

```csharp
Show("C4", ScaleType.PentatonicMajor);   // C4 D4 E4 G4 A4 C5
Show("A4", ScaleType.PentatonicMinor);   // A4 C5 D5 E5 G5 A5
Show("C4", ScaleType.Blues);             // C4 Eb4 F4 F#4 G4 Bb4 C5
Show("C4", ScaleType.WholeTone);         // C4 D4 E4 F#4 G#4 A#4 C5
Show("C4", ScaleType.Chromatic);         // C4 C#4 D4 D#4 E4 F4 F#4 G4 G#4 A4 A#4 B4 C5
```

## Spelling

Because each scale is built by interval, keys far from C come out spelled the way a score would write them:

```csharp
Show("F#4", ScaleType.Major);          // F#4 G#4 A#4 B4 C#5 D#5 E#5 F#5
Show("Gb4", ScaleType.Major);          // Gb4 Ab4 Bb4 Cb5 Db5 Eb5 F5 Gb5
Show("D#4", ScaleType.HarmonicMinor);  // D#4 E#4 F#4 G#4 A#4 B4 C##5 D#5
```

The major, minor and modal scales of all 35 spelled tonics are checked against music21 in the tests. A
tonic whose scale would need triple accidentals is refused:

```csharp
try
{
    new Scale(Note.Parse("B##4"), ScaleType.Major).GetNotes();
}
catch (InvalidOperationException e)
{
    Console.WriteLine(e.Message);   // B##4 transposed by M2 needs 3 sharps on C; at most two are supported.
}
```

## Working with a scale

```csharp
var g = new Scale(Note.Parse("G4"), ScaleType.Major);

Console.WriteLine(string.Join(" ", g.Intervals));                // P1 M2 M3 P4 P5 M6 M7 P8
Console.WriteLine(g.Contains(Note.Parse("F#")));                 // True
Console.WriteLine(g.Contains(Note.Parse("F")));                  // False
Console.WriteLine(g.GetDegree(Note.Parse("D")));                 // 5
Console.WriteLine(g.GetDegree(Note.Parse("C#")) is null);        // True
Console.WriteLine(g.GetNextNoteInScale(Note.Parse("F#2")));      // G2
Console.WriteLine(g.Transpose(Interval.Parse("M2")));            // A4 Major
```

`Contains`, `GetDegree` and `GetNextNoteInScale` compare spellings and ignore octaves: G♭ is not in G major
even though it sounds like F♯.

```csharp
Console.WriteLine(g.Contains(Note.Parse("Gb")));   // False
```

Scales are values:

```csharp
Console.WriteLine(new Scale(Note.Parse("D4"), ScaleType.Dorian) == new Scale(Note.Parse("D4"), ScaleType.Dorian));  // True
```
