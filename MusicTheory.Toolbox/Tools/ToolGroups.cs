namespace MusicTheory.Toolbox.Tools;

/// <summary>A heading of the tool menu and the ids of the tools under it, in menu order.</summary>
public sealed record ToolGroup(string TitleKey, IReadOnlyList<string> Ids);

/// <summary>How the menu and the home page sort the tools; <c>help</c> sits apart, under the groups.</summary>
public static class ToolGroups
{
    public static IReadOnlyList<ToolGroup> All { get; } =
    [
        new("shell.group.practise", ["intervals", "chords", "scales", "progressions", "dictation"]),
        new("shell.group.explore", ["circle", "finder", "fretboard", "voicings", "piano"]),
        new("shell.group.work", ["transpose", "analysis"]),
    ];

    public const string HelpId = "help";

    /// <summary>The registered tools of a group, skipping an id the registry does not know.</summary>
    public static IEnumerable<ToolInfo> Tools(ToolGroup group) =>
        group.Ids.Select(ToolRegistry.Find).OfType<ToolInfo>();
}
