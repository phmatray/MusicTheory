# Scale Generation

The `Scale` class generates sequences of notes following specific interval patterns. The library supports traditional scales, modal scales, and exotic scales.

## Understanding Scales

A scale consists of:

<deflist>
    <def title="Root Note">
        The starting note of the scale (tonic)
    </def>
    <def title="Scale Type">
        The pattern of intervals that defines the scale (Major, Minor, Dorian, etc.)
    </def>
    <def title="Interval Pattern">
        The sequence of whole steps (W) and half steps (H) between notes
    </def>
</deflist>

## Supported Scale Types

<tabs>
    <tab title="Traditional">
        <table>
            <tr>
                <th>Scale</th>
                <th>Pattern</th>
                <th>Example (C)</th>
            </tr>
            <tr>
                <td>Major</td>
                <td>W-W-H-W-W-W-H</td>
                <td>C-D-E-F-G-A-B-C</td>
            </tr>
            <tr>
                <td>Natural Minor</td>
                <td>W-H-W-W-H-W-W</td>
                <td>C-D-Eb-F-G-Ab-Bb-C</td>
            </tr>
            <tr>
                <td>Harmonic Minor</td>
                <td>W-H-W-W-H-WH-H</td>
                <td>C-D-Eb-F-G-Ab-B-C</td>
            </tr>
            <tr>
                <td>Melodic Minor</td>
                <td>W-H-W-W-W-W-H</td>
                <td>C-D-Eb-F-G-A-B-C</td>
            </tr>
        </table>
    </tab>
    <tab title="Modal">
        <table>
            <tr>
                <th>Mode</th>
                <th>Pattern</th>
                <th>Example (C)</th>
            </tr>
            <tr>
                <td>Ionian</td>
                <td>W-W-H-W-W-W-H</td>
                <td>C-D-E-F-G-A-B-C</td>
            </tr>
            <tr>
                <td>Dorian</td>
                <td>W-H-W-W-W-H-W</td>
                <td>C-D-Eb-F-G-A-Bb-C</td>
            </tr>
            <tr>
                <td>Phrygian</td>
                <td>H-W-W-W-H-W-W</td>
                <td>C-Db-Eb-F-G-Ab-Bb-C</td>
            </tr>
            <tr>
                <td>Lydian</td>
                <td>W-W-W-H-W-W-H</td>
                <td>C-D-E-F#-G-A-B-C</td>
            </tr>
            <tr>
                <td>Mixolydian</td>
                <td>W-W-H-W-W-H-W</td>
                <td>C-D-E-F-G-A-Bb-C</td>
            </tr>
            <tr>
                <td>Aeolian</td>
                <td>W-H-W-W-H-W-W</td>
                <td>C-D-Eb-F-G-Ab-Bb-C</td>
            </tr>
            <tr>
                <td>Locrian</td>
                <td>H-W-W-H-W-W-W</td>
                <td>C-Db-Eb-F-Gb-Ab-Bb-C</td>
            </tr>
        </table>
    </tab>
    <tab title="Pentatonic and Exotic">
        <table>
            <tr>
                <th>Scale</th>
                <th>Notes</th>
                <th>Example (C)</th>
            </tr>
            <tr>
                <td>Pentatonic Major</td>
                <td>1-2-3-5-6</td>
                <td>C-D-E-G-A</td>
            </tr>
            <tr>
                <td>Pentatonic Minor</td>
                <td>1-b3-4-5-b7</td>
                <td>C-Eb-F-G-Bb</td>
            </tr>
            <tr>
                <td>Blues</td>
                <td>1-b3-4-b5-5-b7</td>
                <td>C-Eb-F-Gb-G-Bb</td>
            </tr>
            <tr>
                <td>Chromatic</td>
                <td>All 12 notes</td>
                <td>C-C#-D-D#-E-F...</td>
            </tr>
            <tr>
                <td>Whole Tone</td>
                <td>All whole steps</td>
                <td>C-D-E-F#-G#-A#</td>
            </tr>
        </table>
    </tab>
</tabs>

## Creating Scales

### Basic Scale Creation

```C#
// Create root note
var c4 = new Note(NoteName.C, Alteration.Natural, 4);

// Traditional scales
var cMajor = new Scale(c4, ScaleType.Major);
var aMinor = new Scale(new Note(NoteName.A, 3), ScaleType.NaturalMinor);
var dHarmonic = new Scale(new Note(NoteName.D, 4), ScaleType.HarmonicMinor);
var gMelodic = new Scale(new Note(NoteName.G, 4), ScaleType.MelodicMinor);

// Modal scales
var dDorian = new Scale(new Note(NoteName.D, 4), ScaleType.Dorian);
var ePhrygian = new Scale(new Note(NoteName.E, 4), ScaleType.Phrygian);
var fLydian = new Scale(new Note(NoteName.F, 4), ScaleType.Lydian);
var gMixolydian = new Scale(new Note(NoteName.G, 4), ScaleType.Mixolydian);

// Pentatonic and exotic scales
var cPentatonic = new Scale(c4, ScaleType.PentatonicMajor);
var aBlues = new Scale(new Note(NoteName.A, 3), ScaleType.Blues);
var cWholeTone = new Scale(c4, ScaleType.WholeTone);
```

