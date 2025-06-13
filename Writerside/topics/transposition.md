# Transposition Techniques

Transposition is the process of moving musical content up or down in pitch while maintaining the same intervallic relationships. The MusicTheory library provides comprehensive transposition support for notes, chords, and scales.

## Understanding Transposition

<deflist>
    <def title="Transposition">
        Moving all notes by the same interval while preserving their relationships
    </def>
    <def title="Interval-based">
        Transpose by musical intervals (major 3rd, perfect 5th, etc.)
    </def>
    <def title="Semitone-based">
        Transpose by a specific number of half-steps
    </def>
    <def title="Key-to-key">
        Transpose from one key to another
    </def>
</deflist>

## Transposing Notes

### By Interval

```C#
var c4 = new Note(NoteName.C, Alteration.Natural, 4);

// Transpose up by intervals
var e4 = c4.Transpose(new Interval(IntervalQuality.Major, 3), Direction.Up);
var g4 = c4.Transpose(new Interval(IntervalQuality.Perfect, 5)); // Up is default
var c5 = c4.Transpose(new Interval(IntervalQuality.Perfect, 8), Direction.Up);

// Transpose down by intervals
var a3 = c4.Transpose(new Interval(IntervalQuality.Major, 3), Direction.Down);
var f3 = c4.Transpose(new Interval(IntervalQuality.Perfect, 5), Direction.Down);

// Verify results
Console.WriteLine(e4); // E4
Console.WriteLine(a3); // A3
```

### By Semitones

```C#
var c4 = new Note(NoteName.C, Alteration.Natural, 4);

// Transpose up by semitones
var cSharp4 = c4.TransposeBySemitones(1);   // C#4
var d4 = c4.TransposeBySemitones(2);        // D4
var c5 = c4.TransposeBySemitones(12);       // C5 (octave)

// Transpose down by semitones
var b3 = c4.TransposeBySemitones(-1);       // B3
var a3 = c4.TransposeBySemitones(-3);       // A3
var c3 = c4.TransposeBySemitones(-12);      // C3 (octave down)
```

## Transposing Chords

### Basic Chord Transposition

```C#
// Original chord
var cMaj7 = new Chord(new Note(NoteName.C, 4), ChordType.Major7);

// Transpose up a perfect 4th
var fMaj7 = cMaj7.Transpose(new Interval(IntervalQuality.Perfect, 4));
Console.WriteLine(fMaj7.GetSymbol()); // Fmaj7

// Transpose down a major 2nd
var bbMaj7 = cMaj7.Transpose(
    new Interval(IntervalQuality.Major, 2), 
    Direction.Down
);
Console.WriteLine(bbMaj7.GetSymbol()); // Bbmaj7

// Verify the notes transpose correctly
var originalNotes = cMaj7.GetNotes().ToList();    // C, E, G, B
var transposedNotes = fMaj7.GetNotes().ToList();  // F, A, C, E
```

### Maintaining Chord Quality

```C#
// Complex chord with extensions
var c13 = new Chord(new Note(NoteName.C, 4), ChordType.Dominant13);

// Transpose to different keys
var f13 = c13.Transpose(new Interval(IntervalQuality.Perfect, 4));
var g13 = c13.Transpose(new Interval(IntervalQuality.Perfect, 5));
var eb13 = c13.Transpose(new Interval(IntervalQuality.Minor, 3));

// All maintain the same chord quality
Console.WriteLine(c13.GetSymbol());  // C13
Console.WriteLine(f13.GetSymbol());  // F13
Console.WriteLine(g13.GetSymbol());  // G13
Console.WriteLine(eb13.GetSymbol()); // Eb13
```

## Transposing Scales

### Scale Transposition

```C#
// Original scale
var cMajor = new Scale(new Note(NoteName.C, 4), ScaleType.Major);

// Transpose to different keys
var dMajor = cMajor.Transpose(new Interval(IntervalQuality.Major, 2));
var gMajor = cMajor.Transpose(new Interval(IntervalQuality.Perfect, 5));
var bbMajor = cMajor.Transpose(
    new Interval(IntervalQuality.Major, 2), 
    Direction.Down
);

// Compare scale notes
var cNotes = cMajor.GetNotes().Take(8).Select(n => n.ToString());
// C4, D4, E4, F4, G4, A4, B4, C5

var gNotes = gMajor.GetNotes().Take(8).Select(n => n.ToString());
// G4, A4, B4, C5, D5, E5, F#5, G5
```

