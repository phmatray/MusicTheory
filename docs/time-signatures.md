---
title: Rhythm and meter
description: Durations with dots and tuplets, time signatures, measure lengths and tempo.
section: Topics
order: 5
---

## Durations

A `Duration` is a note value (`DurationType`), with up to three dots, or a tuplet.

```csharp
var quarter = new Duration(DurationType.Quarter);
var dottedHalf = new Duration(DurationType.Half, 1);
var triplet = Duration.CreateTuplet(DurationType.Eighth, 3, 2);

Console.WriteLine(quarter);                          // quarter
Console.WriteLine(dottedHalf);                       // dotted half
Console.WriteLine(triplet);                          // 3:2 eighth
Console.WriteLine(quarter.GetValueInWholeNotes());   // 0.25
Console.WriteLine(dottedHalf.GetValueInWholeNotes()); // 0.75
Console.WriteLine(Math.Round(triplet.GetValueInWholeNotes(), 4));   // 0.0833
```

Each dot adds half of the previous value:

```csharp
Console.WriteLine(new Duration(DurationType.Quarter, 2).GetValueInWholeNotes());   // 0.4375
Console.WriteLine(new Duration(DurationType.Quarter, 2));                          // double dotted quarter
```

## Tempo

`GetTimeInSeconds` takes a tempo in quarter-note beats per minute:

```csharp
Console.WriteLine(new Duration(DurationType.Quarter).GetTimeInSeconds(120));   // 0.5
Console.WriteLine(new Duration(DurationType.Whole).GetTimeInSeconds(60));      // 4
```

## Time signatures

```csharp
var waltz = new TimeSignature(3, 4);
var jig = new TimeSignature(6, 8);
var odd = new TimeSignature(7, 8);

Console.WriteLine(waltz.GetCommonName());       // Waltz Time
Console.WriteLine(jig.IsCompound);              // True
Console.WriteLine(jig.GetBeatsPerMeasure());    // 2
Console.WriteLine(jig.GetBeatUnit());           // (3, 8)
Console.WriteLine(odd.IsComplex);               // True
Console.WriteLine(TimeSignature.CommonTime.GetSymbol());   // C
Console.WriteLine(TimeSignature.CutTime.GetSymbol());      // ¢
```

The denominator must be a power of two:

```csharp
try
{
    new TimeSignature(4, 6);
}
catch (ArgumentException e)
{
    Console.WriteLine(e.Message.Split(" (")[0]);   // Denominator must be a power of 2.
}
```

## Durations and measures

```csharp
var common = TimeSignature.CommonTime;
Console.WriteLine(new Duration(DurationType.Half).GetValueInMeasures(common));       // 0.5
Console.WriteLine(new Duration(DurationType.Whole).GetValueInMeasures(waltz));       // 1.3333333333333333
Console.WriteLine(new Duration(DurationType.Half).Divide(new Duration(DurationType.Eighth)));   // 4
```

`Add` sums two durations. The sum is a length, not a note value, so it is only equal to another sum:

```csharp
var sum = new Duration(DurationType.Half).Add(new Duration(DurationType.Quarter));
Console.WriteLine(sum.GetValueInWholeNotes());          // 0.75
Console.WriteLine(sum);                                  // 0.75 whole notes
Console.WriteLine(sum.Equals(new Duration(DurationType.Half, 1)));   // False
```
