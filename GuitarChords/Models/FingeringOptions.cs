using MusicTheory;

namespace GuitarChords.Models;

/// <summary>
/// Options for controlling chord fingering generation.
/// </summary>
public class FingeringOptions
{
    /// <summary>
    /// Maximum fret to consider for fingerings (default 15).
    /// </summary>
    public int MaxFret { get; set; } = 15;

    /// <summary>
    /// Minimum number of strings to play (default 4).
    /// </summary>
    public int MinStrings { get; set; } = 4;

    /// <summary>
    /// Maximum number of strings that can be muted (default 2).
    /// </summary>
    public int MaxMutedStrings { get; set; } = 2;

    /// <summary>
    /// Whether to include barre chord fingerings (default true).
    /// </summary>
    public bool AllowBarreChords { get; set; } = true;

    /// <summary>
    /// Maximum difficulty level to include (default Advanced).
    /// </summary>
    public DifficultyLevel MaxDifficulty { get; set; } = DifficultyLevel.Advanced;

    /// <summary>
    /// Whether the root note must be in the bass (default true for most voicings).
    /// </summary>
    public bool RequireRootInBass { get; set; } = true;

    /// <summary>
    /// Maximum fret span (stretch) between lowest and highest fretted notes (default 4).
    /// </summary>
    public int MaxFretSpan { get; set; } = 4;

    /// <summary>
    /// Maximum number of results to return (default 10).
    /// </summary>
    public int MaxResults { get; set; } = 10;

    /// <summary>
    /// The guitar tuning to use (default standard tuning).
    /// </summary>
    public Note[]? Tuning { get; set; }

    /// <summary>
    /// Prefer voicings with open strings when possible.
    /// </summary>
    public bool PreferOpenStrings { get; set; } = true;

    /// <summary>
    /// Minimum playability score to include (0-100, default 20).
    /// </summary>
    public int MinPlayabilityScore { get; set; } = 20;

    /// <summary>
    /// Whether to include chord inversions (non-root bass notes).
    /// </summary>
    public bool IncludeInversions { get; set; } = false;

    /// <summary>
    /// Specific fret range to search (e.g., for finding moveable shapes).
    /// If set, overrides MaxFret.
    /// </summary>
    public (int Min, int Max)? FretRange { get; set; }

    /// <summary>
    /// Creates default options suitable for beginners.
    /// </summary>
    public static FingeringOptions ForBeginners() => new()
    {
        MaxFret = 5,
        AllowBarreChords = false,
        MaxDifficulty = DifficultyLevel.Beginner,
        RequireRootInBass = true,
        MaxFretSpan = 3,
        PreferOpenStrings = true,
        MinPlayabilityScore = 60
    };

    /// <summary>
    /// Creates default options for intermediate players.
    /// </summary>
    public static FingeringOptions ForIntermediate() => new()
    {
        MaxFret = 12,
        AllowBarreChords = true,
        MaxDifficulty = DifficultyLevel.Intermediate,
        RequireRootInBass = true,
        MaxFretSpan = 4,
        PreferOpenStrings = false,
        MinPlayabilityScore = 40
    };

    /// <summary>
    /// Creates options for finding all possible voicings.
    /// </summary>
    public static FingeringOptions ForAdvanced() => new()
    {
        MaxFret = 15,
        AllowBarreChords = true,
        MaxDifficulty = DifficultyLevel.Expert,
        RequireRootInBass = false,
        MaxFretSpan = 5,
        PreferOpenStrings = false,
        MinPlayabilityScore = 0,
        IncludeInversions = true,
        MaxResults = 20
    };
}
