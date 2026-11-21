using MusicTheory.Toolbox.Tools.Chords;
using MusicTheory.Toolbox.Tools.Circle;
using MusicTheory.Toolbox.Tools.Intervals;
using MusicTheory.Toolbox.Tools.Progressions;
using MusicTheory.Toolbox.Tools.Scales;
using MusicTheory.Toolbox.Tools.Transposer;

namespace MusicTheory.Toolbox.Tools;

/// <summary>A tool the shell can show: its URL id, the string key of its title and its component.</summary>
public sealed record ToolInfo(string Id, string TitleKey, Type Component);

public static class ToolRegistry
{
    /// <summary>Every tool; a new tool adds itself here and in <c>Strings.Tables</c>.</summary>
    public static IReadOnlyList<ToolInfo> All { get; } = [
        new("circle", "circle.title", typeof(CircleTool)),
        new("chords", "chords.title", typeof(ChordDrillTool)),
        new("intervals", "intervals.title", typeof(IntervalDrillTool)),
        new("progressions", "progressions.title", typeof(ProgressionDrillTool)),
        new("scales", "scales.title", typeof(ScaleDrillTool)),
        new("transpose", "transpose.title", typeof(TransposerTool)),
    ];

    public static ToolInfo? Find(string id) => All.FirstOrDefault(t => t.Id == id);
}
