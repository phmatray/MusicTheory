# Time Signatures and Rhythm

The `TimeSignature` and `Duration` classes provide comprehensive support for musical time, rhythm, and meter.

## Understanding Time Signatures

<deflist>
    <def title="Time Signature">
        A notation that specifies how many beats are in each measure and what note value gets one beat
    </def>
    <def title="Numerator">
        The top number - how many beats per measure
    </def>
    <def title="Denominator">
        The bottom number - which note value gets one beat (4 = quarter note, 8 = eighth note)
    </def>
    <def title="Meter">
        The rhythmic pattern created by the time signature (simple, compound, complex)
    </def>
</deflist>

## Creating Time Signatures

### Basic Time Signatures

```C#
// Common time signatures
var fourFour = new TimeSignature(4, 4);      // Common time
var threeFour = new TimeSignature(3, 4);     // Waltz time
var sixEight = new TimeSignature(6, 8);      // Compound duple
var fiveFour = new TimeSignature(5, 4);      // Complex meter
var sevenEight = new TimeSignature(7, 8);    // Complex meter

// Using predefined signatures
var common = TimeSignature.CommonTime;        // 4/4
var cut = TimeSignature.CutTime;             // 2/2 (alla breve)

// Access properties
Console.WriteLine($"{fourFour.Numerator}/{fourFour.Denominator}"); // 4/4
Console.WriteLine($"Beats per measure: {fourFour.BeatsPerMeasure}"); // 4
```

### Time Signature Properties

```C#
var sixEight = new TimeSignature(6, 8);

// Basic properties
int beats = sixEight.Numerator;           // 6
int beatUnit = sixEight.Denominator;      // 8
int beatsPerMeasure = sixEight.BeatsPerMeasure; // 6

// Meter classification
bool isCompound = sixEight.IsCompound;    // true (divisible by 3)
bool isSimple = fourFour.IsSimple;        // true
bool isComplex = fiveFour.IsComplex;      // true (5 or 7 beats)

// Beat grouping
var beatGroups = sixEight.GetBeatGroups(); // [3, 3] for 6/8
```

## Working with Durations

### Creating Durations

```C#
// Basic note durations
var whole = new Duration(DurationType.Whole);
var half = new Duration(DurationType.Half);
var quarter = new Duration(DurationType.Quarter);
var eighth = new Duration(DurationType.Eighth);
var sixteenth = new Duration(DurationType.Sixteenth);

// Dotted notes (add 50% of value)
var dottedHalf = new Duration(DurationType.Half, 1);      // 1 dot
var doubleDottedQuarter = new Duration(DurationType.Quarter, 2); // 2 dots

// Tuplets
var tripletEighth = Duration.CreateTuplet(DurationType.Eighth, 3, 2);
// 3 eighth notes in the time of 2

// Get duration values
double quarterValue = quarter.GetValue();  // 0.25 (quarter of a whole note)
double dottedHalfValue = dottedHalf.GetValue(); // 0.75 (0.5 + 0.25)
```

### Duration Symbols

```C#
// Get music notation symbols
var whole = new Duration(DurationType.Whole);
var quarter = new Duration(DurationType.Quarter);
var eighth = new Duration(DurationType.Eighth);

Console.WriteLine(whole.GetSymbol());    // 𝅝 (whole note)
Console.WriteLine(quarter.GetSymbol());  // ♩ (quarter note)
Console.WriteLine(eighth.GetSymbol());   // ♪ (eighth note)

// Dotted notes
var dottedQuarter = new Duration(DurationType.Quarter, 1);
Console.WriteLine(dottedQuarter.GetSymbol()); // ♩.
```

## Time Calculations

### Duration in Time

