# Analyzing Chord Progressions

This tutorial teaches you how to analyze, create, and work with chord progressions using the MusicTheory library. You'll learn about Roman numeral analysis, common progressions, and how to build your own.

## Prerequisites

Before starting, you should understand:
- [Chords](chords.md) and chord construction
- [Scales](scales.md) and scale degrees
- [Key Signatures](key-signatures.md)
- Basic music theory concepts

## Understanding Chord Progressions

Chord progressions are sequences of chords that create harmonic movement in music. The library provides tools for:
- Roman numeral analysis
- Diatonic chord generation
- Common progression patterns
- Key-based transposition

## Getting Started with ChordProgression

### Creating a Chord Progression Object

```C#
// Create a progression in C major
var cMajorKey = new KeySignature(new Note(NoteName.C), KeyMode.Major);
var progression = new ChordProgression(cMajorKey);

// Create a progression in A minor
var aMinorKey = new KeySignature(new Note(NoteName.A), KeyMode.Minor);
var minorProgression = new ChordProgression(aMinorKey);
```

### Parsing Roman Numerals

```C#
// Classic I-V-vi-IV progression
var chords = progression.ParseProgression("I - V - vi - IV").ToList();

foreach (var chord in chords)
{
    Console.WriteLine($"{chord.GetSymbol()} - Notes: {string.Join(", ", chord.GetNotes())}");
}
// Output:
// C - Notes: C4, E4, G4
// G - Notes: G4, B4, D5
// Am - Notes: A4, C5, E5  
// F - Notes: F4, A4, C5
```

## Roman Numeral Analysis

### Understanding Roman Numerals

Roman numerals indicate:
- **Scale degree**: I, II, III, IV, V, VI, VII
- **Chord quality**: Uppercase = major, lowercase = minor
- **Extensions**: 7, 9, 11, 13
- **Alterations**: b5, #5, sus2, sus4

```C#
// Major key diatonic chords
var majorDiatonic = progression.ParseProgression("I - ii - iii - IV - V - vi - vii°");
// C - Dm - Em - F - G - Am - B°

// Minor key diatonic chords  
var minorDiatonic = minorProgression.ParseProgression("i - ii° - III - iv - v - VI - VII");
// Am - B° - C - Dm - Em - F - G
```

### Extended Chords in Progressions

```C#
// Jazz progression with extensions
var jazzChords = progression.ParseProgression("IMaj7 - vi7 - ii7 - V7");
// CMaj7 - Am7 - Dm7 - G7

// More complex extensions
var complexChords = progression.ParseProgression("IMaj9 - iii7 - vi7 - II7 - V13 - IMaj7");
// CMaj9 - Em7 - Am7 - D7 - G13 - CMaj7
```

## Common Chord Progressions

### Pop/Rock Progressions

```C#
public static class CommonProgressions
{
    // I-V-vi-IV ("Four Chord Song")
    public static string PopProgression = "I - V - vi - IV";
    
    // I-vi-IV-V ("50s Progression")
    public static string FiftiesProgression = "I - vi - IV - V";
    
    // vi-IV-I-V
    public static string SixFourOneProgression = "vi - IV - I - V";
    
    // I-IV-I-V ("12 Bar Blues" start)
    public static string BluesStart = "I - I - I - I - IV - IV - I - I";
}

// Use common progressions
var popChords = progression.ParseProgression(CommonProgressions.PopProgression);
var songChords = popChords.Select(c => c.GetSymbol()).ToList();
Console.WriteLine($"Pop progression in C: {string.Join(" - ", songChords)}");
// Output: C - G - Am - F
```

### Jazz Progressions

