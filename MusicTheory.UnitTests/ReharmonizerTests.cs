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

    [Fact]
    public void TritoneSubstitute_RejectsNull() =>
        Should.Throw<ArgumentNullException>(() => Reharmonizer.TritoneSubstitute(null!));
}
