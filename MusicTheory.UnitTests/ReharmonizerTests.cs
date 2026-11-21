using Shouldly;

namespace MusicTheory.UnitTests;

public class ReharmonizerTests
{
    [Theory]
    [InlineData("G7", "Db7")]
    [InlineData("B7", "F7")]
    [InlineData("F#7", "C7")]
    [InlineData("Db7", "G7")]
    [InlineData("Bb7", "E7")]
    [InlineData("G7b9", "Db7b9")]
    [InlineData("G7/B", "Db7/F")]
    [InlineData("E7", "Bb7")]
    public void TritoneSubstitute_SpellsByInterval(string chord, string expected) =>
        Reharmonizer.TritoneSubstitute(ChordSymbol.Parse(chord)).ToString().ShouldBe(expected);

    [Theory]
    [InlineData("G7sus4")]
    [InlineData("Cmaj7")]
    public void TritoneSubstitute_RejectsNonDominants(string chord) =>
        Should.Throw<ArgumentException>(() => Reharmonizer.TritoneSubstitute(ChordSymbol.Parse(chord)));

    [Fact]
    public void TritoneSubstitute_IsAnInvolution() =>
        Reharmonizer.TritoneSubstitute(Reharmonizer.TritoneSubstitute(ChordSymbol.Parse("E7"))).ToString().ShouldBe("E7");

    [Theory]
    [InlineData("Cmaj7", "Dm7 G7")]
    [InlineData("Am", "Bm7b5 E7")]
    [InlineData("F", "Gm7 C7")]
    [InlineData("Bb", "Cm7 F7")]
    [InlineData("G7", "Am7 D7")]
    [InlineData("F#", "G#m7 C#7")]
    [InlineData("Csus4", "Dm7 G7")]
    [InlineData("C5", "Dm7 G7")]
    [InlineData("C/E", "Dm7 G7")]
    public void TwoFiveTo_ReturnsIiAndV(string target, string expected) =>
        string.Join(' ', Reharmonizer.TwoFiveTo(ChordSymbol.Parse(target))).ShouldBe(expected);

    [Theory]
    [InlineData("Bdim")]
    [InlineData("Caug")]
    public void TwoFiveTo_RejectsDiminishedAndAugmented(string target) =>
        Should.Throw<ArgumentException>(() => Reharmonizer.TwoFiveTo(ChordSymbol.Parse(target)));

    [Fact]
    public void TwoFiveTo_RejectsNull() =>
        Should.Throw<ArgumentNullException>(() => Reharmonizer.TwoFiveTo(null!));

    [Fact]
    public void InsertTwoFive_InsertsBeforeTarget()
    {
        var input = new[] { "C", "F" }.Select(ChordSymbol.Parse).ToList();
        string.Join(' ', Reharmonizer.InsertTwoFive(input, 1)).ShouldBe("C Gm7 C7 F");
        string.Join(' ', input).ShouldBe("C F");
    }

    [Fact]
    public void InsertTwoFive_AtStart() =>
        string.Join(' ', Reharmonizer.InsertTwoFive([ChordSymbol.Parse("C")], 0)).ShouldBe("Dm7 G7 C");

    [Theory]
    [InlineData(-1)]
    [InlineData(1)]
    public void InsertTwoFive_RejectsBadIndex(int index) =>
        Should.Throw<ArgumentOutOfRangeException>(() => Reharmonizer.InsertTwoFive([ChordSymbol.Parse("C")], index));

    [Fact]
    public void InsertTwoFive_RejectsNull() =>
        Should.Throw<ArgumentNullException>(() => Reharmonizer.InsertTwoFive(null!, 0));

    [Fact]
    public void TritoneSubstitute_RejectsNull() =>
        Should.Throw<ArgumentNullException>(() => Reharmonizer.TritoneSubstitute(null!));
}
