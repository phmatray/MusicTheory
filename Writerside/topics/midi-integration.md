# MIDI Integration

The MusicTheory library provides seamless integration with MIDI (Musical Instrument Digital Interface) through conversion between Note objects and MIDI note numbers.

## Understanding MIDI

<deflist>
    <def title="MIDI Note Numbers">
        A standard numbering system from 0-127 representing musical pitches
    </def>
    <def title="Middle C">
        MIDI note 60, corresponding to C4 in scientific pitch notation
    </def>
    <def title="Note Range">
        C-1 (MIDI 0) to G9 (MIDI 127)
    </def>
    <def title="Enharmonic Handling">
        Same MIDI number can represent different note spellings (C# = Db)
    </def>
</deflist>

## MIDI Note Number Reference

<tabs>
    <tab title="Common Notes">
        <table>
            <tr>
                <th>Note</th>
                <th>MIDI Number</th>
                <th>Frequency (Hz)</th>
            </tr>
            <tr>
                <td>A0</td>
                <td>21</td>
                <td>27.50</td>
            </tr>
            <tr>
                <td>C1</td>
                <td>24</td>
                <td>32.70</td>
            </tr>
            <tr>
                <td>C2</td>
                <td>36</td>
                <td>65.41</td>
            </tr>
            <tr>
                <td>C3</td>
                <td>48</td>
                <td>130.81</td>
            </tr>
            <tr>
                <td>C4 (Middle C)</td>
                <td>60</td>
                <td>261.63</td>
            </tr>
            <tr>
                <td>A4</td>
                <td>69</td>
                <td>440.00</td>
            </tr>
            <tr>
                <td>C5</td>
                <td>72</td>
                <td>523.25</td>
            </tr>
            <tr>
                <td>C6</td>
                <td>84</td>
                <td>1046.50</td>
            </tr>
            <tr>
                <td>C7</td>
                <td>96</td>
                <td>2093.00</td>
            </tr>
            <tr>
                <td>C8</td>
                <td>108</td>
                <td>4186.01</td>
            </tr>
        </table>
    </tab>
    <tab title="Octave Ranges">
        <table>
            <tr>
                <th>Octave</th>
                <th>MIDI Range</th>
                <th>Note Range</th>
            </tr>
            <tr>
                <td>-1</td>
                <td>0-11</td>
                <td>C-1 to B-1</td>
            </tr>
            <tr>
                <td>0</td>
                <td>12-23</td>
                <td>C0 to B0</td>
            </tr>
            <tr>
                <td>1</td>
                <td>24-35</td>
                <td>C1 to B1</td>
            </tr>
            <tr>
                <td>2</td>
                <td>36-47</td>
                <td>C2 to B2</td>
            </tr>
            <tr>
                <td>3</td>
                <td>48-59</td>
                <td>C3 to B3</td>
            </tr>
            <tr>
                <td>4</td>
                <td>60-71</td>
                <td>C4 to B4</td>
            </tr>
            <tr>
                <td>5</td>
                <td>72-83</td>
                <td>C5 to B5</td>
            </tr>
            <tr>
                <td>6</td>
                <td>84-95</td>
                <td>C6 to B6</td>
            </tr>
            <tr>
                <td>7</td>
                <td>96-107</td>
                <td>C7 to B7</td>
            </tr>
            <tr>
                <td>8</td>
                <td>108-119</td>
                <td>C8 to B8</td>
            </tr>
            <tr>
                <td>9</td>
                <td>120-127</td>
                <td>C9 to G9</td>
            </tr>
        </table>
    </tab>
</tabs>

## Converting Notes to MIDI

### Basic Conversion

```C#
// Note to MIDI number
var c4 = new Note(NoteName.C, Alteration.Natural, 4);
int midiNumber = c4.MidiNumber; // 60

var a4 = new Note(NoteName.A, Alteration.Natural, 4);
int a4Midi = a4.MidiNumber; // 69

var cSharp5 = new Note(NoteName.C, Alteration.Sharp, 5);
int cSharp5Midi = cSharp5.MidiNumber; // 73

// Enharmonic notes have the same MIDI number
var dFlat5 = new Note(NoteName.D, Alteration.Flat, 5);
int dFlat5Midi = dFlat5.MidiNumber; // 73 (same as C#5)
```

### Handling Out-of-Range Notes

```C#
try
{
    // This note is too low for MIDI
    var tooLow = new Note(NoteName.C, Alteration.Natural, -2);
    int midi = tooLow.MidiNumber; // Throws InvalidOperationException
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    // "Note C-2 is outside the valid MIDI range (0-127)."
}

// Safe conversion with validation
public int? GetMidiNumberSafe(Note note)
{
    try
    {
        return note.MidiNumber;
    }
    catch (InvalidOperationException)
    {
        return null; // Note is out of MIDI range
    }
}
```

## Converting MIDI to Notes

### MIDI to Note Conversion

```C#
// MIDI number to Note
var middleC = Note.FromMidiNumber(60);
Console.WriteLine(middleC); // C4

var a440 = Note.FromMidiNumber(69);
Console.WriteLine(a440); // A4

// Low and high extremes
var lowest = Note.FromMidiNumber(0);   // C-1
var highest = Note.FromMidiNumber(127); // G9
```

### Enharmonic Preferences

```C#
// Default conversion prefers sharps for black keys
var note61 = Note.FromMidiNumber(61);
Console.WriteLine(note61); // C#4

// Prefer flats for black keys
var note61Flat = Note.FromMidiNumber(61, preferFlats: true);
Console.WriteLine(note61Flat); // Db4

// Create a conversion preference based on key
public Note MidiToNoteInKey(int midiNumber, KeySignature key)
{
    // Prefer flats if key has flats
    bool useFlats = key.AccidentalCount < 0;
    return Note.FromMidiNumber(midiNumber, useFlats);
}
```

## Working with MIDI Chords

### Chord to MIDI Numbers

```C#
public class MidiChordHelper
{
    public static List<int> GetMidiNumbers(Chord chord)
    {
        return chord.GetNotes()
            .Select(note => note.MidiNumber)
            .ToList();
    }
    
    public static Dictionary<string, int> GetMidiMap(Chord chord)
    {
        var notes = chord.GetNotes().ToList();
        var degrees = new[] { "Root", "Third", "Fifth", "Seventh", "Ninth", "Eleventh", "Thirteenth" };
        
        var map = new Dictionary<string, int>();
        for (int i = 0; i < notes.Count && i < degrees.Length; i++)
        {
            map[degrees[i]] = notes[i].MidiNumber;
        }
        
        return map;
    }
}

// Usage
var cMaj7 = new Chord(new Note(NoteName.C, 4), ChordType.Major7);
var midiNumbers = MidiChordHelper.GetMidiNumbers(cMaj7);
// [60, 64, 67, 71] (C, E, G, B)

var midiMap = MidiChordHelper.GetMidiMap(cMaj7);
// { "Root": 60, "Third": 64, "Fifth": 67, "Seventh": 71 }
```

### MIDI Numbers to Chord

```C#
public class MidiChordAnalyzer
{
    public static string AnalyzeChord(params int[] midiNumbers)
    {
        if (midiNumbers.Length < 3)
            return "Not enough notes for a chord";
            
        // Convert to notes (prefer appropriate spelling)
        var notes = midiNumbers
            .OrderBy(m => m)
            .Select(m => Note.FromMidiNumber(m))
            .ToList();
            
        // Find the root (lowest note for now)
        var root = notes[0];
        
        // Analyze intervals from root
        var intervals = notes.Skip(1)
            .Select(n => Interval.Between(root, n))
            .ToList();
            
        // Determine chord type based on intervals
        return DetermineChordType(intervals);
    }
    
    private static string DetermineChordType(List<Interval> intervals)
    {
        // Simplified analysis
        if (intervals.Count >= 2)
        {
            var third = intervals[0];
            var fifth = intervals[1];
            
            if (third.Semitones == 4 && fifth.Semitones == 7)
                return "Major";
            if (third.Semitones == 3 && fifth.Semitones == 7)
                return "Minor";
            if (third.Semitones == 3 && fifth.Semitones == 6)
                return "Diminished";
            if (third.Semitones == 4 && fifth.Semitones == 8)
                return "Augmented";
        }
        
        return "Unknown";
    }
}

// Usage
var chordType = MidiChordAnalyzer.AnalyzeChord(60, 64, 67); // "Major"
```

## MIDI Scale Patterns

### Scale to MIDI Sequence

```C#
public class MidiScaleGenerator
{
    public static List<int> GenerateMidiScale(
        int rootMidi, 
        ScaleType scaleType, 
        int octaves = 1)
    {
        var root = Note.FromMidiNumber(rootMidi);
        var scale = new Scale(root, scaleType);
        
        var midiNumbers = new List<int>();
        var scaleNotes = scale.GetNotes().Take(7).ToList();
        
        // Generate for specified octaves
        for (int oct = 0; oct < octaves; oct++)
        {
            foreach (var note in scaleNotes)
            {
                var transposed = note.TransposeBySemitones(oct * 12);
                if (transposed.MidiNumber <= 127)
                {
                    midiNumbers.Add(transposed.MidiNumber);
                }
            }
        }
        
        // Add the octave
        var octaveNote = root.TransposeBySemitones(octaves * 12);
        if (octaveNote.MidiNumber <= 127)
        {
            midiNumbers.Add(octaveNote.MidiNumber);
        }
        
        return midiNumbers;
    }
}

// Generate C major scale over 2 octaves
var cMajorMidi = MidiScaleGenerator.GenerateMidiScale(60, ScaleType.Major, 2);
// [60, 62, 64, 65, 67, 69, 71, 72, 74, 76, 77, 79, 81, 83, 84]
```

## MIDI Velocity and Dynamics

While the library focuses on pitch, here's how to integrate with MIDI velocity:

```C#
public class MidiNoteEvent
{
    public int MidiNumber { get; set; }
    public int Velocity { get; set; }
    public double Duration { get; set; }
    
    public MidiNoteEvent(Note note, int velocity = 64, double duration = 0.5)
    {
        MidiNumber = note.MidiNumber;
        Velocity = Math.Clamp(velocity, 0, 127);
        Duration = duration;
    }
}

public enum Dynamic
{
    PPP = 16,  // Pianississimo
    PP = 33,   // Pianissimo
    P = 49,    // Piano
    MP = 64,   // Mezzo-piano
    MF = 80,   // Mezzo-forte
    F = 96,    // Forte
    FF = 112,  // Fortissimo
    FFF = 127  // Fortississimo
}

// Create notes with dynamics
var softNote = new MidiNoteEvent(
    new Note(NoteName.C, 4), 
    (int)Dynamic.P
);

var loudNote = new MidiNoteEvent(
    new Note(NoteName.C, 4), 
    (int)Dynamic.FF
);
```

## Integration Examples

### MIDI File Creation Helper

```C#
public class MidiTrackBuilder
{
    private List<MidiNoteEvent> events = new List<MidiNoteEvent>();
    private double currentTime = 0;
    
    public MidiTrackBuilder AddNote(Note note, double duration, int velocity = 64)
    {
        events.Add(new MidiNoteEvent(note, velocity, duration));
        currentTime += duration;
        return this;
    }
    
    public MidiTrackBuilder AddChord(Chord chord, double duration, int velocity = 64)
    {
        foreach (var note in chord.GetNotes())
        {
            events.Add(new MidiNoteEvent(note, velocity, duration));
        }
        currentTime += duration;
        return this;
    }
    
    public MidiTrackBuilder AddScale(Scale scale, double noteDuration = 0.25, int velocity = 64)
    {
        foreach (var note in scale.GetNotes())
        {
            events.Add(new MidiNoteEvent(note, velocity, noteDuration));
            currentTime += noteDuration;
        }
        return this;
    }
    
    public List<MidiNoteEvent> Build() => events;
}

// Build a simple melody
var track = new MidiTrackBuilder()
    .AddNote(new Note(NoteName.C, 4), 0.5)
    .AddNote(new Note(NoteName.D, 4), 0.5)
    .AddNote(new Note(NoteName.E, 4), 0.5)
    .AddNote(new Note(NoteName.F, 4), 0.5)
    .AddChord(new Chord(new Note(NoteName.C, 4), ChordType.Major), 1.0)
    .Build();
```

### MIDI Input Processing

```C#
public class MidiInputProcessor
{
    private Dictionary<int, Note> activeNotes = new Dictionary<int, Note>();
    
    public void ProcessNoteOn(int midiNumber, int velocity)
    {
        var note = Note.FromMidiNumber(midiNumber);
        activeNotes[midiNumber] = note;
        
        Console.WriteLine($"Note On: {note} (MIDI {midiNumber}, Velocity {velocity})");
        
        // Analyze currently playing notes
        if (activeNotes.Count >= 3)
        {
            var chordNotes = activeNotes.Values.OrderBy(n => n.MidiNumber).ToList();
            // Analyze for chord detection
        }
    }
    
    public void ProcessNoteOff(int midiNumber)
    {
        if (activeNotes.TryGetValue(midiNumber, out var note))
        {
            activeNotes.Remove(midiNumber);
            Console.WriteLine($"Note Off: {note} (MIDI {midiNumber})");
        }
    }
}
```

## Best Practices

- **Validate MIDI ranges**: Always check that MIDI numbers are within 0-127
- **Handle enharmonics appropriately**: Choose note spellings based on musical context
- **Consider velocity**: MIDI velocity (0-127) affects dynamics and expression
- **Use appropriate timing**: MIDI timing is typically in ticks or milliseconds
- **Buffer management**: When generating MIDI data, manage memory efficiently

## Common MIDI Mappings

### Drum Map (GM Standard)

```C#
public static class GeneralMidiDrums
{
    public const int BassDrum1 = 36;
    public const int SnareDrum = 38;
    public const int ClosedHiHat = 42;
    public const int OpenHiHat = 46;
    public const int CrashCymbal1 = 49;
    public const int RideCymbal1 = 51;
    
    // Note: Drums are typically on MIDI channel 10
}
```

### Program Changes

```C#
public enum GeneralMidiProgram
{
    AcousticGrandPiano = 1,
    ElectricPiano1 = 5,
    Harpsichord = 7,
    Vibraphone = 12,
    AcousticGuitar = 25,
    ElectricGuitarClean = 28,
    DistortionGuitar = 31,
    AcousticBass = 33,
    ElectricBass = 34,
    Violin = 41,
    StringEnsemble1 = 49,
    Trumpet = 57,
    Trombone = 58,
    AltoSax = 66,
    // ... and many more
}
```

## See Also

<seealso>
    <category ref="related">
        <a href="notes.md">Note MIDI Conversion</a>
        <a href="examples.md">MIDI Examples</a>
    </category>
    <category ref="external">
        <a href="https://www.midi.org/">MIDI Manufacturers Association</a>
        <a href="https://en.wikipedia.org/wiki/General_MIDI">General MIDI Specification</a>
    </category>
</seealso>