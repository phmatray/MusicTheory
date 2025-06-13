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
/// Represents the type of chord with all possible variations.
/// </summary>
public enum ChordType
{
    // Triads
    /// <summary>Major triad (1-3-5)</summary>
    Major,
    /// <summary>Minor triad (1-♭3-5)</summary>
    Minor,
    /// <summary>Diminished triad (1-♭3-♭5)</summary>
    Diminished,
    /// <summary>Augmented triad (1-3-♯5)</summary>
    Augmented,
    
    // Seventh chords
    /// <summary>Major 7th (1-3-5-7)</summary>
    Major7,
    /// <summary>Minor 7th (1-♭3-5-♭7)</summary>
    Minor7,
    /// <summary>Dominant 7th (1-3-5-♭7)</summary>
    Dominant7,
    /// <summary>Minor major 7th (1-♭3-5-7)</summary>
    MinorMajor7,
    /// <summary>Half-diminished 7th (1-♭3-♭5-♭7)</summary>
    HalfDiminished7,
    /// <summary>Diminished 7th (1-♭3-♭5-♭♭7)</summary>
    Diminished7,
    /// <summary>Augmented 7th (1-3-♯5-♭7)</summary>
    Augmented7,
    /// <summary>Augmented major 7th (1-3-♯5-7)</summary>
    AugmentedMajor7,
    
    // Suspended chords
    /// <summary>Suspended 2nd (1-2-5)</summary>
    Sus2,
    /// <summary>Suspended 4th (1-4-5)</summary>
    Sus4,
    /// <summary>Suspended 2nd and 4th (1-2-4-5)</summary>
    Sus2Sus4,
    /// <summary>Dominant 7th suspended 4th (1-4-5-♭7)</summary>
    Dominant7Sus4,
    
    // Sixth chords
    /// <summary>Major 6th (1-3-5-6)</summary>
    Major6,
    /// <summary>Minor 6th (1-♭3-5-6)</summary>
    Minor6,
    /// <summary>Major 6th add 9 (1-3-5-6-9)</summary>
    Major6Add9,
    
    // Extended chords
    /// <summary>Major 9th (1-3-5-7-9)</summary>
    Major9,
    /// <summary>Minor 9th (1-♭3-5-♭7-9)</summary>
    Minor9,
    /// <summary>Dominant 9th (1-3-5-♭7-9)</summary>
    Dominant9,
    /// <summary>Major 11th (1-3-5-7-9-11)</summary>
    Major11,
    /// <summary>Minor 11th (1-♭3-5-♭7-9-11)</summary>
    Minor11,
    /// <summary>Dominant 11th (1-3-5-♭7-9-11)</summary>
    Dominant11,
    /// <summary>Major 13th (1-3-5-7-9-11-13)</summary>
    Major13,
    /// <summary>Minor 13th (1-♭3-5-♭7-9-11-13)</summary>
    Minor13,
    /// <summary>Dominant 13th (1-3-5-♭7-9-11-13)</summary>
    Dominant13,
    
    // Add chords
    /// <summary>Major add 9 (1-3-5-9)</summary>
    MajorAdd9,
    /// <summary>Minor add 9 (1-♭3-5-9)</summary>
    MinorAdd9,
    /// <summary>Major add 11 (1-3-5-11)</summary>
    MajorAdd11,
    /// <summary>Minor add 11 (1-♭3-5-11)</summary>
    MinorAdd11,
    
    // Altered chords
    /// <summary>Dominant 7th flat 5 (1-3-♭5-♭7)</summary>
    Dominant7Flat5,
    /// <summary>Dominant 7th sharp 5 (1-3-♯5-♭7)</summary>
    Dominant7Sharp5,
    /// <summary>Dominant 7th flat 9 (1-3-5-♭7-♭9)</summary>
    Dominant7Flat9,
    /// <summary>Dominant 7th sharp 9 (1-3-5-♭7-♯9)</summary>
    Dominant7Sharp9,
    /// <summary>Dominant 7th flat 5 flat 9 (1-3-♭5-♭7-♭9)</summary>
    Dominant7Flat5Flat9,
    /// <summary>Dominant 7th flat 5 sharp 9 (1-3-♭5-♭7-♯9)</summary>
    Dominant7Flat5Sharp9,
    /// <summary>Dominant 7th sharp 5 flat 9 (1-3-♯5-♭7-♭9)</summary>
    Dominant7Sharp5Flat9,
    /// <summary>Dominant 7th sharp 5 sharp 9 (1-3-♯5-♭7-♯9)</summary>
    Dominant7Sharp5Sharp9,
    /// <summary>Dominant 7th altered (1-3-♭5-♯5-♭7-♭9)</summary>
    Dominant7Alt,
    
