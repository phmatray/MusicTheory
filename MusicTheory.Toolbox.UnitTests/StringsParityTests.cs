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
    public void Comprendre_texts_stay_under_120_words()
    {
        foreach (var t in Strings.Tables)
            foreach (var (k, v) in t.En.Concat(t.Fr).Where(p => p.Key.EndsWith(".understand")))
                v.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length.ShouldBeLessThan(120, k);
    }

    [Fact]
    public void Every_literal_key_used_in_the_source_exists_in_the_tables()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (!File.Exists(Path.Combine(dir.FullName, "MusicTheory.sln"))) dir = dir.Parent!;
        var known = Strings.Tables.SelectMany(t => t.En.Keys).ToHashSet();
        var used = Directory.EnumerateFiles(Path.Combine(dir.FullName, "MusicTheory.Toolbox"), "*.razor", SearchOption.AllDirectories)
            .SelectMany(f => System.Text.RegularExpressions.Regex.Matches(File.ReadAllText(f), "\"((?:shell|circle|dictation|intervals|transpose|understand|analysis)\\.[a-z.]+)\"").Select(m => m.Groups[1].Value))
            .Where(k => !k.EndsWith("."))   // "shell.lang." is a prefix completed at run time
            .Distinct().ToList();
        used.ShouldNotBeEmpty();
        used.Where(k => !known.Contains(k)).ShouldBeEmpty();
    }

    [Fact]
    public void A_missing_key_is_visible()
    {
        Strings.T(Lang.Fr, "nope").ShouldBe("[nope]");
        Strings.T(Lang.Fr, "understand.label").ShouldBe("Comprendre");
        Strings.T(Lang.En, "understand.label").ShouldBe("Understand");
    }
}
