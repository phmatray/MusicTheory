# Chord API Reference

Complete API documentation for the `Chord` class.

## Class Definition

```C#
namespace MusicTheory;

public sealed class Chord : IEquatable<Chord>
{
    public Note Root { get; }
    public ChordType Type { get; }
    public ChordQuality Quality { get; }
}
```

## Constructors

### Chord(Note, ChordType)

Creates a new chord with the specified root note and chord type.

```C#
public Chord(Note root, ChordType type)
```

**Parameters:**
- `root`: The root note of the chord
- `type`: The chord type (Major, Minor7, Dominant13, etc.)

**Example:**
```C#
var cMajor = new Chord(new Note(NoteName.C, 4), ChordType.Major);
var dm7 = new Chord(new Note(NoteName.D, 4), ChordType.Minor7);
var g13 = new Chord(new Note(NoteName.G, 4), ChordType.Dominant13);
```

### Chord(Note, ChordQuality) [Legacy]

Creates a new chord with basic quality (for backward compatibility).

```C#
public Chord(Note root, ChordQuality quality)
```

**Parameters:**
- `root`: The root note of the chord
- `quality`: The basic chord quality (Major, Minor, Diminished, Augmented)

**Note:** This constructor maps to basic triad types. Use the ChordType constructor for extended chords.

**Example:**
```C#
var cMajor = new Chord(new Note(NoteName.C, 4), ChordQuality.Major);
```

## Properties

### Root

```C#
public Note Root { get; }
```

Gets the root note of the chord.

### Type

```C#
public ChordType Type { get; }
```

Gets the chord type (40+ types available).

### Quality

```C#
public ChordQuality Quality { get; }
```

Gets the basic quality (Major, Minor, Diminished, Augmented). Derived from Type.

## Instance Methods

### GetNotes()

Gets all notes in the chord.

```C#
public IEnumerable<Note> GetNotes()
```

**Returns:** The chord tones in ascending order

**Example:**
```C#
var cMaj7 = new Chord(new Note(NoteName.C, 4), ChordType.Major7);
var notes = cMaj7.GetNotes().ToList();
// [C4, E4, G4, B4]
```

### GetSymbol()

Gets the chord symbol notation.

```C#
public string GetSymbol()
```

**Returns:** Standard chord symbol (e.g., "C", "Dm7", "G13", "F#maj7")

**Examples:**
- Major → "C"
- Minor → "Cm"
- Major7 → "Cmaj7"
- Dominant7 → "C7"
- Minor7Flat5 → "Cm7b5"
- Diminished7 → "C°7"

### Transpose(Interval, Direction?)

Transposes the chord by the specified interval.

```C#
public Chord Transpose(Interval interval, Direction direction = Direction.Up)
```

**Parameters:**
- `interval`: The interval to transpose by
- `direction`: The direction (Up or Down), default is Up

**Returns:** A new Chord instance with transposed root

**Example:**
```C#
var c7 = new Chord(new Note(NoteName.C, 4), ChordType.Dominant7);
var f7 = c7.Transpose(new Interval(IntervalQuality.Perfect, 4));
```

### TransposeBySemitones(int)

Transposes the chord by a number of semitones.

```C#
public Chord TransposeBySemitones(int semitones)
```

**Parameters:**
- `semitones`: Number of semitones (positive = up, negative = down)

**Returns:** A new Chord instance

### GetEnharmonicEquivalent()

Gets the enharmonic equivalent of this chord.

```C#
public Chord? GetEnharmonicEquivalent()
```

**Returns:** The enharmonic equivalent, or null if root has no enharmonic

**Example:**
```C#
var cSharpMajor = new Chord(new Note(NoteName.C, Alteration.Sharp, 4), ChordType.Major);
var dFlatMajor = cSharpMajor.GetEnharmonicEquivalent(); // Db major
```

### AddExtension(int, IntervalQuality) [Legacy]

Adds an extension to the chord (for backward compatibility).

```C#
public Chord AddExtension(int degree, IntervalQuality quality)
```

**Note:** This method is maintained for backward compatibility. Prefer using specific ChordType values.

### ToString()

Returns a string representation of the chord.

```C#
public override string ToString()
```

**Format:** Same as GetSymbol()

### Equals(Chord?)

Determines if this chord equals another chord.

```C#
public bool Equals(Chord? other)
```

**Note:** Compares root note and chord type.

### GetHashCode()

Returns the hash code for the chord.

```C#
public override int GetHashCode()
```

## Static Methods

### GetIntervalsForChordType(ChordType)

Gets the intervals that define a chord type.

```C#
public static List<Interval> GetIntervalsForChordType(ChordType type)
```

**Parameters:**
- `type`: The chord type

**Returns:** List of intervals from the root

**Example:**
```C#
var major7Intervals = Chord.GetIntervalsForChordType(ChordType.Major7);
// [Unison, Major 3rd, Perfect 5th, Major 7th]
```

## Operators

### Equality Operators

```C#
public static bool operator ==(Chord? left, Chord? right)
public static bool operator !=(Chord? left, Chord? right)
```

Compare chords by root and type.

## ChordType Enumeration

The library supports 40+ chord types:

### Triads
- Major
- Minor
- Diminished
- Augmented

