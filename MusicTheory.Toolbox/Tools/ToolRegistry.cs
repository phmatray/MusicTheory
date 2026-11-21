using MusicTheory.Toolbox.Tools.Analysis;
using MusicTheory.Toolbox.Tools.Circle;
using MusicTheory.Toolbox.Tools.Finder;
using MusicTheory.Toolbox.Tools.Intervals;
using MusicTheory.Toolbox.Tools.Transposer;

namespace MusicTheory.Toolbox.Tools;

/// <summary>A tool the shell can show: its URL id, the string key of its title and its component.</summary>
public sealed record ToolInfo(string Id, string TitleKey, Type Component);

public static class ToolRegistry
{
    /// <summary>Every tool; a new tool adds itself here and in <c>Strings.Tables</c>.</summary>
    public static IReadOnlyList<ToolInfo> All { get; } =
    [
        new("circle", "circle.title", typeof(CircleTool)),
        new("finder", "finder.title", typeof(FinderTool)),
        new("transpose", "transpose.title", typeof(TransposerTool)),
        new("transpose", "transpose.title", typeof(TransposerTool)),
        new("fretboard", "fretboard.title", typeof(Fretboard.FretboardTool)),
        new("analysis", "analysis.title", typeof(AnalysisTool)),
        new("intervals", "intervals.title", typeof(IntervalDrillTool)),
        new("voicings", "voicings.title", typeof(Voicings.VoicingsTool)),
        new("piano", "piano.title", typeof(Piano.PianoTool)),
    ];

    public static ToolInfo? Find(string id) => All.FirstOrDefault(t => t.Id == id);
}
