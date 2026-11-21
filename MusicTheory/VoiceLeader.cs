namespace MusicTheory;

/// <summary>Chooses piano voicings for a chord progression so that each one moves the least from the previous.</summary>
public static class VoiceLeader
{
    private const int LowestMidi = 40, HighestMidi = 84, RulePenalty = 100;

    /// <summary>
    /// Voices the first chord with <see cref="PianoVoicer.Voice"/>; each next chord takes the candidate
    /// with the least total motion, plus 100 per parallel fifth, octave or unison. Greedy, deterministic.
    /// </summary>
    /// <param name="chords">The progression.</param>
    /// <param name="style">The shape; rootless styles try both the A and B forms.</param>
    /// <param name="octave">The octave of the first voicing.</param>
    /// <exception cref="ArgumentNullException"><paramref name="chords"/> is null.</exception>
    /// <exception cref="ArgumentException">A chord cannot be voiced in <paramref name="style"/>; the message names its index.</exception>
    /// <exception cref="InvalidOperationException">A chord after the first has no voicing within MIDI 40 to 84.</exception>
    public static IReadOnlyList<IReadOnlyList<Note>> Lead(IEnumerable<ChordSymbol> chords, PianoVoicingStyle style, int octave = 4)
    {
        ArgumentNullException.ThrowIfNull(chords);
        var result = new List<IReadOnlyList<Note>>();
        foreach (var (chord, index) in chords.Select((c, i) => (c, i)))
        {
            IReadOnlyList<Note> first;
            try
            {
                first = PianoVoicer.Voice(chord, style, octave);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException($"Chord {index}: {ex.Message}", nameof(chords), ex);
            }
            result.Add(index == 0 ? first : Best(chord, style, result[^1]));
        }
        return result;
    }

    private static IReadOnlyList<Note> Best(ChordSymbol chord, PianoVoicingStyle style, IReadOnlyList<Note> previous)
    {
        var styles = style is PianoVoicingStyle.RootlessA or PianoVoicingStyle.RootlessB
            ? new[] { PianoVoicingStyle.RootlessA, PianoVoicingStyle.RootlessB }
            : [style];
        var candidates = new Dictionary<string, IReadOnlyList<Note>>();
        foreach (var s in styles)
            for (int inversion = 0; inversion < 4; inversion++)
                for (int octave = 1; octave <= 7; octave++)
                {
                    IReadOnlyList<Note> voicing;
                    try
                    {
                        voicing = PianoVoicer.Voice(chord, s, octave, inversion);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        continue; // inversion beyond the shape
                    }
                    if (voicing.All(n => n.MidiNumber is >= LowestMidi and <= HighestMidi))
                        candidates[Key(voicing)] = voicing;
                }

        return candidates.Values
            .OrderBy(v => Cost(previous, v))
            .ThenBy(v => v.Sum(n => n.MidiNumber))
            .ThenBy(Key, StringComparer.Ordinal)
            .First(); // InvalidOperationException when nothing fits MIDI 40 to 84
    }

    private static string Key(IReadOnlyList<Note> v) => string.Join(",", v.Select(n => n.MidiNumber.ToString("D3")));

    private static int Cost(IReadOnlyList<Note> p, IReadOnlyList<Note> v)
    {
        if (p.Count != v.Count)
            return v.Sum(n => p.Min(q => Math.Abs(n.MidiNumber - q.MidiNumber)));
        int motion = v.Select((n, i) => Math.Abs(n.MidiNumber - p[i].MidiNumber)).Sum();
        var penalised = VoiceLeadingRules.ValidateVoiceLeading([.. p], [.. v]).Count(x =>
            x.Type is VoiceLeadingViolationType.ParallelFifths or VoiceLeadingViolationType.ParallelOctaves or VoiceLeadingViolationType.ParallelUnisons);
        return motion + RulePenalty * penalised;
    }
}
