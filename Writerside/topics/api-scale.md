# Scale API Reference

Complete API documentation for the `Scale` class.

## Class Definition

```C#
namespace MusicTheory;

public sealed class Scale : IEquatable<Scale>
{
    public Note Root { get; }
    public ScaleType Type { get; }
}
```

## Constructors

### Scale(Note, ScaleType)

Creates a new scale with the specified root note and scale type.

```C#
public Scale(Note root, ScaleType type)
```

**Parameters:**
- `root`: The root note of the scale
- `type`: The scale type (Major, Minor, Dorian, Blues, etc.)

**Example:**
```C#
var cMajor = new Scale(new Note(NoteName.C, 4), ScaleType.Major);
var aMinor = new Scale(new Note(NoteName.A, 4), ScaleType.NaturalMinor);
var dDorian = new Scale(new Note(NoteName.D, 4), ScaleType.Dorian);
```

## Properties

### Root

```C#
public Note Root { get; }
```

Gets the root note of the scale.

### Type

```C#
public ScaleType Type { get; }
```

Gets the scale type.

## Instance Methods

### GetNotes()

Generates all notes in the scale.

```C#
public IEnumerable<Note> GetNotes()
```

**Returns:** An infinite sequence of scale notes ascending from the root

**Note:** Use `Take()` to limit the number of notes.

**Example:**
```C#
var cMajor = new Scale(new Note(NoteName.C, 4), ScaleType.Major);
var firstOctave = cMajor.GetNotes().Take(8).ToList();
// [C4, D4, E4, F4, G4, A4, B4, C5]

var twoOctaves = cMajor.GetNotes().Take(15).ToList();
// [C4, D4, E4, F4, G4, A4, B4, C5, D5, E5, F5, G5, A5, B5, C6]
```

### GetNoteByDegree(int)

Gets a specific scale degree.

```C#
public Note GetNoteByDegree(int degree)
```

**Parameters:**
- `degree`: The scale degree (1-based)

**Returns:** The note at the specified degree

**Exceptions:**
- `ArgumentOutOfRangeException`: If degree is less than 1

**Example:**
```C#
var gMajor = new Scale(new Note(NoteName.G, 4), ScaleType.Major);
var third = gMajor.GetNoteByDegree(3);   // B4
var seventh = gMajor.GetNoteByDegree(7); // F#5
```

### Contains(Note)

Determines if a note belongs to the scale.

```C#
public bool Contains(Note note)
```

**Parameters:**
- `note`: The note to check

**Returns:** True if the note (any octave) is in the scale

**Example:**
```C#
var cMajor = new Scale(new Note(NoteName.C, 4), ScaleType.Major);
bool hasE = cMajor.Contains(new Note(NoteName.E, 4));        // true
bool hasEFlat = cMajor.Contains(new Note(NoteName.E, Alteration.Flat, 4)); // false
bool hasHighC = cMajor.Contains(new Note(NoteName.C, 5));    // true (any octave)
```

### GetDegree(Note)

Gets the scale degree of a note.

```C#
public int GetDegree(Note note)
```

**Parameters:**
- `note`: The note to find

**Returns:** The scale degree (1-7), or 0 if not in scale

**Example:**
```C#
var dMajor = new Scale(new Note(NoteName.D, 4), ScaleType.Major);
int degree = dMajor.GetDegree(new Note(NoteName.F, Alteration.Sharp, 4)); // 3
int notInScale = dMajor.GetDegree(new Note(NoteName.F, 4)); // 0
```

### Transpose(Interval, Direction?)

Transposes the scale by the specified interval.

```C#
public Scale Transpose(Interval interval, Direction direction = Direction.Up)
```

**Parameters:**
- `interval`: The interval to transpose by
- `direction`: The direction (Up or Down), default is Up

**Returns:** A new Scale instance with transposed root

**Example:**
```C#
var cMajor = new Scale(new Note(NoteName.C, 4), ScaleType.Major);
var gMajor = cMajor.Transpose(new Interval(IntervalQuality.Perfect, 5));
```

