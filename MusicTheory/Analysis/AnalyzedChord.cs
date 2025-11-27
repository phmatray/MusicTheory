namespace MusicTheory.Analysis;

/// <summary>
/// Represents a chord with its harmonic analysis data.
/// </summary>
public class AnalyzedChord
{
    /// <summary>
    /// Gets the analyzed chord.
    /// </summary>
    public Chord Chord { get; }

    /// <summary>
    /// Gets the scale degree (1-7) of the chord root within the key.
    /// </summary>
    public int ScaleDegree { get; }

    /// <summary>
    /// Gets the Roman numeral representation (e.g., "I", "ii", "V7", "bVII").
    /// </summary>
    public string RomanNumeral { get; }

    /// <summary>
    /// Gets the harmonic function of the chord.
    /// </summary>
    public HarmonicFunction Function { get; }

    /// <summary>
    /// Gets the key context in which this chord was analyzed.
    /// </summary>
    public KeySignature ContextKey { get; }

    /// <summary>
    /// Gets whether the chord is non-diatonic (borrowed, chromatic, etc.).
    /// </summary>
    public bool IsNonDiatonic { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AnalyzedChord"/> class.
    /// </summary>
    /// <param name="chord">The chord.</param>
    /// <param name="scaleDegree">The scale degree (1-7).</param>
    /// <param name="romanNumeral">The Roman numeral representation.</param>
    /// <param name="function">The harmonic function.</param>
    /// <param name="contextKey">The key context.</param>
    /// <param name="isNonDiatonic">Whether the chord is non-diatonic.</param>
    public AnalyzedChord(
        Chord chord,
        int scaleDegree,
        string romanNumeral,
        HarmonicFunction function,
        KeySignature contextKey,
        bool isNonDiatonic = false)
    {
        Chord = chord;
        ScaleDegree = scaleDegree;
        RomanNumeral = romanNumeral;
        Function = function;
        ContextKey = contextKey;
        IsNonDiatonic = isNonDiatonic;
    }
}
