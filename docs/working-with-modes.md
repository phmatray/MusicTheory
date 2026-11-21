---
title: "Tutorial: working with modes"
description: The seven modes, their characteristic notes, their chords, and modal interchange.
section: Tutorials
order: 3
---

The seven modes are the seven rotations of the major scale. This tutorial compares them, finds what makes
each one sound like itself, builds their chords, and borrows chords from the parallel minor.

## The seven modes on one tonic

Played on the same tonic, the modes differ by a few notes:

```csharp
var modes = new[] { ScaleType.Lydian, ScaleType.Ionian, ScaleType.Mixolydian, ScaleType.Dorian,
                    ScaleType.Aeolian, ScaleType.Phrygian, ScaleType.Locrian };
foreach (var mode in modes)
{
    var notes = new Scale(Note.Parse("C4"), mode).GetNotes().Take(7).Select(n => n.GetName(AccidentalStyle.Unicode));
    Console.WriteLine($"{mode,-10} {string.Join(" ", notes)}");
}
```

Ordered this way, from the brightest to the darkest, each mode lowers one note of the previous one.

## The characteristic note

What sets a mode apart from its nearest major or minor scale:

```csharp
string Characteristic(ScaleType mode, ScaleType parent)
{
    var a = new Scale(Note.Parse("C4"), mode).GetNotes().Take(7).ToList();
    var b = new Scale(Note.Parse("C4"), parent).GetNotes().Take(7).ToList();
    return string.Join(" ", a.Where((n, i) => n != b[i]).Select(n => $"{n.GetName()} (degree {a.IndexOf(n) + 1})"));
}

Console.WriteLine(Characteristic(ScaleType.Lydian, ScaleType.Major));             // F# (degree 4)
Console.WriteLine(Characteristic(ScaleType.Mixolydian, ScaleType.Major));         // Bb (degree 7)
Console.WriteLine(Characteristic(ScaleType.Dorian, ScaleType.NaturalMinor));      // A (degree 6)
Console.WriteLine(Characteristic(ScaleType.Phrygian, ScaleType.NaturalMinor));    // Db (degree 2)
```

## Modes of a parent scale

D Dorian, E Phrygian and the others share the notes of C major; only the tonic moves:

```csharp
var parent = new Scale(Note.Parse("C4"), ScaleType.Major).GetNotes().Take(7).ToList();
var rotations = modes.Select(m => new Scale(parent[new[] { 3, 0, 4, 1, 5, 2, 6 }[Array.IndexOf(modes, m)]], m));
Console.WriteLine(rotations.All(s => s.GetNotes().Take(7).All(n => parent.Any(p => p.Name == n.Name && p.Alteration == n.Alteration))));   // True
```

## Chords of a mode

```csharp
IEnumerable<string> Chords(Scale scale)
{
    var notes = scale.GetNotes().Take(7).ToList();
    return Enumerable.Range(0, 7).Select(i =>
        ChordFinder.Identify(new[] { notes[i], notes[(i + 2) % 7], notes[(i + 4) % 7] })
            .First(m => m.Root.Name == notes[i].Name && m.ExtraNotes == 0).Symbol);
}

Console.WriteLine(string.Join(" ", Chords(new Scale(Note.Parse("D4"), ScaleType.Dorian))));       // Dm Em F G Am Bdim C
Console.WriteLine(string.Join(" ", Chords(new Scale(Note.Parse("G4"), ScaleType.Mixolydian))));   // G Am Bdim C Dm Em F
```

## Modal interchange

Chords borrowed from the parallel minor are marked `Borrowed` by the analyser:

```csharp
var progression = new[] { "C", "Ab", "Bb", "C", "Fm", "C" }.Select(ChordSymbol.Parse);
var analysis = HarmonicAnalyzer.Analyze(progression, KeySignature.Parse("C"));
Console.WriteLine(string.Join(" ", analysis));   // I bVI bVII I iv I
Console.WriteLine(string.Join(" ", analysis.Select(n => n.Function).Distinct()));   // Diatonic Borrowed
```
