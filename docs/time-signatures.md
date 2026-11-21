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
Console.WriteLine(quarter.Value);                    // 1/4
Console.WriteLine(dottedHalf.Value);                 // 3/4
Console.WriteLine(triplet.Value);                    // 1/12
```

`Value` is an exact `Rational` in whole notes, so sums never drift. Three triplet eighths are exactly a quarter:

```csharp
Console.WriteLine(triplet.Value * Rational.Of(3) == quarter.Value);   // True
```

Each dot adds half of the previous value:

```csharp
Console.WriteLine(new Duration(DurationType.Quarter, 2).Value);   // 7/16
Console.WriteLine(new Duration(DurationType.Quarter, 2));                          // double dotted quarter
```

## Tempo

`GetTimeInSeconds` takes a tempo in beats per minute; the beat is a quarter note unless you pass another duration:

```csharp
Console.WriteLine(new Duration(DurationType.Quarter).GetTimeInSeconds(120));   // 0.5
Console.WriteLine(new Duration(DurationType.Whole).GetTimeInSeconds(60));      // 4
Console.WriteLine(new Duration(DurationType.Half, 1).GetTimeInSeconds(60, new Duration(DurationType.Quarter, 1)));   // 2
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

A measure is `MeasureLength` long, an exact `Rational` of whole notes. Divide a duration's `Value` by it to get measures:

```csharp
Console.WriteLine(new Duration(DurationType.Half).Value / TimeSignature.CommonTime.MeasureLength);   // 1/2
Console.WriteLine(new Duration(DurationType.Whole).Value / waltz.MeasureLength);                     // 4/3
```

Durations are not added. Add their `Value`s, then turn the `Rational` back into notatable durations:

```csharp
var total = new Duration(DurationType.Half).Value + new Duration(DurationType.Quarter).Value;
Console.WriteLine(total);                                               // 3/4
Console.WriteLine(string.Join(" + ", Duration.Decompose(Rational.Of(5, 8))));   // half + eighth
Console.WriteLine(Duration.TryFromValue(total, out var d) ? d : null);  // dotted half
```
