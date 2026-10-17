namespace MusicTheory;

/// <summary>
/// A key proposed by <see cref="KeyDetector"/>.
/// </summary>
/// <param name="Key">The key, spelled with the fewest accidentals (Eb, not D#).</param>
/// <param name="Confidence">Share of the evidence, in [0, 1]; the returned list sums to 1.</param>
/// <param name="DiatonicHits">How many chords belong to the key.</param>
/// <param name="Total">How many chords were analysed.</param>
public sealed record KeyCandidate(KeySignature Key, double Confidence, int DiatonicHits, int Total);

/// <summary>
/// Infers the key of a chord sequence with a cheap, explainable score over the 24 major/minor keys.
/// </summary>
/// <remarks>
/// Per key: +1 for each chord whose root and triad quality are diatonic (minor keys also accept the
/// harmonic-minor V and vii°; sus and power chords match any quality; 7ths and extensions count as their
/// triad, so blues I7/IV7 fit), −1 for each chord that is not; +0.5 per tonic chord, +1 when the first
/// chord is the tonic, +1.5 when the last is, and +1 per V→I move. So <c>Am F C G</c> is A minor before
/// C major (it starts on Am), and <c>Am Dm E7 Am</c> is A minor (E7 is only diatonic there).
/// Confidence is <c>(max(score, 0) + 1)</c> normalised over the returned candidates; the +1 keeps a
/// single chord from looking certain.
/// </remarks>
public static class KeyDetector
{
    private enum Triad { Major, Minor, Diminished, Augmented, Any }

    private static readonly (int Step, Triad Triad)[] MajorDegrees =
        [(0, Triad.Major), (2, Triad.Minor), (4, Triad.Minor), (5, Triad.Major), (7, Triad.Major), (9, Triad.Minor), (11, Triad.Diminished)];

    private static readonly (int Step, Triad Triad)[] MinorDegrees =
        [(0, Triad.Minor), (2, Triad.Diminished), (3, Triad.Major), (5, Triad.Minor), (7, Triad.Minor), (8, Triad.Major), (10, Triad.Major),
         (7, Triad.Major), (11, Triad.Diminished)];

    /// <summary>Ranks keys for a chord sequence, best first. Empty input gives an empty list.</summary>
    public static IReadOnlyList<KeyCandidate> Infer(IEnumerable<ChordSymbol> chords, int top = 3)
    {
        var list = chords.Select(c => (Root: MusicTheoryUtilities.GetPitchClass(c.Root), Triad: TriadOf(c.Type))).ToList();
        if (list.Count == 0 || top <= 0)
            return [];

        var c = new KeySignature(new Note(NoteName.C), KeyMode.Major);
        var a = new KeySignature(new Note(NoteName.A), KeyMode.Minor);
        var scored = Enumerable.Range(0, 12)
            .SelectMany(n => new[] { c.TransposeBySemitones(n), a.TransposeBySemitones(n) })
            .Select(key => (Key: key, Result: Score(list, key)))
            .OrderByDescending(x => x.Result.Score)
            .ThenBy(x => x.Key.Mode)                       // major first on a tie
            .ThenBy(x => Math.Abs(x.Key.AccidentalCount))
            .Take(top)
            .ToList();

        double total = scored.Sum(x => Weight(x.Result.Score));
        return scored.Select(x => new KeyCandidate(x.Key, Weight(x.Result.Score) / total, x.Result.Hits, list.Count)).ToList();
    }

    /// <summary>Ranks keys for a sequence of chords; see <see cref="Infer(IEnumerable{ChordSymbol}, int)"/>.</summary>
    public static IReadOnlyList<KeyCandidate> Infer(IEnumerable<Chord> chords, int top = 3) =>
        Infer(chords.Select(ch => new ChordSymbol(ch.Root, ch.Type)), top);

    private static double Weight(double score) => Math.Max(score, 0) + 1;

    private static (double Score, int Hits) Score(List<(int Root, Triad Triad)> chords, KeySignature key)
    {
        int tonic = MusicTheoryUtilities.GetPitchClass(key.Tonic);
        var degrees = key.Mode == KeyMode.Major ? MajorDegrees : MinorDegrees;

        bool Fits((int Root, Triad Triad) ch, int step, Triad triad) =>
            ch.Root == (tonic + step) % 12 && (ch.Triad == Triad.Any || ch.Triad == triad);
        bool IsTonic((int Root, Triad Triad) ch) => Fits(ch, 0, degrees[0].Triad);
        bool IsDominant((int Root, Triad Triad) ch) => Fits(ch, 7, Triad.Major);

        double score = 0;
        int hits = 0;
        for (int i = 0; i < chords.Count; i++)
        {
            var ch = chords[i];
            if (degrees.Any(d => Fits(ch, d.Step, d.Triad)))
            {
                hits++;
                score += 1;
            }
            else
            {
                score -= 1;
            }

            if (IsTonic(ch)) score += 0.5;
            if (i + 1 < chords.Count && IsDominant(ch) && IsTonic(chords[i + 1])) score += 1;
        }

        if (IsTonic(chords[0])) score += 1;
        if (IsTonic(chords[^1])) score += 1.5;
        return (score, hits);
    }

    private static Triad TriadOf(ChordType type) => type switch
    {
        ChordType.Minor or ChordType.Minor7 or ChordType.MinorMajor7 or ChordType.Minor6 or ChordType.Minor9
            or ChordType.Minor11 or ChordType.Minor13 or ChordType.MinorAdd9 or ChordType.MinorAdd11 => Triad.Minor,
        ChordType.Diminished or ChordType.HalfDiminished7 or ChordType.Diminished7 => Triad.Diminished,
        ChordType.Augmented or ChordType.Augmented7 or ChordType.AugmentedMajor7 => Triad.Augmented,
        ChordType.Sus2 or ChordType.Sus4 or ChordType.Sus2Sus4 or ChordType.Dominant7Sus4 or ChordType.Power5 => Triad.Any,
        _ => Triad.Major,
    };
}
