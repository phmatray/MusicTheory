namespace MusicTheory.Guitar;

/// <summary>
/// A guitar tuning: the open-string notes, lowest string first.
/// </summary>
/// <param name="Name">Display name, e.g. "Standard".</param>
/// <param name="Strings">Open-string notes, low to high.</param>
public sealed record Tuning(string Name, IReadOnlyList<Note> Strings)
{
    /// <summary>Standard tuning: E2 A2 D3 G3 B3 E4.</summary>
    public static readonly Tuning Standard = Of("Standard", "E2", "A2", "D3", "G3", "B3", "E4");

    /// <summary>Drop D: D2 A2 D3 G3 B3 E4.</summary>
    public static readonly Tuning DropD = Of("Drop D", "D2", "A2", "D3", "G3", "B3", "E4");

    /// <summary>DADGAD: D2 A2 D3 G3 A3 D4.</summary>
    public static readonly Tuning DADGAD = Of("DADGAD", "D2", "A2", "D3", "G3", "A3", "D4");

    /// <summary>Open G: D2 G2 D3 G3 B3 D4.</summary>
    public static readonly Tuning OpenG = Of("Open G", "D2", "G2", "D3", "G3", "B3", "D4");

    /// <summary>Half step down: Eb2 Ab2 Db3 Gb3 Bb3 Eb4.</summary>
    public static readonly Tuning HalfStepDown = Of("Half step down", "Eb2", "Ab2", "Db3", "Gb3", "Bb3", "Eb4");

    private static Tuning Of(string name, params string[] notes) => new(name, notes.Select(Note.Parse).ToArray());
}
