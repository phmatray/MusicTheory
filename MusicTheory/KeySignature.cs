namespace MusicTheory;

/// <summary>
/// Represents the mode of a key (major or minor).
/// </summary>
public enum KeyMode
{
    /// <summary>Major mode</summary>
    Major,
    /// <summary>Minor mode</summary>
    Minor
}

/// <summary>
/// Represents a key signature in music.
/// </summary>
public class KeySignature
{
    /// <summary>
    /// Gets the tonic (root) note of the key.
    /// </summary>
    public Note Tonic { get; }

    /// <summary>
    /// Gets the mode of the key (major or minor).
    /// </summary>
    public KeyMode Mode { get; }

    /// <summary>
    /// Gets the number of accidentals in the key signature.
    /// Positive values indicate sharps, negative values indicate flats.
    /// </summary>
    public int AccidentalCount { get; }

    /// <summary>
    /// Gets the notes that are altered in this key signature.
    /// </summary>
    public IReadOnlyList<NoteName> AlteredNotes { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="KeySignature"/> class.
    /// </summary>
    /// <param name="tonic">The tonic note of the key.</param>
    /// <param name="mode">The mode of the key.</param>
    public KeySignature(Note tonic, KeyMode mode)
    {
        Tonic = tonic;
        Mode = mode;
        
        (AccidentalCount, AlteredNotes) = CalculateKeySignature();
    }

    /// <summary>
    /// Gets the alteration for a specific note in this key signature.
    /// </summary>
    /// <param name="noteName">The note name to check.</param>
    /// <returns>The alteration for the note in this key.</returns>
    public Alteration GetAlteration(NoteName noteName)
    {
        if (!AlteredNotes.Contains(noteName))
            return Alteration.Natural;

        return AccidentalCount > 0 ? Alteration.Sharp : Alteration.Flat;
    }

    /// <summary>
    /// Gets the next key in the circle of fifths (clockwise, adding sharps).
    /// </summary>
    /// <returns>The next key signature in the circle of fifths.</returns>
    public KeySignature NextInCircle()
    {
        var position = GetCircleOfFifthsPosition();
        var nextPosition = position + 1;
        
        // Handle wrap-around for enharmonic equivalents
        if (nextPosition > 7)
            nextPosition = -5; // C# major -> Db major (enharmonic)
        
        return GetKeyFromPosition(nextPosition, Mode);
    }

    /// <summary>
    /// Gets the previous key in the circle of fifths (counter-clockwise, adding flats).
    /// </summary>
    /// <returns>The previous key signature in the circle of fifths.</returns>
    public KeySignature PreviousInCircle()
    {
        var position = GetCircleOfFifthsPosition();
        var prevPosition = position - 1;
        
        // Handle wrap-around for enharmonic equivalents
        if (prevPosition < -7)
            prevPosition = 5; // Cb major -> B major (enharmonic)
            
        return GetKeyFromPosition(prevPosition, Mode);
    }

    /// <summary>
    /// Gets the relative major or minor key.
    /// </summary>
    /// <returns>The relative key signature.</returns>
    public KeySignature GetRelative()
    {
        if (Mode == KeyMode.Major)
        {
            // Major to minor: down a minor third (3 semitones)
            var relativeNote = Tonic.TransposeBySemitones(-3);
            return new KeySignature(relativeNote, KeyMode.Minor);
        }
        else
        {
            // Minor to major: up a minor third (3 semitones)
            var relativeNote = Tonic.TransposeBySemitones(3);
            return new KeySignature(relativeNote, KeyMode.Major);
        }
    }

    /// <summary>
    /// Gets the parallel major or minor key (same tonic, different mode).
    /// </summary>
    /// <returns>The parallel key signature.</returns>
    public KeySignature GetParallel()
    {
        var parallelMode = Mode == KeyMode.Major ? KeyMode.Minor : KeyMode.Major;
        return new KeySignature(Tonic, parallelMode);
    }

    /// <summary>
    /// Gets all enharmonic equivalent keys (e.g., C# major = Db major).
    /// </summary>
    /// <returns>A list of enharmonically equivalent key signatures.</returns>
    public IEnumerable<KeySignature> GetEnharmonicEquivalents()
    {
        var equivalents = new List<KeySignature>();
        
        // Check for enharmonic equivalents based on position
        var position = GetCircleOfFifthsPosition();
        
        // Sharp keys that have flat equivalents
        if (position == 6 && Mode == KeyMode.Major && Tonic.Name == NoteName.F && Tonic.Alteration == Alteration.Sharp)
        {
            // F# major = Gb major
            equivalents.Add(new KeySignature(new Note(NoteName.G, Alteration.Flat), KeyMode.Major));
        }
        else if (position == 7 && Mode == KeyMode.Major && Tonic.Name == NoteName.C && Tonic.Alteration == Alteration.Sharp)
        {
            // C# major = Db major
            equivalents.Add(new KeySignature(new Note(NoteName.D, Alteration.Flat), KeyMode.Major));
        }
        
        // Flat keys that have sharp equivalents
        else if (position == -6 && Mode == KeyMode.Major && Tonic.Name == NoteName.G && Tonic.Alteration == Alteration.Flat)
        {
            // Gb major = F# major
            equivalents.Add(new KeySignature(new Note(NoteName.F, Alteration.Sharp), KeyMode.Major));
        }
        else if (position == -7 && Mode == KeyMode.Major && Tonic.Name == NoteName.C && Tonic.Alteration == Alteration.Flat)
        {
            // Cb major = B major
            equivalents.Add(new KeySignature(new Note(NoteName.B), KeyMode.Major));
        }
        
        // Similar for minor keys
        if (position == 5 && Mode == KeyMode.Minor && Tonic.Name == NoteName.G && Tonic.Alteration == Alteration.Sharp)
        {
            // G# minor = Ab minor
            equivalents.Add(new KeySignature(new Note(NoteName.A, Alteration.Flat), KeyMode.Minor));
        }
        else if (position == 6 && Mode == KeyMode.Minor && Tonic.Name == NoteName.D && Tonic.Alteration == Alteration.Sharp)
        {
            // D# minor = Eb minor
            equivalents.Add(new KeySignature(new Note(NoteName.E, Alteration.Flat), KeyMode.Minor));
        }
        else if (position == 7 && Mode == KeyMode.Minor && Tonic.Name == NoteName.A && Tonic.Alteration == Alteration.Sharp)
        {
            // A# minor = Bb minor
            equivalents.Add(new KeySignature(new Note(NoteName.B, Alteration.Flat), KeyMode.Minor));
        }
        
        // Flat minor keys
        else if (position == -7 && Mode == KeyMode.Minor && Tonic.Name == NoteName.A && Tonic.Alteration == Alteration.Flat)
        {
            // Ab minor = G# minor
            equivalents.Add(new KeySignature(new Note(NoteName.G, Alteration.Sharp), KeyMode.Minor));
        }
        
        return equivalents;
    }

    /// <summary>
    /// Gets the dominant key (fifth above).
    /// </summary>
    /// <returns>The dominant key signature.</returns>
    public KeySignature GetDominant()
    {
        return NextInCircle();
    }

    /// <summary>
    /// Gets the subdominant key (fifth below).
    /// </summary>
    /// <returns>The subdominant key signature.</returns>
    public KeySignature GetSubdominant()
    {
        return PreviousInCircle();
    }

    /// <summary>
    /// Calculates the key signature based on the circle of fifths.
    /// </summary>
    private (int accidentalCount, IReadOnlyList<NoteName> alteredNotes) CalculateKeySignature()
    {
        // Order of sharps: F# C# G# D# A# E# B#
        var sharpOrder = new[] { NoteName.F, NoteName.C, NoteName.G, NoteName.D, NoteName.A, NoteName.E, NoteName.B };
        
        // Order of flats: Bb Eb Ab Db Gb Cb Fb
        var flatOrder = new[] { NoteName.B, NoteName.E, NoteName.A, NoteName.D, NoteName.G, NoteName.C, NoteName.F };

        // Calculate position in circle of fifths
        int position = GetCircleOfFifthsPosition();
        
        if (position > 0)
        {
            // Sharps
            return (position, sharpOrder.Take(position).ToList());
        }
        else if (position < 0)
        {
            // Flats
            return (position, flatOrder.Take(-position).ToList());
        }
        else
        {
            // No accidentals
            return (0, new List<NoteName>());
        }
    }

    /// <summary>
    /// Gets the position in the circle of fifths.
    /// </summary>
    private int GetCircleOfFifthsPosition()
    {
        // For major keys
        var majorPositions = new Dictionary<NoteName, int>
        {
            { NoteName.C, 0 },
            { NoteName.G, 1 },
            { NoteName.D, 2 },
            { NoteName.A, 3 },
            { NoteName.E, 4 },
            { NoteName.B, 5 },
            { NoteName.F, -1 }
        };

        // For minor keys (relative to major)
        var minorPositions = new Dictionary<NoteName, int>
        {
            { NoteName.A, 0 },  // A minor (relative to C major)
            { NoteName.E, 1 },  // E minor (relative to G major)
            { NoteName.B, 2 },  // B minor (relative to D major)
            { NoteName.F, 3 },  // F# minor (relative to A major)
            { NoteName.C, 4 },  // C# minor (relative to E major)
            { NoteName.G, -2 }, // G minor (relative to Bb major)
            { NoteName.D, -1 }  // D minor (relative to F major)
        };

        // Handle keys with flats in their name
        if (Tonic.Alteration == Alteration.Flat)
        {
            // Special cases for flat keys
            if (Tonic.Name == NoteName.B && Mode == KeyMode.Major)
                return -2; // Bb major
            if (Tonic.Name == NoteName.E && Mode == KeyMode.Major)
                return -3; // Eb major
            if (Tonic.Name == NoteName.A && Mode == KeyMode.Major)
                return -4; // Ab major
            if (Tonic.Name == NoteName.D && Mode == KeyMode.Major)
                return -5; // Db major
            if (Tonic.Name == NoteName.G && Mode == KeyMode.Major)
                return -6; // Gb major
            if (Tonic.Name == NoteName.C && Mode == KeyMode.Major)
                return -7; // Cb major
            
            if (Tonic.Name == NoteName.G && Mode == KeyMode.Minor)
                return -2; // G minor
            if (Tonic.Name == NoteName.C && Mode == KeyMode.Minor)
                return -3; // C minor
            if (Tonic.Name == NoteName.F && Mode == KeyMode.Minor)
                return -4; // F minor
            if (Tonic.Name == NoteName.B && Mode == KeyMode.Minor)
                return -5; // Bb minor
            if (Tonic.Name == NoteName.E && Mode == KeyMode.Minor)
                return -6; // Eb minor
        }

        // Handle keys with sharps in their name
        if (Tonic.Alteration == Alteration.Sharp)
        {
            // Special cases for sharp keys
            if (Tonic.Name == NoteName.F && Mode == KeyMode.Major)
                return 6; // F# major
            if (Tonic.Name == NoteName.C && Mode == KeyMode.Major)
                return 7; // C# major
            
            if (Tonic.Name == NoteName.F && Mode == KeyMode.Minor)
                return 3; // F# minor
            if (Tonic.Name == NoteName.C && Mode == KeyMode.Minor)
                return 4; // C# minor
            if (Tonic.Name == NoteName.G && Mode == KeyMode.Minor)
                return 5; // G# minor
            if (Tonic.Name == NoteName.D && Mode == KeyMode.Minor)
                return 6; // D# minor
            if (Tonic.Name == NoteName.A && Mode == KeyMode.Minor)
                return 7; // A# minor
        }

        // Standard keys (natural)
        return Mode == KeyMode.Major 
            ? majorPositions.GetValueOrDefault(Tonic.Name, 0)
            : minorPositions.GetValueOrDefault(Tonic.Name, 0);
    }

    /// <summary>
    /// Creates a KeySignature from a position in the circle of fifths.
    /// </summary>
    private static KeySignature GetKeyFromPosition(int position, KeyMode mode)
    {
        if (mode == KeyMode.Major)
        {
            return position switch
            {
                0 => new KeySignature(new Note(NoteName.C), KeyMode.Major),
                1 => new KeySignature(new Note(NoteName.G), KeyMode.Major),
                2 => new KeySignature(new Note(NoteName.D), KeyMode.Major),
                3 => new KeySignature(new Note(NoteName.A), KeyMode.Major),
                4 => new KeySignature(new Note(NoteName.E), KeyMode.Major),
                5 => new KeySignature(new Note(NoteName.B), KeyMode.Major),
                6 => new KeySignature(new Note(NoteName.F, Alteration.Sharp), KeyMode.Major),
                7 => new KeySignature(new Note(NoteName.C, Alteration.Sharp), KeyMode.Major),
                -1 => new KeySignature(new Note(NoteName.F), KeyMode.Major),
                -2 => new KeySignature(new Note(NoteName.B, Alteration.Flat), KeyMode.Major),
                -3 => new KeySignature(new Note(NoteName.E, Alteration.Flat), KeyMode.Major),
                -4 => new KeySignature(new Note(NoteName.A, Alteration.Flat), KeyMode.Major),
                -5 => new KeySignature(new Note(NoteName.D, Alteration.Flat), KeyMode.Major),
                -6 => new KeySignature(new Note(NoteName.G, Alteration.Flat), KeyMode.Major),
                -7 => new KeySignature(new Note(NoteName.C, Alteration.Flat), KeyMode.Major),
                _ => throw new ArgumentOutOfRangeException(nameof(position), "Position must be between -7 and 7")
            };
        }
        else
        {
            return position switch
            {
                0 => new KeySignature(new Note(NoteName.A), KeyMode.Minor),
                1 => new KeySignature(new Note(NoteName.E), KeyMode.Minor),
                2 => new KeySignature(new Note(NoteName.B), KeyMode.Minor),
                3 => new KeySignature(new Note(NoteName.F, Alteration.Sharp), KeyMode.Minor),
                4 => new KeySignature(new Note(NoteName.C, Alteration.Sharp), KeyMode.Minor),
                5 => new KeySignature(new Note(NoteName.G, Alteration.Sharp), KeyMode.Minor),
                6 => new KeySignature(new Note(NoteName.D, Alteration.Sharp), KeyMode.Minor),
                7 => new KeySignature(new Note(NoteName.A, Alteration.Sharp), KeyMode.Minor),
                -1 => new KeySignature(new Note(NoteName.D), KeyMode.Minor),
                -2 => new KeySignature(new Note(NoteName.G), KeyMode.Minor),
                -3 => new KeySignature(new Note(NoteName.C), KeyMode.Minor),
                -4 => new KeySignature(new Note(NoteName.F), KeyMode.Minor),
                -5 => new KeySignature(new Note(NoteName.B, Alteration.Flat), KeyMode.Minor),
                -6 => new KeySignature(new Note(NoteName.E, Alteration.Flat), KeyMode.Minor),
                -7 => new KeySignature(new Note(NoteName.A, Alteration.Flat), KeyMode.Minor),
                _ => throw new ArgumentOutOfRangeException(nameof(position), "Position must be between -7 and 7")
            };
        }
    }
}