### Modal Transposition

```C#
// D Dorian (all white keys starting on D)
var dDorian = new Scale(new Note(NoteName.D, 4), ScaleType.Dorian);

// Transpose to A Dorian
var aDorian = dDorian.Transpose(new Interval(IntervalQuality.Perfect, 5));

// A Dorian has one sharp (F#)
var aDorianNotes = aDorian.GetNotes().Take(8).Select(n => n.ToString());
// A4, B4, C5, D5, E5, F#5, G5, A5
```

## Transposing Progressions

### Key-to-Key Transposition

```C#
public class ProgressionTransposer
{
    public List<Chord> TransposeProgression(
        List<Chord> originalChords,
        KeySignature fromKey,
        KeySignature toKey)
    {
        // Calculate the interval between keys
        var interval = Interval.Between(fromKey.Tonic, toKey.Tonic);
        
        // Transpose each chord
        return originalChords
            .Select(chord => chord.Transpose(interval))
            .ToList();
    }
    
    public void TransposeByRomanNumerals(
        string romanProgression,
        KeySignature fromKey,
        KeySignature toKey)
    {
        // Parse in original key
        var fromProg = new ChordProgression(fromKey);
        var originalChords = fromProg.ParseProgression(romanProgression).ToList();
        
        // Parse in new key (maintains functions)
        var toProg = new ChordProgression(toKey);
        var transposedChords = toProg.ParseProgression(romanProgression).ToList();
        
        // Display results
        for (int i = 0; i < originalChords.Count; i++)
        {
            Console.WriteLine(
                $"{originalChords[i].GetSymbol()} → {transposedChords[i].GetSymbol()}"
            );
        }
    }
}

// Usage
var transposer = new ProgressionTransposer();

// Transpose ii-V-I from C to F
transposer.TransposeByRomanNumerals(
    "ii7 - V7 - IMaj7",
    new KeySignature(new Note(NoteName.C), KeyMode.Major),
    new KeySignature(new Note(NoteName.F), KeyMode.Major)
);
// Output: Dm7 → Gm7, G7 → C7, CMaj7 → FMaj7
```

## Common Transposition Patterns

### Transposing for Different Instruments

```C#
public static class InstrumentTransposition
{
    // B♭ instruments (Trumpet, Clarinet, Tenor Sax)
    public static Note TransposeForBbInstrument(Note concertPitch)
    {
        // Up a major 2nd for written pitch
        return concertPitch.Transpose(
            new Interval(IntervalQuality.Major, 2), 
            Direction.Up
        );
    }
    
    // E♭ instruments (Alto Sax, Baritone Sax)
    public static Note TransposeForEbInstrument(Note concertPitch)
    {
        // Up a major 6th for written pitch
        return concertPitch.Transpose(
            new Interval(IntervalQuality.Major, 6), 
            Direction.Up
        );
    }
    
    // F instruments (French Horn)
    public static Note TransposeForFInstrument(Note concertPitch)
    {
        // Up a perfect 5th for written pitch
        return concertPitch.Transpose(
            new Interval(IntervalQuality.Perfect, 5), 
            Direction.Up
        );
    }
}

// Example: Concert C for different instruments
var concertC = new Note(NoteName.C, 4);
var bbInstrument = InstrumentTransposition.TransposeForBbInstrument(concertC); // D
var ebInstrument = InstrumentTransposition.TransposeForEbInstrument(concertC); // A
var fInstrument = InstrumentTransposition.TransposeForFInstrument(concertC);   // G
```

### Capo Transposition for Guitar

