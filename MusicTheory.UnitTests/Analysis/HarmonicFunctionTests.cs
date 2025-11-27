namespace MusicTheory.UnitTests.Analysis;

using MusicTheory.Analysis;

public class HarmonicFunctionTests
{
    [Fact]
    public void HarmonicFunction_ShouldHave_AllExpectedValues()
    {
        // Assert - Verify all expected enum values exist
        Enum.GetValues<HarmonicFunction>().ShouldContain(HarmonicFunction.Tonic);
        Enum.GetValues<HarmonicFunction>().ShouldContain(HarmonicFunction.Subdominant);
        Enum.GetValues<HarmonicFunction>().ShouldContain(HarmonicFunction.Dominant);
        Enum.GetValues<HarmonicFunction>().ShouldContain(HarmonicFunction.SecondaryDominant);
        Enum.GetValues<HarmonicFunction>().ShouldContain(HarmonicFunction.ModalInterchange);
        Enum.GetValues<HarmonicFunction>().ShouldContain(HarmonicFunction.ChromaticMediant);
        Enum.GetValues<HarmonicFunction>().ShouldContain(HarmonicFunction.Passing);
        Enum.GetValues<HarmonicFunction>().ShouldContain(HarmonicFunction.Neighbor);
    }

    [Fact]
    public void HarmonicFunction_Count_ShouldBe8()
    {
        // Assert
        Enum.GetValues<HarmonicFunction>().Length.ShouldBe(8);
    }
}
