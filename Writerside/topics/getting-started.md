# Getting Started

This guide will walk you through the basic concepts and usage patterns of the MusicTheory library.

## Core Concepts

The MusicTheory library is built around several core concepts that mirror traditional music theory:

<deflist>
    <def title="Note">
        The fundamental unit of music - a specific pitch with a name, alteration, and octave.
    </def>
    <def title="Interval">
        The distance between two notes, characterized by quality (Perfect, Major, Minor, etc.) and size.
    </def>
    <def title="Scale">
        A sequence of notes following a specific pattern of intervals.
    </def>
    <def title="Chord">
        A collection of notes played together, built from a root note and following specific interval patterns.
    </def>
    <def title="Key Signature">
        Defines the tonal center and the sharps or flats used in a piece of music.
    </def>
</deflist>

## Basic Usage Patterns

### Creating Notes

Notes are the building blocks of all other musical constructs:

```C#
// Create notes using constructor
var c4 = new Note(NoteName.C, Alteration.Natural, 4);
var fSharp5 = new Note(NoteName.F, Alteration.Sharp, 5);
var bFlat3 = new Note(NoteName.B, Alteration.Flat, 3);

// Parse from string notation
var note = Note.Parse("C#4");
var anotherNote = Note.Parse("Eb5");

// Create from MIDI number
var middleC = Note.FromMidiNumber(60);
var a440 = Note.FromMidiNumber(69);
```

### Working with Intervals

Intervals represent the distance between notes:

```C#
// Create intervals
var perfectFifth = new Interval(IntervalQuality.Perfect, 5);
var majorThird = new Interval(IntervalQuality.Major, 3);
var minorSeventh = new Interval(IntervalQuality.Minor, 7);

// Calculate interval between two notes
var c = new Note(NoteName.C, Alteration.Natural, 4);
var g = new Note(NoteName.G, Alteration.Natural, 4);
var interval = Interval.Between(c, g); // Perfect Fifth

// Transpose notes by intervals
var e = c.Transpose(majorThird, Direction.Up);
var f = c.Transpose(perfectFifth, Direction.Down);
```

### Building Chords

The library supports 40+ chord types:

<tabs>
    <tab title="Basic Chords">
        <code-block lang="C#"><![CDATA[
// Triads
var cMajor = new Chord(c4, ChordType.Major);
var aMinor = new Chord(new Note(NoteName.A), ChordType.Minor);
var bDim = new Chord(new Note(NoteName.B), ChordType.Diminished);
var fAug = new Chord(new Note(NoteName.F), ChordType.Augmented);

// Get chord notes
var notes = cMajor.GetNotes(); // Returns C, E, G
]]></code-block>
    </tab>
    <tab title="Seventh Chords">
        <code-block lang="C#"><![CDATA[
// Seventh chords
var cMaj7 = new Chord(c4, ChordType.Major7);
var dm7 = new Chord(new Note(NoteName.D), ChordType.Minor7);
var g7 = new Chord(new Note(NoteName.G), ChordType.Dominant7);
var bHalfDim = new Chord(new Note(NoteName.B), ChordType.HalfDiminished7);

// Get chord symbol
Console.WriteLine(cMaj7.GetSymbol()); // "Cmaj7"
Console.WriteLine(bHalfDim.GetSymbol()); // "Bø7"
]]></code-block>
    </tab>
    <tab title="Extended Chords">
        <code-block lang="C#"><![CDATA[
// Extended and altered chords
var cMaj9 = new Chord(c4, ChordType.Major9);
var dm11 = new Chord(new Note(NoteName.D), ChordType.Minor11);
var g13 = new Chord(new Note(NoteName.G), ChordType.Dominant13);
var g7b9 = new Chord(new Note(NoteName.G), ChordType.Dominant7Flat9);
var alt = new Chord(new Note(NoteName.C), ChordType.Dominant7Alt);

// Suspended chords
var csus2 = new Chord(c4, ChordType.Sus2);
var gsus4 = new Chord(new Note(NoteName.G), ChordType.Sus4);
]]></code-block>
    </tab>
