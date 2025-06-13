# Working with Modes

This tutorial explores musical modes, their characteristics, and how to use them effectively with the MusicTheory library. You'll learn to create modal scales, understand their unique qualities, and apply them in compositions.

## Prerequisites

Before starting, you should understand:
- [Scales](scales.md) and scale construction
- [Intervals](intervals.md) and their relationships
- [Notes](notes.md) and basic music theory

## Understanding Modes

Modes are scales derived from the major scale by starting on different degrees. Each mode has a unique character and emotional quality.

### The Seven Modes

```C#
// All modes starting from C
var modes = new Dictionary<ScaleType, string>
{
    { ScaleType.Ionian, "Major scale - bright, happy" },
    { ScaleType.Dorian, "Minor with raised 6th - jazzy, sophisticated" },
    { ScaleType.Phrygian, "Minor with lowered 2nd - Spanish, exotic" },
    { ScaleType.Lydian, "Major with raised 4th - dreamy, ethereal" },
    { ScaleType.Mixolydian, "Major with lowered 7th - bluesy, rock" },
    { ScaleType.Aeolian, "Natural minor - sad, melancholic" },
    { ScaleType.Locrian, "Diminished tonic - unstable, tense" }
};

foreach (var mode in modes)
{
    var scale = new Scale(new Note(NoteName.C, 4), mode.Key);
    var notes = scale.GetNotes().Take(8).Select(n => n.ToString());
    Console.WriteLine($"{mode.Key}: {string.Join(" ", notes)}");
    Console.WriteLine($"   Character: {mode.Value}\n");
}
```

## Modes from Parent Scales

### Deriving Modes from Major Scale

```C#
public class ModeDerivation
{
    public static Scale GetModeFromMajorScale(Scale majorScale, int degree)
    {
        if (majorScale.Type != ScaleType.Major)
            throw new ArgumentException("Parent scale must be major");
            
        var scaleNotes = majorScale.GetNotes().Take(7).ToList();
        var modeRoot = scaleNotes[degree - 1];
        
        var modeType = degree switch
        {
            1 => ScaleType.Ionian,
            2 => ScaleType.Dorian,
            3 => ScaleType.Phrygian,
            4 => ScaleType.Lydian,
            5 => ScaleType.Mixolydian,
            6 => ScaleType.Aeolian,
            7 => ScaleType.Locrian,
            _ => throw new ArgumentException("Degree must be 1-7")
        };
        
        return new Scale(modeRoot, modeType);
    }
    
    public static void ShowRelatedModes(Note majorTonic)
    {
        var majorScale = new Scale(majorTonic, ScaleType.Major);
        Console.WriteLine($"Modes of {majorTonic} major:");
        
        for (int degree = 1; degree <= 7; degree++)
        {
            var mode = GetModeFromMajorScale(majorScale, degree);
            var modeName = mode.Type.ToString();
            Console.WriteLine($"  {degree}. {mode.Root} {modeName}");
        }
    }
}

// Show all modes of C major
ModeDerivation.ShowRelatedModes(new Note(NoteName.C));
// Output:
// 1. C Ionian
// 2. D Dorian
// 3. E Phrygian
// 4. F Lydian
// 5. G Mixolydian
// 6. A Aeolian
// 7. B Locrian
```

### Finding Parent Scales

```C#
public static Note FindParentMajorScale(Scale mode)
{
    var semitoneOffset = mode.Type switch
    {
        ScaleType.Ionian => 0,
        ScaleType.Dorian => -2,
        ScaleType.Phrygian => -4,
        ScaleType.Lydian => -5,
        ScaleType.Mixolydian => -7,
        ScaleType.Aeolian => -9,
        ScaleType.Locrian => -11,
        _ => throw new ArgumentException("Not a standard mode")
    };
    
    return mode.Root.TransposeBySemitones(semitoneOffset);
}

// Example: Find parent scale of D Dorian
var dDorian = new Scale(new Note(NoteName.D, 4), ScaleType.Dorian);
var parent = FindParentMajorScale(dDorian);
Console.WriteLine($"D Dorian is derived from {parent} major"); // C major
```

## Modal Characteristics

### Characteristic Tones

