using System.Numerics;

namespace MusicTheory.Midi;

/// <summary>Options for <see cref="MidiWriter"/>.</summary>
/// <param name="TicksPerQuarter">The file's division, 1 to 32767.</param>
/// <param name="Channel">The channel of the note track, 0 to 15.</param>
/// <param name="Velocity">The constant note-on velocity, 1 to 127.</param>
public sealed record MidiWriteOptions(int TicksPerQuarter = 480, int Channel = 0, int Velocity = 90);

/// <summary>Writes Standard MIDI Files.</summary>
public static class MidiWriter
{
    /// <summary>
    /// Writes a format 1 file: track 0 holds the time signature, key signature and tempo meta events, track 1 the notes.
    /// Tied events become one note, chords one note-on per pitch, and rests nothing.
    /// </summary>
    /// <param name="output">The stream to write to.</param>
    /// <param name="measures">The measures, in order; at least one.</param>
    /// <param name="tempo">The tempo map.</param>
    /// <param name="key">The key signature, or null to write none.</param>
    /// <param name="options">The division, channel and velocity; defaults when null.</param>
    /// <exception cref="ArgumentException">No measures, a key with more than 7 accidentals, a position that is not a whole
    /// number of ticks (the message names a <c>TicksPerQuarter</c> that works), or a tempo MIDI cannot hold.</exception>
    /// <exception cref="ArgumentOutOfRangeException">An option out of range.</exception>
    /// <exception cref="InvalidOperationException">A pitch outside MIDI's 0..127.</exception>
    public static void Write(Stream output, IReadOnlyList<Measure> measures, TempoMap tempo,
        KeySignature? key = null, MidiWriteOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(measures);
        ArgumentNullException.ThrowIfNull(tempo);
        var o = options ?? new MidiWriteOptions();
        if (measures.Count == 0)
            throw new ArgumentException("At least one measure is required.", nameof(measures));
        if (o.TicksPerQuarter is < 1 or > short.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(options), o.TicksPerQuarter, "TicksPerQuarter must be 1 to 32767.");
        if (o.Channel is < 0 or > 15)
            throw new ArgumentOutOfRangeException(nameof(options), o.Channel, "Channel must be 0 to 15.");
        if (o.Velocity is < 1 or > 127)
            throw new ArgumentOutOfRangeException(nameof(options), o.Velocity, "Velocity must be 1 to 127.");
        if (key is not null && Math.Abs(key.AccidentalCount) > 7)
            throw new ArgumentException($"{key} has {Math.Abs(key.AccidentalCount)} accidentals; a MIDI key signature holds at most 7.", nameof(key));

        int tpq = o.TicksPerQuarter;
        var meta = new List<(long Tick, int Order, byte[] Data)>();
        var notes = new List<(long Tick, int Order, byte[] Data)>();

        if (key is not null)
            meta.Add((0, 1, [0xFF, 0x59, 0x02, (byte)(sbyte)key.AccidentalCount, (byte)(key.Mode == KeyMode.Major ? 0 : 1)]));
        foreach (var change in tempo.Changes)
        {
            double quartersPerBeat = (change.BeatUnit?.Value ?? Rational.Of(1, 4)).ToDouble() * 4;
            long micros = (long)Math.Round(60_000_000 / (change.Bpm * quartersPerBeat));
            if (micros is < 1 or > 0xFFFFFF)
                throw new ArgumentException($"A tempo of {change.Bpm} bpm does not fit a MIDI Set Tempo event.", nameof(tempo));
            meta.Add((Ticks(change.Position, tpq, "tempo change"), 2,
                [0xFF, 0x51, 0x03, (byte)(micros >> 16), (byte)(micros >> 8), (byte)micros]));
        }

        var flat = new List<(RhythmEvent Event, long Start, long End)>();
        var position = Rational.Zero;
        TimeSignature? meter = null;
        foreach (var measure in measures)
        {
            if (measure.TimeSignature != meter)
            {
                meter = measure.TimeSignature;
                meta.Add((Ticks(position, tpq, $"measure in {meter}"), 0,
                    [0xFF, 0x58, 0x04, (byte)meter.Numerator, (byte)BitOperations.Log2((uint)meter.Denominator), 24, 8]));
            }

            var at = position;
            foreach (var e in measure.Events)
            {
                var end = at + e.Duration.Value;
                flat.Add((e, Ticks(at, tpq, $"start of {Describe(e)}"), Ticks(end, tpq, $"end of {Describe(e)}")));
                at = end;
            }

            position += measure.Capacity;
        }

        var open = new Dictionary<int, long>();
        for (int i = 0; i < flat.Count; i++)
        {
            var (e, start, end) = flat[i];
            var pitches = e.Pitches.Select(p => p.MidiNumber).Distinct().ToList();
            foreach (int midi in pitches.Where(m => !open.ContainsKey(m)))
            {
                open[midi] = start;
                notes.Add((start, 1, [(byte)(0x90 | o.Channel), (byte)midi, (byte)o.Velocity]));
            }

            foreach (int midi in pitches)
            {
                if (e.TiedToNext && i + 1 < flat.Count && flat[i + 1].Event.Pitches.Any(p => p.MidiNumber == midi))
                    continue;
                open.Remove(midi);
                notes.Add((end, 0, [(byte)(0x80 | o.Channel), (byte)midi, 0]));
            }
        }

        long total = Ticks(position, tpq, "end of the score");
        var ordered = notes.OrderBy(n => n.Tick).ThenBy(n => n.Order).Select(n => (n.Tick, n.Data));
        var sortedMeta = meta.OrderBy(m => m.Tick).ThenBy(m => m.Order).Select(m => (m.Tick, m.Data)).ToList();
        SmfEncoder.WriteHeader(output, 2, tpq);
        SmfEncoder.WriteTrack(output, sortedMeta, sortedMeta[^1].Tick);
        SmfEncoder.WriteTrack(output, ordered, total);
    }

    private static string Describe(RhythmEvent e) =>
        e.IsRest ? $"rest of {e.Duration.Value}" : $"{string.Join("+", e.Pitches)} of {e.Duration.Value}";

    private static long Ticks(Rational position, int tpq, string what)
    {
        var quarters = position * Rational.Of(4);
        var ticks = quarters * Rational.Of(tpq);
        if (ticks.Denominator == 1)
            return ticks.Numerator;
        long suggested = tpq / (long)BigInteger.GreatestCommonDivisor(tpq, quarters.Denominator) * quarters.Denominator;
        throw new ArgumentException(
            $"The {what} is at {position} whole notes, which is not a whole number of ticks at {tpq} per quarter. Use a TicksPerQuarter divisible by {quarters.Denominator}, such as {suggested}.");
    }
}
