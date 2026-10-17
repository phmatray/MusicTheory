namespace MusicTheory;

/// <summary>How a chord relates to the key it is analysed in.</summary>
public enum HarmonicFunction
{
    /// <summary>Built on a scale degree with the key's own quality (minor keys include the harmonic-minor V and vii°).</summary>
    Diatonic,
    /// <summary>Taken from the parallel mode, e.g. iv, bVI, bVII in a major key.</summary>
    Borrowed,
    /// <summary>A major or dominant-7th chord a fifth above a diatonic degree (V/V, V7/vi).</summary>
    SecondaryDominant,
    /// <summary>Anything else; the numeral uses the nearest degree.</summary>
    Chromatic
}

/// <summary>
/// A Roman-numeral reading of one chord in a key.
/// </summary>
/// <param name="Degree">Scale degree 1..7 (5 for a secondary dominant).</param>
/// <param name="DegreeAlteration">Flat or sharp relative to the key's scale (bVII, #IV).</param>
/// <param name="Type">The chord type.</param>
/// <param name="Inversion">1, 2 or 3 when the slash bass is the 3rd, 5th or 7th; null in root position.</param>
/// <param name="Function">Diatonic, borrowed, secondary dominant or chromatic.</param>
/// <param name="TargetDegree">The degree a secondary dominant resolves to (V/V → 5).</param>
/// <param name="Mode">Mode of the key, used to write the target of a secondary dominant.</param>
public sealed record RomanNumeral(
    int Degree,
    Alteration DegreeAlteration,
    ChordType Type,
    int? Inversion,
    HarmonicFunction Function,
    int? TargetDegree,
    KeyMode Mode = KeyMode.Major)
{
    private static readonly string[] Numerals = ["I", "II", "III", "IV", "V", "VI", "VII"];

    /// <summary>
    /// Renders the numeral: upper case for major, lower case for minor/diminished, ° ø + for quality,
    /// the type's extension (<c>V7</c>, <c>Imaj7</c>, <c>ii7</c>), figured-bass inversions (<c>V6</c>, <c>I64</c>,
    /// <c>V65</c>, <c>V43</c>, <c>V42</c>) and <c>/target</c> for secondary dominants (<c>V7/vi</c>).
    /// </summary>
    public override string ToString()
    {
        var triad = Diatonic.TriadOf(Type);
        var numeral = Numerals[Degree - 1];
        if (triad is Triad.Minor or Triad.Diminished)
            numeral = numeral.ToLowerInvariant();

        var prefix = DegreeAlteration switch { Alteration.Flat => "b", Alteration.Sharp => "#", _ => "" };
        var text = prefix + numeral + SuffixWithInversion();

        if (Function == HarmonicFunction.SecondaryDominant && TargetDegree is int target)
        {
            var (_, targetTriad) = Diatonic.Degrees(Mode)[target - 1];
            var t = Numerals[target - 1];
            bool lower = targetTriad is Triad.Minor or Triad.Diminished && !(Mode == KeyMode.Minor && target == 5);
            text += "/" + (lower ? t.ToLowerInvariant() : t);
        }
        return text;
    }

    private string Suffix() => Type switch
    {
        ChordType.Diminished => "°",
        ChordType.Diminished7 => "°7",
        ChordType.HalfDiminished7 => "ø7",
        ChordType.Augmented => "+",
        ChordType.Augmented7 => "+7",
        ChordType.AugmentedMajor7 => "+maj7",
        ChordType.MinorMajor7 => "maj7",
        _ when Diatonic.TriadOf(Type) == Triad.Minor => ChordSymbol.AsciiSuffix(Type)[1..],
        _ => ChordSymbol.AsciiSuffix(Type),
    };

    private string SuffixWithInversion()
    {
        var suffix = Suffix();
        if (Inversion is not int inv)
            return suffix;
        if (Type is ChordType.Major or ChordType.Minor or ChordType.Diminished or ChordType.Augmented)
            return suffix + (inv == 1 ? "6" : "64");
        if (IsSeventh)
            return suffix.Replace("7", inv switch { 1 => "65", 2 => "43", _ => "42" });
        return suffix + "/" + (inv switch { 1 => "3", 2 => "5", _ => "7" });
    }

    private bool IsSeventh => Type is ChordType.Major7 or ChordType.Minor7 or ChordType.Dominant7 or ChordType.MinorMajor7
        or ChordType.HalfDiminished7 or ChordType.Diminished7 or ChordType.Augmented7 or ChordType.AugmentedMajor7;
}

