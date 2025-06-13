# Building Chords

The `Chord` class represents a collection of notes played simultaneously. The MusicTheory library supports over 40 chord types, from simple triads to complex jazz voicings.

## Understanding Chords

A chord consists of:

<deflist>
    <def title="Root Note">
        The fundamental note upon which the chord is built
    </def>
    <def title="Chord Type">
        Defines the intervals that make up the chord (Major, Minor7, Dom7b9, etc.)
    </def>
    <def title="Inversion">
        Which note is in the bass (root position, first inversion, etc.)
    </def>
    <def title="Extensions">
        Additional notes beyond the basic chord structure
    </def>
</deflist>

## Supported Chord Types

The library supports 40+ chord types across multiple categories:

<tabs>
    <tab title="Triads">
        <table>
            <tr>
                <th>Type</th>
                <th>Symbol</th>
                <th>Intervals</th>
                <th>Example</th>
            </tr>
            <tr>
                <td>Major</td>
                <td>C</td>
                <td>1-3-5</td>
                <td>C-E-G</td>
            </tr>
            <tr>
                <td>Minor</td>
                <td>Cm</td>
                <td>1-♭3-5</td>
                <td>C-E♭-G</td>
            </tr>
            <tr>
                <td>Diminished</td>
                <td>C°</td>
                <td>1-♭3-♭5</td>
                <td>C-E♭-G♭</td>
            </tr>
            <tr>
                <td>Augmented</td>
                <td>C+</td>
                <td>1-3-♯5</td>
                <td>C-E-G♯</td>
            </tr>
        </table>
    </tab>
    <tab title="Seventh Chords">
        <table>
            <tr>
                <th>Type</th>
                <th>Symbol</th>
                <th>Intervals</th>
                <th>Example</th>
            </tr>
            <tr>
                <td>Major 7th</td>
                <td>Cmaj7</td>
                <td>1-3-5-7</td>
                <td>C-E-G-B</td>
            </tr>
            <tr>
                <td>Minor 7th</td>
                <td>Cm7</td>
                <td>1-♭3-5-♭7</td>
                <td>C-E♭-G-B♭</td>
            </tr>
            <tr>
                <td>Dominant 7th</td>
                <td>C7</td>
                <td>1-3-5-♭7</td>
                <td>C-E-G-B♭</td>
            </tr>
            <tr>
                <td>Half-diminished</td>
                <td>Cø7</td>
                <td>1-♭3-♭5-♭7</td>
                <td>C-E♭-G♭-B♭</td>
            </tr>
            <tr>
                <td>Diminished 7th</td>
                <td>C°7</td>
                <td>1-♭3-♭5-♭♭7</td>
                <td>C-E♭-G♭-B♭♭</td>
            </tr>
        </table>
    </tab>
    <tab title="Extended">
        <table>
            <tr>
                <th>Type</th>
                <th>Symbol</th>
                <th>Notes</th>
            </tr>
            <tr>
                <td>Major 9th</td>
                <td>Cmaj9</td>
                <td>C-E-G-B-D</td>
            </tr>
            <tr>
                <td>Minor 11th</td>
                <td>Cm11</td>
                <td>C-E♭-G-B♭-D-F</td>
            </tr>
            <tr>
                <td>Dominant 13th</td>
                <td>C13</td>
                <td>C-E-G-B♭-D-F-A</td>
            </tr>
        </table>
    </tab>
    <tab title="Altered">
        <table>
            <tr>
                <th>Type</th>
                <th>Symbol</th>
                <th>Notes</th>
            </tr>
            <tr>
                <td>Dom7♭5</td>
                <td>C7♭5</td>
                <td>C-E-G♭-B♭</td>
            </tr>
            <tr>
                <td>Dom7♯9</td>
                <td>C7♯9</td>
                <td>C-E-G-B♭-D♯</td>
            </tr>
            <tr>
                <td>Alt</td>
                <td>C7alt</td>
                <td>C-E-G♭-G♯-B♭-D♭</td>
            </tr>
        </table>
    </tab>
</tabs>

## Creating Chords

### Basic Chord Creation

