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
    MelodicMinor,
    
    // Modal scales
    /// <summary>Ionian mode (W-W-H-W-W-W-H) - same as Major</summary>
    Ionian,
    /// <summary>Dorian mode (W-H-W-W-W-H-W)</summary>
    Dorian,
    /// <summary>Phrygian mode (H-W-W-W-H-W-W)</summary>
    Phrygian,
    /// <summary>Lydian mode (W-W-W-H-W-W-H)</summary>
    Lydian,
    /// <summary>Mixolydian mode (W-W-H-W-W-H-W)</summary>
    Mixolydian,
    /// <summary>Aeolian mode (W-H-W-W-H-W-W) - same as Natural Minor</summary>
    Aeolian,
    /// <summary>Locrian mode (H-W-W-H-W-W-W)</summary>
    Locrian,
    
    // Other scales
    /// <summary>Chromatic scale (all semitones)</summary>
    Chromatic,
    /// <summary>Whole tone scale (all whole tones)</summary>
    WholeTone,
    /// <summary>Blues scale (1-b3-4-#4-5-b7)</summary>
    Blues,
    /// <summary>Major pentatonic scale (1-2-3-5-6)</summary>
    PentatonicMajor,
    /// <summary>Minor pentatonic scale (1-b3-4-5-b7)</summary>
    PentatonicMinor,

    // Jazz scales
    /// <summary>Diminished scale starting with whole step (W-H-W-H-W-H-W-H)</summary>
    DiminishedWholeHalf,
    /// <summary>Diminished scale starting with half step (H-W-H-W-H-W-H-W)</summary>
    DiminishedHalfWhole,
    /// <summary>Altered scale / Super Locrian (H-W-H-W-W-W-W) - 7th mode of melodic minor</summary>
    Altered,
    /// <summary>Lydian dominant scale (W-W-W-H-W-H-W) - 4th mode of melodic minor</summary>
    LydianDominant,
    /// <summary>Locrian #2 scale (W-H-W-H-W-W-W) - 6th mode of melodic minor</summary>
    LocrianSharp2
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
        // Handle special scales differently
        switch (Type)
        {
            case ScaleType.Chromatic:
                return GetChromaticNotes();
            case ScaleType.WholeTone:
                return GetWholeToneNotes();
            case ScaleType.Blues:
                return GetBluesNotes();
            case ScaleType.PentatonicMajor:
                return GetPentatonicMajorNotes();
            case ScaleType.PentatonicMinor:
                return GetPentatonicMinorNotes();
            case ScaleType.DiminishedWholeHalf:
            case ScaleType.DiminishedHalfWhole:
                return GetDiminishedNotes();
            default:
                return GetDiatonicNotes();
        }
    }

    private IEnumerable<Note> GetDiatonicNotes()
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

    private IEnumerable<Note> GetChromaticNotes()
    {
        var currentNote = Root;
        yield return currentNote;

        for (int i = 1; i < MusicTheoryConstants.SemitonesPerOctave; i++)
        {
            currentNote = currentNote.TransposeBySemitones(1);
            yield return currentNote;
        }
    }

    private IEnumerable<Note> GetWholeToneNotes()
    {
        var currentNote = Root;
        yield return currentNote;

        for (int i = 1; i < 6; i++)
        {
            currentNote = currentNote.TransposeBySemitones(2);
            yield return currentNote;
        }
    }

    private IEnumerable<Note> GetBluesNotes()
    {
        yield return Root; // 1
        
        // b3 - minor third
        var minorThird = Root.Transpose(new Interval(IntervalQuality.Minor, 3));
        yield return minorThird;
        
        // 4 - perfect fourth
        var fourth = Root.Transpose(new Interval(IntervalQuality.Perfect, 4));
        yield return fourth;
        
        // #4/b5 - augmented fourth/diminished fifth
        var augmentedFourth = Root.Transpose(new Interval(IntervalQuality.Augmented, 4));
        yield return augmentedFourth;
        
        // 5 - perfect fifth
        var fifth = Root.Transpose(new Interval(IntervalQuality.Perfect, 5));
        yield return fifth;
        
        // b7 - minor seventh
        var minorSeventh = Root.Transpose(new Interval(IntervalQuality.Minor, 7));
        yield return minorSeventh;
    }

    private IEnumerable<Note> GetPentatonicMajorNotes()
    {
        yield return Root; // 1
        yield return Root.TransposeBySemitones(2); // 2
        yield return Root.TransposeBySemitones(4); // 3
        yield return Root.TransposeBySemitones(7); // 5
        yield return Root.TransposeBySemitones(9); // 6
    }

    private IEnumerable<Note> GetPentatonicMinorNotes()
    {
        yield return Root; // 1

        // b3 - minor third
        var minorThird = Root.Transpose(new Interval(IntervalQuality.Minor, 3));
        yield return minorThird;

        // 4 - perfect fourth
        var fourth = Root.Transpose(new Interval(IntervalQuality.Perfect, 4));
        yield return fourth;

        // 5 - perfect fifth
        var fifth = Root.Transpose(new Interval(IntervalQuality.Perfect, 5));
        yield return fifth;

        // b7 - minor seventh
        var minorSeventh = Root.Transpose(new Interval(IntervalQuality.Minor, 7));
        yield return minorSeventh;
    }

    private IEnumerable<Note> GetDiminishedNotes()
    {
        // Diminished scale is 8 notes (octatonic)
        var currentNote = Root;
        yield return currentNote;

        // Determine step pattern based on scale type
        var startWithWhole = Type == ScaleType.DiminishedWholeHalf;

        for (int i = 0; i < 7; i++)
        {
            var step = (i % 2 == 0) == startWithWhole ? 2 : 1;
            currentNote = currentNote.TransposeBySemitones(step);
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
            ScaleType.Major or ScaleType.Ionian =>
            [
                IntervalStep.Whole, IntervalStep.Whole, IntervalStep.Half, 
                IntervalStep.Whole, IntervalStep.Whole, IntervalStep.Whole, IntervalStep.Half
            ],
            ScaleType.NaturalMinor or ScaleType.Aeolian =>
            [
                IntervalStep.Whole, IntervalStep.Half, IntervalStep.Whole, 
                IntervalStep.Whole, IntervalStep.Half, IntervalStep.Whole, IntervalStep.Whole
            ],
            ScaleType.HarmonicMinor =>
            [
                IntervalStep.Whole, IntervalStep.Half, IntervalStep.Whole, 
                IntervalStep.Whole, IntervalStep.Half, IntervalStep.WholeAndHalf, IntervalStep.Half
            ],
            ScaleType.MelodicMinor =>
            [
                IntervalStep.Whole, IntervalStep.Half, IntervalStep.Whole, 
                IntervalStep.Whole, IntervalStep.Whole, IntervalStep.Whole, IntervalStep.Half
            ],
            ScaleType.Dorian =>
            [
                IntervalStep.Whole, IntervalStep.Half, IntervalStep.Whole,
                IntervalStep.Whole, IntervalStep.Whole, IntervalStep.Half, IntervalStep.Whole
            ],
            ScaleType.Phrygian =>
            [
                IntervalStep.Half, IntervalStep.Whole, IntervalStep.Whole,
                IntervalStep.Whole, IntervalStep.Half, IntervalStep.Whole, IntervalStep.Whole
            ],
            ScaleType.Lydian =>
            [
                IntervalStep.Whole, IntervalStep.Whole, IntervalStep.Whole,
                IntervalStep.Half, IntervalStep.Whole, IntervalStep.Whole, IntervalStep.Half
            ],
            ScaleType.Mixolydian =>
            [
                IntervalStep.Whole, IntervalStep.Whole, IntervalStep.Half,
                IntervalStep.Whole, IntervalStep.Whole, IntervalStep.Half, IntervalStep.Whole
            ],
            ScaleType.Locrian =>
            [
                IntervalStep.Half, IntervalStep.Whole, IntervalStep.Whole,
                IntervalStep.Half, IntervalStep.Whole, IntervalStep.Whole, IntervalStep.Whole
            ],
            ScaleType.Altered =>
            [
                IntervalStep.Half, IntervalStep.Whole, IntervalStep.Half,
                IntervalStep.Whole, IntervalStep.Whole, IntervalStep.Whole, IntervalStep.Whole
            ],
            ScaleType.LydianDominant =>
            [
                IntervalStep.Whole, IntervalStep.Whole, IntervalStep.Whole,
                IntervalStep.Half, IntervalStep.Whole, IntervalStep.Half, IntervalStep.Whole
            ],
            ScaleType.LocrianSharp2 =>
            [
                IntervalStep.Whole, IntervalStep.Half, IntervalStep.Whole,
                IntervalStep.Half, IntervalStep.Whole, IntervalStep.Whole, IntervalStep.Whole
            ],
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

        // Get current note semitones from C0
        int currentSemitones = MusicTheoryUtilities.GetTotalSemitones(currentNote);
        int targetSemitones = currentSemitones + semitones;
        
        // Calculate octave and alteration
        int targetOctave = targetSemitones / MusicTheoryConstants.SemitonesPerOctave;
        int semitonesInOctave = targetSemitones % MusicTheoryConstants.SemitonesPerOctave;
        
        // Get expected semitones for the next note name
        int expectedSemitones = MusicTheoryConstants.SemitonesFromC[(int)nextNoteName];
        
        // Calculate alteration needed
        int alterationValue = semitonesInOctave - expectedSemitones;
        
        // Handle octave wrapping
        if (alterationValue < -2)
        {
            alterationValue += MusicTheoryConstants.SemitonesPerOctave;
            targetOctave--;
        }
        else if (alterationValue > 2)
        {
            alterationValue -= MusicTheoryConstants.SemitonesPerOctave;
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
        int actualSemitonesFromC0 = targetOctave * MusicTheoryConstants.SemitonesPerOctave + expectedSemitones;
        alterationValue = targetSemitones - actualSemitonesFromC0;

        var alteration = (Alteration)alterationValue;
        
        return new Note(nextNoteName, alteration, targetOctave);
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

    /// <summary>
    /// Transposes the scale by the specified interval.
    /// </summary>
    /// <param name="interval">The interval to transpose by.</param>
    /// <param name="direction">The direction to transpose (default is Up).</param>
    /// <returns>A new scale transposed by the interval.</returns>
    public Scale Transpose(Interval interval, Direction direction = Direction.Up)
    {
        var newRoot = Root.Transpose(interval, direction);
        return new Scale(newRoot, Type);
    }

    /// <summary>
    /// Gets the next note in the scale after the given note.
    /// </summary>
    /// <param name="note">The current note.</param>
    /// <returns>The next note in the scale.</returns>
    /// <exception cref="ArgumentException">Thrown when the note is not in the scale.</exception>
    public Note GetNextNoteInScale(Note note)
    {
        var scaleNotes = GetNotes().ToList();
        
        // Find the note in the scale (ignoring octave)
        var index = -1;
        for (int i = 0; i < scaleNotes.Count; i++)
        {
            if (scaleNotes[i].Name == note.Name && scaleNotes[i].Alteration == note.Alteration)
            {
                index = i;
                break;
            }
        }
        
        if (index == -1)
        {
            throw new ArgumentException($"Note {note} is not in the {Root.Name} {Type} scale");
        }
        
        // Get next note, wrapping around to root if at end
        var nextIndex = (index + 1) % scaleNotes.Count;
        var nextNote = scaleNotes[nextIndex];
        
        // Check if we need to increment octave
        // If the next note is C and current is B, or if we wrapped to index 0
        if (nextIndex == 0 || (note.Name == NoteName.B && nextNote.Name == NoteName.C))
        {
            return new Note(nextNote.Name, nextNote.Alteration, note.Octave + 1);
        }
        
        // Otherwise, maintain the same octave
        return new Note(nextNote.Name, nextNote.Alteration, note.Octave);
    }

    /// <summary>
    /// Determines if the scale contains the specified note.
    /// </summary>
    /// <param name="note">The note to check.</param>
    /// <returns>True if the note is in the scale; otherwise, false.</returns>
    public bool Contains(Note note)
    {
        return GetNotes().Any(n => n.Name == note.Name && n.Alteration == note.Alteration);
    }

    /// <summary>
    /// Gets the scale degree of the given note.
    /// </summary>
    /// <param name="note">The note to find the degree for.</param>
    /// <returns>The scale degree (1-based), or null if the note is not in the scale.</returns>
    public int? GetDegree(Note note)
    {
        var scaleNotes = GetNotes().ToList();
        
        for (int i = 0; i < scaleNotes.Count; i++)
        {
            if (scaleNotes[i].Name == note.Name && scaleNotes[i].Alteration == note.Alteration)
            {
                return i + 1; // 1-based degree
            }
        }
        
        return null;
    }
}