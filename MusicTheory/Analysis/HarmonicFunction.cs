namespace MusicTheory.Analysis;

/// <summary>
/// Represents the harmonic function of a chord within a key.
/// </summary>
public enum HarmonicFunction
{
    /// <summary>Tonic function (I, vi, iii) - provides stability and resolution</summary>
    Tonic,

    /// <summary>Subdominant function (IV, ii) - creates mild tension, leads to dominant</summary>
    Subdominant,

    /// <summary>Dominant function (V, vii°) - creates strong tension, resolves to tonic</summary>
    Dominant,

    /// <summary>Secondary dominant - dominant of a chord other than I (e.g., V/V)</summary>
    SecondaryDominant,

    /// <summary>Modal interchange - chord borrowed from parallel mode</summary>
    ModalInterchange,

    /// <summary>Chromatic mediant - chord a third away with chromatic alteration</summary>
    ChromaticMediant,

    /// <summary>Passing chord - connects two chords, usually chromatic</summary>
    Passing,

    /// <summary>Neighbor chord - embellishes a chord by moving to adjacent harmony and back</summary>
    Neighbor
}
