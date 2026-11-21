---
title: Tuning and temperaments
description: Read a pitch from a frequency, measure cents, and compare equal, just, Pythagorean and meantone tunings.
section: Topics
order: 6
---

Notes are spelled values, and their frequencies use twelve-tone equal temperament unless you ask for another
tuning. This page goes from hertz to a note, then compares four tuning systems on one interval.

## From Hz to a note

`Note.FromFrequency` finds the nearest note and returns a `TunedPitch`: the note plus how many cents the
frequency lies above it. A cent is a hundredth of an equal semitone, so an octave is 1200 cents.

```csharp
using MusicTheory;

var pitch = Note.FromFrequency(446.0);
Console.WriteLine(pitch.Note);                                  // A4
Console.WriteLine(Math.Round(pitch.Cents, 1));                  // 23.4
Console.WriteLine(pitch);                                       // A4 +23.4¢
Console.WriteLine(TunedPitch.CentsBetween(440, 880));           // 1200
```

`GetFrequency` goes the other way, and takes the reference for A4 when it is not 440 Hz.

```csharp
Console.WriteLine(Math.Round(pitch.GetFrequency(), 1));         // 446
Console.WriteLine(Math.Round(Note.FromFrequency(415.0, a4: 415).Cents, 1));  // 0
```

## Temperaments

A `Temperament` is a table of offsets in cents from equal temperament, one per semitone above a tonic. The
four presets are `Equal`, `Pythagorean`, `JustIntonation` and `QuarterCommaMeantone`.

`OffsetCents(4)` is the major third. A pure 5/4 third is 386.31 cents and a Pythagorean 81/64 third is 407.82,
against 400 in equal temperament, so the offsets are about -13.7 and +7.8.

```csharp
Console.WriteLine(Temperament.Equal.OffsetCents(4));                                  // 0
Console.WriteLine(Math.Round(Temperament.JustIntonation.OffsetCents(4), 1));          // -13.7
Console.WriteLine(Math.Round(Temperament.QuarterCommaMeantone.OffsetCents(4), 1));    // -13.7
Console.WriteLine(Math.Round(Temperament.Pythagorean.OffsetCents(4), 1));             // 7.8
```

`GetFrequency(note, tonic, a4)` sounds a note in a temperament relative to a tonic. The tonic's octave is
ignored, and A stays at `a4`. With A at 440 Hz the just C is 264 Hz (440 x 3/5), so the pure 5/4 third above
it is exactly 330 Hz, against 329.63 Hz in equal temperament.

```csharp
var c = Note.Parse("C4");
var e = Note.Parse("E4");
Console.WriteLine(Math.Round(Temperament.Equal.GetFrequency(e, c), 2));            // 329.63
Console.WriteLine(Math.Round(Temperament.JustIntonation.GetFrequency(e, c), 2));  // 330
```
