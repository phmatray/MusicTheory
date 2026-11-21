---
title: Transposition
description: Transposing notes, chords, scales, keys and chord sheets, by interval or by semitones.
section: Topics
order: 2
---

There are two ways to transpose, and they answer different questions:

- **By interval** keeps the spelling relationship: C up a major second is D, F♯ up a minor second is G.
  Every type has a `Transpose(Interval)`.
- **By semitones** keeps the sound and needs a rule for spelling the result: a key, or sharps by default.

## By interval

```csharp
var up = Interval.Parse("M2");
Console.WriteLine(Note.Parse("F#4").Transpose(up));                                        // G#4
Console.WriteLine(new Chord(Note.Parse("C4"), ChordType.Dominant7).Transpose(up));         // D7
Console.WriteLine(new Scale(Note.Parse("C4"), ScaleType.Major).Transpose(up));             // D4 Major
Console.WriteLine(ChordSymbol.Parse("D/F#").Transpose(Interval.Parse("m2")));              // Eb/G
```

A descending interval transposes down:

```csharp
Console.WriteLine(Note.Parse("C4").Transpose(Interval.Parse("P-4")));                       // G3
Console.WriteLine(ChordSymbol.Parse("Am7").Transpose(Interval.Parse("M-2")));               // Gm7
```

## By semitones

```csharp
var a = Note.Parse("A4");
Console.WriteLine(a.TransposeBySemitones(1));                               // A#4
Console.WriteLine(a.TransposeBySemitones(1, KeySignature.Parse("F")));      // Bb4
Console.WriteLine(a.TransposeBySemitones(-12));                             // A3
Console.WriteLine(KeySignature.Parse("G").TransposeBySemitones(1));         // Ab
```

## Chord sheets

`ChordTransposer` works on chord-symbol strings, the way a lyric sheet is written. Anything that is not a
chord (`N.C.`, `%`, a word) is returned unchanged.

```csharp
Console.WriteLine(ChordTransposer.Transpose("D/F#", 2));                       // E/G#
Console.WriteLine(ChordTransposer.Transpose("Bb7", 2));                        // C7
Console.WriteLine(ChordTransposer.Transpose("F#m7", -1));                      // Fm7
Console.WriteLine(ChordTransposer.Transpose("N.C.", 3));                       // N.C.
Console.WriteLine(ChordTransposer.Transpose("C", 1, KeySignature.Parse("E"))); // C#
Console.WriteLine(ChordTransposer.Transpose("Cfoo/E", 2));                     // Cfoo/E
Console.WriteLine(ChordTransposer.Transpose("Cfoo/E", 2, suffix: SuffixHandling.KeepUnparsed)); // Dfoo/F#
```

The default leaves text it cannot read alone, because a word like `Chorus` also starts with a root. When the
input is known to be chords, pass `SuffixHandling.KeepUnparsed` to move the root and the slash bass and keep
the rest as written.

Transposition also rewrites a suffix to its canonical form. To keep a chart's own notation, pass
`SuffixHandling.Verbatim`: only the root and the slash bass move, and the suffix stays as typed.

```csharp
Console.WriteLine(ChordTransposer.Transpose("C-7", 2));                                     // Dm7
Console.WriteLine(ChordTransposer.Transpose("C-7", 2, suffix: SuffixHandling.Verbatim));    // D-7
```

Moving a whole song from one key to another spells every chord in the target key:

```csharp
var song = new[] { "C", "Am", "F", "G7" };
var inE = song.Select(c => ChordTransposer.Transpose(c, KeySignature.Parse("C"), KeySignature.Parse("E")));
Console.WriteLine(string.Join(" ", inE));   // E C#m A B7
```

Chords move by the interval between the keys, so a borrowed Eb in C is Bb in G. The spelling of the target key is used only to avoid a double accidental.

```csharp
Console.WriteLine(ChordTransposer.Transpose("Eb", KeySignature.Parse("C"), KeySignature.Parse("G")));   // Bb
```

The output keeps the input's style: Unicode accidentals stay Unicode, jazz glyphs stay glyphs.

```csharp
Console.WriteLine(ChordTransposer.Transpose("B♭ø7", 2));   // Cø7
```

## Transposing instruments

A B♭ clarinet sounds a major second below what it reads. To write its part, transpose the concert pitch up
a major second:

```csharp
var concert = new[] { "C4", "E4", "G4" }.Select(Note.Parse);
var clarinet = concert.Select(n => n.Transpose(Interval.Parse("M2")));
Console.WriteLine(string.Join(" ", clarinet));   // D4 F#4 A4

var altoSax = concert.Select(n => n.Transpose(Interval.Parse("M6")));
Console.WriteLine(string.Join(" ", altoSax));    // A4 C#5 E5
```