```C#
// Create root note
var c4 = new Note(NoteName.C, Alteration.Natural, 4);

// Create basic triads
var cMajor = new Chord(c4, ChordType.Major);
var cMinor = new Chord(c4, ChordType.Minor);
var cDim = new Chord(c4, ChordType.Diminished);
var cAug = new Chord(c4, ChordType.Augmented);

// Create seventh chords
var cMaj7 = new Chord(c4, ChordType.Major7);
var cm7 = new Chord(c4, ChordType.Minor7);
var c7 = new Chord(c4, ChordType.Dominant7);
var cHalfDim = new Chord(c4, ChordType.HalfDiminished7);
```

### Extended and Altered Chords

```C#
// Extended chords
var cMaj9 = new Chord(c4, ChordType.Major9);
var cm11 = new Chord(c4, ChordType.Minor11);
var c13 = new Chord(c4, ChordType.Dominant13);

// Altered chords
var c7b5 = new Chord(c4, ChordType.Dominant7Flat5);
var c7sharp9 = new Chord(c4, ChordType.Dominant7Sharp9);
var c7alt = new Chord(c4, ChordType.Dominant7Alt);

// Suspended chords
var csus2 = new Chord(c4, ChordType.Sus2);
var csus4 = new Chord(c4, ChordType.Sus4);
var c7sus4 = new Chord(c4, ChordType.Dominant7Sus4);
```

## Working with Chord Properties

### Getting Chord Information

```C#
var cm7 = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordType.Minor7);

// Basic properties
Note root = cm7.Root;                    // C4
ChordType type = cm7.Type;               // ChordType.Minor7
ChordInversion inv = cm7.Inversion;      // ChordInversion.Root

// Get chord symbol
string symbol = cm7.GetSymbol();         // "Cm7"

// Get notes in the chord
var notes = cm7.GetNotes().ToList();    // [C4, Eb4, G4, Bb4]
```

### Chord Notes and Voicings

```C#
var cMaj7 = new Chord(new Note(NoteName.C, Alteration.Natural, 4), ChordType.Major7);

// Get all notes
foreach (var note in cMaj7.GetNotes())
{
    Console.WriteLine(note); // C4, E4, G4, B4
}

// Get specific note
var third = cMaj7.GetNotes().ElementAt(1);  // E4
var seventh = cMaj7.GetNotes().ElementAt(3); // B4
```

## Chord Inversions

Inversions change which note is in the bass:

```C#
var c = new Note(NoteName.C, Alteration.Natural, 4);
var cMajor = new Chord(c, ChordType.Major);

// Create inversions
var firstInversion = cMajor.WithInversion(ChordInversion.First);   // E in bass
var secondInversion = cMajor.WithInversion(ChordInversion.Second); // G in bass

// Get bass note
var bassNote = firstInversion.GetBassNote(); // E4

// Get notes in inversion order
var inversionNotes = firstInversion.GetNotesInInversion(); // E4, G4, C5

// Slash chord notation
string slashChord = firstInversion.GetSlashChordNotation(); // "C/E"
```

### Inversion Types

- **Root Position**: Root in bass (C major: C-E-G)
- **First Inversion**: Third in bass (C major: E-G-C)
- **Second Inversion**: Fifth in bass (C major: G-C-E)
- **Third Inversion**: Seventh in bass (C7: Bb-C-E-G) - requires 7th chord

## Adding Extensions

Add extensions to existing chords using the fluent API:

```C#
var c = new Note(NoteName.C, Alteration.Natural, 4);
var cMajor = new Chord(c, ChordType.Major);

// Add a major 7th
var cMaj7 = cMajor.AddExtension(7, IntervalQuality.Major);

// Add multiple extensions (fluent API)
var cMaj9add13 = cMajor
    .AddExtension(7, IntervalQuality.Major)
    .AddExtension(9, IntervalQuality.Major)
    .AddExtension(13, IntervalQuality.Major);

// Create complex voicings
var c7sharp11 = new Chord(c, ChordType.Dominant7)
    .AddExtension(11, IntervalQuality.Augmented);
```

## Chord Progressions

Work with chords in the context of a key:

```C#
// Define the key
var cMajorKey = new KeySignature(new Note(NoteName.C), KeyMode.Major);
var progression = new ChordProgression(cMajorKey);

// Get diatonic chords
var diatonicChords = progression.GetDiatonicChords().ToList();
// I (C), ii (Dm), iii (Em), IV (F), V (G), vi (Am), vii° (Bdim)

// Common progressions
var twoFiveOne = progression.ParseProgression("ii7 - V7 - IMaj7");
var popProgression = progression.ParseProgression("I - V - vi - IV");

// Get chord by scale degree
var dominant = progression.GetChordByDegree(5);        // G major
var submediant = progression.GetChordByDegree(6);      // A minor
```

