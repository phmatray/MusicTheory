namespace MusicTheory.Guitar;

/// <summary>
/// A guitar tuning: the open-string notes, lowest string first.
/// </summary>
/// <param name="Name">Display name, e.g. "Standard".</param>
/// <param name="Strings">Open-string notes, low to high.</param>
public sealed record Tuning(string Name, IReadOnlyList<Note> Strings)
{
    /// <summary>The display name.</summary>
    public string Name { get; init => field = value ?? throw new ArgumentNullException(nameof(Name)); } = Name ?? throw new ArgumentNullException(nameof(Name));

    /// <summary>The open strings, low to high; at least one, none null.</summary>
    public IReadOnlyList<Note> Strings { get; init => field = Check(value); } = Check(Strings);

    private static IReadOnlyList<Note> Check(IReadOnlyList<Note> strings)
    {
        ArgumentNullException.ThrowIfNull(strings, nameof(Strings));
        if (strings.Count == 0 || strings.Any(s => s is null))
            throw new ArgumentException("A tuning needs at least one string and no null notes.", nameof(Strings));
        return strings.ToArray();
    }

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
