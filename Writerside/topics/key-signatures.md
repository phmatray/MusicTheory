# Working with Key Signatures

The `KeySignature` class represents the set of sharps or flats that define a musical key. It provides methods for navigating the circle of fifths and understanding key relationships.

## Understanding Key Signatures

A key signature defines:

<deflist>
    <def title="Tonic">
        The home note or tonal center of the key
    </def>
    <def title="Mode">
        Whether the key is major or minor
    </def>
    <def title="Accidentals">
        The sharps or flats that appear in the key
    </def>
    <def title="Scale">
        The collection of notes that belong to the key
    </def>
</deflist>

## Creating Key Signatures

### Basic Creation

```C#
// Major keys
var cMajor = new KeySignature(new Note(NoteName.C), KeyMode.Major);      // 0 sharps/flats
var gMajor = new KeySignature(new Note(NoteName.G), KeyMode.Major);      // 1 sharp (F#)
var dMajor = new KeySignature(new Note(NoteName.D), KeyMode.Major);      // 2 sharps (F#, C#)
var fMajor = new KeySignature(new Note(NoteName.F), KeyMode.Major);      // 1 flat (Bb)
var bbMajor = new KeySignature(new Note(NoteName.B, Alteration.Flat), KeyMode.Major); // 2 flats

// Minor keys
var aMinor = new KeySignature(new Note(NoteName.A), KeyMode.Minor);      // 0 sharps/flats
var eMinor = new KeySignature(new Note(NoteName.E), KeyMode.Minor);      // 1 sharp (F#)
var bMinor = new KeySignature(new Note(NoteName.B), KeyMode.Minor);      // 2 sharps (F#, C#)
var dMinor = new KeySignature(new Note(NoteName.D), KeyMode.Minor);      // 1 flat (Bb)
```

### Key Properties

```C#
var gMajor = new KeySignature(new Note(NoteName.G), KeyMode.Major);

// Basic properties
Note tonic = gMajor.Tonic;                    // G
KeyMode mode = gMajor.Mode;                   // Major
int accidentals = gMajor.AccidentalCount;     // 1 (positive = sharps)

// Get altered notes
var alteredNotes = gMajor.AlteredNotes;       // [F]
var alteration = gMajor.GetAlteration(NoteName.F); // Sharp
```

## Circle of Fifths

Navigate keys using the circle of fifths:

<tabs>
    <tab title="Sharp Keys">
        <code-block lang="C#"><![CDATA[
var c = new KeySignature(new Note(NoteName.C), KeyMode.Major);

// Moving clockwise (adding sharps)
var g = c.NextInCircle();      // G major (1 sharp: F#)
var d = g.NextInCircle();      // D major (2 sharps: F#, C#)
var a = d.NextInCircle();      // A major (3 sharps: F#, C#, G#)
var e = a.NextInCircle();      // E major (4 sharps: F#, C#, G#, D#)
var b = e.NextInCircle();      // B major (5 sharps: F#, C#, G#, D#, A#)
var fSharp = b.NextInCircle(); // F# major (6 sharps: F#, C#, G#, D#, A#, E#)
var cSharp = fSharp.NextInCircle(); // C# major (7 sharps)

// Order of sharps: F-sharp, C-sharp, G-sharp, D-sharp, A-sharp, E-sharp, B-sharp
]]></code-block>
    </tab>
    <tab title="Flat Keys">
        <code-block lang="C#"><![CDATA[
var c = new KeySignature(new Note(NoteName.C), KeyMode.Major);

// Moving counter-clockwise (adding flats)
var f = c.PreviousInCircle();  // F major (1 flat: Bb)
var bb = f.PreviousInCircle();  // Bb major (2 flats: Bb, Eb)
var eb = bb.PreviousInCircle(); // Eb major (3 flats: Bb, Eb, Ab)
var ab = eb.PreviousInCircle(); // Ab major (4 flats: Bb, Eb, Ab, Db)
var db = ab.PreviousInCircle(); // Db major (5 flats: Bb, Eb, Ab, Db, Gb)
var gb = db.PreviousInCircle(); // Gb major (6 flats: Bb, Eb, Ab, Db, Gb, Cb)
var cb = gb.PreviousInCircle(); // Cb major (7 flats)

// Order of flats: B-flat, E-flat, A-flat, D-flat, G-flat, C-flat, F-flat
]]></code-block>
    </tab>
</tabs>

