---
title: Measures and tempo
description: Exact measures, ties, beat groups and beaming, tempo maps, swing, and what replaced the old Duration arithmetic.
section: Topics
order: 12
---

[Time signatures](time-signatures.md) covers single values. This page is about structure over time: bars that
hold events, ties across barlines, which notes beam together, and how positions become seconds. Everything is
an exact `Rational` of whole notes, so a bar is full or it is not, with no rounding in between.

## Measures

A `Measure` is a time signature and the events inside it. `Capacity` is the bar length, `Filled` is what the
events add up to, and `Remaining` is the gap. A 4/4 bar holds one whole note, so three quarters fill 3/4 and
leave 1/4:

```csharp
var common = TimeSignature.CommonTime;
var quarter = new Duration(DurationType.Quarter);
var c4 = Note.Parse("C4");

var bar = new Measure(common, [new RhythmEvent([c4], quarter), new RhythmEvent([c4], quarter), new RhythmEvent([c4], quarter)]);

Console.WriteLine(bar.Capacity);      // 1
Console.WriteLine(bar.Filled);        // 3/4
Console.WriteLine(bar.Remaining);     // 1/4
Console.WriteLine(bar.IsComplete);    // False
```

`Measure.Split` bars a whole stream of events for you. Three half notes are 3/2 of a whole note, so they make
one full 4/4 bar and a half bar:

```csharp
var half = new Duration(DurationType.Half);
var bars = Measure.Split(common, [new RhythmEvent([c4], half), new RhythmEvent([c4], half), new RhythmEvent([c4], half)]);

Console.WriteLine(bars.Count);                                      // 2
Console.WriteLine(bars[0].IsComplete + " " + bars[1].IsComplete);   // True False
Console.WriteLine(bars[1].Filled);                                  // 1/2
```

A pickup (anacrusis) is the capacity of the first bar. With a quarter-note pickup, the quarter fills bar one, a
whole note then fills bar two exactly, and the last quarter starts bar three:

```csharp
var whole = new Duration(DurationType.Whole);
var withPickup = Measure.Split(common, [new RhythmEvent([c4], quarter), new RhythmEvent([c4], whole), new RhythmEvent([c4], quarter)], pickup: Rational.Of(1, 4));

Console.WriteLine(withPickup.Count);                                       // 3
Console.WriteLine(string.Join(" ", withPickup.Select(m => m.Capacity)));   // 1/4 1 1
Console.WriteLine(withPickup[2].IsComplete);                               // False
```

## Ties across barlines

An event that crosses a barline is cut there into notatable parts, and the parts are tied when the event has
pitches. A whole note in 3/4 is a dotted half (3/4) plus a quarter (1/4), tied:

```csharp
var waltz = new TimeSignature(3, 4);
var tied = Measure.Split(waltz, [new RhythmEvent([c4], whole)]);

Console.WriteLine(tied.Count);                          // 2
Console.WriteLine(tied[0].Events[0].Duration);          // dotted half
Console.WriteLine(tied[0].Events[0].TiedToNext);        // True
Console.WriteLine(tied[1].Events[0].Duration);          // quarter
Console.WriteLine(tied[1].Events[0].TiedToNext);        // False
```

Rests are cut the same way but never tied.

## Beat groups and beaming

A time signature knows how its bar divides into beats. By default 7/8 has seven separate eighth beats; give
the grouping and it becomes 2+2+3, each group an exact length in whole notes (2/8 = 1/4, 3/8):

```csharp
var sevenEight = new TimeSignature(7, 8, [2, 2, 3]);

Console.WriteLine(string.Join(" + ", sevenEight.BeatGroups));   // 1/4 + 1/4 + 3/8
```

`Beaming.Group` returns the indexes of the events beamed together. A beam never crosses a beat group, so seven
eighths in 7/8 beam as 2, 2 and 3:

```csharp
var eighth = new Duration(DurationType.Eighth);
var sevenEighths = Enumerable.Range(0, 7).Select(_ => new RhythmEvent([c4], eighth)).ToList();
var oddBar = new Measure(sevenEight, sevenEighths);

Console.WriteLine(string.Join(" | ", Beaming.Group(oddBar).Select(g => string.Join(",", g))));   // 0,1 | 2,3 | 4,5,6
```

Only pitched events shorter than a quarter beam, and a rest breaks the beam.

## Tempo maps

A `TempoMap` starts at a tempo in beats per minute (the beat is a quarter unless you pass a beat unit) and
`WithChange` adds a new tempo at a position in whole notes. At 120 bpm a quarter lasts 0.5 s, so the first
whole note (four quarters) takes 2 s. At 60 bpm a quarter lasts 1 s, so the second whole note takes 4 s and
position 2 is reached at 2 + 4 = 6 s:

```csharp
var tempo = new TempoMap(120).WithChange(Rational.Of(1), 60);

Console.WriteLine(tempo.BpmAt(Rational.Of(1, 2)));     // 120
Console.WriteLine(tempo.BpmAt(Rational.Of(3, 2)));     // 60
Console.WriteLine(tempo.SecondsAt(Rational.Of(1)));    // 2
Console.WriteLine(tempo.SecondsAt(Rational.Of(2)));    // 6
Console.WriteLine(tempo.PositionAt(6));                // 2
```

`PositionAt` is the inverse of `SecondsAt`: it takes seconds and returns the position in whole notes.

## Swing

`Swing.Apply` maps a straight position to its performed position. Pairs of the subdivision are swung so that
the first half takes `ratio` of the pair. With eighth notes and a 2:1 ratio (2/3) the pair spans 1/4, the
first eighth lasts 2/3 of it, and the off-beat eighth at 1/8 lands at 2/3 x 1/4 = 1/6. On-beat positions do
not move:

```csharp
var eighthLength = Rational.Of(1, 8);

Console.WriteLine(Math.Round(Swing.Apply(Rational.Of(1, 8), eighthLength, 2.0 / 3), 4));   // 0.1667
Console.WriteLine(Swing.Apply(Rational.Of(1, 4), eighthLength, 2.0 / 3));                   // 0.25
Console.WriteLine(Swing.Apply(Rational.Of(1, 8), eighthLength, 0.5));                       // 0.125
```

A ratio of 0.5 is straight.

## Moving from the old Duration API

`Duration` and `TimeSignature` now use an exact `Rational`, and three members were removed:

| Removed | Use instead |
|---|---|
| `Duration.Add` | add the `Value`s: `a.Value + b.Value` |
| `Duration.Divide` | divide the `Value`s: `a.Value / b.Value` |
| `CompoundDuration` | a `Rational`, turned back into notatable durations by `Duration.Decompose` |

```csharp
var sum = new Duration(DurationType.Half).Value + new Duration(DurationType.Eighth).Value;
var ratio = new Duration(DurationType.Half).Value / new Duration(DurationType.Quarter).Value;

Console.WriteLine(sum);                                          // 5/8
Console.WriteLine(ratio);                                        // 2
Console.WriteLine(string.Join(" + ", Duration.Decompose(sum)));  // half + eighth
```
