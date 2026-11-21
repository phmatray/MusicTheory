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

    [Fact]
    public void The_menu_groups_every_tool_once_and_help_sits_apart() =>
        ToolGroups.All.SelectMany(g => g.Ids).Append(ToolGroups.HelpId).Order()
            .ShouldBe(ToolRegistry.All.Select(t => t.Id).Order());

    [Fact]
    public void Every_tool_has_a_blurb_for_the_home_index_in_both_languages()
    {
        foreach (var tool in ToolRegistry.All)
            foreach (var lang in new[] { Lang.En, Lang.Fr })
                Strings.T(lang, tool.Id + ".blurb").ShouldNotStartWith("[", Case.Sensitive, tool.Id);
    }
}
