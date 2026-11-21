using MusicTheory.Toolbox.I18n;
using MusicTheory.Toolbox.Tools;

namespace MusicTheory.Toolbox.UnitTests;

/// <summary>Every tool PR appends to the registry and the string tables, so a merge can duplicate an entry.</summary>
public class ToolRegistryTests
{
    [Fact]
    public void Each_tool_is_listed_once() =>
        ToolRegistry.All.Select(t => t.Id).ShouldBeUnique();

    [Fact]
    public void Each_string_table_is_listed_once() =>
        Strings.Tables.Select(t => t.GetType()).ShouldBeUnique();
}
