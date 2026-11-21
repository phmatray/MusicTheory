using MusicTheory.Toolbox.Tools.Circle;
using MusicTheory.Toolbox.Tools.Help;
using MusicTheory.Toolbox.Tools.Intervals;
using MusicTheory.Toolbox.Tools.Transposer;

namespace MusicTheory.Toolbox.Tools;

/// <summary>A tool the shell can show: its URL id, the string key of its title, its component and the URL parameters it reads (besides <c>t</c>, <c>lang</c>, <c>v</c>).</summary>
public sealed record ToolInfo(string Id, string TitleKey, Type Component, IReadOnlyList<string> Parameters);

public static class ToolRegistry
{
    /// <summary>Every tool; a new tool adds itself here and in <c>Strings.Tables</c>.</summary>
    public static IReadOnlyList<ToolInfo> All { get; } = [
        new("circle", "circle.title", typeof(CircleTool), ["key"]),
        new("intervals", "intervals.title", typeof(IntervalDrillTool), ["d", "n", "seed", "key"]),
        new("transpose", "transpose.title", typeof(TransposerTool), ["inst", "dir", "key", "chords"]),
        new("help", "help.title", typeof(HelpTool), []),
    ];

    public static ToolInfo? Find(string id) => All.FirstOrDefault(t => t.Id == id);
}
