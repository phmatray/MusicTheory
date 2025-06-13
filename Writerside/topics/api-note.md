# Note API Reference

Complete API documentation for the `Note` class.

## Class Definition

```C#
namespace MusicTheory;

public sealed class Note : IEquatable<Note>
{
    public NoteName Name { get; }
    public Alteration Alteration { get; }
    public int Octave { get; }
    public double Frequency { get; }
    public int MidiNumber { get; }
    public int SemitonesFromC { get; }
}
```

## Constructors

### Note(NoteName, Alteration, int)

Creates a new note with the specified name, alteration, and octave.

```C#
public Note(NoteName name, Alteration alteration, int octave)
```

**Parameters:**
- `name`: The note name (C through B)
- `alteration`: The alteration (double flat through double sharp)
- `octave`: The octave number (-1 to 10)

**Exceptions:**
- `ArgumentOutOfRangeException`: If octave is outside valid range

**Example:**
```C#
var cSharp4 = new Note(NoteName.C, Alteration.Sharp, 4);
var bFlat3 = new Note(NoteName.B, Alteration.Flat, 3);
```

### Note(NoteName, int)

Creates a new natural note (convenience constructor).

```C#
public Note(NoteName name, int octave)
```

**Example:**
```C#
var c4 = new Note(NoteName.C, 4);  // C natural
```

## Properties

### Name

```C#
public NoteName Name { get; }
```

Gets the note name (C, D, E, F, G, A, or B).

### Alteration

```C#
public Alteration Alteration { get; }
```

Gets the alteration (DoubleFlat, Flat, Natural, Sharp, or DoubleSharp).

### Octave

```C#
public int Octave { get; }
```

Gets the octave number. Middle C is in octave 4.

### Frequency

```C#
public double Frequency { get; }
```

Gets the frequency in Hz using equal temperament tuning (A4 = 440 Hz).

**Calculation:**
```C#
frequency = 440 * Math.Pow(2, (semitonesFromA4 / 12.0))
```

### MidiNumber

```C#
public int MidiNumber { get; }
```

Gets the MIDI note number (0-127). Middle C (C4) = 60.

**Exceptions:**
- `InvalidOperationException`: If the note is outside MIDI range

### SemitonesFromC

```C#
public int SemitonesFromC { get; }
```

Gets the number of semitones from C in the same octave.

## Instance Methods

### Transpose(Interval, Direction?)

Transposes the note by the specified interval.

```C#
public Note Transpose(Interval interval, Direction direction = Direction.Up)
```

**Parameters:**
- `interval`: The interval to transpose by
- `direction`: The direction (Up or Down), default is Up

**Returns:** A new Note instance

**Example:**
```C#
var c4 = new Note(NoteName.C, 4);
var g4 = c4.Transpose(new Interval(IntervalQuality.Perfect, 5));
var f3 = c4.Transpose(new Interval(IntervalQuality.Perfect, 5), Direction.Down);
```

### TransposeBySemitones(int)

Transposes the note by a number of semitones.

```C#
public Note TransposeBySemitones(int semitones)
```

**Parameters:**
- `semitones`: Number of semitones (positive = up, negative = down)

**Returns:** A new Note instance

**Example:**
```C#
var c4 = new Note(NoteName.C, 4);
var cSharp4 = c4.TransposeBySemitones(1);
var b3 = c4.TransposeBySemitones(-1);
```

### IsEnharmonicWith(Note)

Determines if this note is enharmonically equivalent to another.

```C#
public bool IsEnharmonicWith(Note other)
```

**Parameters:**
- `other`: The note to compare with

**Returns:** True if the notes have the same pitch

**Example:**
```C#
var cSharp = new Note(NoteName.C, Alteration.Sharp, 4);
var dFlat = new Note(NoteName.D, Alteration.Flat, 4);
bool same = cSharp.IsEnharmonicWith(dFlat); // true
```

### GetEnharmonicEquivalent()

Gets the enharmonic equivalent of this note.

```C#
public Note? GetEnharmonicEquivalent()
```

**Returns:** The enharmonic equivalent, or null if none exists

**Example:**
```C#
var cSharp = new Note(NoteName.C, Alteration.Sharp, 4);
var dFlat = cSharp.GetEnharmonicEquivalent(); // D♭4
```

### ToString()

Returns a string representation of the note.

```C#
public override string ToString()
```

**Format:** `{NoteName}{Alteration}{Octave}`

