---
title: Overview
description: MusicTheory is a .NET 11 library for spelled music theory, from a single note to an analysed progression.
section: Start
order: 1
---

MusicTheory models music theory the way it is written on the staff. A note has a letter, an accidental
and an octave; an interval counts letters as well as semitones; a scale or a chord is built by interval
from its root. That is what keeps every spelling correct: F♯ major has an E♯, the third of B♭ minor is
D♭, and C up to F𝄪 is a doubly augmented fourth, not "six semitones".

```csharp
var interval = Interval.Between(Note.Parse("C4"), Note.Parse("F##4"));
Console.WriteLine(interval);                  // AA4
Console.WriteLine(interval.Semitones);        // 7

var scale = new Scale(Note.Parse("F#4"), ScaleType.Major);
Console.WriteLine(string.Join(" ", scale.GetNotes())); // F#4 G#4 A#4 B4 C#5 D#5 E#5 F#5
```

## What is in the box

- **[Notes](notes.md)**: parsing (English and German names), MIDI numbers, frequencies at any concert pitch, enharmonic respelling.
- **[Intervals](intervals.md)**: every quality from quintuply diminished to quintuply augmented, direction, compound intervals, inversion.
- **[Chords](chords.md)**: 44 chord types, extensions, inversions, and lead-sheet symbols with tensions such as `C7b9#11`.
- **[Scales](scales.md)**: major, the three minors, the seven modes, pentatonics, blues, whole tone and chromatic.
- **[Keys](key-signatures.md)**: signatures (theoretical keys included), the circle of fifths, relative and parallel keys.
- **[Harmony](chord-progressions.md)**: diatonic triads and sevenths, Roman-numeral analysis, key detection, voice-leading checks.
- **[Rhythm](time-signatures.md)**: durations, dots, tuplets and time signatures.
- **[Measures and tempo](measures-and-tempo.md)**: exact measures, ties across barlines, beat groups, tempo maps and swing.
- **[Tuning and temperaments](tuning-and-temperaments.md)**: pitch from a frequency, cents, and equal, just, Pythagorean and meantone tunings.
- **[Solfège](solfege.md)**: fixed-do and movable-do syllables for any note.
- **[Pitch-class sets](pitch-class-sets.md)**: normal form, prime form, interval vector, Forte names and the neo-Riemannian moves P, L and R.
- **[Mode catalogue](mode-catalogue.md)**: the modes of the major, melodic minor and harmonic minor families, bebop and world scales, and which scales fit a chord.
- **[Piano voicings](piano-voicings.md)**: close, drop 2, drop 3, shell and rootless voicings, and smooth voice-leading.
- **[Advanced harmony](advanced-harmony.md)**: Neapolitan and augmented-sixth chords, cadences, modulation and reharmonisation.
- **[Instruments](instruments.md)**: guitar, ukulele and other tunings, fretboard maps and piano fingering.
- **[Chord sheets and ABC tunes](chord-sheets-and-abc.md)**: parse, transpose and write ChordPro and ABC.
- **[MIDI and MusicXML files](midi-and-musicxml.md)**: write measures to a file and read them back.
- **[Ear training](ear-training.md)**: seeded drills with answer checking and scoring.

## How it is checked

- Intervals, transpositions, scales and key signatures are compared, row by row, with tables computed by
  [music21](https://web.mit.edu/music21/), the reference implementation of spelled theory.
- The libraries are covered at 100% of lines and branches, and the test suite is mutation-tested with
  Stryker.NET.
- Every C# block in this documentation is compiled and run by the test suite, so the pages cannot drift
  from the API.

## Packages

| Package | What it adds |
|---|---|
| `MusicTheory` | The theory: notes, intervals, chords, scales, keys, harmony, rhythm, tuning, set theory, ear training. |
| `MusicTheory.Guitar` | Guitar voicings for every chord type, tunings, capo, and chord-diagram geometry. |
| `MusicTheory.Guitar.Blazor` | Chord playback in the browser for Blazor apps. |
| `MusicTheory.ChordPro` | Parse, edit, transpose and write ChordPro chord sheets. |
| `MusicTheory.Midi` | Write measures to a Standard MIDI file and read them back. |
| `MusicTheory.MusicXml` | Write measures to a MusicXML score and read them back. |
| `MusicTheory.Abc` | Parse and write single-voice ABC tunes. |

```bash
dotnet add package MusicTheory
```

Next: [Getting started](getting-started.md).