### Seventh Chords
- Major7
- Minor7
- Dominant7
- MinorMajor7
- HalfDiminished7 (Minor7Flat5)
- Diminished7
- Augmented7
- AugmentedMajor7

### Extended Chords
- Major9, Minor9, Dominant9
- Major11, Minor11, Dominant11
- Major13, Minor13, Dominant13

### Altered Chords
- Dominant7Flat5
- Dominant7Sharp5
- Dominant7Flat9
- Dominant7Sharp9
- Dominant7Sharp11
- Dominant7Flat13

### Suspended Chords
- Sus2
- Sus4
- Dominant7Sus2
- Dominant7Sus4

### Add Chords
- Add9, MinorAdd9
- Add11, MinorAdd11
- Add13, MinorAdd13

### Other Types
- Power (root + fifth)
- Major6, Minor6
- Dominant6
- MinorMajor9

## Common Patterns

### Creating Common Chords

```C#
// Basic triads
var c = new Chord(new Note(NoteName.C, 4), ChordType.Major);
var dm = new Chord(new Note(NoteName.D, 4), ChordType.Minor);
var bdim = new Chord(new Note(NoteName.B, 4), ChordType.Diminished);

// Seventh chords
var cmaj7 = new Chord(new Note(NoteName.C, 4), ChordType.Major7);
var dm7 = new Chord(new Note(NoteName.D, 4), ChordType.Minor7);
var g7 = new Chord(new Note(NoteName.G, 4), ChordType.Dominant7);

// Extended chords
var cmaj9 = new Chord(new Note(NoteName.C, 4), ChordType.Major9);
var dm11 = new Chord(new Note(NoteName.D, 4), ChordType.Minor11);
var g13 = new Chord(new Note(NoteName.G, 4), ChordType.Dominant13);
```

### Working with Progressions

```C#
// ii-V-I progression
var progression = new[]
{
    new Chord(new Note(NoteName.D, 4), ChordType.Minor7),
    new Chord(new Note(NoteName.G, 4), ChordType.Dominant7),
    new Chord(new Note(NoteName.C, 4), ChordType.Major7)
};

// Transpose to different keys
var interval = new Interval(IntervalQuality.Perfect, 4); // Up a 4th
var transposed = progression
    .Select(chord => chord.Transpose(interval))
    .ToArray();
// Gm7 - C7 - Fmaj7
```

### Chord Analysis

```C#
public class ChordAnalyzer
{
    public static string AnalyzeChord(Chord chord)
    {
        var notes = chord.GetNotes().ToList();
        var intervals = notes.Skip(1)
            .Select(note => Interval.Between(chord.Root, note))
            .ToList();
        
        var analysis = new StringBuilder();
        analysis.AppendLine($"Chord: {chord.GetSymbol()}");
        analysis.AppendLine($"Root: {chord.Root}");
        analysis.AppendLine($"Notes: {string.Join(", ", notes)}");
        analysis.AppendLine($"Intervals: {string.Join(", ", intervals)}");
        
        return analysis.ToString();
    }
}
```

### Building Custom Chord Collections

```C#
// All diatonic seventh chords in C major
var cMajorSevenths = new[]
{
    new Chord(new Note(NoteName.C, 4), ChordType.Major7),      // Imaj7
    new Chord(new Note(NoteName.D, 4), ChordType.Minor7),      // ii7
    new Chord(new Note(NoteName.E, 4), ChordType.Minor7),      // iii7
    new Chord(new Note(NoteName.F, 4), ChordType.Major7),      // IVmaj7
    new Chord(new Note(NoteName.G, 4), ChordType.Dominant7),   // V7
    new Chord(new Note(NoteName.A, 4), ChordType.Minor7),      // vi7
    new Chord(new Note(NoteName.B, 4), ChordType.HalfDiminished7) // viiø7
};
```

## Special Chord Types

### Power Chords

```C#
var powerChord = new Chord(new Note(NoteName.E, 3), ChordType.Power);
// Contains only root and fifth: E3, B3
```

### Sixth Chords

```C#
var c6 = new Chord(new Note(NoteName.C, 4), ChordType.Major6);
// C, E, G, A

var cm6 = new Chord(new Note(NoteName.C, 4), ChordType.Minor6);
// C, Eb, G, A
```

### Altered Dominants

```C#
var alteredDominant = new Chord(new Note(NoteName.G, 4), ChordType.Dominant7Sharp5);
// G, B, D#, F

var jazzDominant = new Chord(new Note(NoteName.G, 4), ChordType.Dominant7Sharp11);
// G, B, D, F, C#
```

## Performance Notes

- Chord note generation uses cached interval lookups
- Symbol generation is optimized with switch expressions
- GetNotes() yields notes lazily when possible
- Transposition creates new instances (immutable)

## Thread Safety

The `Chord` class is immutable and thread-safe. Multiple threads can safely:
- Read all properties
- Call all methods
- Share Chord instances

No synchronization is needed.

## Validation

- Root note must not be null
- ChordType must be a valid enum value
- Invalid combinations are prevented at compile time through the type system

## See Also

<seealso>
    <category ref="related">
        <a href="chords.md">Chords Concept Guide</a>
        <a href="api-note.md">Note API Reference</a>
        <a href="enums.md">ChordType Enum</a>
        <a href="chord-progressions.md">Chord Progressions Guide</a>
    </category>
</seealso>