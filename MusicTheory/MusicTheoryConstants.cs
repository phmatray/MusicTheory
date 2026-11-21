namespace MusicTheory;

/// <summary>Shared constants.</summary>
internal static class MusicTheoryConstants
{
    /// <summary>Semitones from C to each letter, indexed by <see cref="NoteName"/>.</summary>
    public static readonly int[] SemitonesFromC = [0, 2, 4, 5, 7, 9, 11];

    /// <summary>Semitones in an octave.</summary>
    public const int SemitonesPerOctave = 12;
}
