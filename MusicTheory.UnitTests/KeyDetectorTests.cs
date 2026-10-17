namespace MusicTheory.UnitTests;

public class KeyDetectorTests
{
    private static IReadOnlyList<KeyCandidate> Infer(params string[] chords) =>
        KeyDetector.Infer(chords.Select(ChordSymbol.Parse));

    [Theory]
    [InlineData("G", "G", "D", "Em", "C")]
    [InlineData("Am", "Am", "F", "C", "G")]
    [InlineData("Am", "Am", "Dm", "E7", "Am")]
    [InlineData("C", "C", "G", "Am", "F")]
    [InlineData("Eb", "Eb", "Bb", "Cm", "Ab")]
    [InlineData("D", "D", "D/F#", "G", "A7", "D")]
    [InlineData("A", "A7", "D7", "A7", "E7", "D7", "A7")]
    [InlineData("F", "F")]
    public void Infer_BestKey(string expected, params string[] chords)
    {
        Infer(chords)[0].Key.ToString().ShouldBe(expected);
    }

    [Fact]
    public void Infer_RelativeMajorIsSecond_ForAmFCG()
    {
        Infer("Am", "F", "C", "G")[1].Key.ToString().ShouldBe("C");
    }

    [Fact]
    public void Infer_Empty_IsEmpty()
    {
        KeyDetector.Infer(Array.Empty<ChordSymbol>()).ShouldBeEmpty();
    }

    [Fact]
    public void Infer_SingleChord_IsNotConfident()
    {
        Infer("F")[0].Confidence.ShouldBeLessThan(0.6);
    }

    [Fact]
    public void Infer_ConfidencesAreSortedAndSumToOne()
    {
        var result = Infer("G", "D", "Em", "C", "G");
        result.Count.ShouldBe(3);
        result.Sum(c => c.Confidence).ShouldBe(1.0, 1e-9);
        result.ShouldAllBe(c => c.Confidence >= 0 && c.Confidence <= 1);
        result.Select(c => c.Confidence).ShouldBeInOrder(SortDirection.Descending);
        result[0].DiatonicHits.ShouldBe(5);
        result[0].Total.ShouldBe(5);
    }

    [Fact]
    public void Infer_AcceptsChords()
    {
        var chords = new[] { "C", "F", "G7", "C" }.Select(s => ChordSymbol.Parse(s).ToChord());
        KeyDetector.Infer(chords)[0].Key.ToString().ShouldBe("C");
    }
}