### TransposeBySemitones(int)

Transposes the scale by a number of semitones.

```C#
public Scale TransposeBySemitones(int semitones)
```

**Parameters:**
- `semitones`: Number of semitones (positive = up, negative = down)

**Returns:** A new Scale instance

### ToString()

Returns a string representation of the scale.

```C#
public override string ToString()
```

**Format:** `"{Root} {Type}"`

**Examples:**
- "C Major"
- "A NaturalMinor"
- "D Dorian"

### Equals(Scale?)

Determines if this scale equals another scale.

```C#
public bool Equals(Scale? other)
```

**Note:** Compares root note and scale type.

### GetHashCode()

Returns the hash code for the scale.

```C#
public override int GetHashCode()
```

## Static Methods

### GetIntervalPattern(ScaleType)

Gets the interval pattern for a scale type.

```C#
public static int[] GetIntervalPattern(ScaleType type)
```

**Parameters:**
- `type`: The scale type

**Returns:** Array of semitone intervals

**Example:**
```C#
var majorPattern = Scale.GetIntervalPattern(ScaleType.Major);
// [2, 2, 1, 2, 2, 2, 1] (W-W-H-W-W-W-H)

var minorPattern = Scale.GetIntervalPattern(ScaleType.NaturalMinor);
// [2, 1, 2, 2, 1, 2, 2] (W-H-W-W-H-W-W)
```

## Operators

### Equality Operators

```C#
public static bool operator ==(Scale? left, Scale? right)
public static bool operator !=(Scale? left, Scale? right)
```

Compare scales by root and type.

## ScaleType Enumeration

The library supports 15+ scale types:

### Traditional Scales
- Major (Ionian)
- NaturalMinor (Aeolian)
- HarmonicMinor
- MelodicMinor

### Modal Scales
- Ionian (same as Major)
- Dorian
- Phrygian
- Lydian
- Mixolydian
- Aeolian (same as Natural Minor)
- Locrian

### Pentatonic Scales
- MajorPentatonic
- MinorPentatonic

### Other Scales
- Blues
- Chromatic
- WholeTone
- Diminished (Octatonic)
- Augmented

## Scale Interval Patterns

### Common Scale Patterns

| Scale Type | Pattern | Example (C) |
|------------|---------|-------------|
| Major | W-W-H-W-W-W-H | C D E F G A B C |
| Natural Minor | W-H-W-W-H-W-W | C D Eb F G Ab Bb C |
| Harmonic Minor | W-H-W-W-H-Aug2-H | C D Eb F G Ab B C |
| Melodic Minor | W-H-W-W-W-W-H | C D Eb F G A B C |
| Dorian | W-H-W-W-W-H-W | C D Eb F G A Bb C |
| Phrygian | H-W-W-W-H-W-W | C Db Eb F G Ab Bb C |
| Lydian | W-W-W-H-W-W-H | C D E F# G A B C |
| Mixolydian | W-W-H-W-W-H-W | C D E F G A Bb C |
| Locrian | H-W-W-H-W-W-W | C Db Eb F Gb Ab Bb C |

(W = Whole step = 2 semitones, H = Half step = 1 semitone, Aug2 = 3 semitones)

## Common Patterns

### Creating Common Scales

```C#
// Major scales
var cMajor = new Scale(new Note(NoteName.C, 4), ScaleType.Major);
var gMajor = new Scale(new Note(NoteName.G, 4), ScaleType.Major);
var dMajor = new Scale(new Note(NoteName.D, 4), ScaleType.Major);

// Minor scales
var aMinor = new Scale(new Note(NoteName.A, 4), ScaleType.NaturalMinor);
var aHarmonicMinor = new Scale(new Note(NoteName.A, 4), ScaleType.HarmonicMinor);
var aMelodicMinor = new Scale(new Note(NoteName.A, 4), ScaleType.MelodicMinor);

// Modal scales
var dDorian = new Scale(new Note(NoteName.D, 4), ScaleType.Dorian);
var ePhrygian = new Scale(new Note(NoteName.E, 4), ScaleType.Phrygian);
var fLydian = new Scale(new Note(NoteName.F, 4), ScaleType.Lydian);

// Other scales
var aBlues = new Scale(new Note(NoteName.A, 4), ScaleType.Blues);
var cWholeTone = new Scale(new Note(NoteName.C, 4), ScaleType.WholeTone);
```

