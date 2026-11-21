namespace MusicTheory;

/// <summary>A time signature and the events of one voice inside one bar.</summary>
public sealed record Measure
{
    /// <summary>Creates a measure and checks that its events fit.</summary>
    /// <param name="timeSignature">The bar's time signature.</param>
    /// <param name="events">The events, in order.</param>
    /// <param name="capacity">A smaller capacity for a pickup bar; defaults to the full bar.</param>
    /// <exception cref="ArgumentException">The capacity is not in (0, bar length], or the events overflow it.</exception>
    public Measure(TimeSignature timeSignature, IReadOnlyList<RhythmEvent> events, Rational? capacity = null)
    {
        ArgumentNullException.ThrowIfNull(timeSignature);
        ArgumentNullException.ThrowIfNull(events);
        TimeSignature = timeSignature;
        Events = events.ToArray();
        Capacity = capacity ?? timeSignature.MeasureLength;
        if (Capacity <= Rational.Zero || Capacity > timeSignature.MeasureLength)
            throw new ArgumentException($"Capacity {Capacity} must be above 0 and at most {timeSignature.MeasureLength} for {timeSignature}.", nameof(capacity));

        var onsets = new List<Rational>();
        var filled = Rational.Zero;
        foreach (var e in Events)
        {
            onsets.Add(filled);
            filled += e.Duration.Value;
        }

        if (filled > Capacity)
            throw new ArgumentException($"{filled} exceeds the {Capacity} capacity of {timeSignature}.", nameof(events));
        Filled = filled;
        Onsets = onsets;
    }

    /// <summary>Two measures are equal when they have the same time signature, capacity and events.</summary>
    public bool Equals(Measure? other) =>
        other is not null && TimeSignature == other.TimeSignature && Capacity == other.Capacity && Events.SequenceEqual(other.Events);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(TimeSignature);
        hash.Add(Capacity);
        foreach (var e in Events)
            hash.Add(e);
        return hash.ToHashCode();
    }

    /// <summary>The bar's time signature.</summary>
    public TimeSignature TimeSignature { get; }

    /// <summary>The events, in order.</summary>
    public IReadOnlyList<RhythmEvent> Events { get; }

    /// <summary>The whole-note length the bar can hold.</summary>
    public Rational Capacity { get; }

    /// <summary>The summed value of the events, in whole notes.</summary>
    public Rational Filled { get; }

    /// <summary>Room left in the bar.</summary>
    public Rational Remaining => Capacity - Filled;

    /// <summary>True when the bar is exactly full.</summary>
    public bool IsComplete => Filled == Capacity;

    /// <summary>The offset of each event from the barline, in whole notes.</summary>
    public IReadOnlyList<Rational> Onsets { get; }

    /// <summary>
    /// Bars an event stream. An event that crosses a barline is cut there into notatable parts, tied when pitched.
    /// </summary>
    /// <param name="timeSignature">The time signature of every bar.</param>
    /// <param name="events">The events, in order.</param>
    /// <param name="pickup">The first bar's capacity, for an anacrusis.</param>
    /// <returns>The measures; apart from a pickup bar at the start, only the last may be incomplete.</returns>
    /// <exception cref="ArgumentException">A tuplet event crosses a barline.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The pickup is not in (0, bar length].</exception>
    public static IReadOnlyList<Measure> Split(TimeSignature timeSignature, IEnumerable<RhythmEvent> events, Rational? pickup = null)
    {
        ArgumentNullException.ThrowIfNull(timeSignature);
        ArgumentNullException.ThrowIfNull(events);
        if (pickup is { } p && (p <= Rational.Zero || p > timeSignature.MeasureLength))
            throw new ArgumentOutOfRangeException(nameof(pickup), pickup, $"Pickup must be above 0 and at most {timeSignature.MeasureLength}.");
        var bars = new List<Measure>();
        var current = new List<RhythmEvent>();
        var capacity = pickup ?? timeSignature.MeasureLength;
        var remaining = capacity;

        void Close()
        {
            bars.Add(new Measure(timeSignature, current, capacity));
            current = [];
            capacity = timeSignature.MeasureLength;
            remaining = capacity;
        }

        foreach (var e in events)
        {
            ArgumentNullException.ThrowIfNull(e);
            var left = e.Duration.Value;
            while (left > remaining)
            {
                if (e.Duration.IsTuplet)
                    throw new ArgumentException($"A tuplet of {left} crosses a barline of {timeSignature} and cannot be split.", nameof(events));
                // A full bar is closed right away, so remaining is always above zero here.
                AddParts(current, e, remaining, tieLast: true);
                left -= remaining;
                Close();
            }

            if (left == e.Duration.Value)
                current.Add(e);
            else
                AddParts(current, e, left, tieLast: e.TiedToNext);
            remaining -= left;
            if (remaining == Rational.Zero)
                Close();
        }

        if (current.Count > 0)
            bars.Add(new Measure(timeSignature, current, capacity));
        return bars;
    }

    private static void AddParts(List<RhythmEvent> into, RhythmEvent source, Rational value, bool tieLast)
    {
        var parts = Duration.Decompose(value);
        for (var i = 0; i < parts.Count; i++)
            into.Add(new RhythmEvent(source.Pitches, parts[i], !source.IsRest && (i < parts.Count - 1 || tieLast)));
    }
}
