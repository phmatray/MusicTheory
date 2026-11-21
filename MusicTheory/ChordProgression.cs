namespace MusicTheory;

/// <summary>
/// Diatonic harmony in a key: the chords built by stacking the scale's thirds, Roman numerals and
/// progressions written as numerals (<c>ii7 - V7 - Imaj7</c>). Minor keys use the natural minor scale.
/// </summary>
public sealed class ChordProgression
{
    private static readonly string[] Numerals = ["I", "II", "III", "IV", "V", "VI", "VII"];

    /// <summary>The key.</summary>
    public KeySignature Key { get; }

    /// <summary>Creates a progression context for a key.</summary>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
    public ChordProgression(KeySignature key)
    {
        ArgumentNullException.ThrowIfNull(key);
        Key = key;
    }

    private List<Note> ScaleNotes() =>
        new Scale(Key.Tonic, Key.Mode == KeyMode.Major ? ScaleType.Major : ScaleType.NaturalMinor).GetNotes().Take(7).ToList();

    /// <summary>The seven diatonic triads: I ii iii IV V vi vii° in major, i ii° III iv v VI VII in minor.</summary>
    public IEnumerable<Chord> GetDiatonicChords() => Enumerable.Range(1, 7).Select(d => Diatonic(d, seventh: false)).ToList();

    /// <summary>The seven diatonic seventh chords: Imaj7 ii7 iii7 IVmaj7 V7 vi7 viiø7 in major.</summary>
    public IEnumerable<Chord> GetDiatonicSeventhChords() => Enumerable.Range(1, 7).Select(d => Diatonic(d, seventh: true)).ToList();

    // Semitones of the 3rd, 5th and 7th (−1 for a triad) above the root → chord type.
    private static readonly Dictionary<(int, int, int), ChordType> Stacks = new()
    {
        [(4, 7, -1)] = ChordType.Major, [(3, 7, -1)] = ChordType.Minor,
        [(3, 6, -1)] = ChordType.Diminished, [(4, 8, -1)] = ChordType.Augmented,
        [(4, 7, 11)] = ChordType.Major7, [(4, 7, 10)] = ChordType.Dominant7, [(3, 7, 10)] = ChordType.Minor7,
        [(3, 7, 11)] = ChordType.MinorMajor7, [(3, 6, 10)] = ChordType.HalfDiminished7,
        [(3, 6, 9)] = ChordType.Diminished7, [(4, 8, 11)] = ChordType.AugmentedMajor7,
    };

    private Chord Diatonic(int degree, bool seventh)
    {
        var notes = ScaleNotes();
        var root = notes[degree - 1];
        int Above(int thirds) => MusicTheoryUtilities.Mod12(notes[(degree - 1 + 2 * thirds) % 7].PitchClass - root.PitchClass);
        return new Chord(root, Stacks[(Above(1), Above(2), seventh ? Above(3) : -1)]);
    }

    /// <summary>
    /// The Roman numeral of a degree: upper case for major triads, lower case for minor, lower case with ° for
    /// diminished (vii° in major, ii° in minor).
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="degree"/> is outside 1..7.</exception>
    public string GetRomanNumeral(int degree)
    {
        var chord = GetChordByDegree(degree);
        var numeral = Numerals[degree - 1];
        return chord.Quality switch
        {
            ChordQuality.Minor => numeral.ToLowerInvariant(),
            ChordQuality.Diminished => numeral.ToLowerInvariant() + "°",
            _ => numeral,
        };
    }

    /// <summary>The diatonic triad on a degree.</summary>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="degree"/> is outside 1..7.</exception>
    public Chord GetChordByDegree(int degree)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(degree, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(degree, 7);
        return Diatonic(degree, seventh: false);
    }

    /// <summary>
    /// Parses numerals separated by <c>-</c> or <c>,</c>: <c>I - IV - V7 - I</c>, <c>ii7, V7, Imaj7</c>. Case and
    /// <c>°</c> are accepted but the key decides the quality; <c>7</c> gives the diatonic seventh (V7, ii7,
    /// viiø7) and <c>maj7</c> a major seventh on a major or minor triad.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="progressionString"/> is null.</exception>
    /// <exception cref="FormatException">A part is not a numeral, or <c>maj7</c> sits on a diminished or augmented triad.</exception>
    public IReadOnlyList<Chord> ParseProgression(string progressionString)
    {
        ArgumentNullException.ThrowIfNull(progressionString);
        return progressionString
            .Split(['-', ','], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(ParseNumeral)
            .ToList();
    }

    private Chord ParseNumeral(string symbol)
    {
        bool major7 = symbol.EndsWith("maj7", StringComparison.OrdinalIgnoreCase);
        bool seventh = !major7 && symbol.EndsWith('7');
        var numeral = symbol[..^(major7 ? 4 : seventh ? 1 : 0)].Replace("°", "").ToUpperInvariant();
        int degree = Array.IndexOf(Numerals, numeral) + 1;
        if (degree == 0)
            throw new FormatException($"Not a Roman numeral: '{symbol}'.");

        if (seventh)
            return Diatonic(degree, seventh: true);
        var triad = Diatonic(degree, seventh: false);
        if (!major7)
            return triad;
        return triad.Quality switch
        {
            ChordQuality.Major => new Chord(triad.Root, ChordType.Major7),
            ChordQuality.Minor => new Chord(triad.Root, ChordType.MinorMajor7),
            _ => throw new FormatException($"'{symbol}': a major seventh needs a major or minor triad, and {Numerals[degree - 1]} is {triad.Quality.ToString().ToLowerInvariant()} in {Key}."),
        };
    }

    /// <summary>The secondary dominant (V/x): a major triad a perfect fifth above the target degree's root.</summary>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="targetDegree"/> is outside 1..7.</exception>
    public Chord GetSecondaryDominant(int targetDegree) =>
        new(GetChordByDegree(targetDegree).Root.Transpose(new Interval(IntervalQuality.Perfect, 5)), ChordQuality.Major);
}
