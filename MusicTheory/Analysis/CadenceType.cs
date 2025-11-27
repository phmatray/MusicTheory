namespace MusicTheory.Analysis;

/// <summary>
/// Represents the type of harmonic cadence.
/// </summary>
public enum CadenceType
{
    /// <summary>Perfect authentic cadence (V-I with both chords in root position, tonic in soprano)</summary>
    AuthenticPerfect,

    /// <summary>Imperfect authentic cadence (V-I with inversions or non-tonic soprano)</summary>
    AuthenticImperfect,

    /// <summary>Plagal cadence (IV-I) - the "Amen" cadence</summary>
    Plagal,

    /// <summary>Deceptive cadence (V-vi or V-VI in minor) - unexpected resolution</summary>
    Deceptive,

    /// <summary>Half cadence (ends on V) - creates expectation for resolution</summary>
    Half,

    /// <summary>Phrygian half cadence (iv6-V in minor) - distinctive half-step motion in bass</summary>
    Phrygian
}
