using MusicTheory.Toolbox.I18n;

namespace MusicTheory.Toolbox.UnitTests;

public class StringsParityTests
{
    public static TheoryData<int> TableIndexes() => new(Enumerable.Range(0, Strings.Tables.Count));

    [Theory]
    [MemberData(nameof(TableIndexes))]
    public void Every_table_has_the_same_keys_in_both_languages_and_no_empty_value(int i)
    {
        var t = Strings.Tables[i];
        t.En.Keys.OrderBy(k => k).ShouldBe(t.Fr.Keys.OrderBy(k => k));
        t.En.Concat(t.Fr).Where(p => string.IsNullOrWhiteSpace(p.Value)).ShouldBeEmpty();
    }

    [Fact]
    public void Keys_are_unique_across_tables_and_copy_has_no_em_dash()
    {
        Strings.Tables.SelectMany(t => t.En.Keys).GroupBy(k => k).Where(g => g.Count() > 1).ShouldBeEmpty();
        Strings.Tables.SelectMany(t => t.En.Values.Concat(t.Fr.Values)).Where(v => v.Contains('—')).ShouldBeEmpty();
    }

    [Fact]
    public void A_missing_key_is_visible()
    {
        Strings.T(Lang.Fr, "nope").ShouldBe("[nope]");
        Strings.T(Lang.Fr, "understand.label").ShouldBe("Comprendre");
        Strings.T(Lang.En, "understand.label").ShouldBe("Understand");
    }
}
