# Working with Notes

The `Note` class is the fundamental building block of the MusicTheory library. It represents a musical pitch with a specific name, alteration, and octave.

## Understanding Notes

A note in music theory consists of three components:

<deflist>
    <def title="Note Name">
        One of the seven natural notes: C, D, E, F, G, A, B
    </def>
    <def title="Alteration">
        Modifies the pitch: Double Flat (♭♭), Flat (♭), Natural (♮), Sharp (♯), Double Sharp (♯♯)
    </def>
    <def title="Octave">
        The register of the note, following scientific pitch notation (Middle C = C4)
    </def>
</deflist>

## Creating Notes

### Using the Constructor

The most explicit way to create a note:

```C#
// Natural notes
var c4 = new Note(NoteName.C, Alteration.Natural, 4);
var a3 = new Note(NoteName.A, Alteration.Natural, 3);

// Altered notes
var fSharp5 = new Note(NoteName.F, Alteration.Sharp, 5);
var bFlat2 = new Note(NoteName.B, Alteration.Flat, 2);
var gDoubleSharp = new Note(NoteName.G, Alteration.DoubleSharp, 4);

// Simplified constructor for natural notes
var d4 = new Note(NoteName.D); // Defaults to natural, octave 4
var e5 = new Note(NoteName.E, octave: 5);
```

### Parsing from String

Parse notes from standard notation:

```C#
// Basic notation
var c4 = Note.Parse("C4");
var fSharp5 = Note.Parse("F#5");
var bFlat3 = Note.Parse("Bb3");

// Double alterations
var fDoubleSharp = Note.Parse("F##4");
var bDoubleFlat = Note.Parse("Bbb2");

// The parser is case-insensitive
var note = Note.Parse("c#4"); // Same as C#4
```

### From MIDI Numbers

Convert MIDI note numbers to Note objects:

```C#
// Middle C (MIDI 60)
var middleC = Note.FromMidiNumber(60);

// A440 (MIDI 69)
var a440 = Note.FromMidiNumber(69);

// Prefer flats for black keys
var db4 = Note.FromMidiNumber(61, preferFlats: true); // Db4 instead of C#4
var eb4 = Note.FromMidiNumber(63, preferFlats: true); // Eb4 instead of D#4
```

## Note Properties

### Basic Properties

```C#
var fSharp4 = new Note(NoteName.F, Alteration.Sharp, 4);

// Access components
NoteName name = fSharp4.Name;           // NoteName.F
Alteration alt = fSharp4.Alteration;    // Alteration.Sharp
int octave = fSharp4.Octave;           // 4

// String representation
string str = fSharp4.ToString();        // "F#4"
```

### Calculated Properties

```C#
var a4 = new Note(NoteName.A, Alteration.Natural, 4);

// Frequency calculation (A4 = 440 Hz standard)
double freq = a4.Frequency;             // 440.0 Hz

// MIDI number
int midi = a4.MidiNumber;               // 69

// Total semitones from C0
int semitones = a4.GetTotalSemitones(); // 57
```

## Note Operations

### Transposition

Transpose notes by intervals or semitones:

<tabs>
    <tab title="By Interval">
        <code-block lang="C#"><![CDATA[
var c4 = new Note(NoteName.C, Alteration.Natural, 4);

// Transpose by interval
var g4 = c4.Transpose(new Interval(IntervalQuality.Perfect, 5), Direction.Up);
var f3 = c4.Transpose(new Interval(IntervalQuality.Perfect, 5), Direction.Down);

// Common intervals
var majorThird = new Interval(IntervalQuality.Major, 3);
var e4 = c4.Transpose(majorThird); // Defaults to Up

var minorSixth = new Interval(IntervalQuality.Minor, 6);
var ab4 = c4.Transpose(minorSixth, Direction.Up);
]]></code-block>
    </tab>
    <tab title="By Semitones">
        <code-block lang="C#"><![CDATA[
var c4 = new Note(NoteName.C, Alteration.Natural, 4);

// Transpose by semitones
var d4 = c4.TransposeBySemitones(2);   // Up 2 semitones
var bb3 = c4.TransposeBySemitones(-2); // Down 2 semitones

// Octave transposition
var c5 = c4.TransposeBySemitones(12);  // Up one octave
var c3 = c4.TransposeBySemitones(-12); // Down one octave
]]></code-block>
    </tab>
</tabs>

### Enharmonic Equivalents

Get enharmonically equivalent notes (same pitch, different spelling):

