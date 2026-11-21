namespace MusicTheory.Midi;

/// <summary>Reads Standard MIDI Files.</summary>
public static class MidiReader
{
    // Tuplet shapes tried, in order, for a duration that is not a plain or dotted value.
    private static (int Actual, int Normal)[] TupletShapes() => [(3, 2), (5, 4), (6, 4), (7, 4), (9, 8)];

    /// <summary>
    /// Reads a format 0 or 1 file. Notes from all tracks are merged into one voice; only the first time signature
    /// and the first key signature are kept, and velocity is ignored.
    /// </summary>
    /// <param name="input">The stream to read to its end.</param>
    /// <param name="quantizeGrid">The grid, in whole notes, that onsets and durations snap to; 1/64 when null.</param>
    /// <returns>The meter, key, tempo map and measures.</returns>
    /// <exception cref="FormatException">The data is not a well-formed Standard MIDI File.</exception>
    /// <exception cref="NotSupportedException">An SMPTE division, format 2, or merged notes that overlap with different durations.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The grid is not positive.</exception>
    public static MidiImport Read(Stream input, Rational? quantizeGrid = null)
    {
        ArgumentNullException.ThrowIfNull(input);
        var grid = quantizeGrid ?? Rational.Of(1, 64);
        if (grid <= Rational.Zero)
            throw new ArgumentOutOfRangeException(nameof(quantizeGrid), quantizeGrid, "The grid must be positive.");

        var smf = SmfDecoder.Decode(input);
        long ticksPerWhole = 4L * smf.Division;

        var meter = smf.Meters.OrderBy(m => m.Tick).Select(m => new TimeSignature(m.Numerator, m.Denominator)).FirstOrDefault()
            ?? TimeSignature.CommonTime;
        var key = smf.Keys.OrderBy(k => k.Tick).Select(k => KeyOf(k.Sharps, k.Mode)).FirstOrDefault();

        var tempo = new TempoMap(120);
        foreach (var (tick, micros) in smf.Tempos.OrderBy(t => t.Tick))
            tempo = tempo.WithChange(Rational.Of(tick, ticksPerWhole), 60_000_000.0 / micros);

        var chords = smf.Notes
            .Select(n =>
            {
                var start = Snap(Rational.Of(n.Start, ticksPerWhole), grid);
                var end = Snap(Rational.Of(n.End, ticksPerWhole), grid);
                return (Tick: n.Start, Start: start, End: end > start ? end : start + grid, n.Pitch);
            })
            .GroupBy(n => (n.Start, n.End))
            .OrderBy(g => g.Key.Start).ThenBy(g => g.Key.End);

        var events = new List<RhythmEvent>();
        var cursor = Rational.Zero;
        foreach (var chord in chords)
        {
            var (start, end) = chord.Key;
            if (start < cursor)
                throw new NotSupportedException($"Notes overlap with different durations at tick {chord.Min(n => n.Tick)}; only one voice is supported.");
            if (start > cursor)
                events.AddRange(Parts(start - cursor).Select(RhythmEvent.Rest));
            var pitches = chord.Select(n => n.Pitch).Order().Select(p => Spell(p, key)).ToList();
            var parts = Parts(end - start);
            events.AddRange(parts.Select((d, i) => new RhythmEvent(pitches, d, i < parts.Count - 1)));
            cursor = end;
        }

        return new MidiImport(meter, key, tempo, Measure.Split(meter, events));
    }

    private static Rational Snap(Rational whole, Rational grid)
    {
        var steps = whole / grid;
        return grid * Rational.Of((2 * steps.Numerator + steps.Denominator) / (2 * steps.Denominator));
    }

    private static IReadOnlyList<Duration> Parts(Rational value)
    {
        if (Duration.TryFromValue(value, out var plain))
            return [plain];
        foreach (var type in Enum.GetValues<DurationType>())
        {
            foreach (var (actual, normal) in TupletShapes())
            {
                var tuplet = Duration.CreateTuplet(type, actual, normal);
                if (tuplet.Value == value)
                    return [tuplet];
            }
        }

        return Duration.Decompose(value);
    }

    private static KeySignature KeyOf(int sharps, int mode)
    {
        var key = mode == 0 ? new KeySignature(new Note(NoteName.C), KeyMode.Major) : new KeySignature(new Note(NoteName.A), KeyMode.Minor);
        for (int i = 0; i < Math.Abs(sharps); i++)
            key = sharps > 0 ? key.NextInCircle() : key.PreviousInCircle();
        return key;
    }

    private static Note Spell(int midi, KeySignature? key)
    {
        if (key is null)
            return Note.FromMidiNumber(midi);
        var spelled = key.Spell(midi % 12);
        return new Note(spelled.Name, spelled.Alteration, spelled.Octave + (midi - spelled.MidiNumber) / 12);
    }
}
