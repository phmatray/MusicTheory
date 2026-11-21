---
title: Chord progressions
description: Diatonic chords, Roman numerals, secondary dominants, harmonic analysis and key detection.
section: Topics
order: 1
---

`ChordProgression` gives the diatonic harmony of a key; `HarmonicAnalyzer` reads chords back as Roman
numerals; `KeyDetector` guesses the key of a progression.

## Diatonic chords

The diatonic chords are built by stacking the scale's thirds. Minor keys use the natural minor scale.

```csharp
var c = new ChordProgression(KeySignature.Parse("C"));
Console.WriteLine(string.Join(" ", c.GetDiatonicChords()));         // C Dm Em F G Am Bdim
Console.WriteLine(string.Join(" ", c.GetDiatonicSeventhChords()));  // Cmaj7 Dm7 Em7 Fmaj7 G7 Am7 Bm7b5

var a = new ChordProgression(KeySignature.Parse("Am"));
Console.WriteLine(string.Join(" ", a.GetDiatonicChords()));         // Am Bdim C Dm Em F G
```

```csharp
Console.WriteLine(c.GetRomanNumeral(2));         // ii
Console.WriteLine(c.GetRomanNumeral(7));         // vii°
Console.WriteLine(c.GetChordByDegree(5));        // G
Console.WriteLine(c.GetSecondaryDominant(2));    // A
```

## Parsing Roman numerals

`ParseProgression` reads numerals separated by `-` or `,`. The key decides each chord's quality; `7` adds
the diatonic seventh and `maj7` a major seventh.

```csharp
Console.WriteLine(string.Join(" ", c.ParseProgression("I - vi - IV - V")));        // C Am F G
Console.WriteLine(string.Join(" ", c.ParseProgression("ii7, V7, Imaj7")));         // Dm7 G7 Cmaj7
Console.WriteLine(string.Join(" ", c.ParseProgression("vii7")));                   // Bm7b5
Console.WriteLine(string.Join(" ", a.ParseProgression("iMaj7 - iv - VII7")));      // AmMaj7 Dm G7
```

## Analysing chords

`HarmonicAnalyzer.Analyze` works by pitch class, so the spelling on the lead sheet does not have to be
perfect. It recognises diatonic chords (minor keys include the harmonic-minor V and vii°), chords borrowed
from the parallel mode, secondary dominants, and writes inversions in figured bass.

```csharp
string Analyze(string key, params string[] chords) =>
    string.Join(" ", HarmonicAnalyzer.Analyze(chords.Select(ChordSymbol.Parse), KeySignature.Parse(key)));

Console.WriteLine(Analyze("C", "C", "Am", "Dm7", "G7", "C"));       // I vi ii7 V7 I
Console.WriteLine(Analyze("C", "C", "Fm", "Ab", "Bb", "C"));        // I iv bVI bVII I
Console.WriteLine(Analyze("C", "C", "A7", "Dm", "D7", "G"));        // I V7/ii ii V7/V V
Console.WriteLine(Analyze("Am", "Am", "Dm", "E7", "Am"));           // i iv V7 i
Console.WriteLine(Analyze("C", "C/E", "F", "G7/B", "C"));           // I6 IV V65 I
```

Each result is a `RomanNumeral` with its parts:

```csharp
var numeral = HarmonicAnalyzer.Analyze(ChordSymbol.Parse("Bb"), KeySignature.Parse("C"));
Console.WriteLine(numeral.Degree);              // 7
Console.WriteLine(numeral.DegreeAlteration);    // Flat
Console.WriteLine(numeral.Function);            // Borrowed
```

## Detecting the key

`KeyDetector.Infer` scores the 24 major and minor keys and returns the best candidates with a confidence:

```csharp
var keys = KeyDetector.Infer(new[] { "Am", "F", "C", "G" }.Select(ChordSymbol.Parse));
Console.WriteLine(keys[0].Key);   // Am
Console.WriteLine(keys[1].Key);   // C

var blues = KeyDetector.Infer(new[] { "C7", "F7", "C7", "G7", "F7", "C7" }.Select(ChordSymbol.Parse));
Console.WriteLine(blues[0].Key);  // C
```

## Transposing a progression

```csharp
var original = new[] { "Dm7", "G7", "Cmaj7" };
var inBb = original.Select(s => ChordTransposer.Transpose(s, KeySignature.Parse("C"), KeySignature.Parse("Bb")));
Console.WriteLine(string.Join(" ", inBb));   // Cm7 F7 Bbmaj7
```