```C#
public class ModalCharacteristics
{
    public static Dictionary<ScaleType, string> GetCharacteristicIntervals()
    {
        return new Dictionary<ScaleType, string>
        {
            { ScaleType.Dorian, "Major 6th in minor context" },
            { ScaleType.Phrygian, "Minor 2nd from root" },
            { ScaleType.Lydian, "Augmented 4th (#11)" },
            { ScaleType.Mixolydian, "Minor 7th in major context" },
            { ScaleType.Aeolian, "Minor 6th and 7th" },
            { ScaleType.Locrian, "Diminished 5th" }
        };
    }
    
    public static Note GetCharacteristicTone(Scale mode)
    {
        var notes = mode.GetNotes().Take(7).ToList();
        
        return mode.Type switch
        {
            ScaleType.Dorian => notes[5],     // 6th degree
            ScaleType.Phrygian => notes[1],   // 2nd degree
            ScaleType.Lydian => notes[3],     // 4th degree
            ScaleType.Mixolydian => notes[6], // 7th degree
            ScaleType.Locrian => notes[4],    // 5th degree
            _ => notes[0] // Root for others
        };
    }
}

// Demonstrate characteristic tones
var modes = new[] 
{ 
    ScaleType.Dorian, ScaleType.Phrygian, ScaleType.Lydian, 
    ScaleType.Mixolydian, ScaleType.Locrian 
};

foreach (var modeType in modes)
{
    var mode = new Scale(new Note(NoteName.C, 4), modeType);
    var characteristic = ModalCharacteristics.GetCharacteristicTone(mode);
    Console.WriteLine($"{modeType}: Characteristic tone is {characteristic}");
}
```

### Modal Chord Progressions

```C#
public class ModalProgressions
{
    public static List<Chord> GetModalChords(Scale mode)
    {
        var chords = new List<Chord>();
        var notes = mode.GetNotes().Take(7).ToList();
        
        for (int i = 0; i < 7; i++)
        {
            var root = notes[i];
            var third = notes[(i + 2) % 7].TransposeBySemitones(
                i + 2 >= 7 ? 12 : 0
            );
            var fifth = notes[(i + 4) % 7].TransposeBySemitones(
                i + 4 >= 7 ? 12 : 0
            );
            
            // Determine chord quality
            var thirdInterval = Interval.Between(root, third).Semitones;
            var fifthInterval = Interval.Between(root, fifth).Semitones;
            
            ChordType chordType;
            if (thirdInterval == 4 && fifthInterval == 7)
                chordType = ChordType.Major;
            else if (thirdInterval == 3 && fifthInterval == 7)
                chordType = ChordType.Minor;
            else if (thirdInterval == 3 && fifthInterval == 6)
                chordType = ChordType.Diminished;
            else if (thirdInterval == 4 && fifthInterval == 8)
                chordType = ChordType.Augmented;
            else
                continue;
            
            chords.Add(new Chord(root, chordType));
        }
        
        return chords;
    }
    
    public static Dictionary<string, string> GetModalProgressionExamples()
    {
        return new Dictionary<string, string>
        {
            { "Dorian", "i - IV (Em - A in E Dorian)" },
            { "Phrygian", "i - bII (Em - F in E Phrygian)" },
            { "Lydian", "I - II (F - G in F Lydian)" },
            { "Mixolydian", "I - bVII (G - F in G Mixolydian)" },
            { "Aeolian", "i - bVI - bVII (Am - F - G)" },
            { "Locrian", "i° - bII (B° - C in B Locrian)" }
        };
    }
}

// Generate modal chords
var dorianChords = ModalProgressions.GetModalChords(
    new Scale(new Note(NoteName.D, 4), ScaleType.Dorian)
);

Console.WriteLine("D Dorian chords:");
for (int i = 0; i < dorianChords.Count; i++)
{
    Console.WriteLine($"  {i + 1}: {dorianChords[i].GetSymbol()}");
}
```

## Modal Melodies

### Creating Modal Melodies

```C#
public class ModalMelodyGenerator
{
    public static List<Note> GenerateModalMelody(
        Scale mode, 
        int length, 
        bool emphasizeCharacteristic = true)
    {
        var melody = new List<Note>();
        var scaleNotes = mode.GetNotes().Take(7).ToList();
        var characteristic = ModalCharacteristics.GetCharacteristicTone(mode);
        var random = new Random();
        
        // Start on root
        melody.Add(mode.Root);
        
        for (int i = 1; i < length - 1; i++)
        {
            if (emphasizeCharacteristic && i % 4 == 2)
            {
                // Emphasize characteristic tone
                melody.Add(characteristic);
            }
            else
            {
                // Choose random scale note with stepwise preference
                var lastNote = melody.Last();
                var lastDegree = scaleNotes.FindIndex(n => 
                    n.Name == lastNote.Name && 
                    n.Alteration == lastNote.Alteration
                );
                
                // Prefer stepwise motion
                var weights = new int[7];
                for (int d = 0; d < 7; d++)
                {
                    var distance = Math.Abs(d - lastDegree);
                    weights[d] = distance <= 2 ? 3 : 1;
                }
                
                var nextDegree = WeightedRandom(weights, random);
                melody.Add(scaleNotes[nextDegree]);
            }
        }
        
        // End on root
        melody.Add(mode.Root);
        
        return melody;
    }
    
    private static int WeightedRandom(int[] weights, Random random)
    {
        var totalWeight = weights.Sum();
        var randomValue = random.Next(totalWeight);
        
        for (int i = 0; i < weights.Length; i++)
        {
            randomValue -= weights[i];
            if (randomValue < 0)
                return i;
        }
        
        return weights.Length - 1;
    }
}

// Generate melodies in different modes
var phrygianMelody = ModalMelodyGenerator.GenerateModalMelody(
    new Scale(new Note(NoteName.E, 4), ScaleType.Phrygian),
    16,
    true
);

Console.WriteLine("Phrygian melody:");
Console.WriteLine(string.Join(" - ", phrygianMelody.Select(n => n.ToString())));
```

