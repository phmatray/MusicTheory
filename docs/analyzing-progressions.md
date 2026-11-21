---
title: "Tutorial: analysing progressions"
description: Read a lead sheet, find its key, write it in Roman numerals and check its voice leading.
section: Tutorials
order: 2
---

This tutorial takes a lead sheet from text to analysis: parse the chords, find the key, write Roman
numerals, transpose it, and check the voice leading of a four-part realisation.

## From a lead sheet to chords

```csharp
var sheet = "| Cmaj7 | Am7 | Dm7 | G7 | Em7 | A7 | Dm7 G7 | Cmaj7 |";
var chords = sheet.Split(new[] { '|', ' ' }, StringSplitOptions.RemoveEmptyEntries)
    .Select(ChordSymbol.Parse)
    .ToList();
Console.WriteLine(chords.Count);   // 9
```

## Finding the key

```csharp
var best = KeyDetector.Infer(chords)[0];
Console.WriteLine(best.Key);                       // C
Console.WriteLine($"{best.DiatonicHits}/{best.Total}");   // 8/9
```

## Roman numerals

```csharp
var numerals = HarmonicAnalyzer.Analyze(chords, best.Key);
Console.WriteLine(string.Join(" ", numerals));   // Imaj7 vi7 ii7 V7 iii7 V7/ii ii7 V7 Imaj7

var outsiders = chords.Zip(numerals).Where(p => p.Second.Function != HarmonicFunction.Diatonic);
Console.WriteLine(string.Join(", ", outsiders.Select(p => $"{p.First} is {p.Second.Function}")));   // A7 is SecondaryDominant
```

## Common patterns

Find every ii–V–I by its numerals:

```csharp
var text = numerals.Select(n => n.ToString()).ToList();
var twoFiveOnes = Enumerable.Range(0, text.Count - 2)
    .Where(i => text[i] == "ii7" && text[i + 1] == "V7" && text[i + 2] == "Imaj7");
Console.WriteLine(string.Join(", ", twoFiveOnes.Select(i => $"bar {i + 1}")));   // bar 7
```

## Transposing the tune

```csharp
var inF = chords.Select(c => c.Transpose(KeySignature.Parse("C"), KeySignature.Parse("F")));
Console.WriteLine(string.Join(" ", inF));   // Fmaj7 Dm7 Gm7 C7 Am7 D7 Gm7 C7 Fmaj7
```

## Checking the voice leading

`VoiceLeadingRules` checks a four-part realisation chord by chord. Voices are listed bass to soprano.

```csharp
Note[] Voices(string text) => text.Split(' ').Select(Note.Parse).ToArray();

var good = VoiceLeadingRules.ValidateVoiceLeading(Voices("D3 F3 A3 C4"), Voices("G2 F3 B3 D4"));
Console.WriteLine(good.Count);   // 0

var parallel = VoiceLeadingRules.ValidateVoiceLeading(Voices("C3 G3 C4 E4"), Voices("D3 A3 D4 F4"));
Console.WriteLine(string.Join("; ", parallel.Select(v => v.Description)));
// Parallel fifths between voice 0 and voice 1; Parallel octaves between voice 0 and voice 2; Parallel fifths between voice 1 and voice 2
```

The individual checks are public too:

```csharp
Console.WriteLine(VoiceLeadingRules.GetMotionType(Note.Parse("C3"), Note.Parse("D3"), Note.Parse("G4"), Note.Parse("F4")));   // Contrary
Console.WriteLine(VoiceLeadingRules.HasAugmentedInterval(Note.Parse("F4"), Note.Parse("G#4")));   // True
```
