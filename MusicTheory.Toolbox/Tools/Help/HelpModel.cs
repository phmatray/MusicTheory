namespace MusicTheory.Toolbox.Tools.Help;

/// <summary>What the privacy statement lists, read from the registry so it cannot drift from the tools.</summary>
public static class HelpModel
{
    /// <summary>The shell's own parameters, then every tool's, each once.</summary>
    public static IReadOnlyList<string> UrlParameters() =>
        ["t", "lang", "v", .. ToolRegistry.All.SelectMany(t => t.Parameters).Distinct().Order(StringComparer.Ordinal)];
}
