---
title: Advanced harmony
description: Neapolitan and augmented-sixth chords, cadences, modulation and reharmonisation.
section: Topics
order: 11
---

This page goes past the diatonic chords of [Chord progressions](chord-progressions.md): chromatic chords in
`HarmonicAnalyzer`, then `CadenceDetector`, `ModulationDetector` and `Reharmonizer`. For a worked analysis of
a whole lead sheet see [Tutorial: analysing progressions](analyzing-progressions.md).

## Neapolitan and augmented sixths

The Neapolitan chord is a major triad on the lowered second degree, usually in first inversion (the "N6" of
the textbooks). In C major that is Db, written Db/F in first inversion. A German augmented sixth is a
dominant seventh on the lowered sixth degree, here Ab C Eb F#, spelled Ab7 on a lead sheet; the French
version has a flat fifth instead. The analyzer works by pitch class, so the lead-sheet spelling is not
checked.

```csharp
var key = KeySignature.Parse("C");
string Roman(string chord) => HarmonicAnalyzer.Analyze(ChordSymbol.Parse(chord), key).ToString();

Console.WriteLine(Roman("Db"));     // N
Console.WriteLine(Roman("Db/F"));   // N6
Console.WriteLine(Roman("Ab7"));    // Ger+6
Console.WriteLine(HarmonicAnalyzer.Analyze(ChordSymbol.Parse("Db/F"), key).Function);   // Neapolitan
Console.WriteLine(HarmonicAnalyzer.Analyze(ChordSymbol.Parse("Ab7"), key).Function);    // AugmentedSixth
```

Both usually lead to the dominant:

```csharp
Console.WriteLine(string.Join(" ", HarmonicAnalyzer.Analyze(new[] { "C", "Db/F", "G", "C" }.Select(ChordSymbol.Parse), key)));   // I N6 V I
```

## Cadences

`CadenceDetector.Detect` returns each cadence with the index of the chord it arrives on. A `V` to `I` is
authentic, `IV` to `I` plagal, `V` to `vi` deceptive, and a progression that ends on `V` closes with a half
cadence on that last chord.

```csharp
string Cadences(string chords) => string.Join(", ",
    CadenceDetector.Detect(chords.Split(' ').Select(ChordSymbol.Parse).ToList(), key).Select(c => $"{c.Type}@{c.Index}"));

Console.WriteLine(Cadences("C F G7 C"));   // Authentic@3
Console.WriteLine(Cadences("C F C"));   // Plagal@2
Console.WriteLine(Cadences("C Am F G"));   // Half@3
Console.WriteLine(Cadences("C F G Am"));   // Deceptive@3
```

## Modulation

`ModulationDetector.Detect` splits a progression into `KeyRegion`s. `Start` and `End` are inclusive chord
indices, and `PivotIndex` is a chord just before the new region that belongs to both keys (null for the
first region). Here the C chord at index 7 is the pivot: it is I in C and IV in G, so the progression
turns to G major right after it, at index 8.

```csharp
var modulating = "C F G7 C Am Dm G7 C Em Am D7 G C D7 G".Split(' ').Select(ChordSymbol.Parse).ToList();
var regions = ModulationDetector.Detect(modulating);
string Show(KeyRegion r) => $"{r.Key} {r.Start}-{r.End} pivot {r.PivotIndex?.ToString() ?? "none"}";
Console.WriteLine(Show(regions[0]));   // C 0-7 pivot none
Console.WriteLine(Show(regions[1]));   // G 8-14 pivot 7
```

## Reharmonising

`Reharmonizer` makes the usual jazz moves. The tritone substitute of G7 is Db7: both contain the tritone
B and F (spelled Cb and F in Db7).

```csharp
Console.WriteLine(Reharmonizer.TritoneSubstitute(ChordSymbol.Parse("G7")));   // Db7
Console.WriteLine(string.Join(" ", Reharmonizer.TwoFiveTo(ChordSymbol.Parse("Cmaj7"))));   // Dm7 G7
var tune = new[] { "Cmaj7", "Fmaj7", "Cmaj7" }.Select(ChordSymbol.Parse).ToList();
Console.WriteLine(string.Join(" ", Reharmonizer.InsertTwoFive(tune, 2)));   // Cmaj7 Fmaj7 Dm7 G7 Cmaj7
```
