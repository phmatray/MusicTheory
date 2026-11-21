namespace MusicTheory;

/// <summary>
/// A step tempo change at an exact score position.
/// </summary>
/// <param name="Position">The position in whole notes, at least 0.</param>
/// <param name="Bpm">The tempo in beats per minute.</param>
/// <param name="BeatUnit">The duration that counts as one beat; a quarter note when null.</param>
public sealed record TempoChange(Rational Position, double Bpm, Duration? BeatUnit = null)
{
    /// <summary>Seconds that one whole note lasts under this change.</summary>
    internal double SecondsPerWhole =>
        60.0 / Bpm / (BeatUnit ?? new Duration(DurationType.Quarter)).Value.ToDouble();
}

/// <summary>
/// An immutable list of step tempo changes, converting score positions (in whole notes) to seconds and back.
/// </summary>
public sealed record TempoMap
{
    private readonly TempoChange[] _changes;

    /// <summary>
    /// Initializes a map with one tempo from position 0.
    /// </summary>
    /// <param name="initialBpm">The tempo in beats per minute.</param>
    /// <param name="beatUnit">The duration that counts as one beat; a quarter note when null.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="initialBpm"/> is not a positive finite number.</exception>
    public TempoMap(double initialBpm, Duration? beatUnit = null)
        : this([Validated(new TempoChange(Rational.Zero, initialBpm, beatUnit))])
    {
    }

    private TempoMap(TempoChange[] changes) => _changes = changes;

    /// <summary>Two maps are equal when they hold the same changes in the same order.</summary>
    public bool Equals(TempoMap? other) => other is not null && _changes.SequenceEqual(other._changes);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var c in _changes)
            hash.Add(c);
        return hash.ToHashCode();
    }

    /// <summary>
    /// Gets the tempo changes in position order; the first one is at position 0.
    /// </summary>
    public IReadOnlyList<TempoChange> Changes => _changes;

    /// <summary>
    /// Returns a map with a tempo change at the position, replacing any change already there.
    /// </summary>
    /// <param name="position">The position in whole notes.</param>
    /// <param name="bpm">The tempo in beats per minute.</param>
    /// <param name="beatUnit">The duration that counts as one beat; a quarter note when null.</param>
    /// <returns>A new map.</returns>
    /// <exception cref="ArgumentOutOfRangeException">A negative position, or a tempo that is not positive and finite.</exception>
    public TempoMap WithChange(Rational position, double bpm, Duration? beatUnit = null)
    {
        var change = Validated(new TempoChange(position, bpm, beatUnit));
        return new TempoMap(_changes.Where(c => c.Position != position).Append(change).OrderBy(c => c.Position).ToArray());
    }

    /// <summary>
    /// Gets the tempo in force at a position.
    /// </summary>
    /// <param name="position">The position in whole notes.</param>
    /// <returns>The beats per minute.</returns>
    /// <exception cref="ArgumentOutOfRangeException">A negative position.</exception>
    public double BpmAt(Rational position)
    {
        ValidatePosition(position);
        return _changes.Last(c => c.Position <= position).Bpm;
    }

    /// <summary>
    /// Gets the time at which a score position sounds. Segment lengths are exact <see cref="Rational"/>s,
    /// converted to seconds one segment at a time, so rounding does not accumulate with the bar count.
    /// </summary>
    /// <param name="position">The position in whole notes.</param>
    /// <returns>The time in seconds.</returns>
    /// <exception cref="ArgumentOutOfRangeException">A negative position.</exception>
    public double SecondsAt(Rational position)
    {
        ValidatePosition(position);
        double total = 0;
        for (int i = 0; _changes[i].Position < position; i++)
        {
            var end = i + 1 < _changes.Length && _changes[i + 1].Position < position
                ? _changes[i + 1].Position
                : position;
            total += (end - _changes[i].Position).ToDouble() * _changes[i].SecondsPerWhole;
            if (end == position)
                break;
        }

        return total;
    }

    /// <summary>
    /// Gets the score position sounding at a time; the inverse of <see cref="SecondsAt"/>.
    /// </summary>
    /// <param name="seconds">The time in seconds.</param>
    /// <returns>The position in whole notes, as a double (a time is not generally an exact position).</returns>
    /// <exception cref="ArgumentOutOfRangeException">A negative or non-finite time.</exception>
    public double PositionAt(double seconds)
    {
        if (!double.IsFinite(seconds) || seconds < 0)
            throw new ArgumentOutOfRangeException(nameof(seconds), seconds, "Seconds must be a non-negative finite number.");

        double remaining = seconds;
        for (int i = 0; i + 1 < _changes.Length; i++)
        {
            double span = (_changes[i + 1].Position - _changes[i].Position).ToDouble() * _changes[i].SecondsPerWhole;
            if (remaining < span)
                return _changes[i].Position.ToDouble() + remaining / _changes[i].SecondsPerWhole;
            remaining -= span;
        }

        var last = _changes[^1];
        return last.Position.ToDouble() + remaining / last.SecondsPerWhole;
    }

    private static TempoChange Validated(TempoChange change)
    {
        ValidatePosition(change.Position);
        if (!double.IsFinite(change.Bpm) || change.Bpm <= 0)
            throw new ArgumentOutOfRangeException("bpm", change.Bpm, "Tempo must be a positive number.");
        return change;
    }

    private static void ValidatePosition(Rational position)
    {
        if (position < Rational.Zero)
            throw new ArgumentOutOfRangeException(nameof(position), position, "Position must not be negative.");
    }
}
