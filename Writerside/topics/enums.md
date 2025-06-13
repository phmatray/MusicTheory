# Enumerations Reference

Complete reference for all enumerations in the MusicTheory library.

## NoteName

Represents the seven natural note names.

```C#
namespace MusicTheory;

public enum NoteName
{
    C = 0,
    D = 1,
    E = 2,
    F = 3,
    G = 4,
    A = 5,
    B = 6
}
```

**Usage:**
```C#
var note = new Note(NoteName.C, 4);
var fifth = new Note(NoteName.G, 4);
```

**Notes:**
- Values are ordered chromatically
- Used with Alteration enum to create any pitch
- Zero-based for easy arithmetic

## Alteration

Represents chromatic alterations to natural notes.

```C#
namespace MusicTheory;

public enum Alteration
{
    DoubleFlat = -2,
    Flat = -1,
    Natural = 0,
    Sharp = 1,
    DoubleSharp = 2
}
```

**Usage:**
```C#
var cSharp = new Note(NoteName.C, Alteration.Sharp, 4);
var bFlat = new Note(NoteName.B, Alteration.Flat, 3);
var fDoubleSharp = new Note(NoteName.F, Alteration.DoubleSharp, 4);
```

**Symbols:**
- DoubleFlat: ♭♭ (bb)
- Flat: ♭ (b)
- Natural: ♮
- Sharp: # (♯)
- DoubleSharp: ## (𝄪)

## IntervalQuality

Represents the quality of a musical interval.

```C#
namespace MusicTheory;

public enum IntervalQuality
{
    Diminished,
    Minor,
    Major,
    Perfect,
    Augmented
}
```

**Usage:**
```C#
var majorThird = new Interval(IntervalQuality.Major, 3);
var perfectFifth = new Interval(IntervalQuality.Perfect, 5);
var augmentedFourth = new Interval(IntervalQuality.Augmented, 4);
```

**Valid Combinations:**
- Perfect: 1, 4, 5, 8 (unison, fourth, fifth, octave)
- Major/Minor: 2, 3, 6, 7 (second, third, sixth, seventh)
- Diminished/Augmented: Any interval

## ChordQuality

Represents basic chord qualities (legacy enum).

```C#
namespace MusicTheory;

public enum ChordQuality
{
    Major,
    Minor,
    Diminished,
    Augmented
}
```

**Note:** This enum is maintained for backward compatibility. Use ChordType for new code.

## ChordType

Represents specific chord types (40+ types).

```C#
namespace MusicTheory;

public enum ChordType
{
    // Triads
    Major,
    Minor,
    Diminished,
    Augmented,
    
    // Seventh chords
    Major7,
    Minor7,
    Dominant7,
    MinorMajor7,
    HalfDiminished7,
    Diminished7,
    Augmented7,
    AugmentedMajor7,
    
    // Ninth chords
    Major9,
    Minor9,
    Dominant9,
    
    // Eleventh chords
    Major11,
    Minor11,
    Dominant11,
    
    // Thirteenth chords
    Major13,
    Minor13,
    Dominant13,
    
    // Altered dominants
    Dominant7Flat5,
    Dominant7Sharp5,
    Dominant7Flat9,
    Dominant7Sharp9,
    Dominant7Sharp11,
    Dominant7Flat13,
    
    // Suspended chords
    Sus2,
    Sus4,
    Dominant7Sus2,
    Dominant7Sus4,
    
    // Add chords
    Add9,
    MinorAdd9,
    Add11,
    MinorAdd11,
    Add13,
    MinorAdd13,
    
    // Other types
    Power,
    Major6,
    Minor6,
    Dominant6,
    MinorMajor9
}
```

**Usage Examples:**
```C#
var cMaj7 = new Chord(new Note(NoteName.C, 4), ChordType.Major7);
var dm7b5 = new Chord(new Note(NoteName.D, 4), ChordType.HalfDiminished7);
var g13 = new Chord(new Note(NoteName.G, 4), ChordType.Dominant13);
var fsus4 = new Chord(new Note(NoteName.F, 4), ChordType.Sus4);
```

**Chord Symbols:**
| ChordType | Symbol Example |
|-----------|----------------|
| Major | C |
| Minor | Cm |
| Major7 | Cmaj7 |
| Minor7 | Cm7 |
| Dominant7 | C7 |
| HalfDiminished7 | Cm7b5 |
| Diminished7 | C°7 |
| Sus2 | Csus2 |
| Add9 | Cadd9 |

## ScaleType

Represents different scale types.

```C#
namespace MusicTheory;

public enum ScaleType
{
    // Traditional scales
    Major,
    NaturalMinor,
    HarmonicMinor,
    MelodicMinor,
    
    // Modal scales
    Ionian,      // Same as Major
    Dorian,
    Phrygian,
    Lydian,
    Mixolydian,
    Aeolian,     // Same as Natural Minor
    Locrian,
    
    // Pentatonic scales
    MajorPentatonic,
    MinorPentatonic,
    
    // Other scales
    Blues,
    Chromatic,
    WholeTone,
    Diminished,  // Octatonic
    Augmented
}
```

