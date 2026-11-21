---
title: Solfège
description: Fixed do and movable do names for notes, minor-key conventions, and reading syllables back.
section: Topics
order: 7
---

`Solfege` names notes with syllables. Fixed do always calls C `Do`; movable do names the degree of a key,
so the same syllable moves with the tonic.

## Fixed do

`Solfege.FixedDo` writes French names. `SolfegeStyle` chooses how accidentals look: words, ASCII or Unicode.

```csharp
using MusicTheory;

var dSharp = Note.Parse("D#4");
Console.WriteLine(Solfege.FixedDo(dSharp));                              // Ré dièse
Console.WriteLine(Solfege.FixedDo(dSharp, SolfegeStyle.Ascii));          // Ré#
Console.WriteLine(Solfege.FixedDo(Note.Parse("Bb"), SolfegeStyle.Unicode));  // Si♭
Console.WriteLine(Note.Parse("Sib3", NoteNotation.FixedDo));             // Bb3
```

## Movable do

`MovableDo(note, key)` gives the syllable of a note in a key. In G major, `do` is G and C♯ is `fi`.

```csharp
var g = KeySignature.Parse("G");
Console.WriteLine(Solfege.MovableDo(Note.Parse("G"), g));                // do
Console.WriteLine(Solfege.MovableDo(Note.Parse("C#"), g));               // fi
Console.WriteLine(Solfege.MovableDo(Note.Parse("F"), g));                // te
```

In a minor key there are two conventions, chosen with `MinorSolfege`. The default `LaBased` starts A minor
on `la`; `DoBased` calls the minor tonic `do`.

```csharp
var aMinor = KeySignature.Parse("Am");
Console.WriteLine(Solfege.MovableDo(Note.Parse("A"), aMinor));                          // la
Console.WriteLine(Solfege.MovableDo(Note.Parse("A"), aMinor, MinorSolfege.DoBased));    // do
Console.WriteLine(Solfege.MovableDo(Note.Parse("C"), aMinor, MinorSolfege.DoBased));    // me
```

## Reading names back

`TryParseMovableDo` is the inverse, in the same key and convention. It never throws.

```csharp
Console.WriteLine(Solfege.TryParseMovableDo("fi", g, out var note));     // True
Console.WriteLine(note);                                                 // C#4
Console.WriteLine(Solfege.TryParseMovableDo("xx", g, out _));            // False
```