## Working with Scale Notes

### Generating Scale Notes

```C#
var cMajor = new Scale(new Note(NoteName.C, 4), ScaleType.Major);

// Get all notes (includes octave)
var notes = cMajor.GetNotes().ToList();
// [C4, D4, E4, F4, G4, A4, B4, C5]

// Get notes without octave
var uniqueNotes = cMajor.GetNotes().Take(7).ToList();
// [C4, D4, E4, F4, G4, A4, B4]

// Iterate through scale
foreach (var note in cMajor.GetNotes())
{
    Console.WriteLine($"{note} - Frequency: {note.Frequency:F2} Hz");
}
```

### Scale Degrees

Work with scale degrees (1-7):

```C#
var gMajor = new Scale(new Note(NoteName.G, 4), ScaleType.Major);

// Get note at specific degree
var tonic = gMajor.GetNotes().ElementAt(0);      // G (1st degree)
var dominant = gMajor.GetNotes().ElementAt(4);   // D (5th degree)
var leadingTone = gMajor.GetNotes().ElementAt(6); // F# (7th degree)

// Get degree of a note
var noteD = new Note(NoteName.D, 5);
int degree = gMajor.GetDegree(noteD); // 5

// Degree names
var degreeNames = new[] 
{
    "Tonic",      // 1
    "Supertonic", // 2
    "Mediant",    // 3
    "Subdominant",// 4
    "Dominant",   // 5
    "Submediant", // 6
    "Leading Tone"// 7
};
```

## Scale Operations

### Checking Note Membership

```C#
var dMajor = new Scale(new Note(NoteName.D, 4), ScaleType.Major);

// Check if notes are in the scale
bool hasF = dMajor.Contains(new Note(NoteName.F, Alteration.Natural)); // false
bool hasFSharp = dMajor.Contains(new Note(NoteName.F, Alteration.Sharp)); // true
bool hasA = dMajor.Contains(new Note(NoteName.A)); // true

// Works across octaves
bool hasHighA = dMajor.Contains(new Note(NoteName.A, 5)); // true
```

### Finding Next/Previous Notes

```C#
var cMajor = new Scale(new Note(NoteName.C, 4), ScaleType.Major);

// Get next note in scale
var afterE = cMajor.GetNextNoteInScale(new Note(NoteName.E, 4)); // F4
var afterB = cMajor.GetNextNoteInScale(new Note(NoteName.B, 4)); // C5

// Get previous note in scale
var beforeG = cMajor.GetPreviousNoteInScale(new Note(NoteName.G, 4)); // F4
var beforeC = cMajor.GetPreviousNoteInScale(new Note(NoteName.C, 4)); // B3

// Handle notes not in scale
var afterFSharp = cMajor.GetNextNoteInScale(new Note(NoteName.F, Alteration.Sharp, 4)); // G4
```

### Transposing Scales

```C#
var cMajor = new Scale(new Note(NoteName.C, 4), ScaleType.Major);

// Transpose up a perfect fifth
var gMajor = cMajor.Transpose(new Interval(IntervalQuality.Perfect, 5));

// Transpose down a major third
var abMajor = cMajor.Transpose(
    new Interval(IntervalQuality.Major, 3), 
    Direction.Down
);

// Verify transposition
var gNotes = gMajor.GetNotes().Take(7).Select(n => n.ToString());
// ["G4", "A4", "B4", "C5", "D5", "E5", "F#5"]
```

## Modal Relationships

Understanding modes as rotations of major scale:

```C#
// C major scale notes
var cMajor = new Scale(new Note(NoteName.C, 4), ScaleType.Major);
var majorNotes = cMajor.GetNotes().Take(7).ToList();

// Each mode starts on a different degree of C major
var cIonian = new Scale(new Note(NoteName.C, 4), ScaleType.Ionian);     // C-D-E-F-G-A-B
var dDorian = new Scale(new Note(NoteName.D, 4), ScaleType.Dorian);     // D-E-F-G-A-B-C
var ePhrygian = new Scale(new Note(NoteName.E, 4), ScaleType.Phrygian); // E-F-G-A-B-C-D
var fLydian = new Scale(new Note(NoteName.F, 4), ScaleType.Lydian);     // F-G-A-B-C-D-E
var gMixolydian = new Scale(new Note(NoteName.G, 4), ScaleType.Mixolydian); // G-A-B-C-D-E-F
var aAeolian = new Scale(new Note(NoteName.A, 4), ScaleType.Aeolian);   // A-B-C-D-E-F-G
var bLocrian = new Scale(new Note(NoteName.B, 4), ScaleType.Locrian);   // B-C-D-E-F-G-A

// Aeolian mode is the same as natural minor
var aMinor = new Scale(new Note(NoteName.A, 4), ScaleType.NaturalMinor);
// Same notes as A Aeolian
```

