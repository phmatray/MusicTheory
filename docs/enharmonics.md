---
title: Enharmonics
description: Notes, intervals and keys that sound the same but are written differently.
section: Topics
order: 3
---

Two spellings are enharmonic when they sound the same. The library keeps them distinct values, because
the spelling carries meaning: in E major the leading tone is D♯, never E♭.

## Notes

```csharp
var cSharp = Note.Parse("C#4");
var dFlat = Note.Parse("Db4");

Console.WriteLine(cSharp == dFlat);                    // False
Console.WriteLine(cSharp.IsEnharmonicWith(dFlat));     // True
Console.WriteLine(cSharp.PitchClass == dFlat.PitchClass);  // True
```

Enharmonic means the same sounding pitch, octave included, as in music21. When the letter crosses B and C,
the octave number changes even though the pitch does not:

```csharp
Console.WriteLine(Note.Parse("B#3").IsEnharmonicWith(Note.Parse("C4")));  // True
Console.WriteLine(Note.Parse("Cb5").IsEnharmonicWith(Note.Parse("B4")));  // True
Console.WriteLine(Note.Parse("C4").IsEnharmonicWith(Note.Parse("C5")));   // False
```

## Respelling

`Respell` writes the same pitch with another letter, or returns null when that needs more than two
accidentals:

```csharp
var e = Note.Parse("E4");
Console.WriteLine(e.Respell(NoteName.F));             // Fb4
Console.WriteLine(e.Respell(NoteName.D));             // D##4
Console.WriteLine(e.Respell(NoteName.G) is null);     // True
```

`GetEnharmonicEquivalent` gives the usual alternative, `GetAllEnharmonicEquivalents` every one, and
`SimplifyEnharmonic` the plainest spelling:

```csharp
Console.WriteLine(Note.Parse("A#4").GetEnharmonicEquivalent());          // Bb4
Console.WriteLine(Note.Parse("D4").GetEnharmonicEquivalent() is null);   // True
Console.WriteLine(string.Join(" ", Note.Parse("G#4").GetAllEnharmonicEquivalents()));  // Ab4
Console.WriteLine(Note.Parse("Fbb4").SimplifyEnharmonic());              // Eb4
Console.WriteLine(Note.Parse("B#4").SimplifyEnharmonic());               // C5
Console.WriteLine(Note.Parse("F#4").SimplifyEnharmonic());               // F#4
```

## Intervals

An augmented fourth and a diminished fifth are both six semitones. They are enharmonic but not equal:

```csharp
var tritoneUp = Interval.Parse("A4");
var flatFive = Interval.Parse("d5");
Console.WriteLine(tritoneUp == flatFive);                  // False
Console.WriteLine(tritoneUp.IsEnharmonicWith(flatFive));   // True
Console.WriteLine(Interval.Between(Note.Parse("C4"), Note.Parse("F#4")));   // A4
Console.WriteLine(Interval.Between(Note.Parse("C4"), Note.Parse("Gb4")));   // d5
```

## Keys

Three pairs of major keys share their pitches: B and C♭, F♯ and G♭, C♯ and D♭.

```csharp
Console.WriteLine(string.Join(" ", KeySignature.Parse("F#").GetEnharmonicEquivalents()));   // Gb
Console.WriteLine(string.Join(" ", KeySignature.Parse("Db").GetEnharmonicEquivalents()));   // C#
Console.WriteLine(KeySignature.Parse("C").GetEnharmonicEquivalents().Any());               // False
```

## Chords

```csharp
var chord = new Chord(Note.Parse("F#4"), ChordType.Major);
Console.WriteLine(chord.GetEnharmonicEquivalent());                         // Gb
Console.WriteLine(string.Join(" ", chord.GetEnharmonicEquivalent()!.GetNotes()));   // Gb4 Bb4 Db5
```
