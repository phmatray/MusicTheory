using MusicTheory.Toolbox.Tools.Circle;

namespace MusicTheory.Toolbox.Tools;

/// <summary>A tool the shell can show: its URL id, the string key of its title and its component.</summary>
public sealed record ToolInfo(string Id, string TitleKey, Type Component);

public static class ToolRegistry
{
    /// <summary>Every tool; a new tool adds itself here and in <c>Strings.Tables</c>.</summary>
    public static IReadOnlyList<ToolInfo> All { get; } = [new("circle", "circle.title", typeof(CircleTool))];

    public static ToolInfo? Find(string id) => All.FirstOrDefault(t => t.Id == id);
}
