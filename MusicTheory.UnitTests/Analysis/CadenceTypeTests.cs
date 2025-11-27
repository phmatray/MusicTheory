namespace MusicTheory.UnitTests.Analysis;

using MusicTheory.Analysis;

public class CadenceTypeTests
{
    [Fact]
    public void CadenceType_ShouldHave_AllExpectedValues()
    {
        // Assert - Verify all expected enum values exist
        Enum.GetValues<CadenceType>().ShouldContain(CadenceType.AuthenticPerfect);
        Enum.GetValues<CadenceType>().ShouldContain(CadenceType.AuthenticImperfect);
        Enum.GetValues<CadenceType>().ShouldContain(CadenceType.Plagal);
        Enum.GetValues<CadenceType>().ShouldContain(CadenceType.Deceptive);
        Enum.GetValues<CadenceType>().ShouldContain(CadenceType.Half);
        Enum.GetValues<CadenceType>().ShouldContain(CadenceType.Phrygian);
    }

    [Fact]
    public void CadenceType_Count_ShouldBe6()
    {
        // Assert
        Enum.GetValues<CadenceType>().Length.ShouldBe(6);
    }
}
