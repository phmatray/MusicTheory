namespace MusicTheory.UnitTests;

public class ModulationDetectorTests
{
    private const string CtoD = "C F G7 C C F G7 C D G A7 D D G A7 D";
    private const string AmToC = "Am Dm E7 Am C F G7 C C F G7 C";
    private const string Steady = "G D Em C G D Em C";
    private const string CtoG = "C F G7 C C F G7 C G C D7 G G C D7 G";

    private static List<ChordSymbol> Parse(string s) => s.Split(' ').Select(ChordSymbol.Parse).ToList();

    private static IReadOnlyList<KeyRegion> Detect(string s, int window = 4) => ModulationDetector.Detect(Parse(s), window);

    private static bool DiatonicIn(ChordSymbol chord, KeySignature key) =>
        HarmonicAnalyzer.Analyze(chord, key).Function == HarmonicFunction.Diatonic;

    [Fact]
    public void Detect_CToD_TwoRegions()
    {
        var regions = Detect(CtoD);
        regions.Select(r => r.Key.ToString()).ShouldBe(["C", "D"]);
        regions[1].Start.ShouldBeInRange(7, 9);
        regions[0].PivotIndex.ShouldBeNull();
    }

    [Fact]
    public void Detect_Steady_OneRegion()
    {
        var regions = Detect(Steady);
        var region = regions.ShouldHaveSingleItem();
        region.Key.ToString().ShouldBe("G");
        (region.Start, region.End, region.PivotIndex).ShouldBe((0, 7, null));
    }

    [Fact]
    public void Detect_AmToC_TwoRegions()
    {
        var regions = Detect(AmToC);
        regions.Select(r => r.Key.ToString()).ShouldBe(["Am", "C"]);
        regions[1].Start.ShouldBeInRange(3, 5);
    }

    [Theory]
    [InlineData(CtoD)]
    [InlineData(AmToC)]
    [InlineData(Steady)]
    [InlineData(CtoG)]
    [InlineData("C")]
    [InlineData("C F G7 C Am")]
    [InlineData("C F G7 C G D G D G D")]
    [InlineData("C G D G D G D G D G D G D G")]
    public void Detect_RegionsCoverEverythingContiguously(string input)
    {
        var regions = Detect(input);
        int n = Parse(input).Count;
        regions[0].Start.ShouldBe(0);
        regions[^1].End.ShouldBe(n - 1);
        for (int i = 1; i < regions.Count; i++)
            regions[i].Start.ShouldBe(regions[i - 1].End + 1);
        if (n >= 4)
            regions.ShouldAllBe(r => r.End > r.Start);
    }

    [Fact]
    public void Detect_ShortAndEmptyInputs()
    {
        Detect("C F G").ShouldHaveSingleItem().End.ShouldBe(2);
        ModulationDetector.Detect([]).ShouldBeEmpty();
    }

    [Fact]
    public void Detect_BadArguments_Throw()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => Detect("C F G C", window: 1));
        Should.Throw<ArgumentNullException>(() => ModulationDetector.Detect(null!));
    }

    [Theory]
    [InlineData(CtoD)]
    [InlineData(AmToC)]
    [InlineData(CtoG)]
    public void Detect_Pivot_IsDiatonicInBothKeys(string input)
    {
        var chords = Parse(input);
        var regions = ModulationDetector.Detect(chords);
        foreach (var (prev, r) in regions.Zip(regions.Skip(1)))
        {
            if (r.PivotIndex is { } p)
            {
                p.ShouldBeInRange(r.Start - 2, r.Start - 1);
                DiatonicIn(chords[p], prev.Key).ShouldBeTrue();
                DiatonicIn(chords[p], r.Key).ShouldBeTrue();
            }
        }
    }

    [Fact]
    public void Detect_CToG_HasAPivot()
    {
        var regions = Detect(CtoG);
        regions.Count.ShouldBeGreaterThan(1, string.Join(" ", regions.Select(r => $"{r.Key}:{r.Start}-{r.End}")));
        regions.Skip(1).ShouldContain(r => r.PivotIndex != null);
    }
}
