# Code Examples

This page contains practical examples demonstrating common use cases for the MusicTheory library.

## Basic Examples

### Creating a Simple Melody

```C#
using MusicTheory;

// Create a simple melody in C major
var notes = new[]
{
    new Note(NoteName.C, Alteration.Natural, 4),
    new Note(NoteName.D, Alteration.Natural, 4),
    new Note(NoteName.E, Alteration.Natural, 4),
    new Note(NoteName.F, Alteration.Natural, 4),
    new Note(NoteName.G, Alteration.Natural, 4),
    new Note(NoteName.F, Alteration.Natural, 4),
    new Note(NoteName.E, Alteration.Natural, 4),
    new Note(NoteName.D, Alteration.Natural, 4),
    new Note(NoteName.C, Alteration.Natural, 4)
};

// Check if all notes are in C major scale
var cMajor = new Scale(new Note(NoteName.C, 4), ScaleType.Major);
bool allInScale = notes.All(note => cMajor.Contains(note)); // true
```

### Building Basic Chords

```C#
// Create all triads in C major
var key = new KeySignature(new Note(NoteName.C), KeyMode.Major);
var progression = new ChordProgression(key);
var diatonicChords = progression.GetDiatonicChords().ToList();

// Print chord symbols
foreach (var (chord, degree) in diatonicChords.Select((c, i) => (c, i + 1)))
{
    var roman = progression.GetRomanNumeral(degree);
    Console.WriteLine($"{roman}: {chord.GetSymbol()}");
}

// Output:
// I: C
// ii: Dm
// iii: Em
// IV: F
// V: G
// vi: Am
// vii°: B°
```

## Intermediate Examples

### Transposing a Song

```C#
// Original song in C major
var originalKey = new KeySignature(new Note(NoteName.C), KeyMode.Major);
var originalChords = new[]
{
    new Chord(new Note(NoteName.C, 4), ChordType.Major),
    new Chord(new Note(NoteName.A, 3), ChordType.Minor),
    new Chord(new Note(NoteName.F, 4), ChordType.Major),
    new Chord(new Note(NoteName.G, 4), ChordType.Major)
};

// Transpose to G major (up a perfect 5th)
var transpositionInterval = new Interval(IntervalQuality.Perfect, 5);
var transposedChords = originalChords
    .Select(chord => chord.Transpose(transpositionInterval))
    .ToList();

// Result: G, Em, C, D
foreach (var chord in transposedChords)
{
    Console.WriteLine(chord.GetSymbol());
}
```

### Creating a Blues Scale Solo

```C#
// A blues scale
var aBlues = new Scale(new Note(NoteName.A, 3), ScaleType.Blues);
var bluesNotes = aBlues.GetNotes().Take(6).ToList();

// Create a simple blues lick
var lick = new[]
{
    bluesNotes[0], // A (root)
    bluesNotes[2], // D (4th)
    bluesNotes[3], // Eb (b5 - blue note)
    bluesNotes[4], // E (5th)
    bluesNotes[2], // D
    bluesNotes[0]  // A
};

// Add some octave variation
var lickWithOctaves = new[]
{
    lick[0],
    lick[1],
    lick[2],
    lick[3].TransposeBySemitones(12), // E up an octave
    lick[4],
    lick[5].TransposeBySemitones(-12) // A down an octave
};
```

### Chord Substitutions

```C#
// Original ii-V-I in C major
var dm7 = new Chord(new Note(NoteName.D, 4), ChordType.Minor7);
var g7 = new Chord(new Note(NoteName.G, 3), ChordType.Dominant7);
var cMaj7 = new Chord(new Note(NoteName.C, 4), ChordType.Major7);

// Tritone substitution on V7
var db7 = new Chord(new Note(NoteName.D, Alteration.Flat, 3), ChordType.Dominant7);

// New progression: ii-V-I with tritone sub
var substitutedProgression = new[] { dm7, db7, cMaj7 };

// The Db7 resolves to C by half-step instead of fifth
```

