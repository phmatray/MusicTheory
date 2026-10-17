namespace MusicTheory.Guitar.UnitTests;

public class TuningTests
{
    [Fact]
    public void Standard_IsE2A2D3G3B3E4_LowToHigh()
    {
        Tuning.Standard.Strings.Select(n => n.MidiNumber).ShouldBe([40, 45, 50, 55, 59, 64]);
    }

    [Fact]
    public void HalfStepDown_IsStandardMinusOneSemitone()
    {
        Tuning.HalfStepDown.Strings.Select(n => n.MidiNumber).ShouldBe([39, 44, 49, 54, 58, 63]);
    }
}
