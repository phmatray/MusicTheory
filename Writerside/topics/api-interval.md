# Interval API Reference

Complete API documentation for the `Interval` class.

## Class Definition

```C#
namespace MusicTheory;

public sealed class Interval : IEquatable<Interval>
{
    public IntervalQuality Quality { get; }
    public int Number { get; }
    public int Semitones { get; }
}
```

## Constructors

### Interval(IntervalQuality, int)

Creates a new interval with the specified quality and numeric value.

```C#
public Interval(IntervalQuality quality, int number)
```

**Parameters:**
- `quality`: The interval quality (Diminished, Minor, Major, Perfect, Augmented)
- `number`: The interval number (1-15 typically)

**Exceptions:**
- `ArgumentException`: If the quality is invalid for the given interval number
- `ArgumentOutOfRangeException`: If number is less than 1

**Validation Rules:**
- Perfect quality is only valid for unison (1), 4th, 5th, and octave (8)
- Major/Minor quality is only valid for 2nd, 3rd, 6th, and 7th
- Diminished/Augmented can be used with any interval

**Examples:**
```C#
var majorThird = new Interval(IntervalQuality.Major, 3);
var perfectFifth = new Interval(IntervalQuality.Perfect, 5);
var minorSeventh = new Interval(IntervalQuality.Minor, 7);
```

## Properties

### Quality

```C#
public IntervalQuality Quality { get; }
```

Gets the quality of the interval.

### Number

```C#
public int Number { get; }
```

Gets the numeric value of the interval (1 = unison, 2 = second, etc.).

### Semitones

```C#
public int Semitones { get; }
```

Gets the number of semitones (half steps) in the interval.

**Calculation:**
The semitone count is calculated based on:
- The diatonic interval (based on the number)
- The chromatic adjustment (based on the quality)

**Examples:**
- Major 3rd = 4 semitones
- Perfect 5th = 7 semitones
- Minor 7th = 10 semitones

## Instance Methods

### IsEnharmonicWith(Interval)

Determines if this interval is enharmonically equivalent to another.

```C#
public bool IsEnharmonicWith(Interval other)
```

**Parameters:**
- `other`: The interval to compare with

**Returns:** True if both intervals span the same number of semitones

**Example:**
```C#
var augFourth = new Interval(IntervalQuality.Augmented, 4);
var dimFifth = new Interval(IntervalQuality.Diminished, 5);
bool same = augFourth.IsEnharmonicWith(dimFifth); // true (both 6 semitones)
```

### Invert()

Inverts the interval.

```C#
public Interval Invert()
```

**Returns:** The inverted interval

**Inversion Rules:**
- Number: 9 - original number (for simple intervals)
- Quality:
  - Major ↔ Minor
  - Augmented ↔ Diminished
  - Perfect remains Perfect

**Examples:**
```C#
var major3rd = new Interval(IntervalQuality.Major, 3);
var minor6th = major3rd.Invert(); // Minor 6th

var perfect5th = new Interval(IntervalQuality.Perfect, 5);
var perfect4th = perfect5th.Invert(); // Perfect 4th
```

### ToString()

Returns a string representation of the interval.

```C#
public override string ToString()
```

**Format:** `"{Quality} {Number}{Suffix}"`

**Examples:**
- Perfect Unison → "Perfect Unison"
- Major 3rd → "Major 3rd"
- Perfect 5th → "Perfect 5th"
- Minor 7th → "Minor 7th"
- Augmented 4th → "Augmented 4th"

### Equals(Interval?)

Determines if this interval equals another interval.

```C#
public bool Equals(Interval? other)
```

**Note:** Compares by quality and number, not by semitones.

### GetHashCode()

Returns the hash code for the interval.

```C#
public override int GetHashCode()
```

## Static Methods

### Between(Note, Note)

Calculates the interval between two notes.

```C#
public static Interval Between(Note lower, Note higher)
```

**Parameters:**
- `lower`: The lower note
- `higher`: The higher note

**Returns:** The interval from lower to higher

**Algorithm:**
1. Calculate the numeric interval based on note names
2. Calculate the exact semitone distance
3. Determine the appropriate quality

**Example:**
```C#
var c = new Note(NoteName.C, 4);
var e = new Note(NoteName.E, 4);
var g = new Note(NoteName.G, 4);

var third = Interval.Between(c, e);  // Major 3rd
var fifth = Interval.Between(c, g);  // Perfect 5th
```

### FromSemitones(int)

Creates an interval from a semitone count.

```C#
public static Interval FromSemitones(int semitones)
```

**Parameters:**
- `semitones`: Number of semitones

**Returns:** An interval (prefers standard spellings)

**Note:** This method returns the most common spelling for the given semitone count.

**Example:**
```C#
var majorThird = Interval.FromSemitones(4);   // Major 3rd
var tritone = Interval.FromSemitones(6);      // Augmented 4th
```

## Operators

### Equality Operators

```C#
public static bool operator ==(Interval? left, Interval? right)
public static bool operator !=(Interval? left, Interval? right)
```

