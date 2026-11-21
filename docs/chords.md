---
title: Chords
description: A Chord is a root, a chord type, optional extensions and an inversion, spelled by interval from the root.
section: Concepts
order: 3
---

A `Chord` is a root, a `ChordType`, optional extensions and an inversion. Its notes are spelled from the
root by interval, so the third of F♯ major is A♯ and the seventh of B♭7 is A♭. Like every type in the
library, a chord is immutable.

## Chord types

The 42 types cover triads, sevenths, suspended, sixth, extended, add, altered and power chords. Each type is
defined by its intervals above the root:

```csharp
var c = Note.Parse("C4");
Console.WriteLine(string.Join(" ", new Chord(c, ChordType.Major).Intervals));          // M3 P5
Console.WriteLine(string.Join(" ", new Chord(c, ChordType.HalfDiminished7).Intervals)); // m3 d5 m7
Console.WriteLine(string.Join(" ", new Chord(c, ChordType.Dominant13).Intervals));     // M3 P5 m7 M9 P11 M13
Console.WriteLine(string.Join(" ", new Chord(c, ChordType.Dominant7Alt).Intervals));   // M3 d5 A5 m7 m9
```

## Notes

```csharp
Console.WriteLine(string.Join(" ", new Chord(Note.Parse("F#4"), ChordType.Major).GetNotes()));       // F#4 A#4 C#5
Console.WriteLine(string.Join(" ", new Chord(Note.Parse("Bb3"), ChordType.Dominant7).GetNotes()));   // Bb3 D4 F4 Ab4
Console.WriteLine(string.Join(" ", new Chord(Note.Parse("C4"), ChordType.Diminished7).GetNotes()));  // C4 Eb4 Gb4 Bbb4
Console.WriteLine(string.Join(" ", new Chord(Note.Parse("E3"), ChordType.Dominant7Sharp9).GetNotes())); // E3 G#3 B3 D4 F##4
```

`Quality` gives the family of the triad: a minor seventh is minor, a half-diminished seventh diminished,
a 7sus4 suspended.

```csharp
Console.WriteLine(new Chord(c, ChordType.Minor9).Quality);          // Minor
Console.WriteLine(new Chord(c, ChordType.HalfDiminished7).Quality); // Diminished
Console.WriteLine(new Chord(c, ChordType.Dominant7Sus4).Quality);   // Suspended
Console.WriteLine(new Chord(c, ChordType.Power5).Quality);          // Power
```

## Extensions

`AddExtension` returns a new chord. An extension replaces the chord tone of the same degree (a ♭9 replaces
the 9th, a ♭5 the 5th) or is added when that degree is absent.

```csharp
var c7 = new Chord(c, ChordType.Dominant7);
var c7b9 = c7.AddExtension(9, IntervalQuality.Minor);

Console.WriteLine(string.Join(" ", c7.GetNotes()));     // C4 E4 G4 Bb4
Console.WriteLine(string.Join(" ", c7b9.GetNotes()));   // C4 E4 G4 Bb4 Db5
Console.WriteLine(c7b9.GetSymbol());                    // C7b9

var flatFive = c7.AddExtension(5, IntervalQuality.Diminished);
Console.WriteLine(flatFive.GetSymbol());                // C7b5
```

## Inversions

```csharp
var cMajor7 = new Chord(c, ChordType.Major7);
var third = cMajor7.WithInversion(ChordInversion.Third);

Console.WriteLine(third.GetBassNote());                                  // B4
Console.WriteLine(string.Join(" ", third.GetNotesInInversion()));        // B4 C5 E5 G5
Console.WriteLine(third.GetSlashChordNotation());                        // C/B
Console.WriteLine(third.GetSymbol());                                    // Cmaj7/B
```

An inversion that needs more notes than the chord has is refused:

```csharp
try
{
    new Chord(c, ChordType.Major).WithInversion(ChordInversion.Third);
}
catch (ArgumentOutOfRangeException e)
{
    Console.WriteLine(e.Message.Split(" (")[0]);   // C has 3 notes, too few for a third inversion.
}
```

## Chord symbols

`ChordSymbol` reads and writes lead-sheet symbols. It understands the common aliases (`m`, `min`, `-`,
`maj7`, `M7`, `Δ`, `ø`, `°`, `+`, `sus`, `6/9`), a slash bass, and tensions on top of the type:

```csharp
Console.WriteLine(ChordSymbol.Parse("Am7").Type);                 // Minor7
Console.WriteLine(ChordSymbol.Parse("BbΔ7").Type);                // Major7
Console.WriteLine(ChordSymbol.Parse("D/F#").Bass);                // F#4
Console.WriteLine(ChordSymbol.Parse("C7b9#11"));                  // C7b9#11
Console.WriteLine(ChordSymbol.Parse("C7(b9,#11)"));               // C7b9#11
Console.WriteLine(ChordSymbol.Parse("Cmaj7(#11)"));               // Cmaj7#11
Console.WriteLine(ChordSymbol.TryParse("N.C.", out _));           // False
```

```csharp
var symbol = ChordSymbol.Parse("Bbm7b5");
Console.WriteLine(symbol.ToString(ChordSymbolStyle.Unicode));     // B♭ø7
Console.WriteLine(string.Join(" ", ChordSymbol.Parse("C7b9#11").ToChord().GetNotes()));  // C4 E4 G4 Bb4 Db5 F#5
Console.WriteLine(string.Join(" ", ChordSymbol.Parse("C/E").Voice(4)));                  // E3 C4 E4 G4
```

## From chords to symbols and back

```csharp
var chord = new Chord(Note.Parse("D4"), ChordType.Major).AddExtension(7, IntervalQuality.Minor);
Console.WriteLine(chord.ToChordSymbol());                          // D7
Console.WriteLine(chord.Transpose(Interval.Parse("P4")));          // G7
Console.WriteLine(ChordSymbol.Parse("F#m7/A").ToChord().Inversion); // First
```

## Identifying chords from notes

```csharp
var notes = new[] { "Bb3", "D4", "F4", "Ab4" }.Select(Note.Parse);
Console.WriteLine(ChordFinder.BestMatch(notes)!.Symbol);   // Bb7
```