```C#
public static class JazzProgressions
{
    // ii-V-I
    public static string TwoFiveOne = "ii7 - V7 - IMaj7";
    
    // I-vi-ii-V ("Rhythm Changes" A section)
    public static string RhythmChanges = "IMaj7 - vi7 - ii7 - V7";
    
    // iii-vi-ii-V-I
    public static string ThreeSixTwoFive = "iii7 - vi7 - ii7 - V7 - IMaj7";
    
    // I-II7-ii7-V7 (with secondary dominant)
    public static string SecondaryDominant = "IMaj7 - II7 - ii7 - V7";
}

// Create a ii-V-I in different keys
var keys = new[] 
{
    new KeySignature(new Note(NoteName.C), KeyMode.Major),
    new KeySignature(new Note(NoteName.F), KeyMode.Major),
    new KeySignature(new Note(NoteName.Bb, Alteration.Flat), KeyMode.Major)
};

foreach (var key in keys)
{
    var prog = new ChordProgression(key);
    var twoFiveOne = prog.ParseProgression(JazzProgressions.TwoFiveOne);
    var symbols = twoFiveOne.Select(c => c.GetSymbol());
    Console.WriteLine($"{key.Tonic} major: {string.Join(" - ", symbols)}");
}
// Output:
// C major: Dm7 - G7 - CMaj7
// F major: Gm7 - C7 - FMaj7  
// Bb major: Cm7 - F7 - BbMaj7
```

## Analyzing Existing Progressions

### Finding the Key

```C#
public class ProgressionAnalyzer
{
    public static KeySignature FindBestKey(List<Chord> chords)
    {
        var possibleKeys = new List<(KeySignature key, int matches)>();
        
        // Test major keys
        foreach (NoteName noteName in Enum.GetValues(typeof(NoteName)))
        {
            var majorKey = new KeySignature(new Note(noteName), KeyMode.Major);
            var minorKey = new KeySignature(new Note(noteName), KeyMode.Minor);
            
            var majorMatches = CountDiatonicChords(chords, majorKey);
            var minorMatches = CountDiatonicChords(chords, minorKey);
            
            possibleKeys.Add((majorKey, majorMatches));
            possibleKeys.Add((minorKey, minorMatches));
        }
        
        // Return the key with most diatonic chords
        return possibleKeys.OrderByDescending(k => k.matches).First().key;
    }
    
    private static int CountDiatonicChords(List<Chord> chords, KeySignature key)
    {
        var progression = new ChordProgression(key);
        var diatonicChords = new List<Chord>();
        
        // Get all diatonic triads and 7th chords
        for (int degree = 1; degree <= 7; degree++)
        {
            diatonicChords.Add(progression.GetChordByDegree(degree));
            diatonicChords.Add(progression.GetChordByDegree(degree, true)); // 7th
        }
        
        // Count how many input chords match diatonic chords
        return chords.Count(chord => 
            diatonicChords.Any(diatonic => 
                diatonic.Root.Name == chord.Root.Name &&
                diatonic.Type == chord.Type
            )
        );
    }
}
```

### Implementing Roman Numeral Analysis

```C#
public static List<string> AnalyzeProgression(List<Chord> chords, KeySignature key)
{
    var progression = new ChordProgression(key);
    var analysis = new List<string>();
    
    foreach (var chord in chords)
    {
        // Find scale degree of chord root
        var scaleDegree = GetScaleDegree(chord.Root, key);
        if (scaleDegree == 0)
        {
            analysis.Add("?"); // Non-diatonic
            continue;
        }
        
        // Determine Roman numeral
        string roman = GetRomanNumeral(scaleDegree);
        
        // Check chord quality
        if (chord.Type == ChordType.Major || chord.Type == ChordType.Major7)
            roman = roman.ToUpper();
        else if (chord.Type == ChordType.Minor || chord.Type == ChordType.Minor7)
            roman = roman.ToLower();
        else if (chord.Type == ChordType.Diminished)
            roman = roman.ToLower() + "°";
        
        // Add extensions
        if (chord.Type.ToString().Contains("7"))
            roman += "7";
        else if (chord.Type.ToString().Contains("9"))
            roman += "9";
        
        analysis.Add(roman);
    }
    
    return analysis;
}

private static int GetScaleDegree(Note note, KeySignature key)
{
    var scale = new Scale(key.Tonic, 
        key.Mode == KeyMode.Major ? ScaleType.Major : ScaleType.NaturalMinor);
    var scaleNotes = scale.GetNotes().Take(7).ToList();
    
    for (int i = 0; i < scaleNotes.Count; i++)
    {
        if (scaleNotes[i].Name == note.Name)
            return i + 1;
    }
    
    return 0; // Not in scale
}

private static string GetRomanNumeral(int degree)
{
    return degree switch
    {
        1 => "I",
        2 => "II",
        3 => "III",
        4 => "IV",
        5 => "V",
        6 => "VI",
        7 => "VII",
        _ => "?"
    };
}
```

