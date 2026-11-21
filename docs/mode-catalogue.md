---
title: Mode catalogue
description: The modes of the major, melodic minor and harmonic minor families, the bebop and world scales, and which scales fit a chord.
section: Topics
order: 9
---

[Working with modes](working-with-modes.md) walks through the seven diatonic modes. This page is the
reference for the rest: the three mode families, the extra scale types, and chord-scale matching.

## Mode families

`Scale.GetModes()` returns the seven modes of a scale, one per degree: index 0 is the scale itself, index
`i` starts on degree `i + 1` and carries the name of that mode. It works for the major, melodic minor and
harmonic minor families.

Melodic minor (1 2 b3 4 5 6 7) gives, on C, the modes below. The fourth is Lydian dominant (1 2 3 #4 5 6
b7), the fifth is Mixolydian b6 and the seventh is the altered scale.

```csharp
var melodic = new Scale(Note.Parse("C4"), ScaleType.MelodicMinor);
var names = melodic.GetModes().Select(m => $"{m.Root.GetName()} {m.Type}");
Console.WriteLine(string.Join(", ", names));   // C MelodicMinor, D DorianFlat2, Eb LydianAugmented, F LydianDominant, G MixolydianFlat6, A LocrianNatural2, B Altered
```

Harmonic minor (1 2 b3 4 5 b6 7) gives its own seven. The fifth is Phrygian dominant (1 b2 3 4 5 b6 b7).

```csharp
var harmonic = new Scale(Note.Parse("C4"), ScaleType.HarmonicMinor);
Console.WriteLine(string.Join(", ", harmonic.GetModes().Select(m => $"{m.Root.GetName()} {m.Type}")));   // C HarmonicMinor, D LocrianNatural6, Eb IonianAugmented, F DorianSharp4, G PhrygianDominant, Ab LydianSharp2, B Ultralocrian
```

A scale outside these families, such as a pentatonic, has no named modes and `GetModes()` throws
`InvalidOperationException`.

## More scale types

Every `ScaleType` is a list of intervals above the tonic, so the spelling follows from the formula. The
bebop dominant scale is the Mixolydian scale with a passing major seventh: 1 2 3 4 5 6 b7 7. Hirajoshi is
a five-note scale: 1 2 b3 5 b6.

```csharp
string Spell(ScaleType type) =>
    string.Join(" ", new Scale(Note.Parse("C4"), type).GetNotes().SkipLast(1).Select(n => n.GetName()));

Console.WriteLine(Spell(ScaleType.BebopDominant));   // C D E F G A Bb B
Console.WriteLine(Spell(ScaleType.Hirajoshi));       // C D Eb G Ab
```

The altered scale is 1 b2 b3 b4 b5 b6 b7. It is the seventh mode of melodic minor, here D-flat melodic
minor, so its fourth degree is spelled F-flat rather than E.

```csharp
Console.WriteLine(Spell(ScaleType.Altered));   // C Db Eb Fb Gb Ab Bb
```

## Which scale fits this chord

`ChordScale.ScalesFor` returns the scales on the chord's root that contain every chord tone, tensions
included. They are ranked by avoid notes, the scale tones a minor second above a chord tone, fewest first,
and ties keep the `ScaleType` declaration order. A slash bass is ignored.

For G7 (G B D F), Lydian dominant has no avoid note, Mixolydian and Bebop Dorian have one (C above B),
Mixolydian b6 and Bebop dominant have two, and Phrygian dominant has three.

```csharp
var fits = ChordScale.ScalesFor(ChordSymbol.Parse("G7"));
Console.WriteLine(string.Join(", ", fits.Select(s => s.Type)));   // LydianDominant, Mixolydian, BebopDorian, MixolydianFlat6, BebopDominant, PhrygianDominant
```

The `alt` tensions (b9, #9, b5, #5) are chord tones, so only scales that hold them survive, and the
altered scale is among them.

```csharp
var alt = ChordScale.ScalesFor(ChordSymbol.Parse("G7alt"));
Console.WriteLine(alt.Any(s => s.Type == ScaleType.Altered));   // True
```