```C#
// Calculate actual time duration at a given tempo
int bpm = 120; // 120 beats per minute

var quarter = new Duration(DurationType.Quarter);
var half = new Duration(DurationType.Half);
var eighth = new Duration(DurationType.Eighth);

double quarterSeconds = quarter.GetTimeInSeconds(bpm);  // 0.5 seconds
double halfSeconds = half.GetTimeInSeconds(bpm);        // 1.0 seconds
double eighthSeconds = eighth.GetTimeInSeconds(bpm);    // 0.25 seconds

// With different tempos
double slowQuarter = quarter.GetTimeInSeconds(60);      // 1.0 second
double fastQuarter = quarter.GetTimeInSeconds(180);     // 0.333 seconds
```

### Duration in Measures

```C#
var fourFour = new TimeSignature(4, 4);
var threeFour = new TimeSignature(3, 4);

var wholeNote = new Duration(DurationType.Whole);
var halfNote = new Duration(DurationType.Half);
var quarterNote = new Duration(DurationType.Quarter);

// How many measures does each duration span?
double wholeMeasures = wholeNote.GetValueInMeasures(fourFour);    // 1.0
double halfMeasures = halfNote.GetValueInMeasures(fourFour);      // 0.5
double quarterMeasures = quarterNote.GetValueInMeasures(fourFour); // 0.25

// In 3/4 time
double wholeIn34 = wholeNote.GetValueInMeasures(threeFour);       // 1.333
double halfIn34 = halfNote.GetValueInMeasures(threeFour);         // 0.667
```

## Rhythm Patterns

### Building Rhythmic Patterns

```C#
public class RhythmPattern
{
    private List<Duration> durations = new List<Duration>();
    private TimeSignature timeSignature;
    
    public RhythmPattern(TimeSignature timeSignature)
    {
        this.timeSignature = timeSignature;
    }
    
    public RhythmPattern AddDuration(Duration duration)
    {
        durations.Add(duration);
        return this;
    }
    
    public double GetTotalBeats()
    {
        return durations.Sum(d => d.GetValue() * timeSignature.Denominator);
    }
    
    public bool FillsMeasure()
    {
        return Math.Abs(GetTotalBeats() - timeSignature.Numerator) < 0.0001;
    }
}

// Create a basic rock beat pattern
var rockBeat = new RhythmPattern(new TimeSignature(4, 4))
    .AddDuration(new Duration(DurationType.Quarter))
    .AddDuration(new Duration(DurationType.Quarter))
    .AddDuration(new Duration(DurationType.Quarter))
    .AddDuration(new Duration(DurationType.Quarter));

Console.WriteLine(rockBeat.FillsMeasure()); // true
```

### Common Rhythm Patterns

```C#
public static class CommonRhythms
{
    // Clave pattern (2-3)
    public static RhythmPattern Clave23()
    {
        return new RhythmPattern(new TimeSignature(4, 4))
            .AddDuration(new Duration(DurationType.Quarter))        // Beat 1
            .AddDuration(new Duration(DurationType.Quarter))        // Beat 2
            .AddDuration(new Duration(DurationType.Eighth))         // Beat 3 &
            .AddDuration(new Duration(DurationType.Eighth))
            .AddDuration(new Duration(DurationType.Quarter));       // Beat 4
    }
    
    // Shuffle rhythm
    public static RhythmPattern Shuffle()
    {
        var tripletEighth = Duration.CreateTuplet(DurationType.Eighth, 3, 2);
        return new RhythmPattern(new TimeSignature(4, 4))
            .AddDuration(new Duration(DurationType.Quarter))        // First note of triplet
            .AddDuration(tripletEighth)                            // Last note of triplet
            .AddDuration(new Duration(DurationType.Quarter))
            .AddDuration(tripletEighth);
    }
    
    // Waltz pattern
    public static RhythmPattern Waltz()
    {
        return new RhythmPattern(new TimeSignature(3, 4))
            .AddDuration(new Duration(DurationType.Quarter))
            .AddDuration(new Duration(DurationType.Quarter))
            .AddDuration(new Duration(DurationType.Quarter));
    }
}
```

