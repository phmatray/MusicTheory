using MusicTheory.Toolbox.I18n;
using MusicTheory.Toolbox.Tools.Intervals;
using MusicTheory.Training;

namespace MusicTheory.Toolbox.UnitTests;

public class IntervalNamesTests
{
    public static TheoryData<Difficulty> Levels() => new(Enum.GetValues<Difficulty>());

    [Theory]
    [MemberData(nameof(Levels))]
    public void Every_interval_of_the_pool_has_a_distinct_label_in_both_languages(Difficulty d)
    {
        foreach (var lang in Enum.GetValues<Lang>())
        {
            var labels = IntervalDrillOptions.For(d).Pool.Select(i => IntervalNames.Label(i, lang)).ToList();
            labels.ShouldAllBe(l => !string.IsNullOrWhiteSpace(l) && l.Contains(' ') || l == "octave");
            labels.Distinct().Count().ShouldBe(labels.Count);
        }
    }

    [Fact]
    public void Spot_checks()
    {
        IntervalNames.Label(Interval.Parse("m3"), Lang.Fr).ShouldBe("tierce mineure");
        IntervalNames.Label(Interval.Parse("P5"), Lang.En).ShouldBe("perfect fifth");
        IntervalNames.Label(Interval.Parse("A4"), Lang.Fr).ShouldBe("quarte augmentée");
    }

    [Fact]
    public void An_interval_without_a_label_shows_its_name() =>
        IntervalNames.Label(Interval.Parse("d5"), Lang.En).ShouldBe("d5");
}
