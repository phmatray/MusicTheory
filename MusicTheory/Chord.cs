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
    /// Gets the inversion of the chord.
    /// </summary>
    public ChordInversion Inversion { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Chord"/> class.
    /// </summary>
    /// <param name="root">The root note of the chord.</param>
    /// <param name="quality">The quality of the chord.</param>
    public Chord(Note root, ChordQuality quality)
    {
        Root = root;
        Quality = quality;
        Inversion = ChordInversion.Root;
    }

    /// <summary>
    /// Private constructor for creating a chord with all properties.
    /// </summary>
    private Chord(Note root, ChordQuality quality, List<(int Number, IntervalQuality Quality)> extensions, ChordInversion inversion)
    {
        Root = root;
        Quality = quality;
        Extensions = new List<(int Number, IntervalQuality Quality)>(extensions);
        Inversion = inversion;
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
        return new Chord(newRoot, Quality, Extensions, Inversion);
    }

    /// <summary>
    /// Creates a new chord with the specified inversion.
    /// </summary>
    /// <param name="inversion">The inversion to apply.</param>
    /// <returns>A new chord with the specified inversion.</returns>
    public Chord WithInversion(ChordInversion inversion)
    {
        return new Chord(Root, Quality, Extensions, inversion);
    }

    /// <summary>
    /// Gets the bass note of the chord based on its inversion.
    /// </summary>
    /// <returns>The bass note.</returns>
    public Note GetBassNote()
    {
        var notes = GetNotes().ToList();
        
        return Inversion switch
        {
            ChordInversion.Root => notes[0],   // Root
            ChordInversion.First => notes[1],  // Third
            ChordInversion.Second => notes[2], // Fifth
            ChordInversion.Third => notes.Count > 3 ? notes[3] : throw new InvalidOperationException("Third inversion requires a seventh chord"),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    /// <summary>
    /// Gets the notes of the chord arranged according to the inversion.
    /// </summary>
    /// <returns>An enumerable of notes in inversion order.</returns>
    public IEnumerable<Note> GetNotesInInversion()
    {
        var notes = GetNotes().ToList();
        
        switch (Inversion)
        {
            case ChordInversion.Root:
                return notes;
                
            case ChordInversion.First:
                // Move root up an octave
                var rootUpOctave = new Note(notes[0].Name, notes[0].Alteration, notes[0].Octave + 1);
                return new[] { notes[1], notes[2] }.Concat(notes.Skip(3)).Append(rootUpOctave);
                
            case ChordInversion.Second:
                // Move root and third up an octave
                var rootUp = new Note(notes[0].Name, notes[0].Alteration, notes[0].Octave + 1);
                var thirdUp = new Note(notes[1].Name, notes[1].Alteration, notes[1].Octave + 1);
                return new[] { notes[2] }.Concat(notes.Skip(3)).Append(rootUp).Append(thirdUp);
                
            case ChordInversion.Third:
                if (notes.Count < 4)
                    throw new InvalidOperationException("Third inversion requires a seventh chord");
                // Move root, third, and fifth up an octave
                var rootUp3 = new Note(notes[0].Name, notes[0].Alteration, notes[0].Octave + 1);
                var thirdUp3 = new Note(notes[1].Name, notes[1].Alteration, notes[1].Octave + 1);
                var fifthUp3 = new Note(notes[2].Name, notes[2].Alteration, notes[2].Octave + 1);
                return new[] { notes[3] }.Concat(notes.Skip(4)).Append(rootUp3).Append(thirdUp3).Append(fifthUp3);
                
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    /// Gets the slash chord notation for the chord.
    /// </summary>
    /// <returns>The slash chord notation (e.g., "C/E" for C major first inversion).</returns>
    public string GetSlashChordNotation()
    {
        var rootName = Root.Name.ToString();
        var bassNote = GetBassNote();
        var bassName = bassNote.Name.ToString();
        
        if (Inversion == ChordInversion.Root)
            return $"{rootName}/{rootName}";
        
        return $"{rootName}/{bassName}";
    }

    /// <summary>
    /// Gets the enharmonic equivalent of this chord.
    /// </summary>
    /// <returns>A new chord with an enharmonically equivalent root note, or null if no equivalent exists.</returns>
    public Chord? GetEnharmonicEquivalent()
    {
        var enharmonicRoot = Root.GetEnharmonicEquivalent();
        if (enharmonicRoot == null)
            return null;
            
        return new Chord(enharmonicRoot, Quality, Extensions, Inversion);
    }
}