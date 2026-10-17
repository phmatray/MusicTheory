namespace MusicTheory;

/// <summary>
/// Provides utility methods for music theory calculations.
/// </summary>
public static class MusicTheoryUtilities
{
    /// <summary>
    /// Calculates the total number of semitones for a given note.
    /// </summary>
    /// <param name="noteName">The name of the note.</param>
    /// <param name="alteration">The alteration applied to the note.</param>
    /// <param name="octave">The octave of the note.</param>
    /// <returns>The total number of semitones from C0.</returns>
    public static int GetTotalSemitones(NoteName noteName, Alteration alteration, int octave)
    {
        return octave * MusicTheoryConstants.SemitonesPerOctave + 
               MusicTheoryConstants.SemitonesFromC[(int)noteName] + 
               (int)alteration;
    }
    
    /// <summary>
    /// Calculates the total number of semitones for a given note.
    /// </summary>
    /// <param name="note">The note to calculate semitones for.</param>
    /// <returns>The total number of semitones from C0.</returns>
    public static int GetTotalSemitones(Note note)
    {
        return GetTotalSemitones(note.Name, note.Alteration, note.Octave);
    }

    /// <summary>
    /// Gets the pitch class of a note: 0 for C, 1 for C#/Db, ... 11 for B (octave ignored).
    /// </summary>
    /// <param name="note">The note.</param>
    /// <returns>A value in 0..11.</returns>
    public static int GetPitchClass(Note note)
    {
        int pc = MusicTheoryConstants.SemitonesFromC[(int)note.Name] + (int)note.Alteration;
        return ((pc % MusicTheoryConstants.SemitonesPerOctave) + MusicTheoryConstants.SemitonesPerOctave) % MusicTheoryConstants.SemitonesPerOctave;
    }
}
