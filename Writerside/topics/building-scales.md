# Building Scales

This tutorial will guide you through creating and working with scales using the MusicTheory library. You'll learn how to build various types of scales, understand their construction, and use them in your applications.

## Prerequisites

Before starting this tutorial, you should understand:
- [Notes](notes.md) and how to create them
- [Intervals](intervals.md) and their relationships
- Basic music theory concepts

## Understanding Scale Construction

Scales are built using specific interval patterns from a root note. The library provides 15+ scale types, each with its unique pattern.

### The Major Scale Pattern

The major scale follows the pattern: W-W-H-W-W-W-H (where W = whole step, H = half step)

```C#
// Create a C major scale
var cMajor = new Scale(new Note(NoteName.C, 4), ScaleType.Major);

// Get all notes in the scale
var notes = cMajor.GetNotes().Take(8).ToList();
foreach (var note in notes)
{
    Console.WriteLine(note);
}
// Output: C4, D4, E4, F4, G4, A4, B4, C5
```

## Step-by-Step: Building Your First Scale

### Step 1: Choose Your Root Note

```C#
// Define the root note
var root = new Note(NoteName.G, Alteration.Natural, 4);
```

### Step 2: Select the Scale Type

```C#
// Create different scale types from the same root
var gMajor = new Scale(root, ScaleType.Major);
var gMinor = new Scale(root, ScaleType.NaturalMinor);
var gDorian = new Scale(root, ScaleType.Dorian);
var gBlues = new Scale(root, ScaleType.Blues);
```

### Step 3: Explore the Scale

```C#
// Get scale degrees
for (int degree = 1; degree <= 7; degree++)
{
    var note = gMajor.GetNoteByDegree(degree);
    Console.WriteLine($"Degree {degree}: {note}");
}

// Check if notes belong to the scale
var fSharp = new Note(NoteName.F, Alteration.Sharp, 4);
var fNatural = new Note(NoteName.F, Alteration.Natural, 4);

Console.WriteLine($"F# in G major: {gMajor.Contains(fSharp)}"); // true
Console.WriteLine($"F in G major: {gMajor.Contains(fNatural)}"); // false
```

## Working with Different Scale Types

### Traditional Scales

```C#
// Major and Minor scales
var dMajor = new Scale(new Note(NoteName.D, 4), ScaleType.Major);
var aMinor = new Scale(new Note(NoteName.A, 4), ScaleType.NaturalMinor);
var aHarmonicMinor = new Scale(new Note(NoteName.A, 4), ScaleType.HarmonicMinor);
var aMelodicMinor = new Scale(new Note(NoteName.A, 4), ScaleType.MelodicMinor);

// Compare the different minor scales
var naturalNotes = aMinor.GetNotes().Take(8).Select(n => n.ToString()).ToArray();
var harmonicNotes = aHarmonicMinor.GetNotes().Take(8).Select(n => n.ToString()).ToArray();
var melodicNotes = aMelodicMinor.GetNotes().Take(8).Select(n => n.ToString()).ToArray();

Console.WriteLine($"Natural Minor:  {string.Join(", ", naturalNotes)}");
Console.WriteLine($"Harmonic Minor: {string.Join(", ", harmonicNotes)}");
Console.WriteLine($"Melodic Minor:  {string.Join(", ", melodicNotes)}");
```

### Modal Scales

```C#
// The seven modes
var modes = new[]
{
    (ScaleType.Ionian, "Ionian (Major)"),
    (ScaleType.Dorian, "Dorian"),
    (ScaleType.Phrygian, "Phrygian"),
    (ScaleType.Lydian, "Lydian"),
    (ScaleType.Mixolydian, "Mixolydian"),
    (ScaleType.Aeolian, "Aeolian (Natural Minor)"),
    (ScaleType.Locrian, "Locrian")
};

// Build all modes from C
foreach (var (scaleType, name) in modes)
{
    var scale = new Scale(new Note(NoteName.C, 4), scaleType);
    var notes = scale.GetNotes().Take(8).Select(n => n.ToString());
    Console.WriteLine($"{name}: {string.Join(" ", notes)}");
}
```

### Pentatonic and Blues Scales

