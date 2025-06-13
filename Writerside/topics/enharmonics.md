# Enharmonic Equivalence

Enharmonic equivalence refers to notes, intervals, or chords that sound the same but are written differently. Understanding enharmonics is crucial for proper music notation and key relationships.

## Understanding Enharmonics

<deflist>
    <def title="Enharmonic Notes">
        Different spellings of the same pitch (C# = Db, F# = Gb)
    </def>
    <def title="Enharmonic Keys">
        Keys that sound identical but use different notation
    </def>
    <def title="Context Matters">
        The correct spelling depends on the musical context and key
    </def>
    <def title="Readability">
        Proper enharmonic spelling makes music easier to read
    </def>
</deflist>

## Enharmonic Notes

### Basic Enharmonic Pairs

```C#
// Common enharmonic equivalents
var cSharp = new Note(NoteName.C, Alteration.Sharp, 4);
var dFlat = new Note(NoteName.D, Alteration.Flat, 4);

// Check if they're enharmonic
bool areEnharmonic = cSharp.IsEnharmonicWith(dFlat); // true

// Get enharmonic equivalent
var cSharpEquiv = cSharp.GetEnharmonicEquivalent(); // Db4
var dFlatEquiv = dFlat.GetEnharmonicEquivalent();   // C#4

// Verify same pitch
Console.WriteLine(cSharp.MidiNumber == dFlat.MidiNumber); // true (61)
Console.WriteLine(cSharp.Frequency == dFlat.Frequency);   // true (~277.18 Hz)
```

### All Enharmonic Relationships

<tabs>
    <tab title="Sharp to Flat">
        <table>
            <tr>
                <th>Sharp Spelling</th>
                <th>Flat Spelling</th>
                <th>MIDI Number</th>
            </tr>
            <tr>
                <td>C#</td>
                <td>Db</td>
                <td>61</td>
            </tr>
            <tr>
                <td>D#</td>
                <td>Eb</td>
                <td>63</td>
            </tr>
            <tr>
                <td>F#</td>
                <td>Gb</td>
                <td>66</td>
            </tr>
            <tr>
                <td>G#</td>
                <td>Ab</td>
                <td>68</td>
            </tr>
            <tr>
                <td>A#</td>
                <td>Bb</td>
                <td>70</td>
            </tr>
        </table>
    </tab>
    <tab title="Natural Enharmonics">
        <table>
            <tr>
                <th>Note</th>
                <th>Enharmonic</th>
                <th>Context</th>
            </tr>
            <tr>
                <td>B</td>
                <td>Cb</td>
                <td>Gb major scale</td>
            </tr>
            <tr>
                <td>C</td>
                <td>B#</td>
                <td>C# major scale</td>
            </tr>
            <tr>
                <td>E</td>
                <td>Fb</td>
                <td>Cb major scale</td>
            </tr>
            <tr>
                <td>F</td>
                <td>E#</td>
                <td>F# major scale</td>
            </tr>
        </table>
    </tab>
    <tab title="Double Alterations">
        <table>
            <tr>
                <th>Double Sharp</th>
                <th>Natural/Sharp</th>
                <th>Example</th>
            </tr>
            <tr>
                <td>C##</td>
                <td>D</td>
                <td>A# minor scale</td>
            </tr>
            <tr>
                <td>F##</td>
                <td>G</td>
                <td>G# minor scale</td>
            </tr>
            <tr>
                <th>Double Flat</th>
                <th>Natural/Flat</th>
                <th>Example</th>
            </tr>
            <tr>
                <td>Dbb</td>
                <td>C</td>
                <td>Bbb major</td>
            </tr>
            <tr>
                <td>Bbb</td>
                <td>A</td>
                <td>Diminished 7th</td>
            </tr>
        </table>
    </tab>
</tabs>

## Choosing Correct Spelling

### Based on Key Signature

```C#
public class EnharmonicSpelling
{
    public static Note SpellNoteInKey(int midiNumber, KeySignature key)
    {
        // Basic conversion
        var note = Note.FromMidiNumber(midiNumber, key.AccidentalCount < 0);
        
        // Check if note fits the key
        var scale = new Scale(key.Tonic, 
            key.Mode == KeyMode.Major ? ScaleType.Major : ScaleType.NaturalMinor);
        
        if (scale.Contains(note))
        {
            return note;
        }
        
        // Try enharmonic equivalent
        var enharmonic = note.GetEnharmonicEquivalent();
        if (enharmonic != null && scale.Contains(enharmonic))
        {
            return enharmonic;
        }
        
        // Return original if neither fits perfectly
        return note;
    }
    
    public static Note SpellChromatic(Note startNote, int semitones)
    {
        // Ascending chromatics typically use sharps
        // Descending chromatics typically use flats
        if (semitones > 0)
        {
            // Ascending - prefer sharps
            return Note.FromMidiNumber(
                startNote.MidiNumber + semitones, 
                preferFlats: false
            );
        }
        else
        {
            // Descending - prefer flats
            return Note.FromMidiNumber(
                startNote.MidiNumber + semitones, 
                preferFlats: true
            );
        }
    }
}

// Examples
var gMajor = new KeySignature(new Note(NoteName.G), KeyMode.Major);
var note = EnharmonicSpelling.SpellNoteInKey(66, gMajor); // F# (not Gb)

var ebMajor = new KeySignature(new Note(NoteName.E, Alteration.Flat), KeyMode.Major);
var note2 = EnharmonicSpelling.SpellNoteInKey(68, ebMajor); // Ab (not G#)
```

### Based on Chord Context

```C#
public class ChordSpelling
{
    public static List<Note> SpellChordProperly(Chord chord)
    {
        var notes = chord.GetNotes().ToList();
        var correctedNotes = new List<Note> { notes[0] }; // Keep root
        
        // Ensure each scale degree is represented
        var expectedDegrees = GetExpectedDegrees(chord.Type);
        
        for (int i = 1; i < notes.Count; i++)
        {
            var note = notes[i];
            var expectedDegree = expectedDegrees[i];
            
            // Check if note name matches expected degree
            var degreeFromRoot = (note.Name - chord.Root.Name + 7) % 7;
            
            if (degreeFromRoot != expectedDegree - 1)
            {
                // Try enharmonic
                var enharmonic = note.GetEnharmonicEquivalent();
                if (enharmonic != null)
                {
                    var enhDegree = (enharmonic.Name - chord.Root.Name + 7) % 7;
                    if (enhDegree == expectedDegree - 1)
                    {
                        correctedNotes.Add(enharmonic);
                        continue;
                    }
                }
            }
            
            correctedNotes.Add(note);
        }
        
        return correctedNotes;
    }
    
    private static List<int> GetExpectedDegrees(ChordType type)
    {
        // Return expected scale degrees for chord type
        return type switch
        {
            ChordType.Major => new List<int> { 1, 3, 5 },
            ChordType.Minor => new List<int> { 1, 3, 5 },
            ChordType.Major7 => new List<int> { 1, 3, 5, 7 },
            ChordType.Dominant7 => new List<int> { 1, 3, 5, 7 },
            _ => new List<int>()
        };
    }
}
```

## Enharmonic Intervals

### Interval Enharmonics

```C#
// Augmented 4th and Diminished 5th
var aug4 = new Interval(IntervalQuality.Augmented, 4);
var dim5 = new Interval(IntervalQuality.Diminished, 5);

// Both are 6 semitones (tritone)
Console.WriteLine(aug4.Semitones); // 6
Console.WriteLine(dim5.Semitones); // 6
Console.WriteLine(aug4.IsEnharmonicWith(dim5)); // true

// Context determines spelling
var c = new Note(NoteName.C, 4);
var fSharp = c.Transpose(aug4);  // C to F# (augmented 4th)
var gFlat = c.Transpose(dim5);   // C to Gb (diminished 5th)

// Other enharmonic interval pairs
var examples = new[]
{
    (new Interval(IntervalQuality.Minor, 3), new Interval(IntervalQuality.Augmented, 2)), // 3 semitones
    (new Interval(IntervalQuality.Major, 6), new Interval(IntervalQuality.Diminished, 7)), // 9 semitones
    (new Interval(IntervalQuality.Augmented, 6), new Interval(IntervalQuality.Minor, 7))   // 10 semitones
};
```

## Enharmonic Keys

### Key Signature Enharmonics

```C#
// Major key enharmonics
var cSharpMajor = new KeySignature(new Note(NoteName.C, Alteration.Sharp), KeyMode.Major);
var dFlatMajor = new KeySignature(new Note(NoteName.D, Alteration.Flat), KeyMode.Major);

// Get enharmonic equivalents
var cSharpEnharmonics = cSharpMajor.GetEnharmonicEquivalents();
// Returns Db major

// Common enharmonic key pairs
var enharmonicPairs = new[]
{
    ("B major (5#)", "Cb major (7b)"),
    ("F# major (6#)", "Gb major (6b)"),
    ("C# major (7#)", "Db major (5b)"),
    ("G# minor (5#)", "Ab minor (7b)"),
    ("D# minor (6#)", "Eb minor (6b)"),
    ("A# minor (7#)", "Bb minor (5b)")
};

// Choose simpler key signature
public KeySignature ChooseSimpleKey(Note tonic, KeyMode mode)
{
    var key1 = new KeySignature(tonic, mode);
    var enharmonics = key1.GetEnharmonicEquivalents();
    
    if (enharmonics.Any())
    {
        var key2 = enharmonics.First();
        // Choose key with fewer accidentals
        return Math.Abs(key1.AccidentalCount) <= Math.Abs(key2.AccidentalCount) 
            ? key1 : key2;
    }
    
    return key1;
}
```

## Enharmonic Chords

### Chord Enharmonics

```C#
// Enharmonic chord examples
var cSharpMajor = new Chord(new Note(NoteName.C, Alteration.Sharp, 4), ChordType.Major);
var dFlatMajor = cSharpMajor.GetEnharmonicEquivalent();

Console.WriteLine(cSharpMajor.GetSymbol()); // C#
Console.WriteLine(dFlatMajor?.GetSymbol()); // Db

// Augmented sixth chords (classical enharmonics)
// German 6th in C: Ab-C-Eb-F# 
// Enharmonically equals dominant 7th: Ab-C-Eb-Gb

// Diminished seventh chord (fully symmetric)
var dim7 = new Chord(new Note(NoteName.C, 4), ChordType.Diminished7);
// C-Eb-Gb-Bbb can be spelled from any note:
// C°7 = Eb°7 = Gb°7 = A°7 (enharmonically)
```

## Practical Applications

### Smart Transposition

```C#
public class SmartTransposer
{
    public static Note TransposeWithContext(
        Note original, 
        Interval interval, 
        KeySignature targetKey)
    {
        // First transpose normally
        var transposed = original.Transpose(interval);
        
        // Check if it fits the target key
        var scale = new Scale(targetKey.Tonic, 
            targetKey.Mode == KeyMode.Major ? ScaleType.Major : ScaleType.NaturalMinor);
        
        if (!scale.Contains(transposed))
        {
            // Try enharmonic
            var enharmonic = transposed.GetEnharmonicEquivalent();
            if (enharmonic != null && scale.Contains(enharmonic))
            {
                return enharmonic;
            }
        }
        
        return transposed;
    }
}
```

### Readability Optimizer

```C#
public class ReadabilityOptimizer
{
    public static Note OptimizeNoteSpelling(Note note)
    {
        // Avoid double sharps/flats when possible
        if (note.Alteration == Alteration.DoubleSharp || 
            note.Alteration == Alteration.DoubleFlat)
        {
            var enharmonic = note.GetEnharmonicEquivalent();
            if (enharmonic != null && 
                Math.Abs((int)enharmonic.Alteration) < 2)
            {
                return enharmonic;
            }
        }
        
        // Avoid E#, B#, Cb, Fb in isolation
        var awkwardNotes = new[]
        {
            (NoteName.E, Alteration.Sharp),  // Use F
            (NoteName.B, Alteration.Sharp),  // Use C
            (NoteName.C, Alteration.Flat),   // Use B
            (NoteName.F, Alteration.Flat)    // Use E
        };
        
        if (awkwardNotes.Contains((note.Name, note.Alteration)))
        {
            var enharmonic = note.GetEnharmonicEquivalent();
            if (enharmonic != null)
                return enharmonic;
        }
        
        return note;
    }
}
```

### Circle of Fifths Navigation

```C#
public class CircleOfFifthsEnharmonics
{
    public static void NavigateWithEnharmonics()
    {
        var start = new KeySignature(new Note(NoteName.C), KeyMode.Major);
        var current = start;
        
        // Go clockwise (sharps)
        for (int i = 0; i < 12; i++)
        {
            Console.WriteLine($"{current.Tonic} major: {current.AccidentalCount} accidentals");
            
            // Check for enharmonic equivalent
            var enharmonics = current.GetEnharmonicEquivalents();
            if (enharmonics.Any())
            {
                var enh = enharmonics.First();
                Console.WriteLine($"  = {enh.Tonic} major: {enh.AccidentalCount} accidentals");
            }
            
            current = current.NextInCircle();
        }
    }
}
```

## Best Practices

- **Consider the key**: Use spellings that fit the key signature
- **Maintain consistency**: Don't mix enharmonic spellings arbitrarily
- **Think melodically**: Ascending lines use sharps, descending use flats
- **Simplify when possible**: Avoid double sharps/flats unless necessary
- **Respect voice leading**: Proper spelling clarifies melodic direction

## Common Enharmonic Situations

### Leading Tones

```C#
// Leading tones should be spelled to show resolution
var gMinor = new KeySignature(new Note(NoteName.G), KeyMode.Minor);

// Harmonic minor has F# (leading tone to G)
// Not Gb, even though they sound the same
var harmonicMinor = new Scale(gMinor.Tonic, ScaleType.HarmonicMinor);
var leadingTone = harmonicMinor.GetNotes().ElementAt(6); // F#
```

### Chromatic Passages

```C#
// Chromatic scale spelling rules
public static List<Note> GetChromaticScale(Note start, Direction direction)
{
    var notes = new List<Note> { start };
    var current = start;
    
    for (int i = 1; i < 12; i++)
    {
        if (direction == Direction.Up)
        {
            // Ascending: C C# D D# E F F# G G# A A# B
            current = Note.FromMidiNumber(current.MidiNumber + 1, preferFlats: false);
        }
        else
        {
            // Descending: C B Bb A Ab G Gb F E Eb D Db
            current = Note.FromMidiNumber(current.MidiNumber - 1, preferFlats: true);
        }
        notes.Add(current);
    }
    
    return notes;
}
```

## See Also

<seealso>
    <category ref="related">
        <a href="notes.md">Note Enharmonics</a>
        <a href="intervals.md">Interval Enharmonics</a>
        <a href="key-signatures.md">Key Enharmonics</a>
    </category>
    <category ref="tutorials">
        <a href="transposition.md">Enharmonics in Transposition</a>
    </category>
</seealso>