namespace MusicTheory;

/// <summary>The hand a fingering is for.</summary>
public enum Hand
{
    /// <summary>The right hand.</summary>
    Right,
    /// <summary>The left hand.</summary>
    Left,
}

/// <summary>Standard piano fingerings for heptatonic scales and close-position triads.</summary>
public static class PianoFingering
{
    /// <summary>
    /// Fingers 1 (thumb) to 5 for the ascending notes of a heptatonic scale: 7 × <paramref name="octaves"/> + 1 values.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="scale"/> is null.</exception>
    /// <exception cref="ArgumentException">The scale is not heptatonic.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="octaves"/> is below 1.</exception>
    public static IReadOnlyList<int> ForScale(Scale scale, Hand hand, int octaves = 1)
    {
        ArgumentNullException.ThrowIfNull(scale);
        ArgumentOutOfRangeException.ThrowIfLessThan(octaves, 1);
        var all = scale.GetNotes().ToList();
        if (all.Count != 8)
            throw new ArgumentException($"{scale.Type} is not a heptatonic scale, so it has no standard fingering.", nameof(scale));

        // The left hand reads the scale descending from the top tonic.
        var notes = all.Take(7).ToList();
        if (hand == Hand.Left)
            notes = [notes[0], .. notes.Skip(1).Reverse()];

        bool Black(int i) => notes[i].PitchClass is 1 or 3 or 6 or 8 or 10;

        // Thumbs at t and (t + g) % 7; the group starting at t has g notes, the other 7 - g.
        var best = Enumerable.Range(0, 7)
            .SelectMany(t => new[] { 3, 4 }.Select(g => (t, g)))
            .OrderByDescending(c => !Black(c.t) && !Black((c.t + c.g) % 7))
            .ThenByDescending(c => c.t == 0 || (c.t + c.g) % 7 == 0)
            .ThenByDescending(c => Black(((c.g == 4 ? c.t : (c.t + c.g) % 7) + 3) % 7))
            .ThenByDescending(c => (Math.Min(c.t, (c.t + c.g) % 7) == c.t) == (c.g == 3))
            .ThenBy(c => Math.Min(c.t, (c.t + c.g) % 7))
            .First();

        var map = new int[7];
        for (int k = 0; k < best.g; k++)
            map[(best.t + k) % 7] = k + 1;
        for (int k = 0; k < 7 - best.g; k++)
            map[(best.t + best.g + k) % 7] = k + 1;

        var fingers = Enumerable.Range(0, 7 * octaves + 1).Select(i => map[i % 7]).ToList();
        if (map[0] == 1)
            fingers[^1] = fingers[^2] + 1;
        if (hand == Hand.Left)
            fingers.Reverse();
        return fingers;
    }

    /// <summary>Fingers for a close-position triad, aligned with the notes sorted ascending.</summary>
    /// <exception cref="ArgumentNullException"><paramref name="notes"/> is null.</exception>
    /// <exception cref="ArgumentException">Not exactly three distinct pitches within an octave.</exception>
    public static IReadOnlyList<int> ForTriad(IReadOnlyList<Note> notes, Hand hand)
    {
        ArgumentNullException.ThrowIfNull(notes);
        if (notes.Count != 3)
            throw new ArgumentException("A triad fingering needs exactly three notes.", nameof(notes));
        var midi = notes.Select(n => n.Semitones).Order().ToArray();
        if (midi.Distinct().Count() != 3 || midi[2] - midi[0] > 12)
            throw new ArgumentException("A triad fingering needs three distinct pitches spanning at most an octave.", nameof(notes));

        int lower = midi[1] - midi[0], upper = midi[2] - midi[1];
        return hand == Hand.Right
            ? [1, upper >= 5 ? 2 : 3, 5]
            : [5, lower >= 5 ? 2 : 3, 1];
    }
}
