---
title: Instruments
description: Voice chords on guitar, ukulele and other tunings, map notes and scales on the fretboard, and finger piano scales.
section: Topics
order: 13
---

`MusicTheory.Guitar` turns a chord into playable shapes on any fretted instrument, and the core library
finger piano scales. The blocks below share state, like notebook cells.

## Tunings

A `Tuning` is a name and its open strings, listed from the lowest string as the instrument is written.
Presets cover guitar, ukulele, bass, mandolin and the seven-string guitar. A tuning is re-entrant when a
string is lower than the one before it, as on a ukulele whose first string is the high G.

```csharp
using MusicTheory.Guitar;

Console.WriteLine(Tuning.Standard.Strings.Count);   // 6
Console.WriteLine(Tuning.Ukulele.IsReentrant);   // True
Console.WriteLine(Tuning.UkuleleLowG.IsReentrant);   // False
Console.WriteLine(Tuning.Standard.IsReentrant);   // False
```

## Voicings on any tuning

`GuitarVoicingProvider.GetVoicings` takes a chord and `VoicingOptions`; the tuning in the options decides
the string count and the shapes. `ToTab()` writes a voicing low string to high, `x` for a muted string.
The best voicing comes first.

```csharp
var provider = new GuitarVoicingProvider();
var ukulele = new VoicingOptions(Tuning.Ukulele);

string Uke(string name) => provider.GetVoicings(ChordSymbol.Parse(name), ukulele)[0].ToTab();

Console.WriteLine(Uke("C"));   // 0003
Console.WriteLine(Uke("G"));   // 0232
Console.WriteLine(Uke("Am"));   // 2000
Console.WriteLine(Uke("F"));   // 2010
```

An extended-range guitar uses the curated shapes written for it. The seven-string guitar adds a low B
string, which an open C chord leaves muted:

```csharp
var sevenString = new VoicingOptions(Tuning.SevenString);
Console.WriteLine(provider.GetVoicings(ChordSymbol.Parse("C"), sevenString)[0].ToTab());   // xx32010
```

`VoicingDescriber` speaks a chord name or a whole voicing, in English by default and in French when you
pass the culture:

```csharp
var c = ChordSymbol.Parse("C");
var best = provider.GetVoicings(c)[0];
Console.WriteLine(VoicingDescriber.ChordName(c));   // C major
Console.WriteLine(VoicingDescriber.ChordName(c, new System.Globalization.CultureInfo("fr")));   // do majeur
Console.WriteLine(VoicingDescriber.AccessibleLabel(best, new System.Globalization.CultureInfo("fr")));   // do majeur, x-3-2-0-1-0
```

## The fretboard

A `Fretboard` is a tuning and a number of frets. `NoteAt(string, fret)` names a position, counting strings
from the lowest as the tuning lists them. `Positions` finds every place a note, scale or chord sounds.

```csharp
var board = new Fretboard(Tuning.Standard, 12);
Console.WriteLine(board.NoteAt(5, 5));   // A4
Console.WriteLine(board.NoteAt(0, 0));   // E2
```

A note has no scale degree. A scale reports each position's degree, counted from the tonic:

```csharp
var aOnHighE = board.Positions(Note.Parse("A4")).Where(p => p.String == 5).Single();
Console.WriteLine($"string {aOnHighE.String} fret {aOnHighE.Fret}");   // string 5 fret 5

var g = new Scale(Note.Parse("G4"), ScaleType.Major);
var lowE = board.Positions(g).Where(p => p.String == 0 && p.Fret <= 3).ToList();
Console.WriteLine(string.Join(", ", lowE.Select(p => $"{p.Note.GetName(AccidentalStyle.Unicode)} fret {p.Fret} degree {p.Degree}")));   // E fret 0 degree 6, F♯ fret 2 degree 7, G fret 3 degree 1
```

## Piano fingering

`PianoFingering.ForScale` returns the standard fingers, 1 for the thumb to 5 for the little finger, for the
ascending notes of a seven-note scale. The left hand is the standard left-hand pattern for C major, 5 4 3 2 1 3 2 1 up the keyboard. `ForTriad` fingers a close-position triad.

```csharp
var cMajor = new Scale(Note.Parse("C4"), ScaleType.Major);
Console.WriteLine(string.Join(" ", PianoFingering.ForScale(cMajor, Hand.Right)));   // 1 2 3 1 2 3 4 5
Console.WriteLine(string.Join(" ", PianoFingering.ForScale(cMajor, Hand.Left)));   // 5 4 3 2 1 3 2 1
Console.WriteLine(string.Join(" ", PianoFingering.ForTriad(new[] { Note.Parse("C4"), Note.Parse("E4"), Note.Parse("G4") }, Hand.Right)));   // 1 3 5
```
