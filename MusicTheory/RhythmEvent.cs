namespace MusicTheory;

/// <summary>A chord, a single note or a rest with a duration. An empty <see cref="Pitches"/> list is a rest.</summary>
/// <param name="Pitches">The sounding notes; empty for a rest.</param>
/// <param name="Duration">How long the event lasts.</param>
/// <param name="TiedToNext">Whether the event is tied into the next one. Not allowed on a rest.</param>
public sealed record RhythmEvent(IReadOnlyList<Note> Pitches, Duration Duration, bool TiedToNext = false)
{
    /// <summary>The sounding notes; empty for a rest.</summary>
    public IReadOnlyList<Note> Pitches { get; init => field = CheckPitches(value ?? throw new ArgumentNullException(nameof(Pitches)), TiedToNext); } =
        Pitches ?? throw new ArgumentNullException(nameof(Pitches));

    /// <summary>How long the event lasts.</summary>
    public Duration Duration { get; init => field = value ?? throw new ArgumentNullException(nameof(Duration)); } =
        Duration ?? throw new ArgumentNullException(nameof(Duration));

    /// <summary>Whether the event is tied into the next one.</summary>
    /// <exception cref="ArgumentException">Set on a rest.</exception>
    public bool TiedToNext
    {
        get;
        init => field = CheckTie(Pitches, value);
    } = CheckTie(Pitches, TiedToNext);

    /// <summary>True when the event has no pitches.</summary>
    public bool IsRest => Pitches.Count == 0;

    /// <summary>Creates a rest.</summary>
    /// <param name="duration">The rest's duration.</param>
    /// <returns>A rest event.</returns>
    public static RhythmEvent Rest(Duration duration) => new([], duration);

    private static IReadOnlyList<Note> CheckPitches(IReadOnlyList<Note> pitches, bool tied) =>
        tied && pitches.Count == 0 ? throw new ArgumentException("A rest cannot be tied.", nameof(TiedToNext)) : pitches;

    private static bool CheckTie(IReadOnlyList<Note> pitches, bool tied) =>
        tied && pitches.Count == 0 ? throw new ArgumentException("A rest cannot be tied.", nameof(TiedToNext)) : tied;
}
