---
title: Intervals
description: An Interval is a spelled, directed distance between two notes, with a quality, a number and a direction.
section: Concepts
order: 2
---

An `Interval` has three parts:

- **Quality**: perfect, major, minor, or a degree of augmented or diminished.
- **Number**: 1 for a unison, 2 for a second, up to 8 for an octave; 9 and above are compound.
- **Direction**: up or down the staff. A unison has none.

A major third and a diminished fourth both span four semitones, but they are different intervals because
they cover a different number of letters. The library always keeps that distinction.

## Names

Intervals use music21's short names: `P` perfect, `M` major, `m` minor, `d` diminished, `A` augmented,
with the letter repeated for each further degree, and a `-` before the number when the interval descends.

```csharp
Console.WriteLine(Interval.Parse("M3").Semitones);     // 4
Console.WriteLine(Interval.Parse("d4").Semitones);     // 4
Console.WriteLine(Interval.Parse("AA4").Semitones);    // 7
Console.WriteLine(Interval.Parse("dd2").Semitones);    // -1
Console.WriteLine(Interval.Parse("P-5"));              // P-5
Console.WriteLine(Interval.Parse("P-5").Name);         // P5
```

The same intervals can be built from their parts:

```csharp
var majorThird = new Interval(IntervalQuality.Major, 3);
var fifthDown = new Interval(IntervalQuality.Perfect, 5, Direction.Down);
var doublyAugmentedFourth = new Interval(IntervalQuality.DoublyAugmented, 4);

Console.WriteLine(majorThird);              // M3
Console.WriteLine(fifthDown);               // P-5
Console.WriteLine(doublyAugmentedFourth);   // AA4
```

Perfect applies to unisons, fourths, fifths, octaves and their compounds; major and minor to seconds,
thirds, sixths and sevenths. Any other combination is refused:

```csharp
Console.WriteLine(Interval.TryParse("P3", out _));   // False
Console.WriteLine(Interval.TryParse("M5", out _));   // False
```

## Measuring

`Interval.Between` counts letters for the number and follows the staff for the direction:

```csharp
Interval Between(string from, string to) => Interval.Between(Note.Parse(from), Note.Parse(to));

Console.WriteLine(Between("C4", "G4"));     // P5
Console.WriteLine(Between("C4", "F#4"));    // A4
Console.WriteLine(Between("C4", "Gb4"));    // d5
Console.WriteLine(Between("C4", "F##4"));   // AA4
Console.WriteLine(Between("E4", "C4"));     // M-3
Console.WriteLine(Between("C4", "B#3"));    // d-2
Console.WriteLine(Between("C4", "D5"));     // M9
Console.WriteLine(Between("C4", "Cb4"));    // d1
```

It never throws for two valid notes. With accidentals of up to two sharps or flats on each note, the
widest qualities are quintuple: F𝄫 to B𝄪 is a quintuply augmented fourth.

```csharp
Console.WriteLine(Between("Fbb4", "B##4"));   // AAAAA4
Console.WriteLine(Between("B##4", "Fbb5"));   // ddddd5
```

All 3,669 spelled intervals between notes in octaves 3 to 5 are checked against music21 in the tests.

## Size and direction

```csharp
var down = Interval.Parse("M-3");
Console.WriteLine(down.Semitones);          // 4
Console.WriteLine(down.DirectedSemitones);  // -4
Console.WriteLine(down.Direction);          // Down
Console.WriteLine(Interval.Parse("M10").IsCompound);   // True
```

## Inverting, reversing, reducing

```csharp
Console.WriteLine(Interval.Parse("M3").Invert());    // m6
Console.WriteLine(Interval.Parse("A4").Invert());    // d5
Console.WriteLine(Interval.Parse("M10").Invert());   // m6
Console.WriteLine(Interval.Parse("M3").Reverse());   // M-3
Console.WriteLine(Interval.Parse("A1").Reverse());   // d1
Console.WriteLine(Interval.Parse("M9").ToSimple());  // M2
```

## Enharmonic intervals

Two intervals are enharmonic when they move by the same number of semitones in the same direction:

```csharp
Console.WriteLine(Interval.Parse("A4").IsEnharmonicWith(Interval.Parse("d5")));   // True
Console.WriteLine(Interval.Parse("M3").IsEnharmonicWith(Interval.Parse("M-3")));  // False
```

## Transposing

```csharp
var note = Note.Parse("D4");
Console.WriteLine(note.Transpose(Interval.Parse("m3")));    // F4
Console.WriteLine(note.Transpose(Interval.Parse("A2")));    // E#4
Console.WriteLine(note.Transpose(Interval.Parse("P-4")));   // A3
```
