namespace MusicTheory.Training;

/// <summary>How demanding a drill is.</summary>
public enum Difficulty
{
    /// <summary>Few, widely spaced answers.</summary>
    Beginner,

    /// <summary>A wider pool, with descending questions.</summary>
    Intermediate,

    /// <summary>The full pool, with descending and harmonic questions.</summary>
    Advanced,
}

/// <summary>One drill question: what to play, what to choose from and the right answer.</summary>
/// <param name="Prompt">The instruction shown to the learner.</param>
/// <param name="Sounds">The notes to play, in order; notes in the same inner list sound together.</param>
/// <param name="Choices">The possible answers; empty when the answer is a free sequence.</param>
/// <param name="Answer">The expected answer.</param>
/// <param name="Key">The tonal context, when the drill has one.</param>
public sealed record Question<T>(string Prompt, IReadOnlyList<IReadOnlyList<Note>> Sounds, IReadOnlyList<T> Choices, T Answer,
                                 KeySignature? Key = null);

/// <summary>The outcome of checking an answer.</summary>
/// <param name="IsCorrect">Whether the given answer is right.</param>
/// <param name="Score">1 when correct, 0 otherwise.</param>
/// <param name="Expected">The expected answer.</param>
public sealed record AnswerResult<T>(bool IsCorrect, double Score, T Expected);

internal static class DrillRoots
{
    internal static readonly IReadOnlyList<Note> Twelve =
        new[] { "C4", "Db4", "D4", "Eb4", "E4", "F4", "F#4", "G4", "Ab4", "A4", "Bb4", "B4" }
            .Select(Note.Parse).ToArray();

    internal static readonly IReadOnlyList<Note> MinorTonics =
        new[] { "C4", "C#4", "D4", "Eb4", "E4", "F4", "F#4", "G4", "G#4", "A4", "Bb4", "B4" }
            .Select(Note.Parse).ToArray();
}