## Key Relationships

### Relative Keys

Relative keys share the same key signature but have different tonics:

```C#
// Major to relative minor (down a minor 3rd)
var cMajor = new KeySignature(new Note(NoteName.C), KeyMode.Major);
var aMinor = cMajor.GetRelative(); // A minor

var gMajor = new KeySignature(new Note(NoteName.G), KeyMode.Major);
var eMinor = gMajor.GetRelative(); // E minor

// Minor to relative major (up a minor 3rd)
var dMinor = new KeySignature(new Note(NoteName.D), KeyMode.Minor);
var fMajor = dMinor.GetRelative(); // F major

// They share the same sharps/flats
Console.WriteLine(cMajor.AccidentalCount); // 0
Console.WriteLine(aMinor.AccidentalCount); // 0

Console.WriteLine(gMajor.AccidentalCount); // 1 (F#)
Console.WriteLine(eMinor.AccidentalCount); // 1 (F#)
```

### Parallel Keys

Parallel keys share the same tonic but have different modes:

```C#
// Major to parallel minor
var cMajor = new KeySignature(new Note(NoteName.C), KeyMode.Major);
var cMinor = cMajor.GetParallel(); // C minor

// They have different key signatures
Console.WriteLine(cMajor.AccidentalCount); // 0
Console.WriteLine(cMinor.AccidentalCount); // -3 (Bb, Eb, Ab)

// Minor to parallel major
var aMinor = new KeySignature(new Note(NoteName.A), KeyMode.Minor);
var aMajor = aMinor.GetParallel(); // A major

Console.WriteLine(aMinor.AccidentalCount); // 0
Console.WriteLine(aMajor.AccidentalCount); // 3 (F#, C#, G#)
```

### Dominant and Subdominant

Navigate to closely related keys:

```C#
var c = new KeySignature(new Note(NoteName.C), KeyMode.Major);

// Dominant (5th above) - one sharp more
var g = c.GetDominant(); // G major

// Subdominant (5th below) - one flat more
var f = c.GetSubdominant(); // F major

// These are the most closely related keys
// They differ by only one accidental
```

## Enharmonic Keys

Some keys can be spelled differently but sound the same:

```C#
// F# major and Gb major are enharmonic
var fSharpMajor = new KeySignature(
    new Note(NoteName.F, Alteration.Sharp), 
    KeyMode.Major
); // 6 sharps

var gbMajor = new KeySignature(
    new Note(NoteName.G, Alteration.Flat), 
    KeyMode.Major
); // 6 flats

// Get enharmonic equivalents
var enharmonics = fSharpMajor.GetEnharmonicEquivalents();
// Returns Gb major

// Common enharmonic pairs:
// C# major (7 sharps) = Db major (5 flats)
// F# major (6 sharps) = Gb major (6 flats)
// B major (5 sharps) = Cb major (7 flats)
```

## Working with Scales and Chords

### Building Scales from Keys

```C#
var dMajor = new KeySignature(new Note(NoteName.D), KeyMode.Major);

// Create the corresponding scale
var scale = new Scale(dMajor.Tonic, ScaleType.Major);

// The scale will include the sharps from the key signature
var notes = scale.GetNotes().Take(7).Select(n => n.ToString());
// ["D4", "E4", "F#4", "G4", "A4", "B4", "C#5"]

// Check if a note needs alteration in this key
bool fIsSharp = dMajor.GetAlteration(NoteName.F) == Alteration.Sharp; // true
bool cIsSharp = dMajor.GetAlteration(NoteName.C) == Alteration.Sharp; // true
bool gIsNatural = dMajor.GetAlteration(NoteName.G) == Alteration.Natural; // true
```

### Diatonic Chords

Build chords that belong to a key:

```C#
var cMajor = new KeySignature(new Note(NoteName.C), KeyMode.Major);
var progression = new ChordProgression(cMajor);

// Get all diatonic triads
var diatonicChords = progression.GetDiatonicChords().ToList();
// I:   C major
// ii:  D minor
// iii: E minor
// IV:  F major
// V:   G major
// vi:  A minor
// vii°: B diminished

// Roman numeral analysis
string dominant = progression.GetRomanNumeral(5); // "V"
string submediant = progression.GetRomanNumeral(6); // "vi" (lowercase for minor)
```

## Common Key Patterns

### Key Signatures by Accidental Count

