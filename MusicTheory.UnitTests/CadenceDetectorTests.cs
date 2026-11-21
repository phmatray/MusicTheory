namespace MusicTheory.UnitTests;

public class CadenceDetectorTests
{
    private static string Detect(string key, string chords) =>
        string.Join(", ", CadenceDetector
            .Detect(chords.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(ChordSymbol.Parse).ToList(), KeySignature.Parse(key))
            .Select(c => $"{c.Type}@{c.Index}"));

    [Theory]
    [InlineData("C", "C F G C", "Authentic@3")]
    [InlineData("C", "C F C", "Plagal@2")]
    [InlineData("C", "C F G", "Half@2")]
    [InlineData("C", "C F G Am", "Deceptive@3")]
    [InlineData("Am", "Am Dm E7 Am", "Authentic@3")]
    [InlineData("Am", "Am Dm E7 F", "Deceptive@3")]
    [InlineData("C", "C Bdim C", "Authentic@2")]
    [InlineData("C", "C Fm C", "Plagal@2")]
    [InlineData("C", "C D7 G", "Half@2")]
    [InlineData("C", "C G C F C", "Authentic@2, Plagal@4")]
    [InlineData("C", "G", "Half@0")]
    [InlineData("C", "C", "")]
    [InlineData("C", "", "")]
    public void Detect_Cadences(string key, string chords, string expected) =>
        Detect(key, chords).ShouldBe(expected);

    [Fact]
    public void Detect_NullArguments_Throw()
    {
        Should.Throw<ArgumentNullException>(() => CadenceDetector.Detect(null!, KeySignature.Parse("C")));
        Should.Throw<ArgumentNullException>(() => CadenceDetector.Detect([], null!));
    }
}
