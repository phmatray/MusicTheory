namespace MusicTheory;

/// <summary>
/// Represents different types of musical scales.
/// </summary>
public enum ScaleType
{
    /// <summary>Major scale (W-W-H-W-W-W-H)</summary>
    Major,
    /// <summary>Natural minor scale (W-H-W-W-H-W-W)</summary>
    NaturalMinor,
    /// <summary>Harmonic minor scale (W-H-W-W-H-WH-H)</summary>
    HarmonicMinor,
    /// <summary>Melodic minor scale (W-H-W-W-W-W-H ascending)</summary>
    MelodicMinor
}

/// <summary>
/// Represents a musical scale.
/// </summary>
public class Scale
{
    /// <summary>
    /// Gets the root note of the scale.
    /// </summary>
    public Note Root { get; }

    /// <summary>
    /// Gets the type of the scale.
    /// </summary>
    public ScaleType Type { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Scale"/> class.
    /// </summary>
    /// <param name="root">The root note of the scale.</param>
    /// <param name="type">The type of scale.</param>
    public Scale(Note root, ScaleType type)
    {
        Root = root;
        Type = type;
    }

    /// <summary>
    /// Gets the notes in the scale.
    /// </summary>
    /// <returns>An enumerable of notes in the scale, including the octave.</returns>
    public IEnumerable<Note> GetNotes()
    {
        yield return Root;

        // Get the interval pattern for the scale type
        var intervals = GetIntervalPattern();
        var currentNote = Root;

        foreach (var interval in intervals.Take(7)) // Take 7 to get to the octave
        {
            currentNote = GetNextNoteInScale(currentNote, interval);
            yield return currentNote;
        }
    }

    /// <summary>
    /// Gets the interval pattern for the scale type.
    /// </summary>
    private IntervalStep[] GetIntervalPattern()
    {
        return Type switch
        {
            ScaleType.Major => new[] 
            { 
                IntervalStep.Whole, IntervalStep.Whole, IntervalStep.Half, 
                IntervalStep.Whole, IntervalStep.Whole, IntervalStep.Whole, IntervalStep.Half 
            },
            ScaleType.NaturalMinor => new[] 
            { 
                IntervalStep.Whole, IntervalStep.Half, IntervalStep.Whole, 
                IntervalStep.Whole, IntervalStep.Half, IntervalStep.Whole, IntervalStep.Whole 
            },
            ScaleType.HarmonicMinor => new[] 
            { 
                IntervalStep.Whole, IntervalStep.Half, IntervalStep.Whole, 
                IntervalStep.Whole, IntervalStep.Half, IntervalStep.WholeAndHalf, IntervalStep.Half 
            },
            ScaleType.MelodicMinor => new[] 
            { 
                IntervalStep.Whole, IntervalStep.Half, IntervalStep.Whole, 
                IntervalStep.Whole, IntervalStep.Whole, IntervalStep.Whole, IntervalStep.Half 
            },
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    /// <summary>
    /// Gets the next note in the scale based on the interval step.
    /// </summary>
    private Note GetNextNoteInScale(Note currentNote, IntervalStep step)
    {
        // Calculate the next note name
        var nextNoteName = (NoteName)(((int)currentNote.Name + 1) % 7);
        
        // Calculate semitones to add
        int semitones = step switch
        {
            IntervalStep.Half => 1,
            IntervalStep.Whole => 2,
            IntervalStep.WholeAndHalf => 3,
            _ => throw new ArgumentOutOfRangeException()
        };

        // Create interval and calculate the actual next note
        var interval = new Interval(IntervalQuality.Major, 2); // This is temporary, we need to calculate proper interval
        
        // Get current note semitones from C0
        int currentSemitones = GetTotalSemitones(currentNote);
        int targetSemitones = currentSemitones + semitones;
        
        // Calculate octave and alteration
        int targetOctave = targetSemitones / 12;
        int semitonesInOctave = targetSemitones % 12;
        
        // Get expected semitones for the next note name
        int[] semitonesFromC = { 0, 2, 4, 5, 7, 9, 11 };
        int expectedSemitones = semitonesFromC[(int)nextNoteName];
        
        // Calculate alteration needed
        int alterationValue = semitonesInOctave - expectedSemitones;
        
        // Handle octave wrapping
        if (alterationValue < -2)
        {
            alterationValue += 12;
            targetOctave--;
        }
        else if (alterationValue > 2)
        {
            alterationValue -= 12;
            targetOctave++;
        }

        // Ensure we use the correct octave when moving from B to C
        if (currentNote.Name == NoteName.B && nextNoteName == NoteName.C)
        {
            targetOctave = currentNote.Octave + 1;
        }
        else if (nextNoteName < currentNote.Name)
        {
            targetOctave = currentNote.Octave + 1;
        }
        else
        {
            targetOctave = currentNote.Octave;
        }

        // Recalculate alteration with correct octave
        targetSemitones = currentSemitones + semitones;
        int actualSemitonesFromC0 = targetOctave * 12 + expectedSemitones;
        alterationValue = targetSemitones - actualSemitonesFromC0;

        var alteration = (Alteration)alterationValue;
        
        return new Note(nextNoteName, alteration, targetOctave);
    }

    /// <summary>
    /// Gets the total semitones from C0 for a given note.
    /// </summary>
    private static int GetTotalSemitones(Note note)
    {
        int[] semitonesFromC = { 0, 2, 4, 5, 7, 9, 11 };
        return note.Octave * 12 + semitonesFromC[(int)note.Name] + (int)note.Alteration;
    }

    /// <summary>
    /// Represents the interval steps in a scale.
    /// </summary>
    private enum IntervalStep
    {
        Half,
        Whole,
        WholeAndHalf
    }
}