</tabs>

### Generating Scales

Create and work with various scale types:

<code-block lang="C#"><![CDATA[
// Traditional scales
var cMajor = new Scale(c4, ScaleType.Major);
var aMinor = new Scale(new Note(NoteName.A), ScaleType.NaturalMinor);
var dHarmonic = new Scale(new Note(NoteName.D), ScaleType.HarmonicMinor);

// Modal scales
var dDorian = new Scale(new Note(NoteName.D), ScaleType.Dorian);
var ePhrygian = new Scale(new Note(NoteName.E), ScaleType.Phrygian);
var fLydian = new Scale(new Note(NoteName.F), ScaleType.Lydian);

// Get scale notes
var notes = cMajor.GetNotes(); // C, D, E, F, G, A, B, C

// Check if a note is in the scale
var isInScale = cMajor.Contains(new Note(NoteName.F, Alteration.Sharp)); // false

// Get scale degree
var degree = cMajor.GetDegree(new Note(NoteName.G)); // 5
]]></code-block>

## Common Patterns

### Chord Progressions

Work with chord progressions using Roman numeral analysis:

```C#
// Create a key signature
var key = new KeySignature(new Note(NoteName.C), KeyMode.Major);

// Create a progression
var progression = new ChordProgression(key);

// Get diatonic chords
var chords = progression.GetDiatonicChords(); // I, ii, iii, IV, V, vi, vii°

// Parse Roman numerals
var iiVi = progression.ParseProgression("ii - V - I");

// Get specific chord by degree
var dominant = progression.GetChordByDegree(5); // G major
```

### Transposition

Transpose musical elements up or down:

```C#
// Transpose a note
var c = new Note(NoteName.C, Alteration.Natural, 4);
var d = c.Transpose(new Interval(IntervalQuality.Major, 2), Direction.Up);

// Transpose by semitones
var eb = c.TransposeBySemitones(3);

// Transpose a chord
var cMajor = new Chord(c, ChordType.Major);
var dMajor = cMajor.Transpose(new Interval(IntervalQuality.Major, 2));

// Transpose a scale
var cScale = new Scale(c, ScaleType.Major);
var gScale = cScale.Transpose(new Interval(IntervalQuality.Perfect, 5));
```

### MIDI Integration

Convert between musical notation and MIDI:

```C#
// Note to MIDI
var c4 = new Note(NoteName.C, Alteration.Natural, 4);
int midiNumber = c4.MidiNumber; // 60

// MIDI to Note
var note = Note.FromMidiNumber(69); // A4
var flatNote = Note.FromMidiNumber(61, preferFlats: true); // Db4 instead of C#4

// Check frequency
double frequency = c4.Frequency; // 261.63 Hz (using A4 = 440 Hz)
```

## Best Practices

- **Immutability**: All objects are immutable. Methods return new instances rather than modifying existing ones.
- **Fluent API**: Chain method calls for readable code: `chord.AddExtension(7, IntervalQuality.Major).WithInversion(ChordInversion.First)`
- **Type Safety**: Use enums like `NoteName`, `ChordType`, and `ScaleType` instead of strings
- **Error Handling**: The library throws descriptive exceptions for invalid operations

## Next Steps

Now that you understand the basics, explore these topics:

- [Deep dive into Notes](notes.md)
- [Understanding Intervals](intervals.md)
- [Advanced Chord Techniques](chords.md)
- [Scale Theory and Modes](scales.md)
- [Working with Key Signatures](key-signatures.md)

<seealso>
    <category ref="tutorials">
        <a href="chord-progressions.md">Building Chord Progressions</a>
        <a href="transposition.md">Transposition Techniques</a>
        <a href="midi-integration.md">MIDI Integration</a>
    </category>
</seealso>