```C#
// Pentatonic scales (5 notes)
var cMajorPentatonic = new Scale(new Note(NoteName.C, 4), ScaleType.MajorPentatonic);
var aMinorPentatonic = new Scale(new Note(NoteName.A, 4), ScaleType.MinorPentatonic);

// Blues scales (6 notes - pentatonic + blue note)
var aBlues = new Scale(new Note(NoteName.A, 4), ScaleType.Blues);

// Get the notes
var pentatonicNotes = aMinorPentatonic.GetNotes().Take(6).ToList();
var bluesNotes = aBlues.GetNotes().Take(7).ToList();

Console.WriteLine("A Minor Pentatonic: " + string.Join(", ", pentatonicNotes));
Console.WriteLine("A Blues: " + string.Join(", ", bluesNotes));
```

## Advanced Scale Techniques

### Finding Common Notes Between Scales

```C#
public static List<Note> FindCommonNotes(Scale scale1, Scale scale2)
{
    var notes1 = scale1.GetNotes().Take(7).ToList();
    var notes2 = scale2.GetNotes().Take(7).ToList();
    
    return notes1
        .Where(n1 => notes2.Any(n2 => n1.Name == n2.Name && n1.Alteration == n2.Alteration))
        .ToList();
}

// Find common notes between C major and G major
var cMajor = new Scale(new Note(NoteName.C, 4), ScaleType.Major);
var gMajor = new Scale(new Note(NoteName.G, 4), ScaleType.Major);

var commonNotes = FindCommonNotes(cMajor, gMajor);
Console.WriteLine($"Common notes: {string.Join(", ", commonNotes)}");
// Output: C, D, E, G, A, B (all except F/F#)
```

### Building Chords from Scale Degrees

```C#
public static List<Chord> BuildScaleChords(Scale scale)
{
    var chords = new List<Chord>();
    var scaleNotes = scale.GetNotes().Take(7).ToList();
    
    for (int i = 0; i < 7; i++)
    {
        // Build triad from every scale degree
        var root = scaleNotes[i];
        var third = scaleNotes[(i + 2) % 7];
        var fifth = scaleNotes[(i + 4) % 7];
        
        // Determine chord quality based on intervals
        var thirdInterval = Interval.Between(root, third);
        var fifthInterval = Interval.Between(root, fifth);
        
        ChordType chordType;
        if (thirdInterval.Semitones == 4 && fifthInterval.Semitones == 7)
            chordType = ChordType.Major;
        else if (thirdInterval.Semitones == 3 && fifthInterval.Semitones == 7)
            chordType = ChordType.Minor;
        else if (thirdInterval.Semitones == 3 && fifthInterval.Semitones == 6)
            chordType = ChordType.Diminished;
        else
            continue; // Skip unusual intervals
        
        chords.Add(new Chord(root, chordType));
    }
    
    return chords;
}

// Build all diatonic chords in C major
var cMajorChords = BuildScaleChords(cMajor);
foreach (var chord in cMajorChords)
{
    Console.WriteLine(chord.GetSymbol());
}
// Output: C, Dm, Em, F, G, Am, B°
```

### Scale Pattern Analysis

```C#
public class ScaleAnalyzer
{
    public static string AnalyzeScalePattern(Scale scale)
    {
        var notes = scale.GetNotes().Take(8).ToList();
        var intervals = new List<string>();
        
        for (int i = 0; i < 7; i++)
        {
            var interval = Interval.Between(notes[i], notes[i + 1]);
            intervals.Add(interval.Semitones == 2 ? "W" : "H");
        }
        
        return string.Join("-", intervals);
    }
    
    public static Dictionary<string, int> GetScaleIntervals(Scale scale)
    {
        var root = scale.Root;
        var notes = scale.GetNotes().Take(7).ToList();
        var intervals = new Dictionary<string, int>();
        
        for (int i = 1; i < 7; i++)
        {
            var interval = Interval.Between(root, notes[i]);
            var degreeName = GetDegreeName(i + 1);
            intervals[degreeName] = interval.Semitones;
        }
        
        return intervals;
    }
    
    private static string GetDegreeName(int degree)
    {
        return degree switch
        {
            2 => "Second",
            3 => "Third",
            4 => "Fourth",
            5 => "Fifth",
            6 => "Sixth",
            7 => "Seventh",
            _ => degree.ToString()
        };
    }
}

// Analyze different scales
var majorPattern = ScaleAnalyzer.AnalyzeScalePattern(cMajor);
var dorianPattern = ScaleAnalyzer.AnalyzeScalePattern(new Scale(new Note(NoteName.D, 4), ScaleType.Dorian));

Console.WriteLine($"Major pattern: {majorPattern}");   // W-W-H-W-W-W-H
Console.WriteLine($"Dorian pattern: {dorianPattern}"); // W-H-W-W-W-H-W
```