## Common Scale Patterns

### Building Chords from Scales

```C#
var cMajor = new Scale(new Note(NoteName.C, 4), ScaleType.Major);
var scaleNotes = cMajor.GetNotes().Take(7).ToList();

// Build triads on each scale degree
var triads = new List<(int degree, string name, ChordQuality quality)>();
for (int i = 0; i < 7; i++)
{
    var root = scaleNotes[i];
    var third = scaleNotes[(i + 2) % 7];
    var fifth = scaleNotes[(i + 4) % 7];
    
    // Determine chord quality based on intervals
    // In major scale: I, IV, V are major; ii, iii, vi are minor; vii° is diminished
}

// Result: C, Dm, Em, F, G, Am, Bdim
```

### Pentatonic Applications

```C#
// Major pentatonic - remove 4th and 7th degrees
var cMajorPent = new Scale(new Note(NoteName.C, 4), ScaleType.PentatonicMajor);
// C-D-E-G-A (no F or B)

// Minor pentatonic - remove 2nd and 6th degrees
var aMinorPent = new Scale(new Note(NoteName.A, 3), ScaleType.PentatonicMinor);
// A-C-D-E-G (no B or F)

// Blues scale - minor pentatonic + blue note
var aBlues = new Scale(new Note(NoteName.A, 3), ScaleType.Blues);
// A-C-D-Eb-E-G (added Eb as blue note)
```

### Chromatic Passages

```C#
// Generate chromatic scale
var chromatic = new Scale(new Note(NoteName.C, 4), ScaleType.Chromatic);
var chromaticNotes = chromatic.GetNotes().Take(12).ToList();
// All 12 semitones: C, C#, D, D#, E, F, F#, G, G#, A, A#, B

// Use for chromatic runs
var startNote = new Note(NoteName.C, 4);
var endNote = new Note(NoteName.E, 4);
var chromaticRun = chromatic.GetNotes()
    .SkipWhile(n => n.CompareTo(startNote) < 0)
    .TakeWhile(n => n.CompareTo(endNote) <= 0);
// C, C#, D, D#, E
```

## Scale Analysis

### Identifying Scale Types

```C#
// Given a set of notes, identify possible scales
var notes = new[]
{
    new Note(NoteName.C),
    new Note(NoteName.D),
    new Note(NoteName.E),
    new Note(NoteName.F),
    new Note(NoteName.G),
    new Note(NoteName.A),
    new Note(NoteName.B)
};

// Check against known scale patterns
// These notes match C major / A natural minor
```

### Key Signature Integration

```C#
// Create scale from key signature
var gMajorKey = new KeySignature(new Note(NoteName.G), KeyMode.Major);
var gMajorScale = new Scale(gMajorKey.Tonic, ScaleType.Major);

// The scale will have F# (from key signature)
var hasSharp = gMajorScale.Contains(new Note(NoteName.F, Alteration.Sharp)); // true
```

## Advanced Topics

### Exotic Scale Patterns

```C#
// Whole tone scale - all whole steps
var wholeTone = new Scale(new Note(NoteName.C, 4), ScaleType.WholeTone);
// C-D-E-F#-G#-A# (6 notes, no half steps)

// Hungarian minor (harmonic minor with raised 4th)
// Would need custom implementation or extension

// Bebop scales (8-note scales)
// Would need custom implementation
```

### Scale Relationships

```C#
// Relative major/minor
var cMajor = new Scale(new Note(NoteName.C, 4), ScaleType.Major);
var aMinor = new Scale(new Note(NoteName.A, 3), ScaleType.NaturalMinor);
// Same notes, different tonic

// Parallel major/minor
var cMinor = new Scale(new Note(NoteName.C, 4), ScaleType.NaturalMinor);
// Same tonic, different notes (Eb, Ab, Bb instead of E, A, B)
```

## Best Practices

- **Choose appropriate scale types**: Match the scale to the musical context
- **Consider enharmonic spelling**: Use appropriate note names for the key
- **Understand modal characteristics**: Each mode has unique color and feeling
- **Use scale degrees**: Think in terms of scale degrees for transposition

## See Also

<seealso>
    <category ref="related">
        <a href="notes.md">Working with Notes</a>
        <a href="intervals.md">Scale Intervals</a>
        <a href="key-signatures.md">Scales and Keys</a>
        <a href="working-with-modes.md">Modal Theory Tutorial</a>
    </category>
    <category ref="api">
        <a href="api-scale.md">Scale API Reference</a>
        <a href="enums.md">ScaleType Enum</a>
    </category>
</seealso>