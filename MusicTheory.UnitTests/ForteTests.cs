namespace MusicTheory.UnitTests;

public class ForteTests
{
    [Theory]
    [InlineData(new[] { 0, 4, 7 }, "3-11")]
    [InlineData(new[] { 0, 3, 7 }, "3-11")]
    [InlineData(new[] { 0, 1, 4, 6 }, "4-Z15")]
    [InlineData(new[] { 0, 1, 3, 7 }, "4-Z29")]
    [InlineData(new int[0], "0-1")]
    [InlineData(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }, "12-1")]
    public void ForteName_NamesTheSetClass(int[] pcs, string expected) =>
        PitchClassSet.Of(pcs).ForteName.ShouldBe(expected);

    [Fact]
    public void FromForteName_ReturnsThePrimeForm()
    {
        PitchClassSet.FromForteName("4-Z15").PitchClasses.ShouldBe([0, 1, 4, 6]);
        PitchClassSet.FromForteName("4-z15").ShouldBe(PitchClassSet.Of(0, 1, 4, 6));
    }

    [Fact]
    public void FromForteName_SuggestsTheMissingZ()
    {
        var ex = Should.Throw<FormatException>(() => PitchClassSet.FromForteName("4-15"));
        ex.Message.ShouldContain("4-Z15");
    }

    [Theory]
    [InlineData("9-99")]
    [InlineData("x")]
    public void FromForteName_RejectsUnknownNames(string name) =>
        Should.Throw<FormatException>(() => PitchClassSet.FromForteName(name)).Message.ShouldNotContain("Did you mean");

    [Fact]
    public void FromForteName_RejectsNull() => Should.Throw<ArgumentNullException>(() => PitchClassSet.FromForteName(null!));

    [Fact]
    public void TryFromForteName_NeverThrows()
    {
        PitchClassSet.TryFromForteName("x", out var none).ShouldBeFalse();
        none.ShouldBeNull();
        PitchClassSet.TryFromForteName(null, out _).ShouldBeFalse();
        PitchClassSet.TryFromForteName("3-11", out var triad).ShouldBeTrue();
        triad.ShouldBe(PitchClassSet.Of(0, 3, 7));
    }

    [Fact]
    public void ZPartner_PairsTheTwoSetClasses()
    {
        PitchClassSet.Of(0, 1, 4, 6).ZPartner!.ForteName.ShouldBe("4-Z29");
        PitchClassSet.Of(0, 1, 3, 7).ZPartner!.ForteName.ShouldBe("4-Z15");
        PitchClassSet.Of(0, 4, 7).IsZRelated.ShouldBeFalse();
        PitchClassSet.Of(0, 4, 7).ZPartner.ShouldBeNull();
        PitchClassSet.Of(0, 1, 4, 6).IsZRelated.ShouldBeTrue();
    }

    [Fact]
    public void Catalogue_Has224ClassesThatRoundTrip()
    {
        var names = File.ReadLines(Path.Combine(AppContext.BaseDirectory, "Reference", "pcsets.csv")).Skip(1)
            .Select(l => l.Split(',')[4]).Append("0-1").Distinct().ToList();
        names.Count.ShouldBe(224);
        names.ShouldAllBe(n => PitchClassSet.FromForteName(n).ForteName == n);
    }
}
