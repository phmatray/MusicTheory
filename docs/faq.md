---
title: FAQ
description: Answers to the questions people ask first, from spelling choices to version 2 changes.
section: Help
order: 2
---

## Why is C♯ not equal to D♭?

Because they are different notes that sound the same. In A major the third is C♯; in B♭ minor the third is
D♭. Equality compares spelling; `IsEnharmonicWith` compares sound.

```csharp
Console.WriteLine(Note.Parse("C#4") == Note.Parse("Db4"));                  // False
Console.WriteLine(Note.Parse("C#4").IsEnharmonicWith(Note.Parse("Db4")));   // True
```

## Why does B♯3 have octave 3?

Scientific pitch notation changes the octave number at C. B♯3 is the B♯ just below C4, and sounds like C4.

```csharp
Console.WriteLine(Note.Parse("B#3").MidiNumber);   // 60
```

## How do I get the pitch class, or the MIDI number?

```csharp
Console.WriteLine(Note.Parse("Eb4").PitchClass);   // 3
Console.WriteLine(Note.Parse("Eb4").MidiNumber);   // 63
```

## How do I tune to A = 442 Hz?

```csharp
Console.WriteLine(Note.Parse("A4").GetFrequency(442));   // 442
```

## Why does transposing B𝄪 up a major second throw?

The result would be C𝄪♯, a triple sharp. The library supports up to two accidentals per note and refuses
anything it cannot spell instead of returning a wrong note.

## How do I write a German H?

```csharp
Console.WriteLine(Note.Parse("H", NoteNotation.German));   // B4
```

## What changed in version 2?

- `Note`, `Interval`, `Chord`, `Scale` and `KeySignature` are immutable records with value equality, and
  they validate their arguments.
- `Interval` has qualities up to quintuply augmented and diminished, a direction, music21 names, `Parse`,
  `Reverse` and `ToSimple`. `Transpose` no longer takes a `Direction`: the interval carries it.
- `Interval.Between` counts letters and follows the staff; it is checked against music21.
- `Chord.AddExtension` returns a new chord and replaces the degree it alters.
- `ChordSymbol` parses tensions: `C7b9#11`, `Cmaj7(#11)`.
- Every scale ends on the octave, and is built by interval.
- `KeySignature` supports theoretical keys, and the circle of fifths no longer skips a key at C♯ and C♭.
- `IsEnharmonicWith` compares the sounding pitch, octave included.
- Every `Parse` throws `FormatException`.
- `MusicTheoryUtilities` and `MusicTheoryConstants` are internal; use `Note.PitchClass`, `Note.Respell`
  and `Interval.Semitones` instead.
- The library targets .NET 11.

## Is it checked against a reference?

Yes. The tests replay intervals, transpositions, scales and key signatures computed by music21, the theory
toolkit from MIT. See `tools/reference/generate.py` in the repository.