## Advanced Examples

### Modal Interchange

```C#
// C major with borrowed chords from C minor
var cMajorKey = new KeySignature(new Note(NoteName.C), KeyMode.Major);
var cMinorKey = new KeySignature(new Note(NoteName.C), KeyMode.Minor);

var majorProg = new ChordProgression(cMajorKey);
var minorProg = new ChordProgression(cMinorKey);

// Standard C major chords
var I = majorProg.GetChordByDegree(1);    // C
var IV = majorProg.GetChordByDegree(4);   // F

// Borrowed from C minor
var bVII = new Chord(new Note(NoteName.B, Alteration.Flat, 3), ChordType.Major); // Bb
var iv = new Chord(new Note(NoteName.F, 4), ChordType.Minor); // Fm

// Modal interchange progression: I - IV - iv - I - bVII - I
var modalProgression = new[] { I, IV, iv, I, bVII, I };
```

### Complex Jazz Voicings

```C#
// Create extended jazz chords
var root = new Note(NoteName.C, 4);

// Cmaj7#11
var cMaj7Sharp11 = new Chord(root, ChordType.Major7)
    .AddExtension(11, IntervalQuality.Augmented);

// C13(#11)
var c13Sharp11 = new Chord(root, ChordType.Dominant13)
    .AddExtension(11, IntervalQuality.Augmented);

// Get all notes
var voicing = c13Sharp11.GetNotes().ToList();
// C, E, G, Bb, D, F#, A

// Create rootless voicing (common in jazz piano)
var rootless = voicing.Skip(1).ToList(); // Remove root
// E, G, Bb, D, F#, A
```

### Analyzing a Chord Progression

```C#
public class ProgressionAnalyzer
{
    private readonly ChordProgression progression;
    
    public ProgressionAnalyzer(KeySignature key)
    {
        progression = new ChordProgression(key);
    }
    
    public void Analyze(string progressionString)
    {
        var chords = progression.ParseProgression(progressionString).ToList();
        
        for (int i = 0; i < chords.Count - 1; i++)
        {
            var current = chords[i];
            var next = chords[i + 1];
            
            // Analyze movement
            var rootInterval = Interval.Between(current.Root, next.Root);
            Console.WriteLine($"{current.GetSymbol()} → {next.GetSymbol()}:");
            Console.WriteLine($"  Root motion: {rootInterval.Quality} {rootInterval.Number}");
            
            // Check for common tones
            var currentNotes = current.GetNotes().ToList();
            var nextNotes = next.GetNotes().ToList();
            var commonTones = currentNotes.Intersect(nextNotes, new NoteComparer()).ToList();
            Console.WriteLine($"  Common tones: {string.Join(", ", commonTones)}");
        }
    }
}

// Usage
var analyzer = new ProgressionAnalyzer(
    new KeySignature(new Note(NoteName.C), KeyMode.Major)
);
analyzer.Analyze("I - vi - ii - V");
```

### Building Scales in All Keys

```C#
// Generate major scales in all 12 keys
var scalesByKey = new Dictionary<string, Scale>();

var chromaticNotes = new[]
{
    new Note(NoteName.C),
    new Note(NoteName.D, Alteration.Flat),
    new Note(NoteName.D),
    new Note(NoteName.E, Alteration.Flat),
    new Note(NoteName.E),
    new Note(NoteName.F),
    new Note(NoteName.G, Alteration.Flat),
    new Note(NoteName.G),
    new Note(NoteName.A, Alteration.Flat),
    new Note(NoteName.A),
    new Note(NoteName.B, Alteration.Flat),
    new Note(NoteName.B)
};

foreach (var root in chromaticNotes)
{
    var scale = new Scale(root, ScaleType.Major);
    var keyName = root.ToString();
    scalesByKey[keyName] = scale;
    
    var notes = scale.GetNotes().Take(7).Select(n => n.ToString());
    Console.WriteLine($"{keyName} Major: {string.Join(" ", notes)}");
}
```

