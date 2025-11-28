namespace GuitarChords.Models;

/// <summary>
/// Difficulty level for a chord voicing.
/// </summary>
public enum DifficultyLevel
{
    /// <summary>Easy chords suitable for beginners (open chords, few fingers).</summary>
    Beginner,

    /// <summary>Moderate difficulty (barre chords, more complex fingerings).</summary>
    Intermediate,

    /// <summary>Challenging chords (large stretches, complex fingerings).</summary>
    Advanced,

    /// <summary>Very difficult chords (jazz voicings, extreme stretches).</summary>
    Expert
}

/// <summary>
/// Represents the playability score for a guitar voicing.
/// Higher scores indicate easier playability.
/// </summary>
public class PlayabilityScore
{
    /// <summary>
    /// The overall playability score (0-100, higher is easier).
    /// </summary>
    public int TotalScore { get; init; }

    /// <summary>
    /// The difficulty level based on the total score.
    /// </summary>
    public DifficultyLevel Difficulty { get; init; }

    /// <summary>
    /// Notes about challenges in playing this voicing.
    /// </summary>
    public List<string> ChallengeNotes { get; init; } = new();

    /// <summary>
    /// Score component for fret stretch (higher = less stretch = easier).
    /// </summary>
    public int FretStretchScore { get; init; }

    /// <summary>
    /// Score component for barre complexity (higher = no barre or simple barre = easier).
    /// </summary>
    public int BarreComplexityScore { get; init; }

    /// <summary>
    /// Score component for number of fingers required (higher = fewer fingers = easier).
    /// </summary>
    public int FingerCountScore { get; init; }

    /// <summary>
    /// Score component for fret position (higher = lower frets = easier).
    /// </summary>
    public int PositionScore { get; init; }

    /// <summary>
    /// Score component for open strings (higher = more open strings = easier).
    /// </summary>
    public int OpenStringScore { get; init; }

    /// <summary>
    /// Score component for string spacing (higher = natural spacing = easier).
    /// </summary>
    public int StringSpacingScore { get; init; }

    /// <summary>
    /// Gets a color representing the difficulty for UI display.
    /// </summary>
    public string DifficultyColor => Difficulty switch
    {
        DifficultyLevel.Beginner => "success",
        DifficultyLevel.Intermediate => "warning",
        DifficultyLevel.Advanced => "error",
        DifficultyLevel.Expert => "dark",
        _ => "default"
    };

    /// <summary>
    /// Gets the difficulty as a display string.
    /// </summary>
    public string DifficultyText => Difficulty.ToString();
}