## Complex Meters

### Working with Odd Time Signatures

```C#
// 5/4 time (3+2 or 2+3)
var fiveFour = new TimeSignature(5, 4);
var beatGroups54 = fiveFour.GetBeatGroups(); // Could be [3, 2] or [2, 3]

// 7/8 time (common groupings)
var sevenEight = new TimeSignature(7, 8);
// Common groupings: 3+2+2, 2+3+2, 2+2+3

// Create a 7/8 pattern
var sevenEightPattern = new RhythmPattern(sevenEight)
    .AddDuration(new Duration(DurationType.Eighth))    // 1
    .AddDuration(new Duration(DurationType.Eighth))    // 2
    .AddDuration(new Duration(DurationType.Eighth))    // 3
    .AddDuration(new Duration(DurationType.Eighth))    // 4
    .AddDuration(new Duration(DurationType.Eighth))    // 5
    .AddDuration(new Duration(DurationType.Eighth))    // 6
    .AddDuration(new Duration(DurationType.Eighth));   // 7
```

### Compound Meters

```C#
// 6/8 - Compound duple (2 main beats, each divided into 3)
var sixEight = new TimeSignature(6, 8);
bool isCompound = sixEight.IsCompound; // true

// 9/8 - Compound triple (3 main beats)
var nineEight = new TimeSignature(9, 8);

// 12/8 - Compound quadruple (4 main beats)
var twelveEight = new TimeSignature(12, 8);

// Create a 6/8 pattern with proper feel
var sixEightPattern = new RhythmPattern(sixEight)
    .AddDuration(new Duration(DurationType.DottedQuarter))  // Main beat 1
    .AddDuration(new Duration(DurationType.DottedQuarter)); // Main beat 2
```

## Tempo and BPM

### Working with Tempo

```C#
public class Tempo
{
    public int BPM { get; }
    public string Marking { get; }
    
    public Tempo(int bpm, string marking = "")
    {
        BPM = bpm;
        Marking = marking;
    }
    
    // Common tempo markings
    public static Tempo Grave => new Tempo(40, "Grave");
    public static Tempo Largo => new Tempo(50, "Largo");
    public static Tempo Adagio => new Tempo(70, "Adagio");
    public static Tempo Andante => new Tempo(90, "Andante");
    public static Tempo Moderato => new Tempo(110, "Moderato");
    public static Tempo Allegro => new Tempo(130, "Allegro");
    public static Tempo Vivace => new Tempo(160, "Vivace");
    public static Tempo Presto => new Tempo(180, "Presto");
    public static Tempo Prestissimo => new Tempo(200, "Prestissimo");
    
    public double GetBeatDuration(TimeSignature timeSignature)
    {
        // Duration of one beat in seconds
        return 60.0 / BPM;
    }
    
    public double GetMeasureDuration(TimeSignature timeSignature)
    {
        // Duration of one measure in seconds
        return (60.0 / BPM) * timeSignature.BeatsPerMeasure;
    }
}

// Calculate timing
var allegro = Tempo.Allegro; // 130 BPM
var fourFour = new TimeSignature(4, 4);

double beatDuration = allegro.GetBeatDuration(fourFour);     // ~0.46 seconds
double measureDuration = allegro.GetMeasureDuration(fourFour); // ~1.85 seconds
```

## Syncopation and Rhythm

### Creating Syncopated Rhythms

```C#
public class SyncopatedRhythm
{
    // Basic syncopation - emphasis on off-beats
    public static RhythmPattern BasicSyncopation()
    {
        return new RhythmPattern(new TimeSignature(4, 4))
            .AddDuration(new Duration(DurationType.Eighth))
            .AddDuration(new Duration(DurationType.Quarter))
            .AddDuration(new Duration(DurationType.Eighth))
            .AddDuration(new Duration(DurationType.Quarter))
            .AddDuration(new Duration(DurationType.Quarter));
    }
    
    // Anticipated beat
    public static RhythmPattern Anticipation()
    {
        var tiedEighth = new Duration(DurationType.Eighth);
        return new RhythmPattern(new TimeSignature(4, 4))
            .AddDuration(new Duration(DurationType.Quarter))
            .AddDuration(new Duration(DurationType.Quarter))
            .AddDuration(new Duration(DurationType.DottedQuarter))
            .AddDuration(tiedEighth); // Anticipates next measure
    }
}
```

