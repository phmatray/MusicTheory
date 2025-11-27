namespace MusicTheory.Jazz;

/// <summary>
/// Represents the source mode from which a chord is borrowed.
/// </summary>
public enum BorrowedChordSource
{
    /// <summary>
    /// The chord is not borrowed (diatonic).
    /// </summary>
    None,

    /// <summary>
    /// Borrowed from Aeolian mode (natural minor).
    /// Common borrowings: bIII, iv, bVI, bVII
    /// </summary>
    Aeolian,

    /// <summary>
    /// Borrowed from Dorian mode.
    /// Common borrowings: IV in minor, ii in minor
    /// </summary>
    Dorian,

    /// <summary>
    /// Borrowed from Phrygian mode.
    /// Common borrowings: bII (Neapolitan)
    /// </summary>
    Phrygian,

    /// <summary>
    /// Borrowed from Lydian mode.
    /// Common borrowings: #IV°
    /// </summary>
    Lydian,

    /// <summary>
    /// Borrowed from Mixolydian mode.
    /// Common borrowings: bVII, v
    /// </summary>
    Mixolydian,

    /// <summary>
    /// Borrowed from Locrian mode.
    /// Rarely used directly.
    /// </summary>
    Locrian,

    /// <summary>
    /// Borrowed from harmonic minor.
    /// Common borrowings: V in minor key
    /// </summary>
    HarmonicMinor,

    /// <summary>
    /// Borrowed from melodic minor.
    /// Common borrowings: IV in minor key
    /// </summary>
    MelodicMinor
}