<tabs>
    <tab title="Sharp Keys">
        <table>
            <tr>
                <th>Sharps</th>
                <th>Major Key</th>
                <th>Minor Key</th>
                <th>Sharps Added</th>
            </tr>
            <tr>
                <td>0</td>
                <td>C major</td>
                <td>A minor</td>
                <td>-</td>
            </tr>
            <tr>
                <td>1</td>
                <td>G major</td>
                <td>E minor</td>
                <td>F#</td>
            </tr>
            <tr>
                <td>2</td>
                <td>D major</td>
                <td>B minor</td>
                <td>F#, C#</td>
            </tr>
            <tr>
                <td>3</td>
                <td>A major</td>
                <td>F# minor</td>
                <td>F#, C#, G#</td>
            </tr>
            <tr>
                <td>4</td>
                <td>E major</td>
                <td>C# minor</td>
                <td>F#, C#, G#, D#</td>
            </tr>
            <tr>
                <td>5</td>
                <td>B major</td>
                <td>G# minor</td>
                <td>F#, C#, G#, D#, A#</td>
            </tr>
            <tr>
                <td>6</td>
                <td>F# major</td>
                <td>D# minor</td>
                <td>F#, C#, G#, D#, A#, E#</td>
            </tr>
            <tr>
                <td>7</td>
                <td>C# major</td>
                <td>A# minor</td>
                <td>F#, C#, G#, D#, A#, E#, B#</td>
            </tr>
        </table>
    </tab>
    <tab title="Flat Keys">
        <table>
            <tr>
                <th>Flats</th>
                <th>Major Key</th>
                <th>Minor Key</th>
                <th>Flats Added</th>
            </tr>
            <tr>
                <td>0</td>
                <td>C major</td>
                <td>A minor</td>
                <td>-</td>
            </tr>
            <tr>
                <td>1</td>
                <td>F major</td>
                <td>D minor</td>
                <td>Bb</td>
            </tr>
            <tr>
                <td>2</td>
                <td>Bb major</td>
                <td>G minor</td>
                <td>Bb, Eb</td>
            </tr>
            <tr>
                <td>3</td>
                <td>Eb major</td>
                <td>C minor</td>
                <td>Bb, Eb, Ab</td>
            </tr>
            <tr>
                <td>4</td>
                <td>Ab major</td>
                <td>F minor</td>
                <td>Bb, Eb, Ab, Db</td>
            </tr>
            <tr>
                <td>5</td>
                <td>Db major</td>
                <td>Bb minor</td>
                <td>Bb, Eb, Ab, Db, Gb</td>
            </tr>
            <tr>
                <td>6</td>
                <td>Gb major</td>
                <td>Eb minor</td>
                <td>Bb, Eb, Ab, Db, Gb, Cb</td>
            </tr>
            <tr>
                <td>7</td>
                <td>Cb major</td>
                <td>Ab minor</td>
                <td>Bb, Eb, Ab, Db, Gb, Cb, Fb</td>
            </tr>
        </table>
    </tab>
</tabs>

### Modulation Paths

Common key changes in music:

```C#
var cMajor = new KeySignature(new Note(NoteName.C), KeyMode.Major);

// Closely related keys (differ by one accidental)
var dominant = cMajor.GetDominant();        // G major
var subdominant = cMajor.GetSubdominant();  // F major
var relative = cMajor.GetRelative();        // A minor
var dominantRelative = dominant.GetRelative(); // E minor
var subdominantRelative = subdominant.GetRelative(); // D minor

// Common modulation: to dominant
// C major → G major (very common in classical music)

// Common modulation: to relative minor
// C major → A minor (mood change)

// Dramatic modulation: to parallel minor
var parallel = cMajor.GetParallel();        // C minor
```

## Best Practices

- **Choose appropriate enharmonic spelling**: Use F# major rather than Gb major when possible (fewer accidentals)
- **Understand key relationships**: Closely related keys share many common tones
- **Consider musical context**: Key choice affects the mood and playability
- **Use the circle of fifths**: It's the foundation of Western harmony

## See Also

<seealso>
    <category ref="related">
        <a href="scales.md">Scales in Different Keys</a>
        <a href="chord-progressions.md">Chord Progressions in Keys</a>
        <a href="transposition.md">Transposing Between Keys</a>
    </category>
    <category ref="api">
        <a href="api-overview.md">KeySignature API Reference</a>
        <a href="enums.md">KeyMode Enum</a>
    </category>
</seealso>