/// <summary>
/// Roman-numeral analysis of chords in a key, by pitch class (so a mis-spelled <c>F##m</c> in Eb is still iii).
/// </summary>
/// <remarks>
/// Rules, in order: diatonic (root and triad quality on a degree); borrowed from the parallel mode;
/// secondary dominant (major or dominant-7th triad a fifth above degrees 2..7, not onto a diminished
/// chord); otherwise chromatic, on the degree whose letter the key spells the root with (Db in C → bII).
/// Inversions use figured bass for triads (<c>6</c>, <c>64</c>) and seventh chords (<c>65</c>, <c>43</c>, <c>42</c>).
/// </remarks>
public static class HarmonicAnalyzer
{
    /// <summary>Analyses one chord in <paramref name="key"/>.</summary>
    public static RomanNumeral Analyze(ChordSymbol chord, KeySignature key)
    {
        int tonic = MusicTheoryUtilities.GetPitchClass(key.Tonic);
        int interval = (MusicTheoryUtilities.GetPitchClass(chord.Root) - tonic + 12) % 12;
        var triad = Diatonic.TriadOf(chord.Type);
        int? inversion = chord.Bass is null ? null : chord.ToChord().Inversion switch
        {
            ChordInversion.First => 1, ChordInversion.Second => 2, ChordInversion.Third => 3, _ => null,
        };
        var home = Diatonic.Degrees(key.Mode);

        RomanNumeral Make(int index, (int Step, Triad Triad)[] table, HarmonicFunction function)
        {
            int degree = Diatonic.DegreeOf(index);
            // The harmonic-minor V and vii° are diatonic, so they carry no accidental.
            int alteration = function == HarmonicFunction.Diatonic ? 0 : table[index].Step - home[degree - 1].Step;
            return new(degree, (Alteration)alteration, chord.Type, inversion, function, null, key.Mode);
        }

        bool Fits((int Step, Triad Triad) d) => d.Step == interval && (triad == Triad.Any || d.Triad == triad);

        for (int i = 0; i < home.Length; i++)
            if (Fits(home[i]))
                return Make(i, home, HarmonicFunction.Diatonic);

        var parallel = Diatonic.Degrees(key.Mode == KeyMode.Major ? KeyMode.Minor : KeyMode.Major);
        for (int i = 0; i < parallel.Length; i++)
            if (Fits(parallel[i]))
                return Make(i, parallel, HarmonicFunction.Borrowed);

        if (triad == Triad.Major)
            for (int d = 1; d < 7; d++)
                if (home[d].Triad != Triad.Diminished && (home[d].Step + 7) % 12 == interval)
                    return new RomanNumeral(5, Alteration.Natural, chord.Type, inversion, HarmonicFunction.SecondaryDominant, d + 1, key.Mode);

        // Chromatic: the degree is the letter the key spells this root with (Db in C is bII, F# in Am is #VI).
        var spelled = key.Spell(tonic + interval);
        int degree = ((int)spelled.Name - (int)key.Tonic.Name + 7) % 7;
        int alteration = (interval - home[degree].Step + 18) % 12 - 6;
        return new RomanNumeral(degree + 1, (Alteration)alteration, chord.Type, inversion, HarmonicFunction.Chromatic, null, key.Mode);
    }

    /// <summary>Analyses a sequence of chords in <paramref name="key"/>.</summary>
    public static IReadOnlyList<RomanNumeral> Analyze(IEnumerable<ChordSymbol> chords, KeySignature key) =>
        chords.Select(c => Analyze(c, key)).ToList();
}
