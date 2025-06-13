# KeySignature API Reference

Complete API documentation for the `KeySignature` class.

## Class Definition

```C#
namespace MusicTheory;

public sealed class KeySignature : IEquatable<KeySignature>
{
    public Note Tonic { get; }
    public KeyMode Mode { get; }
    public int AccidentalCount { get; }
    public List<NoteName> AlteredNotes { get; }
}
```

## Constructors

### KeySignature(Note, KeyMode)

Creates a new key signature with the specified tonic and mode.

```C#
public KeySignature(Note tonic, KeyMode mode)
```

**Parameters:**
- `tonic`: The tonic (root) note of the key
- `mode`: The mode (Major or Minor)

**Example:**
```C#
var cMajor = new KeySignature(new Note(NoteName.C), KeyMode.Major);
var aMinor = new KeySignature(new Note(NoteName.A), KeyMode.Minor);
var gMajor = new KeySignature(new Note(NoteName.G), KeyMode.Major);
var ebMinor = new KeySignature(new Note(NoteName.E, Alteration.Flat), KeyMode.Minor);
```

## Properties

### Tonic

```C#
public Note Tonic { get; }
```

Gets the tonic (root) note of the key.

### Mode

```C#
public KeyMode Mode { get; }
```

Gets the mode (Major or Minor).

### AccidentalCount

```C#
public int AccidentalCount { get; }
```

Gets the number of accidentals in the key signature.
- Positive values indicate sharps
- Negative values indicate flats
- Zero indicates no accidentals (C major/A minor)

**Examples:**
- C major: 0
- G major: 1 (one sharp)
- F major: -1 (one flat)
- D major: 2 (two sharps)
- Bb major: -2 (two flats)

### AlteredNotes

```C#
public List<NoteName> AlteredNotes { get; }
```

Gets the list of note names that are altered in this key.

**Order:**
- Sharps: F, C, G, D, A, E, B
- Flats: B, E, A, D, G, C, F

## Instance Methods

### GetAlteration(NoteName)

Gets the alteration for a specific note in this key.

```C#
public Alteration GetAlteration(NoteName noteName)
```

**Parameters:**
- `noteName`: The note name to check

**Returns:** The alteration (Sharp, Flat, or Natural)

**Example:**
```C#
var dMajor = new KeySignature(new Note(NoteName.D), KeyMode.Major);
var fAlteration = dMajor.GetAlteration(NoteName.F); // Sharp
var cAlteration = dMajor.GetAlteration(NoteName.C); // Sharp
var eAlteration = dMajor.GetAlteration(NoteName.E); // Natural
```

### GetRelativeKey()

Gets the relative major or minor key.

```C#
public KeySignature GetRelativeKey()
```

**Returns:** 
- For major keys: the relative minor (down a minor 3rd)
- For minor keys: the relative major (up a minor 3rd)

**Example:**
```C#
var cMajor = new KeySignature(new Note(NoteName.C), KeyMode.Major);
var aMinor = cMajor.GetRelativeKey(); // A minor

var dMinor = new KeySignature(new Note(NoteName.D), KeyMode.Minor);
var fMajor = dMinor.GetRelativeKey(); // F major
```

### GetParallelKey()

Gets the parallel major or minor key.

```C#
public KeySignature GetParallelKey()
```

**Returns:** The key with the same tonic but opposite mode

**Example:**
```C#
var cMajor = new KeySignature(new Note(NoteName.C), KeyMode.Major);
var cMinor = cMajor.GetParallelKey(); // C minor

var aMinor = new KeySignature(new Note(NoteName.A), KeyMode.Minor);
var aMajor = aMinor.GetParallelKey(); // A major
```

### GetEnharmonicEquivalents()

Gets enharmonically equivalent key signatures.

```C#
public List<KeySignature> GetEnharmonicEquivalents()
```