### MIDI Integration Example

```C#
public class MidiChordPlayer
{
    public List<int> GetMidiNumbers(Chord chord)
    {
        return chord.GetNotes()
            .Select(note => note.MidiNumber)
            .ToList();
    }
    
    public void PlayArpeggio(Chord chord, int delayMs = 100)
    {
        var midiNumbers = GetMidiNumbers(chord);
        
        foreach (var midiNumber in midiNumbers)
        {
            Console.WriteLine($"Play MIDI note: {midiNumber}");
            // In real implementation, send MIDI message
            Thread.Sleep(delayMs);
        }
    }
    
    public void PlayChordProgression(string progression, KeySignature key)
    {
        var prog = new ChordProgression(key);
        var chords = prog.ParseProgression(progression);
        
        foreach (var chord in chords)
        {
            var midiNotes = GetMidiNumbers(chord);
            Console.WriteLine($"{chord.GetSymbol()}: [{string.Join(", ", midiNotes)}]");
            // Play chord
            Thread.Sleep(1000); // 1 second per chord
        }
    }
}
```

### Circle of Fifths Explorer

```C#
public class CircleOfFifthsExplorer
{
    public void ExploreCircle(bool clockwise = true)
    {
        var current = new KeySignature(new Note(NoteName.C), KeyMode.Major);
        var keys = new List<KeySignature> { current };
        
        // Go around the circle
        for (int i = 0; i < 11; i++)
        {
            current = clockwise ? current.NextInCircle() : current.PreviousInCircle();
            keys.Add(current);
        }
        
        // Display results
        Console.WriteLine($"Circle of Fifths ({(clockwise ? "Clockwise" : "Counter-clockwise")}):");
        foreach (var key in keys)
        {
            var sharpsFlats = key.AccidentalCount > 0 
                ? $"{key.AccidentalCount} sharps" 
                : key.AccidentalCount < 0 
                    ? $"{-key.AccidentalCount} flats"
                    : "no sharps/flats";
            
            Console.WriteLine($"{key.Tonic} major: {sharpsFlats}");
        }
    }
}
```

## Practical Applications

### Chord Chart Generator

```C#
public class ChordChart
{
    public void GenerateChart(string songTitle, string key, string progression)
    {
        var keySignature = ParseKey(key);
        var prog = new ChordProgression(keySignature);
        var chords = prog.ParseProgression(progression).ToList();
        
        Console.WriteLine($"=== {songTitle} ===");
        Console.WriteLine($"Key: {keySignature.Tonic} {keySignature.Mode}");
        Console.WriteLine();
        
        // Print measures (4 chords per line)
        for (int i = 0; i < chords.Count; i += 4)
        {
            var measure = chords.Skip(i).Take(4);
            Console.WriteLine($"| {string.Join(" | ", measure.Select(c => c.GetSymbol().PadRight(6)))} |");
        }
    }
    
    private KeySignature ParseKey(string key)
    {
        // Simple parser - extend as needed
        var parts = key.Split(' ');
        var note = Note.Parse(parts[0] + "4");
        var mode = parts.Length > 1 && parts[1].ToLower() == "minor" 
            ? KeyMode.Minor 
            : KeyMode.Major;
        return new KeySignature(note, mode);
    }
}

// Usage
var chart = new ChordChart();
chart.GenerateChart(
    "Autumn Leaves",
    "G major",
    "ii - V - I - IV - vii - III - vi - vi - ii - V - I - I - ii - V - I - I"
);
```

## See Also

<seealso>
    <category ref="tutorials">
        <a href="getting-started.md">Getting Started Guide</a>
        <a href="building-scales.md">Building Scales Tutorial</a>
        <a href="analyzing-progressions.md">Analyzing Progressions</a>
    </category>
    <category ref="api">
        <a href="api-overview.md">API Reference</a>
    </category>
</seealso>