```C#
public class GuitarCapo
{
    private readonly int capoFret;
    
    public GuitarCapo(int fret)
    {
        if (fret < 0 || fret > 12)
            throw new ArgumentException("Capo fret must be between 0 and 12");
        capoFret = fret;
    }
    
    // Get the actual sounding pitch
    public Note GetSoundingPitch(Note frettedNote)
    {
        return frettedNote.TransposeBySemitones(capoFret);
    }
    
    // Get the chord shape to play for desired sound
    public Chord GetChordShape(Chord desiredSound)
    {
        return desiredSound.Transpose(
            new Interval(IntervalQuality.Perfect, 1), 
            Direction.Down
        ).TransposeBySemitones(-capoFret);
    }
}

// Capo on 3rd fret
var capo3 = new GuitarCapo(3);

// Playing C shape sounds as Eb
var cShape = new Chord(new Note(NoteName.C), ChordType.Major);
var actualSound = cShape.Transpose(
    new Interval(IntervalQuality.Perfect, 1)
).TransposeBySemitones(3); // Eb major
```

## Advanced Transposition

### Enharmonic Considerations

```C#
public class EnharmonicTransposer
{
    public Note TransposeWithBestSpelling(
        Note original, 
        Interval interval, 
        KeySignature targetKey)
    {
        // Transpose normally
        var transposed = original.Transpose(interval);
        
        // Check if the note fits the key
        if (targetKey.AlteredNotes.Contains(transposed.Name))
        {
            var alteration = targetKey.GetAlteration(transposed.Name);
            if (alteration != transposed.Alteration)
            {
                // Consider enharmonic equivalent
                var enharmonic = transposed.GetEnharmonicEquivalent();
                if (enharmonic != null && 
                    !targetKey.AlteredNotes.Contains(enharmonic.Name))
                {
                    return enharmonic;
                }
            }
        }
        
        return transposed;
    }
}
```

### Chromatic Transposition

```C#
public class ChromaticTransposer
{
    public List<Note> TransposeChromaticLine(
        List<Note> chromaticNotes, 
        int semitones)
    {
        return chromaticNotes
            .Select(note => note.TransposeBySemitones(semitones))
            .ToList();
    }
    
    public List<Note> TransposeDiatonically(
        List<Note> notes,
        Scale fromScale,
        Scale toScale,
        int degreesUp)
    {
        var transposed = new List<Note>();
        
        foreach (var note in notes)
        {
            // Find degree in original scale
            var degree = fromScale.GetDegree(note);
            if (degree > 0)
            {
                // Get corresponding degree in new scale
                var newDegree = ((degree - 1 + degreesUp) % 7) + 1;
                var newNote = toScale.GetNotes().ElementAt(newDegree - 1);
                transposed.Add(newNote);
            }
            else
            {
                // Non-diatonic note - transpose chromatically
                var interval = Interval.Between(fromScale.Root, toScale.Root);
                transposed.Add(note.Transpose(interval));
            }
        }
        
        return transposed;
    }
}
```

## Best Practices

- **Consider the musical context**: Choose transposition that maintains playability
- **Watch for enharmonics**: F# and Gb may require different transposition strategies
- **Preserve chord functions**: When transposing progressions, maintain the harmonic relationships
- **Check instrument ranges**: Ensure transposed notes are playable on the target instrument
- **Use appropriate spelling**: Choose sharps or flats based on the target key

## Common Use Cases

### Song Key Changes

```C#
// Original song in G major
var originalKey = new KeySignature(new Note(NoteName.G), KeyMode.Major);

// Singer needs it in E major (down a minor 3rd)
var newKey = new KeySignature(new Note(NoteName.E), KeyMode.Major);
var interval = Interval.Between(originalKey.Tonic, newKey.Tonic);

// Transpose all elements
var originalMelody = new[] { /* notes */ };
var transposedMelody = originalMelody
    .Select(n => n.Transpose(interval))
    .ToArray();
```

### Modulation Within a Piece

```C#
// Start in C major
var section1 = new[] 
{
    new Chord(new Note(NoteName.C), ChordType.Major),
    new Chord(new Note(NoteName.F), ChordType.Major),
    new Chord(new Note(NoteName.G), ChordType.Major)
};

// Modulate up a half step for dramatic effect
var section2 = section1
    .Select(chord => chord.TransposeBySemitones(1))
    .ToArray(); // Db, Gb, Ab
```

## See Also

<seealso>
    <category ref="related">
        <a href="intervals.md">Understanding Intervals</a>
        <a href="key-signatures.md">Working with Keys</a>
        <a href="notes.md">Note Transposition</a>
        <a href="chords.md">Chord Transposition</a>
    </category>
    <category ref="tutorials">
        <a href="examples.md">Transposition Examples</a>
    </category>
</seealso>