```C#
var cSharp = new Note(NoteName.C, Alteration.Sharp, 4);
var dFlat = cSharp.GetEnharmonicEquivalent(); // Db4

var e = new Note(NoteName.E, Alteration.Natural, 4);
var fFlat = e.GetEnharmonicEquivalent(); // Fb4

var gDoubleSharp = new Note(NoteName.G, Alteration.DoubleSharp, 4);
var a = gDoubleSharp.GetEnharmonicEquivalent(); // A4

// Check if two notes are enharmonic
bool areEnharmonic = cSharp.IsEnharmonicWith(dFlat); // true
```

## Frequency and Tuning

The library uses A4 = 440 Hz as the standard tuning reference:

```C#
// Standard frequencies
var a4 = new Note(NoteName.A, Alteration.Natural, 4);
Console.WriteLine(a4.Frequency); // 440.0 Hz

var c4 = new Note(NoteName.C, Alteration.Natural, 4);
Console.WriteLine(c4.Frequency); // 261.63 Hz

// Frequency calculation follows equal temperament
var cSharp4 = new Note(NoteName.C, Alteration.Sharp, 4);
Console.WriteLine(cSharp4.Frequency); // 277.18 Hz
```

## Common Patterns

### Note Comparison

Notes can be compared for ordering:

```C#
var c4 = new Note(NoteName.C, Alteration.Natural, 4);
var g4 = new Note(NoteName.G, Alteration.Natural, 4);
var c5 = new Note(NoteName.C, Alteration.Natural, 5);

// Comparison
bool isLower = c4.CompareTo(g4) < 0;  // true
bool isHigher = c5.CompareTo(c4) > 0; // true

// Sorting
var notes = new[] { g4, c5, c4 };
Array.Sort(notes); // c4, g4, c5
```

### Note Validation

Check if note properties are valid:

```C#
try
{
    // This will throw - MIDI range is 0-127
    var tooHigh = new Note(NoteName.C, Alteration.Natural, 10);
    int midi = tooHigh.MidiNumber; // Throws InvalidOperationException
}
catch (InvalidOperationException ex)
{
    Console.WriteLine(ex.Message);
}

// Parsing validation
if (Note.TryParse("C#4", out var note))
{
    Console.WriteLine($"Parsed: {note}");
}
else
{
    Console.WriteLine("Invalid note string");
}
```

### Working with Note Collections

Common operations with multiple notes:

```C#
// Create a chromatic scale
var chromaticScale = new List<Note>();
var c4 = new Note(NoteName.C, Alteration.Natural, 4);
for (int i = 0; i < 12; i++)
{
    chromaticScale.Add(c4.TransposeBySemitones(i));
}

// Find interval between notes
var e4 = new Note(NoteName.E, Alteration.Natural, 4);
var interval = Interval.Between(c4, e4); // Major third

// Check octave relationships
var c5 = new Note(NoteName.C, Alteration.Natural, 5);
bool isSameNoteClass = c4.Name == c5.Name; // true
int octaveDifference = c5.Octave - c4.Octave; // 1
```

## Advanced Topics

### Enharmonic Spelling

Understanding when to use sharps vs flats:

```C#
// In the key of G major, use F# not Gb
var gMajorScale = new Scale(new Note(NoteName.G), ScaleType.Major);
var seventhDegree = gMajorScale.GetNotes().ElementAt(6); // F#4

// In the key of F major, use Bb not A#
var fMajorScale = new Scale(new Note(NoteName.F), ScaleType.Major);
var fourthDegree = fMajorScale.GetNotes().ElementAt(3); // Bb4
```

### Octave Designation

The library follows scientific pitch notation:

- C-1 to B-1: Sub-contra octave (MIDI 0-11)
- C0 to B0: Contra octave (MIDI 12-23)
- C1 to B1: Great octave (MIDI 24-35)
- C2 to B2: Small octave (MIDI 36-47)
- C3 to B3: One-line octave (MIDI 48-59)
- C4 to B4: Two-line octave (Middle C) (MIDI 60-71)
- C5 to B5: Three-line octave (MIDI 72-83)
- C6 to B6: Four-line octave (MIDI 84-95)
- C7 to B7: Five-line octave (MIDI 96-107)
- C8 to B8: Six-line octave (MIDI 108-119)
- C9 to G9: Seven-line octave (MIDI 120-127)

## See Also

<seealso>
    <category ref="related">
        <a href="intervals.md">Working with Intervals</a>
        <a href="scales.md">Building Scales from Notes</a>
        <a href="chords.md">Creating Chords from Notes</a>
    </category>
    <category ref="api">
        <a href="api-note.md">Note API Reference</a>
        <a href="enums.md">NoteName Enum</a>
        <a href="enums.md">Alteration Enum</a>
    </category>
</seealso>