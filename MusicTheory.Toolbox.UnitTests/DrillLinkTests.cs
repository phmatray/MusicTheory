using MusicTheory.Toolbox.Drills;
using MusicTheory.Toolbox.I18n;
using MusicTheory.Toolbox.State;
using MusicTheory.Training;

namespace MusicTheory.Toolbox.UnitTests;

public class DrillLinkTests
{
    private static DrillLink Read(string query) =>
        DrillLink.FromState(ToolboxUrl.Parse(new Uri("https://x.test/toolbox/" + query), "en"));

    [Fact]
    public void Round_trips_through_the_url()
    {
        var link = new DrillLink("intervals", Difficulty.Advanced, 18446744073709551615UL, 25, true);
        var url = ToolboxUrl.Build(link.ToState(Lang.Fr));
        url.ShouldBe("?t=intervals&lang=fr&v=1&d=advanced&key=1&n=25&seed=18446744073709551615");
        Read(url).ShouldBe(link);
    }

    [Theory]
    [InlineData("?t=intervals&seed=abc")]
    [InlineData("?t=intervals&seed=-5")]
    [InlineData("?t=intervals&seed=99999999999999999999999")]
    [InlineData("?t=intervals")]
    public void An_invalid_or_missing_seed_is_null_not_an_exception(string query) =>
        Read(query).Seed.ShouldBeNull();

    [Theory]
    [InlineData("0", 1)]
    [InlineData("500", 50)]
    [InlineData("12", 12)]
    [InlineData("x", 10)]
    [InlineData("-3", 10)]
    [InlineData("99999999999999", 10)]
    public void Count_is_clamped_or_defaulted(string n, int expected) =>
        Read("?t=intervals&seed=1&n=" + n).Count.ShouldBe(expected);

    [Theory]
    [InlineData("nope", Difficulty.Beginner)]
    [InlineData("INTERMEDIATE", Difficulty.Intermediate)]
    public void Unknown_difficulty_is_beginner(string d, Difficulty expected) =>
        Read("?t=intervals&d=" + d).Difficulty.ShouldBe(expected);

    [Fact]
    public void The_teacher_key_needs_key_equal_1() =>
        (Read("?t=intervals&key=1").TeacherKey, Read("?t=intervals&key=yes").TeacherKey).ShouldBe((true, false));
}
