---
title: Pitch-class sets
description: Classify chords and scales as pitch-class sets with normal form, prime form, interval-class vector and Forte names, then walk triads with the neo-Riemannian moves P, L and R.
section: Topics
order: 8
---

A `PitchClassSet` is an unordered set of pitch classes, the numbers 0 to 11 with C as 0. Octave, order and
spelling are gone, so it answers the question "what shape is this?" rather than "how is it written?". The
second half of this page uses the same C major triad to walk between triads with the neo-Riemannian moves.

## Pitch-class sets

Build a set from numbers with `Of`, or from spelled notes with `FromNotes`. A set prints as its members in
ascending order.

```csharp
var triad = PitchClassSet.Of(0, 4, 7);
var fromNotes = PitchClassSet.FromNotes(new[] { "C4", "E4", "G4" }.Select(Note.Parse));

Console.WriteLine(triad);                 // {0,4,7}
Console.WriteLine(fromNotes == triad);    // True
Console.WriteLine(triad.Cardinality);     // 3
```

The normal form is the most compact rotation of the set. The prime form is the most compact form of the set
and of its inversion, moved to start on 0, so a major and a minor triad share it. The interval-class vector
counts the interval classes 1 to 6 between every pair of members.

```csharp
Console.WriteLine(string.Join(",", triad.NormalForm));           // 0,4,7
Console.WriteLine(string.Join(",", triad.PrimeForm));            // 0,3,7
Console.WriteLine(string.Join("", triad.IntervalClassVector));   // 001110
```

`Transpose`, `Invert` and `Complement` return new sets. `Invert(n)` maps each pitch class `p` to `n - p`.

```csharp
Console.WriteLine(triad.Transpose(2));              // {2,6,9}
Console.WriteLine(triad.Invert());                  // {0,5,8}
Console.WriteLine(triad.Complement.Cardinality);    // 9
```

## Forte names

Every set class has a Forte name: its cardinality, a dash and its position in Forte's catalogue. The C major
triad is 3-11, and so is C minor, because they share a prime form.

```csharp
var minor = PitchClassSet.Of(0, 3, 7);

Console.WriteLine(triad.ForteName);                                // 3-11
Console.WriteLine(minor.ForteName);                                // 3-11
Console.WriteLine(triad.IsTranspositionOrInversionOf(minor));      // True
Console.WriteLine(PitchClassSet.FromForteName("3-11"));            // {0,3,7}
```

Two set classes are Z-related when they share an interval-class vector but are not related by transposition
or inversion. 4-Z15 and 4-Z29 are the classic pair, both with the vector 111111.

```csharp
var z15 = PitchClassSet.FromForteName("4-Z15");
var z29 = z15.ZPartner!;

Console.WriteLine(z15.IsZRelated);                                                  // True
Console.WriteLine(z29.ForteName);                                                   // 4-Z29
Console.WriteLine(string.Join("", z15.IntervalClassVector));                        // 111111
Console.WriteLine(z15.IntervalClassVector.SequenceEqual(z29.IntervalClassVector));  // True
Console.WriteLine(triad.IsZRelated);                                                // False
```

## Neo-Riemannian moves

The moves `P`, `L` and `R` each turn a major or minor triad into the other mode by moving one note by a step.
`P` (parallel) keeps the root and fifth, `L` (leading-tone exchange) keeps the third and fifth, `R`
(relative) gives the relative key. They are involutions: applying one twice returns the triad. Results are
spelled from the interval moved, so no letter is guessed.

```csharp
var cMajor = new Chord(Note.Parse("C4"), ChordType.Major);

Console.WriteLine(NeoRiemannian.P(cMajor).ToChordSymbol());   // Cm
Console.WriteLine(NeoRiemannian.L(cMajor).ToChordSymbol());   // Em
Console.WriteLine(NeoRiemannian.R(cMajor).ToChordSymbol());   // Am
```

`Apply` runs a string of moves left to right, ignoring case and whitespace. Reading the result as a chord
checks the spelling: R of A minor is C major, and L of A minor is F major.

```csharp
Console.WriteLine(NeoRiemannian.Apply(cMajor, "PL").ToChordSymbol());      // Ab
Console.WriteLine(NeoRiemannian.Apply(cMajor, "RL").ToChordSymbol());      // F
Console.WriteLine(NeoRiemannian.Apply(cMajor, "p l r").ToChordSymbol());   // Fm
```
