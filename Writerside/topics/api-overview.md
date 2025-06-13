# API Overview

The MusicTheory library provides a comprehensive set of classes and enums for working with music theory concepts programmatically. This reference guide covers all public APIs.

## Core Classes

### Domain Objects

<deflist>
    <def title="Note">
        Represents a musical note with name, alteration, and octave. Immutable.
    </def>
    <def title="Interval">
        Represents the distance between two notes. Includes quality and numeric value.
    </def>
    <def title="Scale">
        Generates notes following specific interval patterns from a root note.
    </def>
    <def title="Chord">
        Built from a root note and chord type, generates chord tones.
    </def>
    <def title="KeySignature">
        Represents a musical key with tonic and mode.
    </def>
    <def title="ChordProgression">
        Analyzes and generates chord progressions in a given key.
    </def>
</deflist>

### Time and Rhythm

<deflist>
    <def title="TimeSignature">
        Represents meter with numerator and denominator.
    </def>
    <def title="Duration">
        Musical note duration with dots and tuplets.
    </def>
</deflist>

## Enumerations

### Note-related Enums

```C#
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

public enum Alteration
{
    DoubleFlat = -2,
    Flat = -1,
    Natural = 0,
    Sharp = 1,
    DoubleSharp = 2
}
```

### Musical Structure Enums

```C#
public enum IntervalQuality
{
    Diminished,
    Minor,
    Major,
    Perfect,
    Augmented
}

public enum ScaleType
{
    // Traditional
    Major,
    NaturalMinor,
    HarmonicMinor,
    MelodicMinor,
    
    // Modes
    Ionian,
    Dorian,
    Phrygian,
    Lydian,
    Mixolydian,
    Aeolian,
    Locrian,
    
    // Pentatonic
    MajorPentatonic,
    MinorPentatonic,
    
    // Other
    Blues,
    Chromatic,
    WholeTone,
    Diminished,
    Augmented
}

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
    
    // Extended chords
    Major9, Minor9, Dominant9,
    Major11, Minor11, Dominant11,
    Major13, Minor13, Dominant13,
    
    // Altered chords
    Dominant7Flat5, Dominant7Sharp5,
    Dominant7Flat9, Dominant7Sharp9,
    Dominant7Sharp11,
    Dominant7Flat13,
    
    // Suspended
    Sus2, Sus4,
    Dominant7Sus2, Dominant7Sus4,
    
    // Add chords
    Add9, MinorAdd9,
    Add11, MinorAdd11,
    Add13, MinorAdd13,
    
    // Other
    Power,
    Major6, Minor6,
    Dominant6,
    MinorMajor9
}

public enum KeyMode
{
    Major,
    Minor
}
```

### Time and Duration Enums

```C#
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

public enum Direction
{
    Up,
    Down
}
```

## Common Methods

### Note Methods

```C#
// Construction
var note = new Note(NoteName.C, Alteration.Sharp, 4);

// Properties
int midiNumber = note.MidiNumber;        // 61
double frequency = note.Frequency;        // 277.18 Hz
int semitones = note.SemitonesFromC;     // 1

// Methods
var transposed = note.Transpose(interval);
var bySteps = note.TransposeBySemitones(5);
bool isEnharmonic = note.IsEnharmonicWith(otherNote);
var equivalent = note.GetEnharmonicEquivalent();

// Static methods
var fromMidi = Note.FromMidiNumber(60);
bool parsed = Note.TryParse("C#4", out var parsedNote);
```

### Interval Methods

```C#
// Construction
var interval = new Interval(IntervalQuality.Major, 3);

// Properties
int semitones = interval.Semitones;      // 4
string name = interval.ToString();        // "Major 3rd"

// Static factory
var between = Interval.Between(note1, note2);

// Methods
bool isEnharmonic = interval.IsEnharmonicWith(otherInterval);
var inverted = interval.Invert();
```

### Scale Methods

```C#
// Construction
var scale = new Scale(root, ScaleType.Major);

// Properties
Note root = scale.Root;
ScaleType type = scale.Type;

// Methods
var notes = scale.GetNotes();             // IEnumerable<Note>
var note = scale.GetNoteByDegree(3);      // 3rd degree
bool contains = scale.Contains(note);
int degree = scale.GetDegree(note);       // 1-7 or 0
var transposed = scale.Transpose(interval);
```

### Chord Methods

```C#
// Construction
var chord = new Chord(root, ChordType.Major7);

// Properties
Note root = chord.Root;
ChordType type = chord.Type;

// Methods
var notes = chord.GetNotes();             // IEnumerable<Note>
string symbol = chord.GetSymbol();        // "CMaj7"
var transposed = chord.Transpose(interval);
var enharmonic = chord.GetEnharmonicEquivalent();
```

### KeySignature Methods

```C#
// Construction
var key = new KeySignature(tonic, KeyMode.Major);

// Properties
Note tonic = key.Tonic;
KeyMode mode = key.Mode;
int accidentalCount = key.AccidentalCount;
List<NoteName> alteredNotes = key.AlteredNotes;

// Methods
Alteration alteration = key.GetAlteration(noteName);
var relative = key.GetRelativeKey();
var parallel = key.GetParallelKey();
var enharmonics = key.GetEnharmonicEquivalents();
var next = key.NextInCircle();
var previous = key.PreviousInCircle();
```

### ChordProgression Methods

```C#
// Construction
var progression = new ChordProgression(key);

// Properties
KeySignature key = progression.Key;

// Methods
var chord = progression.GetChordByDegree(5);      // Dominant
var seventh = progression.GetChordByDegree(2, true); // ii7
var chords = progression.ParseProgression("I - V - vi - IV");
```

## Extension Methods

The library uses extension methods sparingly, preferring explicit methods on domain objects.

## Design Principles

### Immutability
All domain objects are immutable. Methods that appear to modify objects actually return new instances.

```C#
var c = new Note(NoteName.C, 4);
var d = c.TransposeBySemitones(2);  // c is unchanged, d is new
```

### Value Equality
Domain objects implement value equality based on their properties.

```C#
var note1 = new Note(NoteName.C, Alteration.Natural, 4);
var note2 = new Note(NoteName.C, Alteration.Natural, 4);
bool equal = note1.Equals(note2);  // true
```

### Validation
Constructors validate input and throw meaningful exceptions.

```C#
try
{
    var note = new Note(NoteName.C, Alteration.Natural, 15); // Invalid octave
}
catch (ArgumentOutOfRangeException ex)
{
    // "Octave must be between -1 and 10"
}
```

## Performance Considerations

### Caching
Frequently calculated values are cached:
- Note frequencies
- Interval semitones
- Scale note collections

### Lazy Evaluation
Scale notes are generated on-demand using `yield return`.

### Memory Usage
Domain objects are lightweight:
- Note: ~12 bytes
- Interval: ~8 bytes
- Chord: ~16 bytes + notes
- Scale: ~16 bytes + pattern

## Thread Safety

All domain objects are immutable and therefore thread-safe for read operations. No synchronization is needed when sharing objects between threads.

## Versioning

The library follows semantic versioning:
- Major version: Breaking changes
- Minor version: New features, backward compatible
- Patch version: Bug fixes

## See Also

<seealso>
    <category ref="detailed-api">
        <a href="api-note.md">Note API Reference</a>
        <a href="api-interval.md">Interval API Reference</a>
        <a href="api-chord.md">Chord API Reference</a>
        <a href="api-scale.md">Scale API Reference</a>
        <a href="api-keysignature.md">KeySignature API Reference</a>
    </category>
</seealso>