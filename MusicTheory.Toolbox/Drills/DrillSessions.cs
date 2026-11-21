using MusicTheory.Training;

namespace MusicTheory.Toolbox.Drills;

/// <summary>Builds the question list of a link for each drill family: the same link always gives the same questions.</summary>
public static class DrillSessions
{
    private static DrillSessionModel<T> Make<T>(DrillLink l, Func<Question<T>> next, Func<Question<T>, T, AnswerResult<T>> check) =>
        new(Enumerable.Range(0, l.Count).Select(_ => next()).ToArray(), check);

    public static DrillSessionModel<ChordType> Chords(DrillLink l, ulong seed)
    {
        var drill = new ChordDrill(ChordDrillOptions.For(l.Difficulty), seed);
        return Make(l, drill.Next, drill.Check);
    }

    public static DrillSessionModel<ScaleType> Scales(DrillLink l, ulong seed)
    {
        var drill = new ScaleDrill(ScaleDrillOptions.For(l.Difficulty), seed);
        return Make(l, drill.Next, drill.Check);
    }

    public static DrillSessionModel<IReadOnlyList<int>> Progressions(DrillLink l, ulong seed)
    {
        var drill = new ProgressionDrill(ProgressionDrillOptions.For(l.Difficulty), seed);
        return Make(l, drill.Next, drill.Check);
    }
}
