namespace MusicTheory;

/// <summary>Piano voicing shapes.</summary>
public enum PianoVoicingStyle
{
    /// <summary>Core tones stacked in thirds.</summary>
    Close,
    /// <summary>The close voicing with its second-highest note dropped an octave.</summary>
    Drop2,
    /// <summary>The close voicing with its third-highest note dropped an octave.</summary>
    Drop3,
    /// <summary>Root below, then the third and seventh.</summary>
    Shell,
    /// <summary>Third, fifth, seventh, ninth.</summary>
    RootlessA,
    /// <summary>Seventh, ninth, third, fifth.</summary>
    RootlessB,
}

/// <summary>Voices chord symbols as the shapes pianists play (drop 2/3, shell, rootless).</summary>
public static class PianoVoicer
{
    /// <summary>
    /// Voices <paramref name="chord"/> in <paramref name="style"/>, ascending, spelled by interval. A slash bass is ignored.
    /// </summary>
    /// <param name="chord">The chord; every style but Close needs a 7th or 6th.</param>
    /// <param name="style">The shape.</param>
    /// <param name="octave">The octave of the lowest core tone (the root of a Shell sits one lower).</param>
    /// <param name="inversion">Close/Drop2/Drop3: 0 to the number of core tones minus one; Shell: 0 or 1; rootless: 0.</param>
    /// <exception cref="ArgumentNullException"><paramref name="chord"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Undefined style or inversion out of range.</exception>
    /// <exception cref="ArgumentException">The chord has no 7th or 6th and the style is not Close.</exception>
    public static IReadOnlyList<Note> Voice(ChordSymbol chord, PianoVoicingStyle style, int octave = 4, int inversion = 0)
    {
        ArgumentNullException.ThrowIfNull(chord);
        if (!Enum.IsDefined(style))
            throw new ArgumentOutOfRangeException(nameof(style));

        var intervals = chord.ToChord().Intervals;
        var third = intervals.FirstOrDefault(i => i.ToSimple().Number == 3)
            ?? intervals.FirstOrDefault(i => i.Number is 2 or 4);
        var fifth = intervals.FirstOrDefault(i => i.Number == 5);
        var seventh = intervals.FirstOrDefault(i => i.Number == 7) ?? intervals.FirstOrDefault(i => i.Number == 6);
        var ninth = intervals.FirstOrDefault(i => i.Number == 9) ?? new Interval(IntervalQuality.Major, 9);

        if (style != PianoVoicingStyle.Close && seventh is null)
            throw new ArgumentException("A voicing needs a 7th or 6th.", nameof(chord));

        Note Tone(Interval i) => chord.Root.Transpose(i);

        var core = new List<Note> { chord.Root };
        core.AddRange(new[] { third, fifth, seventh }.OfType<Interval>().Select(Tone));
        int max = style switch
        {
            PianoVoicingStyle.Close or PianoVoicingStyle.Drop2 or PianoVoicingStyle.Drop3 => core.Count - 1,
            PianoVoicingStyle.Shell => 1,
            _ => 0,
        };
        ArgumentOutOfRangeException.ThrowIfNegative(inversion);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(inversion, max);

        switch (style)
        {
            case PianoVoicingStyle.Shell:
                Note[] upper = inversion == 0 ? [Tone(third!), Tone(seventh!)] : [Tone(seventh!), Tone(third!)];
                return Stack([chord.Root, .. upper], octave - 1);
            case PianoVoicingStyle.RootlessA:
                return Stack([Tone(third!), Tone(fifth!), Tone(seventh!), Tone(ninth)], octave);
            case PianoVoicingStyle.RootlessB:
                return Stack([Tone(seventh!), Tone(ninth), Tone(third!), Tone(fifth!)], octave);
        }

        var close = Stack([.. core[inversion..], .. core[..inversion]], octave);
        if (style == PianoVoicingStyle.Close)
            return close;
        int drop = style == PianoVoicingStyle.Drop2 ? close.Count - 2 : close.Count - 3;
        var dropped = close[drop];
        return close.Select((n, k) => k == drop ? new Note(dropped.Name, dropped.Alteration, dropped.Octave - 1) : n)
            .OrderBy(MusicTheoryUtilities.GetTotalSemitones).ToList();
    }

    // The first note sits in startOctave, each next one at the lowest placement strictly above the previous.
    private static List<Note> Stack(Note[] tones, int startOctave)
    {
        var result = new List<Note> { new(tones[0].Name, tones[0].Alteration, startOctave) };
        foreach (var tone in tones.Skip(1))
        {
            var previous = MusicTheoryUtilities.GetTotalSemitones(result[^1]);
            int o = result[^1].Octave - 1;
            while (MusicTheoryUtilities.GetTotalSemitones(tone.Name, tone.Alteration, o) <= previous)
                o++;
            result.Add(new Note(tone.Name, tone.Alteration, o));
        }
        return result;
    }
}
