using System.Diagnostics.CodeAnalysis;

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
            // Major to minor: the sixth degree
            return new KeySignature(Spell(PitchClass(Tonic) + 9), KeyMode.Minor);
        }
        else
        {
            // Minor to major: the third degree
            return new KeySignature(Spell(PitchClass(Tonic) + 3), KeyMode.Major);
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
    /// Parses a key name: a tonic (any case, ASCII or Unicode accidentals) optionally followed by
    /// <c>m</c>/<c>min</c>/<c>minor</c> or <c>maj</c>/<c>major</c>/<c>M</c>, e.g. <c>Eb</c>, <c>E♭ minor</c>, <c>F#m</c>, <c>C major</c>.
    /// </summary>
    /// <exception cref="FormatException">The text is not a key.</exception>
    public static KeySignature Parse(string text) =>
        TryParse(text, out var key) ? key : throw new FormatException($"Not a key: '{text}'.");

    /// <summary>Tries to parse a key name; see <see cref="Parse"/>.</summary>
    public static bool TryParse(string? text, [NotNullWhen(true)] out KeySignature? key)
    {
        key = null;
        var s = text.AsSpan().Trim();
        if (!Note.TryParsePitchClass(s, out var tonic, out var consumed))
            return false;

        var rest = s[consumed..].Trim().ToString();
        KeyMode? mode = rest == "M" ? KeyMode.Major : rest.ToLowerInvariant() switch
        {
            "" or "maj" or "major" => KeyMode.Major,
            "m" or "min" or "minor" => KeyMode.Minor,
            _ => null,
        };
        if (mode is null || Math.Abs(new KeySignature(tonic, mode.Value).AccidentalCount) > 7)
            return false;

        key = new KeySignature(tonic, mode.Value);
        return true;
    }

    /// <summary>
    /// The key <paramref name="semitones"/> above (or below) this one, same mode, spelled with the fewest
    /// accidentals: G +1 is Ab major, not G# major. When both spellings have six accidentals
    /// (F#/Gb major, D#/Eb minor) the side of this key wins, and flats win from a key with none.
    /// </summary>
    public KeySignature TransposeBySemitones(int semitones)
    {
        int target = MusicTheoryUtilities.GetPitchClass(Tonic) + semitones;
        return Spellings(target)
            .Select(n => new KeySignature(n, Mode))
            .OrderBy(k => Math.Abs(k.AccidentalCount))
            .ThenBy(k => AccidentalCount > 0 ? -k.AccidentalCount : k.AccidentalCount)
            .First();
    }

    /// <summary>
    /// Spells a pitch class (0 = C) in this key: scale notes (and, in minor, the raised 6th and 7th) use
    /// the key's spelling; other black keys are sharps in sharp keys and flats in flat keys; in C major /
    /// A minor they are Db Eb F# Ab Bb. The returned note is in octave 4.
    /// </summary>
    public Note Spell(int pitchClass)
    {
        int pc = Mod12(pitchClass);
        int tonicPc = MusicTheoryUtilities.GetPitchClass(Tonic);
        int[] steps = Mode == KeyMode.Major ? [0, 2, 4, 5, 7, 9, 11] : [0, 2, 3, 5, 7, 8, 10, 9, 11];
        for (int i = 0; i < steps.Length; i++)
        {
            int degree = i < 7 ? i : i - 2; // minor: 7, 8 are the raised 6th and 7th
            var letter = (NoteName)(((int)Tonic.Name + degree) % 7);
            if (Mod12(tonicPc + steps[i]) != pc)
                continue;
            int alteration = Mod12(pc - MusicTheoryConstants.SemitonesFromC[(int)letter] + 6) - 6;
            if (Math.Abs(alteration) <= 2)
                return new Note(letter, (Alteration)alteration);
        }

        int natural = Array.IndexOf(MusicTheoryConstants.SemitonesFromC, pc);
        if (natural >= 0)
            return new Note((NoteName)natural);

        bool sharp = AccidentalCount > 0 || (AccidentalCount == 0 && pc == 6);
        return sharp
            ? new Note((NoteName)Array.IndexOf(MusicTheoryConstants.SemitonesFromC, pc - 1), Alteration.Sharp)
            : new Note((NoteName)Array.IndexOf(MusicTheoryConstants.SemitonesFromC, pc + 1), Alteration.Flat);
    }

    /// <summary>The key name, e.g. <c>Ab</c>, <c>F#m</c> or (Unicode) <c>A♭</c>.</summary>
    public string ToString(AccidentalStyle style) =>
        ChordSymbol.Spell(Tonic, style) + (Mode == KeyMode.Minor ? "m" : "");

    /// <summary>The key name in ASCII, e.g. <c>Ab</c> or <c>F#m</c>.</summary>
    public override string ToString() => ToString(AccidentalStyle.Ascii);

    private static IEnumerable<Note> Spellings(int pitchClass)
    {
        int pc = Mod12(pitchClass);
        for (int letter = 0; letter < 7; letter++)
            for (int alt = -1; alt <= 1; alt++)
                if (Mod12(MusicTheoryConstants.SemitonesFromC[letter] + alt) == pc)
                    yield return new Note((NoteName)letter, (Alteration)alt);
    }

    private static int Mod12(int n) => ((n % 12) + 12) % 12;

    private static int PitchClass(Note note) => MusicTheoryUtilities.GetPitchClass(note);

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
    /// Gets the position in the circle of fifths (sharps positive, flats negative), from the tonic's
    /// place on the line of fifths: F=-1 C=0 G=1 D=2 A=3 E=4 B=5, ±7 per sharp/flat, minor keys three
    /// fifths below their relative major.
    /// </summary>
    private int GetCircleOfFifthsPosition()
    {
        int[] lineOfFifths = [0, 2, 4, -1, 1, 3, 5]; // C D E F G A B
        return lineOfFifths[(int)Tonic.Name] + 7 * (int)Tonic.Alteration - (Mode == KeyMode.Minor ? 3 : 0);
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