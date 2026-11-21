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

    [Fact]
    public void Ukulele_IsG4C4E4A4() => Tuning.Ukulele.Strings.Select(n => n.MidiNumber).ShouldBe([67, 60, 64, 69]);

    [Fact]
    public void UkuleleLowG_IsG3C4E4A4() => Tuning.UkuleleLowG.Strings.Select(n => n.MidiNumber).ShouldBe([55, 60, 64, 69]);

    [Fact]
    public void Bass_IsE1A1D2G2() => Tuning.Bass.Strings.Select(n => n.MidiNumber).ShouldBe([28, 33, 38, 43]);

    [Fact]
    public void Mandolin_IsG3D4A4E5() => Tuning.Mandolin.Strings.Select(n => n.MidiNumber).ShouldBe([55, 62, 69, 76]);

    [Fact]
    public void SevenString_IsB1E2A2D3G3B3E4() =>
        Tuning.SevenString.Strings.Select(n => n.MidiNumber).ShouldBe([35, 40, 45, 50, 55, 59, 64]);

    [Fact]
    public void IsReentrant_IsTrueOnlyWhenAStringSoundsBelowThePreviousOne()
    {
        Tuning.Ukulele.IsReentrant.ShouldBeTrue();
        Tuning.Standard.IsReentrant.ShouldBeFalse();
        Tuning.UkuleleLowG.IsReentrant.ShouldBeFalse();
    }
}