Compare intervals by quality and number.

**Example:**
```C#
var int1 = new Interval(IntervalQuality.Major, 3);
var int2 = new Interval(IntervalQuality.Major, 3);
var int3 = new Interval(IntervalQuality.Minor, 3);

bool equal1 = int1 == int2;  // true
bool equal2 = int1 == int3;  // false
```

## Interval Reference Tables

### Simple Intervals (Within an Octave)

| Number | Perfect | Diminished | Minor | Major | Augmented |
|--------|---------|------------|-------|-------|-----------|
| 1 | 0 | -1 | - | - | 1 |
| 2 | - | 0 | 1 | 2 | 3 |
| 3 | - | 2 | 3 | 4 | 5 |
| 4 | 5 | 4 | - | - | 6 |
| 5 | 7 | 6 | - | - | 8 |
| 6 | - | 7 | 8 | 9 | 10 |
| 7 | - | 9 | 10 | 11 | 12 |
| 8 | 12 | 11 | - | - | 13 |

### Compound Intervals (Beyond an Octave)

| Number | Common Name | Semitones | Simple Equivalent |
|--------|-------------|-----------|------------------|
| 9 | Ninth | 14 (Major) | 2nd |
| 10 | Tenth | 16 (Major) | 3rd |
| 11 | Eleventh | 17 (Perfect) | 4th |
| 12 | Twelfth | 19 (Perfect) | 5th |
| 13 | Thirteenth | 21 (Major) | 6th |
| 14 | Fourteenth | 23 (Major) | 7th |
| 15 | Fifteenth | 24 (Perfect) | Octave |

## Common Patterns

### Creating Common Intervals

```C#
// Perfect intervals
var unison = new Interval(IntervalQuality.Perfect, 1);
var fourth = new Interval(IntervalQuality.Perfect, 4);
var fifth = new Interval(IntervalQuality.Perfect, 5);
var octave = new Interval(IntervalQuality.Perfect, 8);

// Major/Minor intervals
var majorSecond = new Interval(IntervalQuality.Major, 2);
var minorThird = new Interval(IntervalQuality.Minor, 3);
var majorSixth = new Interval(IntervalQuality.Major, 6);
var minorSeventh = new Interval(IntervalQuality.Minor, 7);

// Altered intervals
var augmentedFourth = new Interval(IntervalQuality.Augmented, 4);
var diminishedFifth = new Interval(IntervalQuality.Diminished, 5);
```

### Working with Interval Collections

```C#
// Major scale intervals from root
var majorScaleIntervals = new[]
{
    new Interval(IntervalQuality.Perfect, 1),    // Root
    new Interval(IntervalQuality.Major, 2),      // Major 2nd
    new Interval(IntervalQuality.Major, 3),      // Major 3rd
    new Interval(IntervalQuality.Perfect, 4),    // Perfect 4th
    new Interval(IntervalQuality.Perfect, 5),    // Perfect 5th
    new Interval(IntervalQuality.Major, 6),      // Major 6th
    new Interval(IntervalQuality.Major, 7),      // Major 7th
    new Interval(IntervalQuality.Perfect, 8)     // Octave
};

// Build scale from intervals
var root = new Note(NoteName.C, 4);
var scaleNotes = majorScaleIntervals
    .Select(interval => root.Transpose(interval))
    .ToList();
```

### Interval Analysis

```C#
public class IntervalAnalyzer
{
    public static string DescribeInterval(Interval interval)
    {
        var consonance = interval.Semitones switch
        {
            0 or 12 => "Perfect Consonance",
            5 or 7 => "Perfect Consonance",
            3 or 4 or 8 or 9 => "Imperfect Consonance",
            1 or 2 or 6 or 10 or 11 => "Dissonance",
            _ => "Unknown"
        };
        
        return $"{interval} ({interval.Semitones} semitones) - {consonance}";
    }
}
```

## Error Handling

### Invalid Quality for Number

```C#
try
{
    // Invalid: can't have a major 5th
    var invalid = new Interval(IntervalQuality.Major, 5);
}
catch (ArgumentException ex)
{
    // "Major quality is not valid for interval number 5"
}
```

### Interval Calculation Edge Cases

```C#
// Handling enharmonic notes
var c = new Note(NoteName.C, 4);
var bSharp = new Note(NoteName.B, Alteration.Sharp, 3);

var interval = Interval.Between(bSharp, c); // Diminished 2nd (0 semitones)
```

## Performance Notes

- Semitone calculation is performed once in constructor
- Interval comparison uses cached semitone values
- String representation is generated on demand
- Static factory methods use lookup tables for efficiency

## Thread Safety

The `Interval` class is immutable and thread-safe. Multiple threads can safely:
- Read all properties
- Call all methods
- Share Interval instances

No synchronization is needed.

## See Also

<seealso>
    <category ref="related">
        <a href="intervals.md">Intervals Concept Guide</a>
        <a href="api-note.md">Note API Reference</a>
        <a href="enums.md">IntervalQuality Enum</a>
    </category>
</seealso>