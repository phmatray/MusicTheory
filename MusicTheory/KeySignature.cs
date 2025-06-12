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
}