## Modal Interchange and Borrowed Chords

### Parallel Modes

```C#
public class ParallelModes
{
    public static Dictionary<ScaleType, Scale> GetParallelModes(Note root)
    {
        var modes = new Dictionary<ScaleType, Scale>();
        var modeTypes = new[]
        {
            ScaleType.Ionian, ScaleType.Dorian, ScaleType.Phrygian,
            ScaleType.Lydian, ScaleType.Mixolydian, ScaleType.Aeolian,
            ScaleType.Locrian
        };
        
        foreach (var modeType in modeTypes)
        {
            modes[modeType] = new Scale(root, modeType);
        }
        
        return modes;
    }
    
    public static List<Chord> GetBorrowableChords(Scale homeMode, Scale borrowMode)
    {
        var homeChords = ModalProgressions.GetModalChords(homeMode);
        var borrowChords = ModalProgressions.GetModalChords(borrowMode);
        
        // Find chords that are different
        var borrowable = new List<Chord>();
        
        for (int i = 0; i < Math.Min(homeChords.Count, borrowChords.Count); i++)
        {
            var homeChord = homeChords[i];
            var borrowChord = borrowChords[i];
            
            if (homeChord.Type != borrowChord.Type || 
                homeChord.Root.Alteration != borrowChord.Root.Alteration)
            {
                borrowable.Add(borrowChord);
            }
        }
        
        return borrowable;
    }
}

// Find chords to borrow from parallel modes
var cIonian = new Scale(new Note(NoteName.C, 4), ScaleType.Ionian);
var cAeolian = new Scale(new Note(NoteName.C, 4), ScaleType.Aeolian);

var borrowableChords = ParallelModes.GetBorrowableChords(cIonian, cAeolian);
Console.WriteLine("Chords from C Aeolian that can be borrowed in C Ionian:");
foreach (var chord in borrowableChords)
{
    Console.WriteLine($"  {chord.GetSymbol()}");
}
```

## Practical Applications

### Modal Composition Helper

```C#
public class ModalComposer
{
    private Scale mode;
    private List<Chord> modalChords;
    
    public ModalComposer(Scale mode)
    {
        this.mode = mode;
        this.modalChords = ModalProgressions.GetModalChords(mode);
    }
    
    public List<Chord> CreateModalProgression(int length)
    {
        var progression = new List<Chord>();
        var random = new Random();
        
        // Start with tonic
        progression.Add(modalChords[0]);
        
        for (int i = 1; i < length - 1; i++)
        {
            // Avoid V-I in modal music (except Mixolydian)
            var availableChords = modalChords.ToList();
            
            if (mode.Type != ScaleType.Mixolydian && 
                progression.Last() == modalChords[4]) // If last was V
            {
                availableChords.RemoveAt(0); // Remove I
            }
            
            progression.Add(availableChords[random.Next(availableChords.Count)]);
        }
        
        // End on tonic
        progression.Add(modalChords[0]);
        
        return progression;
    }
    
    public string AnalyzeModalStrength(List<Chord> progression)
    {
        // Check how well the progression establishes the mode
        var tonicCount = progression.Count(c => c.Root.Name == mode.Root.Name);
        var hasCharacteristic = ContainsCharacteristicChord(progression);
        var avoidsV = !progression.Any(c => c == modalChords[4]) || 
                      mode.Type == ScaleType.Mixolydian;
        
        if (tonicCount >= progression.Count / 3 && hasCharacteristic && avoidsV)
            return "Strong modal character";
        else if (hasCharacteristic)
            return "Moderate modal character";
        else
            return "Weak modal character - sounds more tonal";
    }
    
    private bool ContainsCharacteristicChord(List<Chord> progression)
    {
        // Check for mode-specific chord movements
        return mode.Type switch
        {
            ScaleType.Dorian => progression.Any(c => c.Type == ChordType.Major && 
                                                     c.Root.Name == modalChords[3].Root.Name),
            ScaleType.Phrygian => progression.Any(c => c.Root.Name == modalChords[1].Root.Name),
            ScaleType.Lydian => progression.Any(c => c.Root.Name == modalChords[1].Root.Name),
            ScaleType.Mixolydian => progression.Any(c => c.Root.Name == modalChords[6].Root.Name),
            _ => true
        };
    }
}

// Compose in Dorian mode
var composer = new ModalComposer(
    new Scale(new Note(NoteName.D, 4), ScaleType.Dorian)
);

var modalProgression = composer.CreateModalProgression(8);
var strength = composer.AnalyzeModalStrength(modalProgression);

Console.WriteLine("Modal progression: " + 
    string.Join(" - ", modalProgression.Select(c => c.GetSymbol())));
Console.WriteLine($"Analysis: {strength}");
```

