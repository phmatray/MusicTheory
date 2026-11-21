namespace MusicTheory;

/// <summary>The kind of a cadence.</summary>
public enum CadenceType
{
    /// <summary>V (or vii°) to I.</summary>
    Authentic,
    /// <summary>IV (or iv) to I.</summary>
    Plagal,
    /// <summary>A V that ends the input.</summary>
    Half,
    /// <summary>V to vi.</summary>
    Deceptive
}

/// <summary>A cadence arriving on the chord at <paramref name="Index"/>.</summary>
/// <param name="Type">The kind of cadence.</param>
/// <param name="Index">Index of the arrival chord in the analysed sequence.</param>
public sealed record Cadence(CadenceType Type, int Index);

/// <summary>Finds authentic, plagal, half and deceptive cadences in a chord sequence, on <see cref="HarmonicAnalyzer"/> numerals.</summary>
public static class CadenceDetector
{
    /// <summary>
    /// Detects cadences in <paramref name="chords"/> in <paramref name="key"/>, ordered by index.
    /// Each consecutive pair yields at most one cadence; a final V adds a half cadence.
    /// </summary>
    /// <exception cref="ArgumentNullException">An argument is null.</exception>
    public static IReadOnlyList<Cadence> Detect(IReadOnlyList<ChordSymbol> chords, KeySignature key)
    {
        ArgumentNullException.ThrowIfNull(chords);
        ArgumentNullException.ThrowIfNull(key);

        var rn = HarmonicAnalyzer.Analyze(chords, key);
        var quality = chords.Select(c => c.ToChord().Quality).ToList();
        bool Diatonic(int i, int degree) => rn[i].Degree == degree && rn[i].Function == HarmonicFunction.Diatonic;
        bool D(int i) => Diatonic(i, 5) && rn[i].DegreeAlteration == Alteration.Natural && quality[i] == ChordQuality.Major;
        bool L(int i) => Diatonic(i, 7) && quality[i] == ChordQuality.Diminished;
        bool T(int i) => Diatonic(i, 1);
        bool S(int i) => rn[i].Degree == 4 && rn[i].Function is HarmonicFunction.Diatonic or HarmonicFunction.Borrowed;

        var result = new List<Cadence>();
        for (int i = 1; i < chords.Count; i++)
        {
            if ((D(i - 1) || L(i - 1)) && T(i))
                result.Add(new(CadenceType.Authentic, i));
            else if (S(i - 1) && T(i))
                result.Add(new(CadenceType.Plagal, i));
            else if (D(i - 1) && Diatonic(i, 6))
                result.Add(new(CadenceType.Deceptive, i));
        }

        if (chords.Count > 0 && D(chords.Count - 1))
            result.Add(new(CadenceType.Half, chords.Count - 1));
        return result;
    }
}