## Building Custom Progressions

### Chord Substitution

```C#
public class ChordSubstitution
{
    // Tritone substitution for dominant chords
    public static Chord TritoneSubstitute(Chord dominant7)
    {
        if (!dominant7.Type.ToString().Contains("Dominant"))
            throw new ArgumentException("Chord must be a dominant 7th");
        
        // Substitute with dominant 7th a tritone away
        var newRoot = dominant7.Root.TransposeBySemitones(6);
        return new Chord(newRoot, ChordType.Dominant7);
    }
    
    // Relative minor/major substitution
    public static Chord RelativeSubstitute(Chord chord)
    {
        if (chord.Type == ChordType.Major || chord.Type == ChordType.Major7)
        {
            // Major to relative minor (down minor 3rd)
            var relativeRoot = chord.Root.Transpose(
                new Interval(IntervalQuality.Minor, 3), 
                Direction.Down
            );
            var newType = chord.Type == ChordType.Major ? 
                ChordType.Minor : ChordType.Minor7;
            return new Chord(relativeRoot, newType);
        }
        else if (chord.Type == ChordType.Minor || chord.Type == ChordType.Minor7)
        {
            // Minor to relative major (up minor 3rd)
            var relativeRoot = chord.Root.Transpose(
                new Interval(IntervalQuality.Minor, 3), 
                Direction.Up
            );
            var newType = chord.Type == ChordType.Minor ? 
                ChordType.Major : ChordType.Major7;
            return new Chord(relativeRoot, newType);
        }
        
        return chord; // No substitution
    }
}

// Example: Substitute chords in ii-V-I
var originalProg = progression.ParseProgression("ii7 - V7 - IMaj7").ToList();
var substituted = new List<Chord>
{
    originalProg[0], // Keep ii7
    ChordSubstitution.TritoneSubstitute(originalProg[1]), // Tritone sub for V7
    originalProg[2]  // Keep IMaj7
};

Console.WriteLine("Original: " + string.Join(" - ", originalProg.Select(c => c.GetSymbol())));
Console.WriteLine("Substituted: " + string.Join(" - ", substituted.Select(c => c.GetSymbol())));
// Original: Dm7 - G7 - CMaj7
// Substituted: Dm7 - Db7 - CMaj7
```

### Voice Leading

