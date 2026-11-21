---
title: Keys and signatures
description: A KeySignature is a tonic and a mode, with its signature computed on the line of fifths.
section: Concepts
order: 5
---

A `KeySignature` is a tonic and a `KeyMode`, major or minor. Its signature is computed on the line of fifths,
so every key gets its sharps or flats in the order they enter a signature.

## Signatures

```csharp
var e = KeySignature.Parse("E");
Console.WriteLine(e.AccidentalCount);                 // 4
Console.WriteLine(string.Join(" ", e.AlteredNotes));  // F C G D
Console.WriteLine(e.GetAlteration(NoteName.G));       // Sharp
Console.WriteLine(e.GetAlteration(NoteName.A));       // Natural

var bFlatMinor = KeySignature.Parse("Bbm");
Console.WriteLine(bFlatMinor.AccidentalCount);                 // -5
Console.WriteLine(string.Join(" ", bFlatMinor.AlteredNotes));  // B E A D G
```

Sharps are positive, flats negative. Theoretical keys are supported as long as no note needs more than two
accidentals, which is the case up to 14 sharps or flats: G♯ major has eight sharps, F𝄪 among them.

```csharp
var gSharp = new KeySignature(Note.Parse("G#"), KeyMode.Major);
Console.WriteLine(gSharp.AccidentalCount);              // 8
Console.WriteLine(gSharp.GetAlteration(NoteName.F));    // DoubleSharp
```

## Parsing

`KeySignature.Parse` reads a tonic followed by an optional mode: `m`, `min`, `minor`, `maj`, `major`, or `M`.
It accepts keys up to the constructor's limit of fourteen accidentals, theoretical ones included.

```csharp
Console.WriteLine(KeySignature.Parse("F#m"));         // F#m
Console.WriteLine(KeySignature.Parse("E♭ minor"));    // Ebm
Console.WriteLine(KeySignature.Parse("c major"));     // C
Console.WriteLine(KeySignature.TryParse("G#", out _)); // True
```

## Related keys

```csharp
var c = KeySignature.Parse("C");
Console.WriteLine(c.GetRelative());         // Am
Console.WriteLine(c.GetParallel());         // Cm
Console.WriteLine(c.GetDominant());         // G
Console.WriteLine(c.GetSubdominant());      // F
Console.WriteLine(KeySignature.Parse("C#").GetRelative());   // A#m
```

## The circle of fifths

`NextInCircle` and `PreviousInCircle` move a fifth up or down. Past seven accidentals the circle continues
on the enharmonic side: the fifth above C♯ major is G♯ major, written A♭ major.

```csharp
var key = KeySignature.Parse("C");
var tour = new List<string>();
for (int i = 0; i < 12; i++)
{
    key = key.NextInCircle();
    tour.Add(key.ToString());
}
Console.WriteLine(string.Join(" ", tour));   // G D A E B F# C# Ab Eb Bb F C
```

```csharp
Console.WriteLine(string.Join(" ", KeySignature.Parse("B").GetEnharmonicEquivalents()));   // Cb
Console.WriteLine(string.Join(" ", KeySignature.Parse("D#m").GetEnharmonicEquivalents())); // Ebm
```

## Spelling in a key

`Spell` names a pitch class the way the key would: scale notes with the key's letters, and other black keys
with sharps in sharp keys and flats in flat keys.

```csharp
Console.WriteLine(KeySignature.Parse("E").Spell(10).GetName());    // A#
Console.WriteLine(KeySignature.Parse("Ab").Spell(10).GetName());   // Bb
Console.WriteLine(KeySignature.Parse("Am").Spell(8).GetName());    // G#
```

`TransposeBySemitones` moves a key and picks the spelling with the fewest accidentals:

```csharp
Console.WriteLine(KeySignature.Parse("G").TransposeBySemitones(1));   // Ab
Console.WriteLine(KeySignature.Parse("D").TransposeBySemitones(-2));  // C
```

Keys compare by tonic spelling and mode; the tonic's octave does not matter:

```csharp
Console.WriteLine(new KeySignature(Note.Parse("D2"), KeyMode.Minor) == KeySignature.Parse("Dm"));   // True
```