## Transposition

Transpose chords to different keys:

```C#
var c = new Note(NoteName.C, Alteration.Natural, 4);
var cm7 = new Chord(c, ChordType.Minor7);

// Transpose up a perfect fifth
var gm7 = cm7.Transpose(new Interval(IntervalQuality.Perfect, 5), Direction.Up);

// Transpose down a major third
var abm7 = cm7.Transpose(new Interval(IntervalQuality.Major, 3), Direction.Down);

// Verify transposition
Console.WriteLine(gm7.GetSymbol());  // "Gm7"
Console.WriteLine(abm7.GetSymbol()); // "Abm7"
```

## Common Chord Patterns

### Jazz Chord Voicings

```C#
// ii-V-I in C major
var dm7 = new Chord(new Note(NoteName.D, 4), ChordType.Minor7);
var g7 = new Chord(new Note(NoteName.G, 3), ChordType.Dominant7);
var cMaj7 = new Chord(new Note(NoteName.C, 4), ChordType.Major7);

// Altered dominant
var g7alt = new Chord(new Note(NoteName.G, 3), ChordType.Dominant7Alt);

// Extended voicings
var dm11 = new Chord(new Note(NoteName.D, 4), ChordType.Minor11);
var g13 = new Chord(new Note(NoteName.G, 3), ChordType.Dominant13);
```

### Pop/Rock Patterns

```C#
// Power chords
var a5 = new Chord(new Note(NoteName.A, 2), ChordType.Power5);
var d5 = new Chord(new Note(NoteName.D, 3), ChordType.Power5);

// Add chords
var cAdd9 = new Chord(new Note(NoteName.C, 4), ChordType.MajorAdd9);
var fAdd9 = new Chord(new Note(NoteName.F, 4), ChordType.MajorAdd9);

// Suspended chords
var dsus2 = new Chord(new Note(NoteName.D, 4), ChordType.Sus2);
var gsus4 = new Chord(new Note(NoteName.G, 4), ChordType.Sus4);
```

## Enharmonic Equivalents

Get enharmonically equivalent chords:

```C#
var cSharpMajor = new Chord(
    new Note(NoteName.C, Alteration.Sharp, 4), 
    ChordType.Major
);

var enharmonic = cSharpMajor.GetEnharmonicEquivalent(); // Db major
Console.WriteLine(enharmonic?.GetSymbol()); // "Db"
```

## Best Practices

- **Choose appropriate chord types**: Use the specific ChordType enum values rather than building chords with extensions
- **Consider voice leading**: When creating progressions, consider the movement between chord tones
- **Use proper enharmonic spelling**: Choose sharps or flats based on the key context
- **Leverage the fluent API**: Chain methods for readable code

## Advanced Topics

### Voice Leading

Consider smooth voice movement between chords:

```C#
// Good voice leading: minimal movement
var c = new Chord(new Note(NoteName.C, 4), ChordType.Major);
var am = new Chord(new Note(NoteName.A, 3), ChordType.Minor);
var f = new Chord(new Note(NoteName.F, 4), ChordType.Major);
var g = new Chord(new Note(NoteName.G, 3), ChordType.Major);

// Common tones: C is in both C major and A minor
// Stepwise motion: E → E, G → A
```

### Chord Substitutions

Common jazz substitutions:

```C#
// Tritone substitution
var g7 = new Chord(new Note(NoteName.G, 3), ChordType.Dominant7);
var db7 = new Chord(new Note(NoteName.D, Alteration.Flat, 3), ChordType.Dominant7);
// G7 can be substituted with Db7 (tritone away)

// Minor for major substitution
var cMajor = new Chord(new Note(NoteName.C, 4), ChordType.Major);
var aMinor = new Chord(new Note(NoteName.A, 3), ChordType.Minor);
// C major can be substituted with A minor (relative minor)
```

## See Also

<seealso>
    <category ref="related">
        <a href="notes.md">Working with Notes</a>
        <a href="intervals.md">Understanding Intervals</a>
        <a href="chord-progressions.md">Building Chord Progressions</a>
        <a href="key-signatures.md">Chords in Key Context</a>
    </category>
    <category ref="api">
        <a href="api-chord.md">Chord API Reference</a>
        <a href="enums.md">ChordType Enum Reference</a>
    </category>
</seealso>