```C#
public class VoiceLeading
{
    public static List<Note> OptimizeVoiceLeading(Chord from, Chord to)
    {
        var fromNotes = from.GetNotes().ToList();
        var toNotes = to.GetNotes().ToList();
        var optimized = new List<Note>();
        
        foreach (var toNote in toNotes)
        {
            // Find the closest note in the 'from' chord
            var closest = fromNotes
                .OrderBy(fn => Math.Abs(fn.MidiNumber - toNote.MidiNumber))
                .First();
            
            // Choose the octave that minimizes movement
            var candidates = new[]
            {
                new Note(toNote.Name, toNote.Alteration, toNote.Octave - 1),
                new Note(toNote.Name, toNote.Alteration, toNote.Octave),
                new Note(toNote.Name, toNote.Alteration, toNote.Octave + 1)
            };
            
            var best = candidates
                .Where(n => n.MidiNumber >= 0 && n.MidiNumber <= 127)
                .OrderBy(n => Math.Abs(n.MidiNumber - closest.MidiNumber))
                .First();
            
            optimized.Add(best);
        }
        
        return optimized;
    }
}

// Optimize voice leading in a progression
var voiceOptimized = new List<List<Note>>();
var progressionChords = progression.ParseProgression("I - vi - ii - V").ToList();

for (int i = 0; i < progressionChords.Count; i++)
{
    if (i == 0)
    {
        voiceOptimized.Add(progressionChords[i].GetNotes().ToList());
    }
    else
    {
        var optimizedVoicing = VoiceLeading.OptimizeVoiceLeading(
            new Chord(progressionChords[i-1].Root, progressionChords[i-1].Type),
            progressionChords[i]
        );
        voiceOptimized.Add(optimizedVoicing);
    }
}
```

## Advanced Progression Techniques

### Modal Interchange

```C#
public class ModalInterchange
{
    public static List<Chord> GetBorrowedChords(KeySignature key)
    {
        var borrowedChords = new List<Chord>();
        
        // If major key, borrow from parallel minor
        if (key.Mode == KeyMode.Major)
        {
            var parallelMinor = new KeySignature(key.Tonic, KeyMode.Minor);
            var minorProg = new ChordProgression(parallelMinor);
            
            // Common borrowed chords: bIII, bVI, bVII, iv
            borrowedChords.Add(minorProg.GetChordByDegree(3)); // bIII
            borrowedChords.Add(minorProg.GetChordByDegree(6)); // bVI
            borrowedChords.Add(minorProg.GetChordByDegree(7)); // bVII
            borrowedChords.Add(minorProg.GetChordByDegree(4)); // iv (minor)
        }
        // If minor key, borrow from parallel major
        else
        {
            var parallelMajor = new KeySignature(key.Tonic, KeyMode.Major);
            var majorProg = new ChordProgression(parallelMajor);
            
            // Common borrowed: IV (major), V (major)
            borrowedChords.Add(majorProg.GetChordByDegree(4)); // IV
            borrowedChords.Add(majorProg.GetChordByDegree(5)); // V
        }
        
        return borrowedChords;
    }
}

// Use modal interchange in C major
var borrowedChords = ModalInterchange.GetBorrowedChords(cMajorKey);
Console.WriteLine("Borrowed chords in C major:");
foreach (var chord in borrowedChords)
{
    Console.WriteLine($"- {chord.GetSymbol()}");
}
// Output: Eb, Ab, Bb, Fm
```

### Secondary Dominants

```C#
public static Chord GetSecondaryDominant(Chord targetChord)
{
    // Secondary dominant is V7 of the target chord
    var fifthAbove = targetChord.Root.Transpose(
        new Interval(IntervalQuality.Perfect, 5)
    );
    
    return new Chord(fifthAbove, ChordType.Dominant7);
}

// Add secondary dominants to progression
var basicProg = progression.ParseProgression("I - vi - ii - V").ToList();
var withSecondaries = new List<Chord>();

for (int i = 0; i < basicProg.Count; i++)
{
    // Add secondary dominant before non-tonic chords
    if (i > 0 && basicProg[i].Root.Name != progression.Key.Tonic.Name)
    {
        withSecondaries.Add(GetSecondaryDominant(basicProg[i]));
    }
    withSecondaries.Add(basicProg[i]);
}

Console.WriteLine("With secondary dominants: " + 
    string.Join(" - ", withSecondaries.Select(c => c.GetSymbol())));
// Output: C - E7 - Am - A7 - Dm - D7 - G
```

## Practical Examples

### Song Structure Builder