## Polyrhythms

### Creating Polyrhythms

```C#
public class Polyrhythm
{
    public static void Create3Against2()
    {
        // 3 notes against 2 notes in same time span
        var threeNotes = Duration.CreateTuplet(DurationType.Quarter, 3, 2);
        var twoNotes = new Duration(DurationType.Quarter);
        
        // Both fill the same time
        var pattern1 = new List<Duration> { threeNotes, threeNotes, threeNotes };
        var pattern2 = new List<Duration> { twoNotes, twoNotes };
    }
    
    public static void Create4Against3()
    {
        // 4 notes against 3 notes
        var fourNotes = Duration.CreateTuplet(DurationType.Quarter, 4, 3);
        var threeNotes = Duration.CreateTuplet(DurationType.Quarter, 3, 4);
    }
}
```

## Practical Applications

### Metronome Implementation

```C#
public class Metronome
{
    private readonly int bpm;
    private readonly TimeSignature timeSignature;
    private int currentBeat = 1;
    
    public Metronome(int bpm, TimeSignature timeSignature)
    {
        this.bpm = bpm;
        this.timeSignature = timeSignature;
    }
    
    public double GetIntervalMs()
    {
        return 60000.0 / bpm; // Milliseconds between beats
    }
    
    public string GetBeatType()
    {
        if (currentBeat == 1)
            return "Downbeat";
        else if (timeSignature.IsCompound && 
                currentBeat % (timeSignature.Numerator / timeSignature.GetBeatGroups().Count) == 1)
            return "Strong";
        else
            return "Weak";
    }
    
    public void NextBeat()
    {
        currentBeat++;
        if (currentBeat > timeSignature.BeatsPerMeasure)
            currentBeat = 1;
    }
}
```

### Rhythm Notation Helper

```C#
public class RhythmNotation
{
    public static string GetBeaming(List<Duration> durations)
    {
        var result = new StringBuilder();
        
        foreach (var duration in durations)
        {
            if (duration.Type == DurationType.Eighth)
                result.Append("♫ "); // Beamed eighths
            else if (duration.Type == DurationType.Sixteenth)
                result.Append("♬ "); // Beamed sixteenths
            else
                result.Append(duration.GetSymbol() + " ");
        }
        
        return result.ToString();
    }
    
    public static string GetCountingPattern(TimeSignature ts)
    {
        if (ts.Equals(new TimeSignature(4, 4)))
            return "1 e & a 2 e & a 3 e & a 4 e & a";
        else if (ts.Equals(new TimeSignature(3, 4)))
            return "1 e & a 2 e & a 3 e & a";
        else if (ts.Equals(new TimeSignature(6, 8)))
            return "1 2 3 4 5 6";
        else
            return "Custom counting pattern";
    }
}
```

## Best Practices

- **Choose appropriate time signatures**: Match the natural feel of the music
- **Consider beat grouping**: 6/8 feels different from 3/4 despite same total
- **Use tuplets carefully**: They add rhythmic interest but can be complex
- **Respect the downbeat**: Strong beats provide rhythmic foundation
- **Match tempo to style**: Different genres have typical tempo ranges

## See Also

<seealso>
    <category ref="related">
        <a href="api-overview.md">TimeSignature API Reference</a>
        <a href="api-overview.md">Duration API Reference</a>
    </category>
    <category ref="tutorials">
        <a href="examples.md">Rhythm Examples</a>
    </category>
</seealso>