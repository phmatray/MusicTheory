namespace MusicTheory;

/// <summary>
/// Represents the quality of a chord.
/// </summary>
public enum ChordQuality
{
    /// <summary>Major chord (1-3-5)</summary>
    Major,
    /// <summary>Minor chord (1-♭3-5)</summary>
    Minor,
    /// <summary>Diminished chord (1-♭3-♭5)</summary>
    Diminished,
    /// <summary>Augmented chord (1-3-♯5)</summary>
    Augmented
}

/// <summary>
/// Represents a musical chord.
/// </summary>
public class Chord
{
    /// <summary>
    /// Gets the root note of the chord.
    /// </summary>
    public Note Root { get; }

    /// <summary>
    /// Gets the quality of the chord.
    /// </summary>
    public ChordQuality Quality { get; }

    /// <summary>
    /// Gets the extensions added to the chord.
    /// </summary>
    private List<(int Number, IntervalQuality Quality)> Extensions { get; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Chord"/> class.
    /// </summary>
    /// <param name="root">The root note of the chord.</param>
    /// <param name="quality">The quality of the chord.</param>
    public Chord(Note root, ChordQuality quality)
    {
        Root = root;
        Quality = quality;
    }

    /// <summary>
    /// Private constructor for creating a transposed chord with extensions.
    /// </summary>
    private Chord(Note root, ChordQuality quality, List<(int Number, IntervalQuality Quality)> extensions)
    {
        Root = root;
        Quality = quality;
        Extensions = new List<(int Number, IntervalQuality Quality)>(extensions);
    }

    /// <summary>
    /// Adds an extension to the chord.
    /// </summary>
    /// <param name="intervalNumber">The interval number (e.g., 7 for seventh, 9 for ninth).</param>
    /// <param name="quality">The quality of the interval.</param>
    /// <returns>The chord instance for fluent chaining.</returns>
    public Chord AddExtension(int intervalNumber, IntervalQuality quality)
    {
        Extensions.Add((intervalNumber, quality));
        return this;
    }

    /// <summary>
    /// Gets the notes that make up the chord.
    /// </summary>
    /// <returns>An enumerable of notes in the chord.</returns>
    public IEnumerable<Note> GetNotes()
    {
        yield return Root;

        var intervals = Quality switch
        {
            ChordQuality.Major => new[] { (IntervalQuality.Major, 3), (IntervalQuality.Perfect, 5) },
            ChordQuality.Minor => new[] { (IntervalQuality.Minor, 3), (IntervalQuality.Perfect, 5) },
            ChordQuality.Diminished => new[] { (IntervalQuality.Minor, 3), (IntervalQuality.Diminished, 5) },
            ChordQuality.Augmented => new[] { (IntervalQuality.Major, 3), (IntervalQuality.Augmented, 5) },
            _ => throw new ArgumentOutOfRangeException()
        };

        foreach (var (quality, number) in intervals)
        {
            var interval = new Interval(quality, number);
            yield return GetNoteAtInterval(Root, interval);
        }

        // Add extension notes
        foreach (var (number, quality) in Extensions)
        {
            var interval = new Interval(quality, number);
            yield return GetNoteAtInterval(Root, interval);
        }
    }

    /// <summary>
    /// Gets a note at a specific interval from a base note.
    /// </summary>
    private static Note GetNoteAtInterval(Note baseNote, Interval interval)
    {
        // Calculate target semitones from C0
        int baseSemitones = GetTotalSemitones(baseNote);
        int targetSemitones = baseSemitones + interval.Semitones;

        // Calculate target octave and note within octave
        int targetOctave = targetSemitones / 12;
        int semitonesInOctave = targetSemitones % 12;

        // Find the note name and alteration
        // This is a simplified approach - in practice, we'd need to consider enharmonic equivalents
        int[] semitonesFromC = { 0, 2, 4, 5, 7, 9, 11 };
        
        // Calculate the expected note based on interval number
        int baseNoteIndex = (int)baseNote.Name;
        int targetNoteIndex = (baseNoteIndex + interval.Number - 1) % 7;
        NoteName targetNoteName = (NoteName)targetNoteIndex;

        // Calculate required alteration
        int expectedSemitones = semitonesFromC[targetNoteIndex];
        int actualSemitonesInOctave = semitonesInOctave;
        int alterationValue = actualSemitonesInOctave - expectedSemitones;

        // Handle wrapping around octave
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

        Alteration alteration = (Alteration)alterationValue;

        return new Note(targetNoteName, alteration, targetOctave);
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
    /// Transposes the chord by the specified interval.
    /// </summary>
    /// <param name="interval">The interval to transpose by.</param>
    /// <param name="direction">The direction to transpose (default is Up).</param>
    /// <returns>A new chord transposed by the interval.</returns>
    public Chord Transpose(Interval interval, Direction direction = Direction.Up)
    {
        var newRoot = Root.Transpose(interval, direction);
        return new Chord(newRoot, Quality, Extensions);
    }
}