```C#
public class SongStructure
{
    public Dictionary<string, List<Chord>> Sections { get; } = new();
    
    public void AddSection(string name, ChordProgression progression, string romanNumerals)
    {
        Sections[name] = progression.ParseProgression(romanNumerals).ToList();
    }
    
    public List<Chord> BuildSong(params string[] sectionOrder)
    {
        var song = new List<Chord>();
        
        foreach (var section in sectionOrder)
        {
            if (Sections.ContainsKey(section))
            {
                song.AddRange(Sections[section]);
            }
        }
        
        return song;
    }
}

// Build a pop song structure
var song = new SongStructure();
var prog = new ChordProgression(cMajorKey);

song.AddSection("Verse", prog, "I - V - vi - IV");
song.AddSection("Chorus", prog, "vi - IV - I - V");
song.AddSection("Bridge", prog, "IV - V - iii - vi - IV - V - I");

var fullSong = song.BuildSong("Verse", "Verse", "Chorus", 
                               "Verse", "Chorus", "Bridge", "Chorus");
```

### Progression Generator

```C#
public class ProgressionGenerator
{
    private Random random = new Random();
    
    public List<Chord> GenerateProgression(ChordProgression progression, int length)
    {
        var chords = new List<Chord>();
        var lastDegree = 1; // Start on tonic
        
        for (int i = 0; i < length; i++)
        {
            int nextDegree;
            
            if (i == length - 1)
            {
                // End on tonic
                nextDegree = 1;
            }
            else
            {
                // Choose next chord based on common movements
                nextDegree = GetNextChord(lastDegree);
            }
            
            chords.Add(progression.GetChordByDegree(nextDegree, random.Next(2) == 0));
            lastDegree = nextDegree;
        }
        
        return chords;
    }
    
    private int GetNextChord(int currentDegree)
    {
        // Common chord movements
        var movements = currentDegree switch
        {
            1 => new[] { 2, 4, 5, 6 },
            2 => new[] { 5, 7 },
            3 => new[] { 4, 6 },
            4 => new[] { 1, 2, 5 },
            5 => new[] { 1, 6 },
            6 => new[] { 2, 4, 5 },
            7 => new[] { 1, 3 },
            _ => new[] { 1 }
        };
        
        return movements[random.Next(movements.Length)];
    }
}

// Generate random progressions
var generator = new ProgressionGenerator();
var randomProg = generator.GenerateProgression(progression, 8);
Console.WriteLine("Generated: " + string.Join(" - ", randomProg.Select(c => c.GetSymbol())));
```

## Exercises

### Exercise 1: Progression Transposer

Create a method that transposes a progression to all 12 keys:

```C#
public static Dictionary<string, List<Chord>> TransposeToAllKeys(string romanNumerals)
{
    // Your implementation here
    // Should return a dictionary with key names and their chord progressions
}
```

### Exercise 2: Progression Matcher

Find songs that use a specific progression:

```C#
public static bool ProgressionMatches(List<Chord> song, string targetProgression)
{
    // Your implementation here
    // Should detect if the target progression appears in the song
}
```

### Exercise 3: Complexity Analyzer

Rate the complexity of a progression:

```C#
public static int AnalyzeComplexity(List<Chord> progression)
{
    // Your implementation here
    // Consider: number of different chords, non-diatonic chords, extensions
}
```

## Best Practices

- **Start simple**: Master basic triads before adding extensions
- **Consider the genre**: Different styles favor different progressions
- **Think functionally**: Understand tonic, subdominant, and dominant functions
- **Listen actively**: Train your ear to recognize common progressions
- **Experiment**: Try substitutions and variations on classic progressions

## Next Steps

- Study [Working with Modes](working-with-modes.md) for modal progressions
- Learn about [Transposition](transposition.md) techniques
- Explore [Advanced Examples](examples.md)
- Read about [Key Signatures](key-signatures.md) in depth

## See Also

<seealso>
    <category ref="related">
        <a href="chord-progressions.md">Chord Progression Reference</a>
        <a href="chords.md">Understanding Chords</a>
        <a href="key-signatures.md">Key Signatures</a>
    </category>
</seealso>