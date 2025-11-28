using GuitarChords.Models;
using MusicTheory;

namespace GuitarChords.Services;

/// <summary>
/// Interface for generating guitar chord fingerings algorithmically.
/// </summary>
public interface IFingeringGenerator
{
    /// <summary>
    /// Generates possible voicings for a chord.
    /// </summary>
    /// <param name="chord">The chord to generate voicings for.</param>
    /// <param name="options">Options controlling the generation.</param>
    /// <returns>A collection of possible voicings, sorted by playability.</returns>
    IEnumerable<GuitarVoicing> GenerateVoicings(Chord chord, FingeringOptions? options = null);

    /// <summary>
    /// Generates a single "best" voicing for a chord based on the options.
    /// </summary>
    /// <param name="chord">The chord to generate a voicing for.</param>
    /// <param name="options">Options controlling the generation.</param>
    /// <returns>The best voicing, or null if none found.</returns>
    GuitarVoicing? GenerateBestVoicing(Chord chord, FingeringOptions? options = null);

    /// <summary>
    /// Generates all voicings for a chord within a specific fret range.
    /// </summary>
    /// <param name="chord">The chord to generate voicings for.</param>
    /// <param name="minFret">Minimum fret position.</param>
    /// <param name="maxFret">Maximum fret position.</param>
    /// <returns>All valid voicings in the specified range.</returns>
    IEnumerable<GuitarVoicing> GenerateVoicingsInRange(Chord chord, int minFret, int maxFret);
}

/// <summary>
/// Interface for scoring the playability of guitar voicings.
/// </summary>
public interface IPlayabilityScorer
{
    /// <summary>
    /// Calculates the playability score for a voicing.
    /// </summary>
    /// <param name="voicing">The voicing to score.</param>
    /// <returns>The playability score with breakdown.</returns>
    PlayabilityScore Score(GuitarVoicing voicing);

    /// <summary>
    /// Determines the difficulty level for a voicing.
    /// </summary>
    /// <param name="voicing">The voicing to evaluate.</param>
    /// <returns>The difficulty level.</returns>
    DifficultyLevel GetDifficulty(GuitarVoicing voicing);
}
