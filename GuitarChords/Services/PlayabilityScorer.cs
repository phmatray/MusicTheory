using GuitarChords.Models;

namespace GuitarChords.Services;

/// <summary>
/// Service for scoring the playability of guitar voicings.
/// </summary>
public class PlayabilityScorer : IPlayabilityScorer
{
    // Score weights (total should allow max 100)
    private const int MaxFretStretchScore = 25;
    private const int MaxBarreScore = 20;
    private const int MaxFingerCountScore = 20;
    private const int MaxPositionScore = 15;
    private const int MaxOpenStringScore = 10;
    private const int MaxStringSpacingScore = 10;

    /// <inheritdoc/>
    public PlayabilityScore Score(GuitarVoicing voicing)
    {
        var challenges = new List<string>();

        int fretStretchScore = CalculateFretStretchScore(voicing, challenges);
        int barreScore = CalculateBarreScore(voicing, challenges);
        int fingerCountScore = CalculateFingerCountScore(voicing, challenges);
        int positionScore = CalculatePositionScore(voicing, challenges);
        int openStringScore = CalculateOpenStringScore(voicing);
        int stringSpacingScore = CalculateStringSpacingScore(voicing, challenges);

        int totalScore = fretStretchScore + barreScore + fingerCountScore +
                        positionScore + openStringScore + stringSpacingScore;

        // Clamp to 0-100
        totalScore = Math.Clamp(totalScore, 0, 100);

        var difficulty = GetDifficultyFromScore(totalScore);

        return new PlayabilityScore
        {
            TotalScore = totalScore,
            Difficulty = difficulty,
            ChallengeNotes = challenges,
            FretStretchScore = fretStretchScore,
            BarreComplexityScore = barreScore,
            FingerCountScore = fingerCountScore,
            PositionScore = positionScore,
            OpenStringScore = openStringScore,
            StringSpacingScore = stringSpacingScore
        };
    }

    /// <inheritdoc/>
    public DifficultyLevel GetDifficulty(GuitarVoicing voicing)
    {
        return Score(voicing).Difficulty;
    }

    private int CalculateFretStretchScore(GuitarVoicing voicing, List<string> challenges)
    {
        int span = voicing.FretSpan;

        if (span == 0)
            return MaxFretStretchScore; // No stretch (all open or single fret)

        if (span <= 2)
            return MaxFretStretchScore; // Very comfortable

        if (span == 3)
            return (int)(MaxFretStretchScore * 0.7); // Moderate stretch

        if (span == 4)
        {
            challenges.Add("Requires 4-fret stretch");
            return (int)(MaxFretStretchScore * 0.4);
        }

        challenges.Add($"Requires difficult {span}-fret stretch");
        return 0; // Very difficult stretch
    }

    private int CalculateBarreScore(GuitarVoicing voicing, List<string> challenges)
    {
        if (!voicing.IsBarreChord)
            return MaxBarreScore; // No barre = full score

        int barredStrings = voicing.BarredStrings.Count;

        if (barredStrings <= 2)
        {
            return (int)(MaxBarreScore * 0.8); // Partial barre
        }

        if (barredStrings <= 4)
        {
            challenges.Add("Barre chord");
            return (int)(MaxBarreScore * 0.5);
        }

        challenges.Add("Full barre chord");
        return (int)(MaxBarreScore * 0.3); // Full barre is harder
    }

    private int CalculateFingerCountScore(GuitarVoicing voicing, List<string> challenges)
    {
        int fingers = voicing.FingersRequired;

        if (fingers >= 4)
        {
            challenges.Add("Uses all 4 fingers");
        }

        return fingers switch
        {
            1 => MaxFingerCountScore,
            2 => (int)(MaxFingerCountScore * 0.9),
            3 => (int)(MaxFingerCountScore * 0.7),
            4 => (int)(MaxFingerCountScore * 0.4),
            _ => 0
        };
    }

    private int CalculatePositionScore(GuitarVoicing voicing, List<string> challenges)
    {
        int baseFret = voicing.BaseFret;

        if (baseFret == 0)
            return MaxPositionScore; // Open position

        if (baseFret <= 3)
            return (int)(MaxPositionScore * 0.9); // First few frets

        if (baseFret <= 5)
            return (int)(MaxPositionScore * 0.7);

        if (baseFret <= 7)
        {
            return (int)(MaxPositionScore * 0.5);
        }

        if (baseFret <= 10)
        {
            challenges.Add($"High position (fret {baseFret})");
            return (int)(MaxPositionScore * 0.3);
        }

        challenges.Add($"Very high position (fret {baseFret})");
        return 0;
    }

    private int CalculateOpenStringScore(GuitarVoicing voicing)
    {
        int openStrings = voicing.OpenStrings;

        // More open strings = easier
        return openStrings switch
        {
            >= 3 => MaxOpenStringScore,
            2 => (int)(MaxOpenStringScore * 0.8),
            1 => (int)(MaxOpenStringScore * 0.5),
            _ => 0
        };
    }

    private int CalculateStringSpacingScore(GuitarVoicing voicing, List<string> challenges)
    {
        // Check for awkward string skips in the fingering
        var frettedPositions = voicing.FretPositions
            .Select((fret, index) => new { Fret = fret, Index = index })
            .Where(x => x.Fret > 0)
            .OrderBy(x => x.Index)
            .ToList();

        if (frettedPositions.Count <= 1)
            return MaxStringSpacingScore;

        // Check for gaps larger than 1 string
        int maxGap = 0;
        for (int i = 1; i < frettedPositions.Count; i++)
        {
            int gap = frettedPositions[i].Index - frettedPositions[i - 1].Index - 1;
            maxGap = Math.Max(maxGap, gap);
        }

        if (maxGap == 0)
            return MaxStringSpacingScore; // Adjacent strings

        if (maxGap == 1)
            return (int)(MaxStringSpacingScore * 0.8); // One string gap

        if (maxGap == 2)
        {
            challenges.Add("String skip required");
            return (int)(MaxStringSpacingScore * 0.5);
        }

        challenges.Add("Large string skip required");
        return 0;
    }

    private static DifficultyLevel GetDifficultyFromScore(int score)
    {
        return score switch
        {
            >= 70 => DifficultyLevel.Beginner,
            >= 50 => DifficultyLevel.Intermediate,
            >= 30 => DifficultyLevel.Advanced,
            _ => DifficultyLevel.Expert
        };
    }
}