**Returns:** List of enharmonic keys (e.g., C# major ↔ Db major)

**Example:**
```C#
var cSharpMajor = new KeySignature(new Note(NoteName.C, Alteration.Sharp), KeyMode.Major);
var enharmonics = cSharpMajor.GetEnharmonicEquivalents();
// Returns: Db major

var fSharpMajor = new KeySignature(new Note(NoteName.F, Alteration.Sharp), KeyMode.Major);
var enharmonics2 = fSharpMajor.GetEnharmonicEquivalents();
// Returns: Gb major
```

### NextInCircle(Direction?)

Gets the next key in the circle of fifths.

```C#
public KeySignature NextInCircle(Direction direction = Direction.Up)
```

**Parameters:**
- `direction`: Up for clockwise/sharps, Down for counterclockwise/flats

**Returns:** The next key in the circle of fifths

**Example:**
```C#
var c = new KeySignature(new Note(NoteName.C), KeyMode.Major);
var g = c.NextInCircle();           // G major (up a fifth)
var f = c.NextInCircle(Direction.Down); // F major (down a fifth)

// Complete circle
var current = c;
for (int i = 0; i < 12; i++)
{
    Console.WriteLine($"{current.Tonic} {current.Mode}: {current.AccidentalCount} accidentals");
    current = current.NextInCircle();
}
```

### PreviousInCircle()

Gets the previous key in the circle of fifths.

```C#
public KeySignature PreviousInCircle()
```

**Returns:** The previous key (equivalent to NextInCircle(Direction.Down))

### ToString()

Returns a string representation of the key signature.

```C#
public override string ToString()
```

**Format:** `"{Tonic} {mode}"`

**Examples:**
- "C major"
- "A minor"
- "G major"
- "Eb minor"

### Equals(KeySignature?)

Determines if this key signature equals another.

```C#
public bool Equals(KeySignature? other)
```

### GetHashCode()

Returns the hash code for the key signature.

```C#
public override int GetHashCode()
```

## Static Methods

### GetAllKeys()

Gets all standard key signatures.

```C#
public static List<KeySignature> GetAllKeys()
```

**Returns:** List of all 30 standard keys (15 major + 15 minor)

**Example:**
```C#
var allKeys = KeySignature.GetAllKeys();
foreach (var key in allKeys.Where(k => k.Mode == KeyMode.Major))
{
    Console.WriteLine($"{key}: {key.AccidentalCount} accidentals");
}
```

## Operators

### Equality Operators

```C#
public static bool operator ==(KeySignature? left, KeySignature? right)
public static bool operator !=(KeySignature? left, KeySignature? right)
```

Compare key signatures by tonic and mode.

## Key Signature Reference

### Major Keys

| Key | Accidentals | Notes Altered |
|-----|-------------|---------------|
| C | 0 | None |
| G | 1# | F# |
| D | 2# | F#, C# |
| A | 3# | F#, C#, G# |
| E | 4# | F#, C#, G#, D# |
| B | 5# | F#, C#, G#, D#, A# |
| F# | 6# | F#, C#, G#, D#, A#, E# |
| C# | 7# | F#, C#, G#, D#, A#, E#, B# |
| F | 1b | Bb |
| Bb | 2b | Bb, Eb |
| Eb | 3b | Bb, Eb, Ab |
| Ab | 4b | Bb, Eb, Ab, Db |
| Db | 5b | Bb, Eb, Ab, Db, Gb |
| Gb | 6b | Bb, Eb, Ab, Db, Gb, Cb |
| Cb | 7b | Bb, Eb, Ab, Db, Gb, Cb, Fb |

### Minor Keys

| Key | Accidentals | Relative Major |
|-----|-------------|----------------|
| A | 0 | C |
| E | 1# | G |
| B | 2# | D |
| F# | 3# | A |
| C# | 4# | E |
| G# | 5# | B |
| D# | 6# | F# |
| A# | 7# | C# |
| D | 1b | F |
| G | 2b | Bb |
| C | 3b | Eb |
| F | 4b | Ab |
| Bb | 5b | Db |
| Eb | 6b | Gb |
| Ab | 7b | Cb |

## Common Patterns

### Working with Keys

```C#
// Common keys
var cMajor = new KeySignature(new Note(NoteName.C), KeyMode.Major);
var aMinor = new KeySignature(new Note(NoteName.A), KeyMode.Minor);
var gMajor = new KeySignature(new Note(NoteName.G), KeyMode.Major);
var dMajor = new KeySignature(new Note(NoteName.D), KeyMode.Major);

// Keys with flats
var fMajor = new KeySignature(new Note(NoteName.F), KeyMode.Major);
var bbMajor = new KeySignature(new Note(NoteName.B, Alteration.Flat), KeyMode.Major);
var ebMajor = new KeySignature(new Note(NoteName.E, Alteration.Flat), KeyMode.Major);
```

### Key Relationships

```C#
public class KeyRelationships
{
    public static int GetDistance(KeySignature key1, KeySignature key2)
    {
        // Distance on circle of fifths
        return Math.Abs(key1.AccidentalCount - key2.AccidentalCount);
    }
    
    public static bool AreCloselyRelated(KeySignature key1, KeySignature key2)
    {
        // Keys are closely related if they differ by 1 accidental or less
        return GetDistance(key1, key2) <= 1 ||
               key1.Equals(key2.GetRelativeKey()) ||
               key1.Equals(key2.GetParallelKey());
    }
    
    public static List<KeySignature> GetCloselyRelatedKeys(KeySignature key)
    {
        var related = new List<KeySignature>
        {
            key.GetRelativeKey(),
            key.NextInCircle(),
            key.PreviousInCircle(),
            key.NextInCircle().GetRelativeKey(),
            key.PreviousInCircle().GetRelativeKey()
        };
        
        return related.Distinct().ToList();
    }
}
```

### Finding Key from Notes

```C#
public static KeySignature? FindKeyFromNotes(List<Note> notes)
{
    var allKeys = KeySignature.GetAllKeys();
    var bestMatch = (key: (KeySignature?)null, score: 0);
    
    foreach (var key in allKeys)
    {
        var scale = new Scale(key.Tonic, 
            key.Mode == KeyMode.Major ? ScaleType.Major : ScaleType.NaturalMinor);
        
        var matches = notes.Count(note => scale.Contains(note));
        if (matches > bestMatch.score)
        {
            bestMatch = (key, matches);
        }
    }
    
    return bestMatch.score > notes.Count * 0.8 ? bestMatch.key : null;
}
```

## Performance Notes

- Accidental count is calculated once during construction
- Circle of fifths navigation uses efficient modular arithmetic
- Key equality comparison is optimized
- Static key lists are cached

## Thread Safety

The `KeySignature` class is immutable and thread-safe. Multiple threads can safely:
- Read all properties
- Call all methods
- Share KeySignature instances

No synchronization is needed.

## See Also

<seealso>
    <category ref="related">
        <a href="key-signatures.md">Key Signatures Concept Guide</a>
        <a href="api-note.md">Note API Reference</a>
        <a href="chord-progressions.md">Chord Progressions Guide</a>
        <a href="enums.md">KeyMode Enum</a>
    </category>
</seealso>