using System.Text.RegularExpressions;
using MusicTheory.Toolbox.Tools;
using MusicTheory.Toolbox.Tools.Help;

namespace MusicTheory.Toolbox.UnitTests;

public partial class HelpModelTests
{
    [Fact]
    public void The_list_is_the_shell_parameters_plus_the_union_of_the_tools_parameters() =>
        HelpModel.UrlParameters().ShouldBe(["t", "lang", "v", "chord", "chords", "d", "dir", "frets", "hand", "inst", "key", "n", "oct", "pcs", "root", "scale", "seed", "show", "triad", "tuning", "type"]);

    [Fact]
    public void Every_parameter_the_source_reads_is_declared_by_a_tool()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (!File.Exists(Path.Combine(dir.FullName, "MusicTheory.sln"))) dir = dir.Parent!;
        var read = Directory.EnumerateFiles(Path.Combine(dir.FullName, "MusicTheory.Toolbox", "Tools"), "*.razor", SearchOption.AllDirectories)
            .Concat(Directory.EnumerateFiles(Path.Combine(dir.FullName, "MusicTheory.Toolbox", "Drills"), "*.cs"))
            .SelectMany(f => Read().Matches(File.ReadAllText(f)).Select(m => m.Groups[1].Value))
            .Distinct().ToList();
        read.ShouldNotBeEmpty();
        read.Except(HelpModel.UrlParameters()).ShouldBeEmpty();
    }

    [Fact]
    public void Every_tool_declares_its_parameters_without_duplicates() =>
        ToolRegistry.All.ShouldAllBe(t => t.Parameters.Distinct().Count() == t.Parameters.Count);

    [GeneratedRegex(@"(?:Params\.TryGetValue|Get)\(""([a-z]+)""")]
    private static partial Regex Read();
}