**Usage:**
```C#
var cMajor = new Scale(new Note(NoteName.C, 4), ScaleType.Major);
var aMinor = new Scale(new Note(NoteName.A, 4), ScaleType.NaturalMinor);
var dDorian = new Scale(new Note(NoteName.D, 4), ScaleType.Dorian);
var aBlues = new Scale(new Note(NoteName.A, 4), ScaleType.Blues);
```

**Scale Note Counts:**
- Heptatonic (7 notes): Major, Minor, Modes
- Pentatonic (5 notes): Major/Minor Pentatonic
- Hexatonic (6 notes): Blues, Augmented, Whole Tone
- Octatonic (8 notes): Diminished
- Chromatic (12 notes): All semitones

## KeyMode

Represents the mode of a key signature.

```C#
namespace MusicTheory;

public enum KeyMode
{
    Major,
    Minor
}
```

**Usage:**
```C#
var cMajor = new KeySignature(new Note(NoteName.C), KeyMode.Major);
var aMinor = new KeySignature(new Note(NoteName.A), KeyMode.Minor);
```

**Notes:**
- Used with KeySignature class
- Determines scale pattern and chord relationships
- Minor refers to natural minor for key signatures

## DurationType

Represents note duration values.

```C#
namespace MusicTheory;

public enum DurationType
{
    Whole = 1,
    Half = 2,
    Quarter = 4,
    Eighth = 8,
    Sixteenth = 16,
    ThirtySecond = 32,
    SixtyFourth = 64
}
```

**Usage:**
```C#
var wholeNote = new Duration(DurationType.Whole);
var quarter = new Duration(DurationType.Quarter);
var dottedHalf = new Duration(DurationType.Half, dots: 1);
```

**Notes:**
- Values represent subdivision of whole note
- Used with Duration class for rhythm
- Can be modified with dots and tuplets

## Direction

Represents musical direction for transposition and intervals.

```C#
namespace MusicTheory;

public enum Direction
{
    Up,
    Down
}
```

**Usage:**
```C#
var note = new Note(NoteName.C, 4);
var up = note.Transpose(new Interval(IntervalQuality.Perfect, 5), Direction.Up);
var down = note.Transpose(new Interval(IntervalQuality.Perfect, 5), Direction.Down);
```

**Default Behavior:**
- Most methods default to Direction.Up
- Circle of fifths: Up = clockwise (sharps), Down = counterclockwise (flats)

## Best Practices

### Using Enums Effectively

```C#
// Check if an interval can be perfect
public static bool CanBePerfect(int intervalNumber)
{
    return intervalNumber == 1 || intervalNumber == 4 || 
           intervalNumber == 5 || intervalNumber == 8;
}

// Get chord complexity
public static int GetChordComplexity(ChordType type)
{
    return type switch
    {
        ChordType.Major or ChordType.Minor => 1,
        ChordType.Major7 or ChordType.Minor7 or ChordType.Dominant7 => 2,
        ChordType.Major9 or ChordType.Minor9 or ChordType.Dominant9 => 3,
        ChordType.Major11 or ChordType.Minor11 or ChordType.Dominant11 => 4,
        ChordType.Major13 or ChordType.Minor13 or ChordType.Dominant13 => 5,
        _ => 0
    };
}

// Convert scale type to mode number
public static int GetModeNumber(ScaleType scaleType)
{
    return scaleType switch
    {
        ScaleType.Ionian or ScaleType.Major => 1,
        ScaleType.Dorian => 2,
        ScaleType.Phrygian => 3,
        ScaleType.Lydian => 4,
        ScaleType.Mixolydian => 5,
        ScaleType.Aeolian or ScaleType.NaturalMinor => 6,
        ScaleType.Locrian => 7,
        _ => 0
    };
}
```

### Enum Validation

```C#
// Validate enum values
if (!Enum.IsDefined(typeof(ChordType), chordType))
{
    throw new ArgumentException($"Invalid chord type: {chordType}");
}

// Check enum flags (if using Flags attribute)
public static bool HasQuality(IntervalQuality quality, IntervalQuality check)
{
    return quality == check;
}
```

### Enum Extensions

```C#
public static class EnumExtensions
{
    public static string GetSymbol(this Alteration alteration)
    {
        return alteration switch
        {
            Alteration.DoubleFlat => "bb",
            Alteration.Flat => "b",
            Alteration.Natural => "",
            Alteration.Sharp => "#",
            Alteration.DoubleSharp => "##",
            _ => ""
        };
    }
    
    public static bool IsDiatonic(this ScaleType scaleType)
    {
        return scaleType switch
        {
            ScaleType.Chromatic => false,
            ScaleType.WholeTone => false,
            _ => true
        };
    }
}
```

## See Also

<seealso>
    <category ref="api">
        <a href="api-overview.md">API Overview</a>
        <a href="api-note.md">Note API Reference</a>
        <a href="api-interval.md">Interval API Reference</a>
        <a href="api-chord.md">Chord API Reference</a>
        <a href="api-scale.md">Scale API Reference</a>
    </category>
</seealso>