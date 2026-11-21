---
title: Chord sheets and ABC
description: Parse, edit, transpose and write ChordPro chord sheets and ABC tunes with the two plain-text packages.
section: Topics
order: 14
---

Two packages read and write plain-text music formats: `MusicTheory.ChordPro` for chord sheets and
`MusicTheory.Abc` for tunes. Both are read with `Parse` and written back with `ToString`. The ABC section uses
`Measure` and `Rational` from [Measures and tempo](measures-and-tempo.md).

## Installing

```
dotnet add package MusicTheory.ChordPro
dotnet add package MusicTheory.Abc
```

Each package adds its own namespace, so a file that uses them starts with `using MusicTheory.ChordPro;` or
`using MusicTheory.Abc;` next to `using MusicTheory;`.

## ChordPro

A ChordPro sheet is lyrics with chords in square brackets and directives in braces. `ChordProDocument.Parse`
reads it into lines and exposes the common directives as properties: `Title`, `Key`, `Capo`, and `Chords`, the
chord symbols in order of appearance. Lines it cannot classify become lyric lines, so `Parse` never throws on
content.

```csharp
using MusicTheory.ChordPro;

var text = "{t: Amazing Grace}\n{key: G}\n{capo: 2}\n[G]Amazing [C]grace";
var song = ChordProDocument.Parse(text);

Console.WriteLine(song.Title);                       // Amazing Grace
Console.WriteLine(song.Key);                         // G
Console.WriteLine(song.Capo);                        // 2
Console.WriteLine(string.Join(" ", song.Chords));    // G C
Console.WriteLine(song.ToString() == text);          // True
```

Every line keeps its raw text, which is what makes the round trip exact. `Lines` holds one entry per line, each
a `DirectiveLine`, a `LyricLine` or a `CommentLine`:

```csharp
foreach (var line in song.Lines)
{
    Console.WriteLine(line.GetType().Name);
}
```

### Transposing

`Transpose(int)` moves every chord by semitones and rewrites the `{key:}` directive. Up two semitones G becomes
A, C becomes D, and a G major sheet lands in A major, a whole tone higher.

```csharp
var up = song.Transpose(2);
Console.WriteLine(up.ToString().Split('\n')[1]);      // {key: A}
Console.WriteLine(string.Join(" ", up.Chords));      // A D
```

`Transpose(KeySignature)` moves the sheet from its own key to the key you give, and spells the chords for it.
G major to Bb major is three semitones up:

```csharp
var inBb = song.Transpose(new KeySignature(Note.Parse("Bb"), KeyMode.Major));
Console.WriteLine(inBb.ToString().Split('\n')[1]);    // {key: Bb}
Console.WriteLine(string.Join(" ", inBb.Chords));    // Bb Eb
```

It needs a `{key:}` directive it can read, and throws `InvalidOperationException` otherwise.

Chord suffixes are rewritten to their canonical spelling by default, so `CM7` becomes `Dmaj7`. Pass
`SuffixHandling.Verbatim` to keep the writer's spelling and move only root and bass, and
`SuffixHandling.KeepUnparsed` to also move the root and bass of chords the library cannot read. Lyrics,
annotations and tab blocks are left as written.

```csharp
var jazz = ChordProDocument.Parse("[CM7]Hello");
Console.WriteLine(jazz.Transpose(2).ToString());                                  // [Dmaj7]Hello
Console.WriteLine(jazz.Transpose(2, SuffixHandling.Verbatim).ToString());         // [DM7]Hello

var odd = ChordProDocument.Parse("[Gfoo/B]Hello");
Console.WriteLine(odd.Transpose(2).ToString());                                   // [Gfoo/B]Hello
Console.WriteLine(odd.Transpose(2, SuffixHandling.KeepUnparsed).ToString());      // [Afoo/C#]Hello
```

## ABC

`AbcTune.Parse` reads the first tune of an ABC text into its header fields and measures of spelled notes with
exact durations. `TryParse` does the same without throwing. With `L:1/8` the unit length is an eighth note,
and `K:D` has two sharps.

```csharp
using MusicTheory.Abc;

var tune = AbcTune.Parse("X:1\nT:Speed the Plough\nM:4/4\nL:1/8\nK:D\nDEFG ABcd|dcBA GFED|");

Console.WriteLine(tune.Title);                       // Speed the Plough
Console.WriteLine(tune.Key);                         // D
Console.WriteLine(tune.TimeSignature);               // 4/4
Console.WriteLine(tune.UnitLength);                  // 1/8
Console.WriteLine(tune.Measures.Count);              // 2
```

`ToString()` writes the canonical ABC form, and parsing it back gives an equal tune. `Measure` and `AbcTune` have value equality:

```csharp
var again = AbcTune.Parse(tune.ToString());
Console.WriteLine(again == tune);                              // True
Console.WriteLine(again.Measures.SequenceEqual(tune.Measures));    // True
Console.WriteLine(AbcTune.TryParse("not a tune", out _));    // False
```

Modal keys are stored as the major key with the same signature, so `K:Ador` and `K:Gmaj` both give G major.
Repeats are plain barlines, and voices (`V:`) and lyrics (`w:`) throw `NotSupportedException`.
