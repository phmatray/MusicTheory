namespace MusicTheory;

/// <summary>
/// Provides constant values used throughout the MusicTheory library.
/// </summary>
public static class MusicTheoryConstants
{
    /// <summary>
    /// The number of semitones from C to each natural note.
    /// Index corresponds to NoteName enum values (C=0, D=1, E=2, F=3, G=4, A=5, B=6).
    /// </summary>
    public static readonly int[] SemitonesFromC = { 0, 2, 4, 5, 7, 9, 11 };
    
    /// <summary>
    /// The standard reference frequency for A4 in Hz.
    /// </summary>
    public const double A4Frequency = 440.0;
    
    /// <summary>
    /// The number of semitones in an octave.
    /// </summary>
    public const int SemitonesPerOctave = 12;
}