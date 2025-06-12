namespace MusicTheory;

/// <summary>
/// Represents the inversion of a chord.
/// </summary>
public enum ChordInversion
{
    /// <summary>Root position - root note in bass</summary>
    Root,
    /// <summary>First inversion - third in bass</summary>
    First,
    /// <summary>Second inversion - fifth in bass</summary>
    Second,
    /// <summary>Third inversion - seventh in bass (for seventh chords)</summary>
    Third
}