    // Power chords
    /// <summary>Power chord (1-5)</summary>
    Power5
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
    /// Gets the type of the chord.
    /// </summary>
    public ChordType Type { get; }

    /// <summary>
    /// Gets the extensions added to the chord.
    /// </summary>
    private List<(int Number, IntervalQuality Quality)> Extensions { get; } = new();

    /// <summary>
    /// Gets the inversion of the chord.
    /// </summary>
    public ChordInversion Inversion { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Chord"/> class with ChordQuality.
    /// </summary>
    /// <param name="root">The root note of the chord.</param>
    /// <param name="quality">The quality of the chord.</param>
    public Chord(Note root, ChordQuality quality)
    {
        Root = root;
        Quality = quality;
        Type = quality switch
        {
            ChordQuality.Major => ChordType.Major,
            ChordQuality.Minor => ChordType.Minor,
            ChordQuality.Diminished => ChordType.Diminished,
            ChordQuality.Augmented => ChordType.Augmented,
            _ => throw new ArgumentOutOfRangeException(nameof(quality))
        };
        Inversion = ChordInversion.Root;
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Chord"/> class with ChordType.
    /// </summary>
    /// <param name="root">The root note of the chord.</param>
    /// <param name="type">The type of the chord.</param>
    public Chord(Note root, ChordType type)
    {
        Root = root;
        Type = type;
        Quality = type switch
        {
            ChordType.Major => ChordQuality.Major,
            ChordType.Minor => ChordQuality.Minor,
            ChordType.Diminished => ChordQuality.Diminished,
            ChordType.Augmented => ChordQuality.Augmented,
            _ => ChordQuality.Major // Default for extended chords
        };
        Inversion = ChordInversion.Root;
    }

    /// <summary>
    /// Private constructor for creating a chord with all properties.
    /// </summary>
    private Chord(Note root, ChordQuality quality, ChordType type, List<(int Number, IntervalQuality Quality)> extensions, ChordInversion inversion)
    {
        Root = root;
        Quality = quality;
        Type = type;
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

        var intervals = GetIntervalsForChordType(Type);

        foreach (var (quality, number) in intervals)
        {
            var interval = new Interval(quality, number);
            yield return GetNoteAtInterval(Root, interval);
        }

        // Add extension notes (for backwards compatibility)
        foreach (var (number, quality) in Extensions)
        {
            var interval = new Interval(quality, number);
            yield return GetNoteAtInterval(Root, interval);
        }
    }
    
    /// <summary>
    /// Gets the intervals for a specific chord type.
    /// </summary>
    private static (IntervalQuality Quality, int Number)[] GetIntervalsForChordType(ChordType type)
    {
        return type switch
        {
            // Triads
            ChordType.Major => new[] { (IntervalQuality.Major, 3), (IntervalQuality.Perfect, 5) },
            ChordType.Minor => new[] { (IntervalQuality.Minor, 3), (IntervalQuality.Perfect, 5) },
            ChordType.Diminished => new[] { (IntervalQuality.Minor, 3), (IntervalQuality.Diminished, 5) },
            ChordType.Augmented => new[] { (IntervalQuality.Major, 3), (IntervalQuality.Augmented, 5) },
            
            // Seventh chords
            ChordType.Major7 => new[] { (IntervalQuality.Major, 3), (IntervalQuality.Perfect, 5), (IntervalQuality.Major, 7) },
            ChordType.Minor7 => new[] { (IntervalQuality.Minor, 3), (IntervalQuality.Perfect, 5), (IntervalQuality.Minor, 7) },
            ChordType.Dominant7 => new[] { (IntervalQuality.Major, 3), (IntervalQuality.Perfect, 5), (IntervalQuality.Minor, 7) },
            ChordType.MinorMajor7 => new[] { (IntervalQuality.Minor, 3), (IntervalQuality.Perfect, 5), (IntervalQuality.Major, 7) },
            ChordType.HalfDiminished7 => new[] { (IntervalQuality.Minor, 3), (IntervalQuality.Diminished, 5), (IntervalQuality.Minor, 7) },
            ChordType.Diminished7 => new[] { (IntervalQuality.Minor, 3), (IntervalQuality.Diminished, 5), (IntervalQuality.Diminished, 7) },
            ChordType.Augmented7 => new[] { (IntervalQuality.Major, 3), (IntervalQuality.Augmented, 5), (IntervalQuality.Minor, 7) },
            ChordType.AugmentedMajor7 => new[] { (IntervalQuality.Major, 3), (IntervalQuality.Augmented, 5), (IntervalQuality.Major, 7) },
            
            // Suspended chords
            ChordType.Sus2 => new[] { (IntervalQuality.Major, 2), (IntervalQuality.Perfect, 5) },
            ChordType.Sus4 => new[] { (IntervalQuality.Perfect, 4), (IntervalQuality.Perfect, 5) },
            ChordType.Sus2Sus4 => new[] { (IntervalQuality.Major, 2), (IntervalQuality.Perfect, 4), (IntervalQuality.Perfect, 5) },
            ChordType.Dominant7Sus4 => new[] { (IntervalQuality.Perfect, 4), (IntervalQuality.Perfect, 5), (IntervalQuality.Minor, 7) },
            
            // Sixth chords
            ChordType.Major6 => new[] { (IntervalQuality.Major, 3), (IntervalQuality.Perfect, 5), (IntervalQuality.Major, 6) },
            ChordType.Minor6 => new[] { (IntervalQuality.Minor, 3), (IntervalQuality.Perfect, 5), (IntervalQuality.Major, 6) },
            ChordType.Major6Add9 => new[] { (IntervalQuality.Major, 3), (IntervalQuality.Perfect, 5), (IntervalQuality.Major, 6), (IntervalQuality.Major, 9) },
            
            // Extended chords
            ChordType.Major9 => new[] { (IntervalQuality.Major, 3), (IntervalQuality.Perfect, 5), (IntervalQuality.Major, 7), (IntervalQuality.Major, 9) },
            ChordType.Minor9 => new[] { (IntervalQuality.Minor, 3), (IntervalQuality.Perfect, 5), (IntervalQuality.Minor, 7), (IntervalQuality.Major, 9) },
            ChordType.Dominant9 => new[] { (IntervalQuality.Major, 3), (IntervalQuality.Perfect, 5), (IntervalQuality.Minor, 7), (IntervalQuality.Major, 9) },
            ChordType.Major11 => new[] { (IntervalQuality.Major, 3), (IntervalQuality.Perfect, 5), (IntervalQuality.Major, 7), (IntervalQuality.Major, 9), (IntervalQuality.Perfect, 11) },
            ChordType.Minor11 => new[] { (IntervalQuality.Minor, 3), (IntervalQuality.Perfect, 5), (IntervalQuality.Minor, 7), (IntervalQuality.Major, 9), (IntervalQuality.Perfect, 11) },
            ChordType.Dominant11 => new[] { (IntervalQuality.Major, 3), (IntervalQuality.Perfect, 5), (IntervalQuality.Minor, 7), (IntervalQuality.Major, 9), (IntervalQuality.Perfect, 11) },
            ChordType.Major13 => new[] { (IntervalQuality.Major, 3), (IntervalQuality.Perfect, 5), (IntervalQuality.Major, 7), (IntervalQuality.Major, 9), (IntervalQuality.Perfect, 11), (IntervalQuality.Major, 13) },
            ChordType.Minor13 => new[] { (IntervalQuality.Minor, 3), (IntervalQuality.Perfect, 5), (IntervalQuality.Minor, 7), (IntervalQuality.Major, 9), (IntervalQuality.Perfect, 11), (IntervalQuality.Major, 13) },
            ChordType.Dominant13 => new[] { (IntervalQuality.Major, 3), (IntervalQuality.Perfect, 5), (IntervalQuality.Minor, 7), (IntervalQuality.Major, 9), (IntervalQuality.Perfect, 11), (IntervalQuality.Major, 13) },
            
            // Add chords
            ChordType.MajorAdd9 => new[] { (IntervalQuality.Major, 3), (IntervalQuality.Perfect, 5), (IntervalQuality.Major, 9) },
            ChordType.MinorAdd9 => new[] { (IntervalQuality.Minor, 3), (IntervalQuality.Perfect, 5), (IntervalQuality.Major, 9) },
            ChordType.MajorAdd11 => new[] { (IntervalQuality.Major, 3), (IntervalQuality.Perfect, 5), (IntervalQuality.Perfect, 11) },
            ChordType.MinorAdd11 => new[] { (IntervalQuality.Minor, 3), (IntervalQuality.Perfect, 5), (IntervalQuality.Perfect, 11) },
            
            // Altered chords
            ChordType.Dominant7Flat5 => new[] { (IntervalQuality.Major, 3), (IntervalQuality.Diminished, 5), (IntervalQuality.Minor, 7) },
            ChordType.Dominant7Sharp5 => new[] { (IntervalQuality.Major, 3), (IntervalQuality.Augmented, 5), (IntervalQuality.Minor, 7) },
            ChordType.Dominant7Flat9 => new[] { (IntervalQuality.Major, 3), (IntervalQuality.Perfect, 5), (IntervalQuality.Minor, 7), (IntervalQuality.Minor, 9) },
            ChordType.Dominant7Sharp9 => new[] { (IntervalQuality.Major, 3), (IntervalQuality.Perfect, 5), (IntervalQuality.Minor, 7), (IntervalQuality.Augmented, 9) },
            ChordType.Dominant7Flat5Flat9 => new[] { (IntervalQuality.Major, 3), (IntervalQuality.Diminished, 5), (IntervalQuality.Minor, 7), (IntervalQuality.Minor, 9) },
            ChordType.Dominant7Flat5Sharp9 => new[] { (IntervalQuality.Major, 3), (IntervalQuality.Diminished, 5), (IntervalQuality.Minor, 7), (IntervalQuality.Augmented, 9) },
            ChordType.Dominant7Sharp5Flat9 => new[] { (IntervalQuality.Major, 3), (IntervalQuality.Augmented, 5), (IntervalQuality.Minor, 7), (IntervalQuality.Minor, 9) },
            ChordType.Dominant7Sharp5Sharp9 => new[] { (IntervalQuality.Major, 3), (IntervalQuality.Augmented, 5), (IntervalQuality.Minor, 7), (IntervalQuality.Augmented, 9) },
            ChordType.Dominant7Alt => new[] { (IntervalQuality.Major, 3), (IntervalQuality.Diminished, 5), (IntervalQuality.Augmented, 5), (IntervalQuality.Minor, 7), (IntervalQuality.Minor, 9) },
            
            // Power chords
            ChordType.Power5 => new[] { (IntervalQuality.Perfect, 5) },
            
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    /// <summary>
    /// Gets a note at a specific interval from a base note.
    /// </summary>
    private static Note GetNoteAtInterval(Note baseNote, Interval interval)
    {
        // Calculate target semitones from C0
        int baseSemitones = MusicTheoryUtilities.GetTotalSemitones(baseNote);
        int targetSemitones = baseSemitones + interval.Semitones;

        // Calculate target octave and note within octave
        int targetOctave = targetSemitones / MusicTheoryConstants.SemitonesPerOctave;
        int semitonesInOctave = targetSemitones % MusicTheoryConstants.SemitonesPerOctave;

        // Find the note name and alteration
        // This is a simplified approach - in practice, we'd need to consider enharmonic equivalents
        
        // Calculate the expected note based on interval number
        int baseNoteIndex = (int)baseNote.Name;
        int targetNoteIndex = (baseNoteIndex + interval.Number - 1) % 7;
        NoteName targetNoteName = (NoteName)targetNoteIndex;

        // Calculate required alteration
        int expectedSemitones = MusicTheoryConstants.SemitonesFromC[targetNoteIndex];
        int actualSemitonesInOctave = semitonesInOctave;
        int alterationValue = actualSemitonesInOctave - expectedSemitones;

        // Handle wrapping around octave
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

        Alteration alteration = (Alteration)alterationValue;

        return new Note(targetNoteName, alteration, targetOctave);
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
        return new Chord(newRoot, Quality, Type, Extensions, Inversion);
    }

    /// <summary>
    /// Creates a new chord with the specified inversion.
    /// </summary>
    /// <param name="inversion">The inversion to apply.</param>
    /// <returns>A new chord with the specified inversion.</returns>
    public Chord WithInversion(ChordInversion inversion)
    {
        return new Chord(Root, Quality, Type, Extensions, inversion);
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
            
        return new Chord(enharmonicRoot, Quality, Type, Extensions, Inversion);
    }
    
    /// <summary>
    /// Gets the chord symbol representation.
    /// </summary>
    /// <returns>The chord symbol as a string.</returns>
    public string GetSymbol()
    {
        var rootSymbol = Root.Name.ToString();
        if (Root.Alteration == Alteration.Sharp)
            rootSymbol += "#";
        else if (Root.Alteration == Alteration.Flat)
            rootSymbol += "b";
        else if (Root.Alteration == Alteration.DoubleSharp)
            rootSymbol += "##";
        else if (Root.Alteration == Alteration.DoubleFlat)
            rootSymbol += "bb";
        
        var typeSymbol = Type switch
        {
            // Triads
            ChordType.Major => "",
            ChordType.Minor => "m",
            ChordType.Diminished => "°",
            ChordType.Augmented => "+",
            
            // Seventh chords
            ChordType.Major7 => "maj7",
            ChordType.Minor7 => "m7",
            ChordType.Dominant7 => "7",
            ChordType.MinorMajor7 => "m(maj7)",
            ChordType.HalfDiminished7 => "ø7",
            ChordType.Diminished7 => "°7",
            ChordType.Augmented7 => "+7",
            ChordType.AugmentedMajor7 => "+maj7",
            
            // Suspended chords
            ChordType.Sus2 => "sus2",
            ChordType.Sus4 => "sus4",
            ChordType.Sus2Sus4 => "sus2sus4",
            ChordType.Dominant7Sus4 => "7sus4",
            
            // Sixth chords
            ChordType.Major6 => "6",
            ChordType.Minor6 => "m6",
            ChordType.Major6Add9 => "6/9",
            
            // Extended chords
            ChordType.Major9 => "maj9",
            ChordType.Minor9 => "m9",
            ChordType.Dominant9 => "9",
            ChordType.Major11 => "maj11",
            ChordType.Minor11 => "m11",
            ChordType.Dominant11 => "11",
            ChordType.Major13 => "maj13",
            ChordType.Minor13 => "m13",
            ChordType.Dominant13 => "13",
            
            // Add chords
            ChordType.MajorAdd9 => "add9",
            ChordType.MinorAdd9 => "m(add9)",
            ChordType.MajorAdd11 => "add11",
            ChordType.MinorAdd11 => "m(add11)",
            
            // Altered chords
            ChordType.Dominant7Flat5 => "7♭5",
            ChordType.Dominant7Sharp5 => "7♯5",
            ChordType.Dominant7Flat9 => "7♭9",
            ChordType.Dominant7Sharp9 => "7♯9",
            ChordType.Dominant7Flat5Flat9 => "7♭5♭9",
            ChordType.Dominant7Flat5Sharp9 => "7♭5♯9",
            ChordType.Dominant7Sharp5Flat9 => "7♯5♭9",
            ChordType.Dominant7Sharp5Sharp9 => "7♯5♯9",
            ChordType.Dominant7Alt => "7alt",
            
            // Power chords
            ChordType.Power5 => "5",
            
            _ => throw new ArgumentOutOfRangeException()
        };
        
        return rootSymbol + typeSymbol;
    }
}