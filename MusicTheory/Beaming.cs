namespace MusicTheory;

/// <summary>Reads which events of a measure are beamed together.</summary>
public static class Beaming
{
    /// <summary>
    /// Groups the events of a measure that are beamed together at the primary level. Only pitched
    /// events shorter than a quarter beam, a beam never crosses a beat group of the time signature,
    /// and a rest or longer event breaks it. A group always holds at least two events.
    /// </summary>
    /// <param name="measure">The measure to read.</param>
    /// <returns>The event indexes of each beam group, in order.</returns>
    public static IReadOnlyList<IReadOnlyList<int>> Group(Measure measure)
    {
        ArgumentNullException.ThrowIfNull(measure);
        var quarter = Rational.Of(1, 4);
        var boundaries = new List<Rational>();
        var end = Rational.Zero;
        foreach (var length in measure.TimeSignature.BeatGroups)
            boundaries.Add(end += length);

        var groups = new List<IReadOnlyList<int>>();
        var run = new List<int>();
        var runBeat = -1;

        void Close()
        {
            if (run.Count >= 2)
                groups.Add(run);
            run = [];
            runBeat = -1;
        }

        for (var i = 0; i < measure.Events.Count; i++)
        {
            var e = measure.Events[i];
            if (e.IsRest || e.Duration.Value >= quarter)
            {
                Close();
                continue;
            }

            var onset = measure.Onsets[i];
            var beat = boundaries.Count(b => b <= onset);
            if (beat != runBeat)
                Close();
            runBeat = beat;
            run.Add(i);
        }

        Close();
        return groups;
    }
}
