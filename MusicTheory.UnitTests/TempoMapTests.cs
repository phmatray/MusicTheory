namespace MusicTheory.UnitTests;

public class TempoMapTests
{
    private static readonly Duration Quarter = new(DurationType.Quarter);

    private static TempoMap Changing() => new TempoMap(120).WithChange(Rational.Of(2), 90);

    [Fact]
    public void Equality_IsByValue()
    {
        Changing().ShouldBe(Changing());
        Changing().GetHashCode().ShouldBe(Changing().GetHashCode());
        Changing().Equals(null).ShouldBeFalse();
        Changing().ShouldNotBe(new TempoMap(120));
        Changing().ShouldNotBe(new TempoMap(120).WithChange(Rational.Of(2), 91));
    }

    [Fact]
    public void SecondsAt_OneQuarterBar_IsTwoSeconds()
    {
        new TempoMap(120).SecondsAt(Rational.One).ShouldBe(2.0, 1e-12);
    }

    [Fact]
    public void SecondsAt_AfterChange_SumsSegments()
    {
        Changing().SecondsAt(Rational.Of(3)).ShouldBe(4 + 8.0 / 3, 1e-12);
        Changing().SecondsAt(Rational.Of(2)).ShouldBe(4.0, 1e-12);
        Changing().SecondsAt(Rational.Of(1)).ShouldBe(2.0, 1e-12);
        Changing().SecondsAt(Rational.Zero).ShouldBe(0.0);
    }

    [Fact]
    public void SecondsAt_SkipsEarlierSegmentsCompletely()
    {
        var map = Changing().WithChange(Rational.Of(4), 60);
        map.SecondsAt(Rational.Of(5)).ShouldBe(4 + 2 * 8.0 / 3 + 4, 1e-12);
    }

    [Fact]
    public void PositionAt_InvertsSecondsAt_AcrossThreeChanges()
    {
        var map = new TempoMap(120).WithChange(Rational.Of(3, 2), 90).WithChange(Rational.Of(7, 2), 200).WithChange(Rational.Of(5), 45);
        for (int i = 0; i < 200; i++)
        {
            var p = Rational.Of(i, 30);
            map.PositionAt(map.SecondsAt(p)).ShouldBe(p.ToDouble(), 1e-12);
        }
    }

    [Fact]
    public void PositionAt_IssueExample()
    {
        Changing().PositionAt(5.0).ShouldBe(2.375, 1e-12);
    }

    [Theory]
    [InlineData(-1.0)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void PositionAt_InvalidSeconds_Throws(double seconds)
    {
        Should.Throw<ArgumentOutOfRangeException>(() => Changing().PositionAt(seconds)).ParamName.ShouldBe("seconds");
    }

    [Fact]
    public void DottedQuarterBeat_Ac4()
    {
        new TempoMap(60, new Duration(DurationType.Quarter, 1)).SecondsAt(Rational.Of(3, 8)).ShouldBe(1.0, 1e-12);
    }

    [Fact]
    public void WithChange_AtExistingPosition_Replaces()
    {
        var map = Changing().WithChange(Rational.Of(2), 60).WithChange(Rational.Zero, 100);
        map.Changes.Select(c => (c.Position, c.Bpm)).ShouldBe([(Rational.Zero, 100.0), (Rational.Of(2), 60.0)]);
    }

    [Fact]
    public void WithChange_KeepsChangesSorted_AndOriginalUntouched()
    {
        var original = new TempoMap(120);
        var map = original.WithChange(Rational.Of(4), 60).WithChange(Rational.Of(2), 90);
        map.Changes.Select(c => c.Position).ShouldBe([Rational.Zero, Rational.Of(2), Rational.Of(4)]);
        original.Changes.Count.ShouldBe(1);
    }

    [Fact]
    public void BpmAt_UsesChangeInForce()
    {
        Changing().BpmAt(Rational.Of(5, 2)).ShouldBe(90);
        Changing().BpmAt(Rational.Of(2)).ShouldBe(90);
        Changing().BpmAt(Rational.Of(3, 2)).ShouldBe(120);
    }

    [Fact]
    public void BeatUnit_PerChange()
    {
        var map = new TempoMap(120).WithChange(Rational.One, 60, new Duration(DurationType.Half));
        map.SecondsAt(Rational.Of(2)).ShouldBe(2 + 2.0, 1e-12);
    }

    [Fact]
    public void NegativePosition_Throws()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => Changing().SecondsAt(Rational.Of(-1, 4)));
        Should.Throw<ArgumentOutOfRangeException>(() => Changing().BpmAt(Rational.Of(-1, 4)));
        Should.Throw<ArgumentOutOfRangeException>(() => Changing().WithChange(Rational.Of(-1), 100));
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-5.0)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void InvalidBpm_Throws(double bpm)
    {
        Should.Throw<ArgumentOutOfRangeException>(() => new TempoMap(bpm));
        Should.Throw<ArgumentOutOfRangeException>(() => Changing().WithChange(Rational.One, bpm));
    }
}