## Practical Examples

### Scale Practice Generator

```C#
public class ScalePracticeGenerator
{
    private Random random = new Random();
    
    public List<Note> GenerateScaleExercise(Scale scale, int noteCount)
    {
        var scaleNotes = scale.GetNotes().Take(7).ToList();
        var exercise = new List<Note>();
        
        // Ascending
        exercise.AddRange(scaleNotes);
        exercise.Add(scale.Root.TransposeBySemitones(12)); // Add octave
        
        // Descending
        for (int i = scaleNotes.Count - 1; i >= 0; i--)
        {
            exercise.Add(scaleNotes[i].TransposeBySemitones(12));
        }
        exercise.Add(scale.Root);
        
        // Random pattern
        for (int i = 0; i < noteCount; i++)
        {
            var randomNote = scaleNotes[random.Next(scaleNotes.Count)];
            exercise.Add(randomNote);
        }
        
        return exercise;
    }
}

// Generate practice patterns
var generator = new ScalePracticeGenerator();
var gMajorExercise = generator.GenerateScaleExercise(gMajor, 8);
```

### Key Signature Helper

```C#
public static Scale GetScaleFromKeySignature(KeySignature key)
{
    var scaleType = key.Mode == KeyMode.Major 
        ? ScaleType.Major 
        : ScaleType.NaturalMinor;
    
    return new Scale(key.Tonic, scaleType);
}

// Get scales from key signatures
var dMajorKey = new KeySignature(new Note(NoteName.D), KeyMode.Major);
var dMajorScale = GetScaleFromKeySignature(dMajorKey);

var bMinorKey = new KeySignature(new Note(NoteName.B), KeyMode.Minor);
var bMinorScale = GetScaleFromKeySignature(bMinorKey);
```

## Exercises

### Exercise 1: Scale Builder

Create a method that builds a scale from a string input:

```C#
public static Scale ParseScale(string input)
{
    // Parse inputs like "C major", "D dorian", "F# minor"
    var parts = input.Split(' ');
    if (parts.Length != 2)
        throw new ArgumentException("Invalid scale format");
    
    // Parse the root note
    var noteStr = parts[0];
    // Your implementation here...
    
    // Parse the scale type
    var scaleTypeStr = parts[1];
    // Your implementation here...
    
    return new Scale(root, scaleType);
}
```

### Exercise 2: Scale Comparison

Write a method to find the differences between two scales:

```C#
public static List<Note> FindScaleDifferences(Scale scale1, Scale scale2)
{
    // Return notes that are in scale1 but not in scale2
    // Your implementation here...
}
```

### Exercise 3: Mode Generator

Create all modes from a given major scale:

```C#
public static List<Scale> GenerateAllModes(Scale majorScale)
{
    // Generate all seven modes from the major scale
    // Your implementation here...
}
```

## Best Practices

- **Always specify octaves**: Include octave numbers when creating root notes
- **Use appropriate scale types**: Choose the ScaleType that matches your musical intent
- **Handle enharmonics carefully**: F# major and Gb major are different scales
- **Consider the context**: Some scales work better in certain keys
- **Test edge cases**: Scales with many sharps or flats need special attention

## Next Steps

- Learn about [Analyzing Chord Progressions](analyzing-progressions.md)
- Explore [Working with Modes](working-with-modes.md)
- Study [Key Signatures](key-signatures.md) in depth
- Practice with [Scale Examples](examples.md)

## See Also

<seealso>
    <category ref="related">
        <a href="scales.md">Scale Reference</a>
        <a href="api-scale.md">Scale API Documentation</a>
        <a href="intervals.md">Understanding Intervals</a>
    </category>
</seealso>