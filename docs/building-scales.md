---
title: "Tutorial: building scales"
description: Build scales from their intervals, compare them, and derive chords and modes from them.
section: Tutorials
order: 1
---

This tutorial builds a few small tools on top of `Scale`: a scale table, a comparison, chords on each
degree and a practice exercise. Each block runs as written.

## Every scale on one tonic

```csharp
var tonic = Note.Parse("D4");
foreach (var type in new[] { ScaleType.Major, ScaleType.NaturalMinor, ScaleType.HarmonicMinor, ScaleType.MelodicMinor })
{
    var names = new Scale(tonic, type).GetNotes().Select(n => n.GetName(AccidentalStyle.Unicode));
    Console.WriteLine($"{type,-14} {string.Join(" ", names)}");
}
```

## What a scale is made of

`Intervals` gives the formula; the steps between neighbours give the familiar whole and half steps.

```csharp
string Steps(ScaleType type)
{
    var notes = new Scale(Note.Parse("C4"), type).GetNotes().ToList();
    return string.Join("-", notes.Zip(notes.Skip(1), (a, b) => Interval.Between(a, b).Semitones switch
    {
        1 => "H",
        2 => "W",
        _ => "W+H",
    }));
}

Console.WriteLine(Steps(ScaleType.Major));           // W-W-H-W-W-W-H
Console.WriteLine(Steps(ScaleType.NaturalMinor));    // W-H-W-W-H-W-W
Console.WriteLine(Steps(ScaleType.HarmonicMinor));   // W-H-W-W-H-W+H-H
```

## Comparing two scales

The notes two scales share, and the ones that differ, by spelling:

```csharp
(string Shared, string OnlyFirst) Compare(Scale a, Scale b)
{
    var first = a.GetNotes().Take(7).ToList();
    var shared = first.Where(b.Contains).Select(n => n.GetName());
    var only = first.Where(n => !b.Contains(n)).Select(n => n.GetName());
    return (string.Join(" ", shared), string.Join(" ", only));
}

var (shared, onlyMajor) = Compare(new Scale(Note.Parse("C4"), ScaleType.Major), new Scale(Note.Parse("A4"), ScaleType.HarmonicMinor));
Console.WriteLine(shared);      // C D E F A B
Console.WriteLine(onlyMajor);   // G
```

## Chords on every degree

Stacking every other scale note gives the chord on each degree. `ChordFinder` names it:

```csharp
IEnumerable<string> Triads(Scale scale)
{
    var notes = scale.GetNotes().Take(7).ToList();
    for (int i = 0; i < 7; i++)
    {
        var triad = new[] { notes[i], notes[(i + 2) % 7], notes[(i + 4) % 7] };
        yield return ChordFinder.Identify(triad).First(m => m.Root.Name == notes[i].Name && m.ExtraNotes == 0).Symbol;
    }
}

Console.WriteLine(string.Join(" ", Triads(new Scale(Note.Parse("A4"), ScaleType.HarmonicMinor))));   // Am Bdim Caug Dm E F G#dim
```

`ChordProgression` does the same for major and natural minor keys:

```csharp
Console.WriteLine(string.Join(" ", new ChordProgression(KeySignature.Parse("Eb")).GetDiatonicChords()));   // Eb Fm Gm Ab Bb Cm Ddim
```

## A practice exercise

Scales in thirds, the classic warm-up, built with `GetNextNoteInScale`:

```csharp
IEnumerable<string> InThirds(Scale scale, Note start, int count)
{
    var note = start;
    for (int i = 0; i < count; i++)
    {
        var third = scale.GetNextNoteInScale(scale.GetNextNoteInScale(note));
        yield return $"{note}-{third}";
        note = scale.GetNextNoteInScale(note);
    }
}

Console.WriteLine(string.Join(" ", InThirds(new Scale(Note.Parse("G4"), ScaleType.Major), Note.Parse("G4"), 4)));
// G4-B4 A4-C5 B4-D5 C5-E5
```