### Scale Analysis

```C#
public class ScaleAnalyzer
{
    public static void AnalyzeScale(Scale scale)
    {
        var notes = scale.GetNotes().Take(8).ToList();
        Console.WriteLine($"Scale: {scale}");
        Console.WriteLine($"Notes: {string.Join(" ", notes)}");
        
        // Show intervals
        for (int i = 0; i < notes.Count - 1; i++)
        {
            var interval = Interval.Between(notes[i], notes[i + 1]);
            Console.WriteLine($"  {notes[i]} to {notes[i + 1]}: {interval.Semitones} semitones");
        }
        
        // Show scale degrees
        for (int degree = 1; degree <= 7; degree++)
        {
            var note = scale.GetNoteByDegree(degree);
            Console.WriteLine($"  Degree {degree}: {note}");
        }
    }
}
```

### Building Chords from Scales

```C#
public static List<Chord> GetDiatonicTriads(Scale scale)
{
    var triads = new List<Chord>();
    var scaleNotes = scale.GetNotes().Take(7).ToList();
    
    for (int i = 0; i < 7; i++)
    {
        var root = scaleNotes[i];
        var third = scaleNotes[(i + 2) % 7];
        var fifth = scaleNotes[(i + 4) % 7];
        
        // Adjust octaves if wrapped
        if ((i + 2) >= 7) third = third.TransposeBySemitones(12);
        if ((i + 4) >= 7) fifth = fifth.TransposeBySemitones(12);
        
        // Determine chord quality
        var thirdInterval = Interval.Between(root, third).Semitones;
        var fifthInterval = Interval.Between(root, fifth).Semitones;
        
        ChordType chordType;
        if (thirdInterval == 4 && fifthInterval == 7)
            chordType = ChordType.Major;
        else if (thirdInterval == 3 && fifthInterval == 7)
            chordType = ChordType.Minor;
        else if (thirdInterval == 3 && fifthInterval == 6)
            chordType = ChordType.Diminished;
        else
            continue;
        
        triads.Add(new Chord(root, chordType));
    }
    
    return triads;
}
```

### Common Tones Between Scales

```C#
public static List<Note> FindCommonTones(Scale scale1, Scale scale2)
{
    var notes1 = scale1.GetNotes().Take(7).ToList();
    var notes2 = scale2.GetNotes().Take(7).ToList();
    
    return notes1
        .Where(n1 => notes2.Any(n2 => 
            n1.Name == n2.Name && 
            n1.Alteration == n2.Alteration))
        .ToList();
}

// Example: Common tones between C major and G major
var cMajor = new Scale(new Note(NoteName.C, 4), ScaleType.Major);
var gMajor = new Scale(new Note(NoteName.G, 4), ScaleType.Major);
var common = FindCommonTones(cMajor, gMajor);
// Result: C, D, E, G, A, B (all except F/F#)
```

## Performance Notes

- GetNotes() uses lazy evaluation with yield return
- Scale patterns are cached statically
- Contains() method uses efficient note comparison
- GetDegree() performs linear search (scales have max 7 unique notes)

## Thread Safety

The `Scale` class is immutable and thread-safe. Multiple threads can safely:
- Read all properties
- Call all methods
- Share Scale instances

No synchronization is needed.

## Validation

- Root note must not be null
- ScaleType must be a valid enum value
- GetNoteByDegree validates degree >= 1

## See Also

<seealso>
    <category ref="related">
        <a href="scales.md">Scales Concept Guide</a>
        <a href="api-note.md">Note API Reference</a>
        <a href="enums.md">ScaleType Enum</a>
        <a href="building-scales.md">Building Scales Tutorial</a>
    </category>
</seealso>