### Mode Detection

```C#
public class ModeDetector
{
    public static ScaleType? DetectMode(List<Note> melody)
    {
        // Find the most likely root note (most common or emphasized)
        var noteCounts = melody
            .GroupBy(n => new { n.Name, n.Alteration })
            .OrderByDescending(g => g.Count())
            .ToList();
        
        var likelyRoot = new Note(
            noteCounts.First().Key.Name,
            noteCounts.First().Key.Alteration,
            4
        );
        
        // Try each mode and see which fits best
        var modeTypes = new[]
        {
            ScaleType.Ionian, ScaleType.Dorian, ScaleType.Phrygian,
            ScaleType.Lydian, ScaleType.Mixolydian, ScaleType.Aeolian,
            ScaleType.Locrian
        };
        
        var bestMatch = (mode: ScaleType.Ionian, score: 0);
        
        foreach (var modeType in modeTypes)
        {
            var scale = new Scale(likelyRoot, modeType);
            var scaleNotes = scale.GetNotes().Take(7).ToList();
            
            var matches = melody.Count(note => 
                scaleNotes.Any(sn => sn.Name == note.Name && 
                                    sn.Alteration == note.Alteration)
            );
            
            if (matches > bestMatch.score)
            {
                bestMatch = (modeType, matches);
            }
        }
        
        return bestMatch.score > melody.Count * 0.8 ? bestMatch.mode : null;
    }
}
```

## Modal Practice Exercises

### Exercise 1: Mode Comparison

Create a method that compares two modes and highlights their differences:

```C#
public static void CompareModes(Scale mode1, Scale mode2)
{
    // Your implementation
    // Should show:
    // - Different notes
    // - Different intervals from root
    // - Different chord qualities
}
```

### Exercise 2: Modal Transposition

Transpose a modal melody while preserving its character:

```C#
public static List<Note> TransposeModalMelody(
    List<Note> melody, 
    ScaleType modeType, 
    Note newRoot)
{
    // Your implementation
    // Should maintain the mode's characteristic intervals
}
```

### Exercise 3: Modal Harmonization

Harmonize a modal melody with appropriate chords:

```C#
public static List<Chord> HarmonizeModalMelody(
    List<Note> melody, 
    Scale mode)
{
    // Your implementation
    // Should choose chords that support the modal character
}
```

## Best Practices

- **Establish the tonic**: Use pedal tones or drones to reinforce the root
- **Emphasize characteristic tones**: Highlight what makes each mode unique
- **Avoid strong V-I cadences**: These make music sound tonal rather than modal
- **Use appropriate chord progressions**: Each mode has its own harmonic tendencies
- **Think horizontally**: Modal music often emphasizes melody over harmony

## Common Modal Applications

### Jazz and Fusion
- Dorian: Minor jazz standards, funk grooves
- Mixolydian: Dominant vamps, blues-rock
- Lydian: Modern jazz, film scores

### Folk and Traditional
- Aeolian: Celtic music, folk ballads
- Dorian: English folk songs, sea shanties
- Mixolydian: Scottish bagpipe tunes

### Contemporary Music
- Phrygian: Metal, electronic music
- Locrian: Experimental, avant-garde
- All modes: Film scoring, game music

## Next Steps

- Explore [Chord Progressions](chord-progressions.md) in modal contexts
- Study [Examples](examples.md) of modal compositions
- Learn about [Transposition](transposition.md) of modal music
- Read more about [Scales](scales.md) and their relationships

## See Also

<seealso>
    <category ref="related">
        <a href="scales.md">Modal Scales Reference</a>
        <a href="building-scales.md">Building Scales Tutorial</a>
        <a href="examples.md">Modal Examples</a>
    </category>
</seealso>