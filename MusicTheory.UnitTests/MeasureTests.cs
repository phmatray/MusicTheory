namespace MusicTheory.UnitTests;

public class MeasureTests
{
    private static readonly Note C4 = Note.Parse("C4");
    private static RhythmEvent Ev(DurationType t, int dots = 0) => new([C4], new Duration(t, dots));

    [Fact]
    public void FourQuarters_AreCompleteWithOnsets()
    {
        var m = new Measure(TimeSignature.CommonTime, [Ev(DurationType.Quarter), Ev(DurationType.Quarter), Ev(DurationType.Quarter), Ev(DurationType.Quarter)]);
        m.IsComplete.ShouldBeTrue();
        m.Capacity.ShouldBe(Rational.One);
        m.Filled.ShouldBe(Rational.One);
        m.Onsets.ShouldBe([Rational.Zero, Rational.Of(1, 4), Rational.Of(1, 2), Rational.Of(3, 4)]);
    }

    [Fact]
    public void Overflow_ThrowsNamingBothValues()
    {
        var ex = Should.Throw<ArgumentException>(() => new Measure(TimeSignature.CommonTime, [Ev(DurationType.Whole), Ev(DurationType.Quarter)]));
        ex.Message.ShouldContain("5/4 exceeds the 1 capacity of 4/4");
    }

    [Fact]
    public void Half_InThreeFour_LeavesAQuarter()
    {
        var m = new Measure(new TimeSignature(3, 4), [Ev(DurationType.Half)]);
        m.Remaining.ShouldBe(Rational.Of(1, 4));
        m.IsComplete.ShouldBeFalse();
    }

    [Fact]
    public void PickupCapacity_CanBeFilled() =>
        new Measure(TimeSignature.CommonTime, [Ev(DurationType.Quarter)], Rational.Of(1, 4)).IsComplete.ShouldBeTrue();

    [Theory]
    [InlineData(0, 1)]
    [InlineData(5, 4)]
    public void BadCapacity_Throws(long n, long d) =>
        Should.Throw<ArgumentException>(() => new Measure(TimeSignature.CommonTime, [], Rational.Of(n, d)));

    [Fact]
    public void NullArguments_Throw()
    {
        Should.Throw<ArgumentNullException>(() => new Measure(null!, []));
        Should.Throw<ArgumentNullException>(() => new Measure(TimeSignature.CommonTime, null!));
    }

    [Fact]
    public void TiedRest_Throws()
    {
        var rest = RhythmEvent.Rest(new Duration(DurationType.Quarter));
        rest.IsRest.ShouldBeTrue();
        Should.Throw<ArgumentException>(() => rest with { TiedToNext = true });
        Should.Throw<ArgumentException>(() => new RhythmEvent([], new Duration(DurationType.Quarter), true));
    }

    [Fact]
    public void RhythmEvent_ValidatesAndAllowsTiedNote()
    {
        Should.Throw<ArgumentNullException>(() => new RhythmEvent(null!, new Duration(DurationType.Quarter)));
        Should.Throw<ArgumentNullException>(() => new RhythmEvent([C4], null!));
        Should.Throw<ArgumentNullException>(() => Ev(DurationType.Quarter) with { Pitches = null! });
        Should.Throw<ArgumentNullException>(() => Ev(DurationType.Quarter) with { Duration = null! });
        (Ev(DurationType.Quarter) with { TiedToNext = true }).TiedToNext.ShouldBeTrue();
    }
}
