namespace MusicTheory;

/// <summary>Pitch arithmetic shared by the domain types.</summary>
internal static class MusicTheoryUtilities
{
    /// <summary>Semitones above C0.</summary>
    public static int GetTotalSemitones(NoteName noteName, Alteration alteration, int octave) =>
        octave * MusicTheoryConstants.SemitonesPerOctave + MusicTheoryConstants.SemitonesFromC[(int)noteName] + (int)alteration;

    /// <summary>Semitones above C0.</summary>
    public static int GetTotalSemitones(Note note) => GetTotalSemitones(note.Name, note.Alteration, note.Octave);

    /// <summary>The pitch class, 0..11.</summary>
    public static int GetPitchClass(Note note) => note.PitchClass;

    /// <summary><paramref name="n"/> modulo 12, always in 0..11.</summary>
    public static int Mod12(int n) => ((n % 12) + 12) % 12;

    /// <summary><paramref name="n"/> modulo 7, always in 0..6.</summary>
    public static int Mod7(int n) => ((n % 7) + 7) % 7;

    /// <summary>Integer division rounded toward negative infinity.</summary>
    public static int FloorDiv(int n, int d) => (int)Math.Floor((double)n / d);
}