**Examples:**
- C4 → "C4"
- C#4 → "C#4"
- Bb3 → "Bb3"
- F##5 → "F##5"

### Equals(Note?)

Determines if this note equals another note.

```C#
public bool Equals(Note? other)
```

**Note:** Compares by value (name, alteration, octave), not by enharmonic equivalence.

### GetHashCode()

Returns the hash code for the note.

```C#
public override int GetHashCode()
```

## Static Methods

### FromMidiNumber(int, bool)

Creates a note from a MIDI number.

```C#
public static Note FromMidiNumber(int midiNumber, bool preferFlats = false)
```

**Parameters:**
- `midiNumber`: The MIDI number (0-127)
- `preferFlats`: Whether to prefer flats for black keys

**Returns:** A new Note instance

**Exceptions:**
- `ArgumentOutOfRangeException`: If MIDI number is outside 0-127

**Example:**
```C#
var middleC = Note.FromMidiNumber(60);        // C4
var cSharp = Note.FromMidiNumber(61);         // C#4
var dFlat = Note.FromMidiNumber(61, true);    // Db4
```

### TryParse(string, out Note?)

Tries to parse a note from a string.

```C#
public static bool TryParse(string input, out Note? note)
```

**Parameters:**
- `input`: String representation (e.g., "C4", "F#5", "Bb3")
- `note`: The parsed note, or null if parsing fails

**Returns:** True if parsing succeeded

**Supported formats:**
- Natural: "C4", "D5"
- Sharp: "C#4", "F#5"
- Flat: "Bb3", "Eb4"
- Double sharp: "F##5", "C##4"
- Double flat: "Bbb3", "Abb4"

**Example:**
```C#
if (Note.TryParse("C#4", out var note))
{
    Console.WriteLine(note.Frequency); // 277.18
}
```

## Operators

### Equality Operators

```C#
public static bool operator ==(Note? left, Note? right)
public static bool operator !=(Note? left, Note? right)
```

Compare notes by value (not enharmonic equivalence).

**Example:**
```C#
var c1 = new Note(NoteName.C, 4);
var c2 = new Note(NoteName.C, 4);
var cSharp = new Note(NoteName.C, Alteration.Sharp, 4);

bool equal1 = c1 == c2;      // true
bool equal2 = c1 == cSharp;  // false
```

## Constants and Limits

### Octave Range
- Minimum: -1
- Maximum: 10

### MIDI Range
- Minimum: 0 (C-1)
- Maximum: 127 (G9)

### Frequency Range
- Lowest: ~8.18 Hz (C-1)
- Highest: ~12543.85 Hz (G9)
- Reference: A4 = 440 Hz

## Common Patterns

### Creating Common Notes

```C#
// Middle C
var middleC = new Note(NoteName.C, 4);

// A440 (tuning reference)
var a440 = new Note(NoteName.A, 4);

// Black keys
var cSharp = new Note(NoteName.C, Alteration.Sharp, 4);
var dFlat = new Note(NoteName.D, Alteration.Flat, 4);
```

### Working with Collections

```C#
// Create chromatic scale
var chromaticScale = Enumerable.Range(60, 13)
    .Select(midi => Note.FromMidiNumber(midi))
    .ToList();

// Sort notes by pitch
var notes = new List<Note> { /* ... */ };
var sorted = notes.OrderBy(n => n.MidiNumber).ToList();
```

### Enharmonic Handling

```C#
public Note GetPreferredSpelling(Note note, KeySignature key)
{
    // Check if note fits the key
    if (key.GetAlteration(note.Name) == note.Alteration)
        return note;
    
    // Try enharmonic equivalent
    var enharmonic = note.GetEnharmonicEquivalent();
    if (enharmonic != null && 
        key.GetAlteration(enharmonic.Name) == enharmonic.Alteration)
        return enharmonic;
    
    return note; // Return original if no better spelling
}
```

## Performance Notes

- Frequency calculation is cached after first access
- MIDI number calculation is performed on construction
- Enharmonic lookups use efficient switch expressions
- String parsing uses optimized character processing

## Thread Safety

The `Note` class is immutable and thread-safe. Multiple threads can safely:
- Read all properties
- Call all methods
- Share Note instances

No synchronization is needed.

## See Also

<seealso>
    <category ref="related">
        <a href="notes.md">Notes Concept Guide</a>
        <a href="api-interval.md">Interval API Reference</a>
        <a href="enums.md">NoteName Enum</a>
        <a href="enums.md">Alteration Enum</a>
    </category>
</seealso>