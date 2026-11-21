---
title: Notes
description: A Note is a spelled pitch, with a letter, an accidental and an octave in scientific pitch notation.
section: Concepts
order: 1
---

A `Note` is a spelled pitch: a letter (`NoteName`), an accidental (`Alteration`) and an octave in scientific
pitch notation, where middle C is C4. It is an immutable value: two notes are equal when all three match.

## Creating notes

```csharp
var c4 = new Note(NoteName.C);                            // natural, octave 4
var bFlat3 = new Note(NoteName.B, Alteration.Flat, 3);
var fDoubleSharp = new Note(NoteName.F, Alteration.DoubleSharp, 5);

Console.WriteLine(c4);              // C4
Console.WriteLine(bFlat3);          // Bb3
Console.WriteLine(fDoubleSharp);    // F##5
```

Octaves run from -1 to 9, which covers the whole MIDI range. Anything outside, or an undefined letter or
accidental, is refused at construction:

```csharp
try
{
    new Note(NoteName.C, Alteration.Natural, 10);
}
catch (ArgumentOutOfRangeException e)
{
    Console.WriteLine(e.ParamName);  // octave
}
```

## Parsing

`Note.Parse` reads a letter in any case, up to two accidentals in ASCII (`#`, `b`) or Unicode (♯ ♭ 𝄪 𝄫),
and an optional octave. Without an octave, the note is in octave 4.

```csharp
Console.WriteLine(Note.Parse("C#4"));   // C#4
Console.WriteLine(Note.Parse("bb3"));   // Bb3
Console.WriteLine(Note.Parse("E♭"));    // Eb4
Console.WriteLine(Note.Parse("G𝄪-1"));  // G##-1

Console.WriteLine(Note.TryParse("X4", out var note));   // False
```

German names are available as an option: H is B, B is B♭, and accidentals are the suffixes -is and -es.

```csharp
Console.WriteLine(Note.Parse("H4", NoteNotation.German));    // B4
Console.WriteLine(Note.Parse("B", NoteNotation.German));     // Bb4
Console.WriteLine(Note.Parse("Fis3", NoteNotation.German));  // F#3
Console.WriteLine(Note.Parse("Es", NoteNotation.German));    // Eb4
```

Every `Parse` in the library throws `FormatException` on bad input, and every `TryParse` returns false
instead of throwing.

## Equality

```csharp
var a = Note.Parse("C#4");
var b = new Note(NoteName.C, Alteration.Sharp, 4);
var flat = Note.Parse("Db4");

Console.WriteLine(a == b);                           // True
Console.WriteLine(a == flat);                        // False
Console.WriteLine(a.IsEnharmonicWith(flat));         // True
Console.WriteLine(new HashSet<Note> { a, b }.Count); // 1
```

`IsEnharmonicWith` compares the sounding pitch, octave included, as music21 does: B♯3 is enharmonic with
C4, and C♯3 is not enharmonic with D♭5. To compare without octaves, compare `PitchClass`.

```csharp
Console.WriteLine(Note.Parse("B#3").IsEnharmonicWith(Note.Parse("C4")));   // True
Console.WriteLine(Note.Parse("C#3").IsEnharmonicWith(Note.Parse("Db5")));  // False
Console.WriteLine(Note.Parse("C#3").PitchClass == Note.Parse("Db5").PitchClass);  // True
```

## Pitch, MIDI and frequency

```csharp
var a4 = Note.Parse("A4");
Console.WriteLine(a4.PitchClass);                  // 9
Console.WriteLine(a4.MidiNumber);                  // 69
Console.WriteLine(Note.FromMidiNumber(61));        // C#4
Console.WriteLine(Note.FromMidiNumber(61, preferFlats: true));  // Db4
```

Frequencies use twelve-tone equal temperament. `Frequency` is at concert pitch, A4 = 440 Hz;
`GetFrequency` takes any other reference.

```csharp
Console.WriteLine(Note.Parse("A4").Frequency);                     // 440
Console.WriteLine(Note.Parse("A4").GetFrequency(415));             // 415
Console.WriteLine(Math.Round(Note.Parse("C4").Frequency, 2));      // 261.63
Console.WriteLine(Math.Round(Note.Parse("C4").GetFrequency(442), 2));  // 262.81
```

## Transposing

`Transpose` moves the letter by the interval number, so the spelling comes out right:

```csharp
var c = Note.Parse("C4");
Console.WriteLine(c.Transpose(Interval.Parse("A4")));    // F#4
Console.WriteLine(c.Transpose(Interval.Parse("d5")));    // Gb4
Console.WriteLine(c.Transpose(Interval.Parse("M-3")));   // Ab3
Console.WriteLine(c.Transpose(Interval.Parse("M9")));    // D5
```

A result that would need three accidentals is refused rather than silently respelled:

```csharp
try
{
    Note.Parse("B##4").Transpose(Interval.Parse("M2"));
}
catch (InvalidOperationException e)
{
    Console.WriteLine(e.Message);  // B##4 transposed by M2 needs 3 sharps on C; at most two are supported.
}
```

`TransposeBySemitones` moves by a number of semitones. Without a key it spells black keys with sharps;
with a key it follows the key.

```csharp
Console.WriteLine(Note.Parse("A4").TransposeBySemitones(1));                          // A#4
Console.WriteLine(Note.Parse("A4").TransposeBySemitones(1, KeySignature.Parse("Eb"))); // Bb4
```

## Respelling

```csharp
var middleC = Note.Parse("C4");
Console.WriteLine(middleC.Respell(NoteName.B));    // B#3
Console.WriteLine(middleC.Respell(NoteName.D));    // Dbb4
Console.WriteLine(middleC.Respell(NoteName.E) is null);  // True

Console.WriteLine(Note.Parse("C#4").GetEnharmonicEquivalent());   // Db4
Console.WriteLine(Note.Parse("E##4").SimplifyEnharmonic());       // F#4
Console.WriteLine(string.Join(" ", middleC.GetAllEnharmonicEquivalents()));  // Dbb4 B#3
```

## Names

```csharp
var note = Note.Parse("Bb3");
Console.WriteLine(note.GetName());                          // Bb
Console.WriteLine(note.GetName(AccidentalStyle.Unicode));   // B♭
```
