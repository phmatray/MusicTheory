namespace MusicTheory.UnitTests;

public class MeasureSplitTests
{
    private static readonly Note C4 = Note.Parse("C4");
    private static readonly Note D4 = Note.Parse("D4");
    private static Duration D(DurationType t, int dots = 0) => new(t, dots);
    private static RhythmEvent N(Note n, Duration d, bool tied = false) => new([n], d, tied);

    private static string Shape(IEnumerable<Measure> bars) =>
        string.Join("|", bars.Select(b => string.Join(" ", b.Events.Select(e => e.Duration.Value + (e.TiedToNext ? "~" : "")))));

    [Fact]
    public void DottedHalfThenHalf_TiesAcrossTheBarline()
    {
        var bars = Measure.Split(TimeSignature.CommonTime, [N(C4, D(DurationType.Half, 1)), N(D4, D(DurationType.Half))]);
        Shape(bars).ShouldBe("3/4 1/4~|1/4");
        bars[0].IsComplete.ShouldBeTrue();
        bars[1].Remaining.ShouldBe(Rational.Of(3, 4));
    }

    [Fact]
    public void WholeNote_InTwoTwo_GivesTwoTiedHalves() =>
        Shape(Measure.Split(new TimeSignature(2, 4), [N(C4, D(DurationType.Whole))])).ShouldBe("1/2~|1/2");

    [Fact]
    public void WholeRest_HasNoTies() =>
        Shape(Measure.Split(new TimeSignature(2, 4), [RhythmEvent.Rest(D(DurationType.Whole))])).ShouldBe("1/2|1/2");

    [Fact]
    public void Pickup_SetsFirstCapacity()
    {
        var bars = Measure.Split(new TimeSignature(3, 4),
            [N(C4, D(DurationType.Quarter)), N(C4, D(DurationType.Half, 1)), N(C4, D(DurationType.Quarter))], Rational.Of(1, 4));
        bars.Select(b => b.Capacity).ShouldBe([Rational.Of(1, 4), Rational.Of(3, 4), Rational.Of(3, 4)]);
        bars[2].IsComplete.ShouldBeFalse();
    }

    [Fact]
    public void TupletCrossingBarline_Throws() =>
        Should.Throw<ArgumentException>(() => Measure.Split(TimeSignature.CommonTime,
            [N(C4, D(DurationType.Half, 1)), N(C4, D(DurationType.Eighth)), N(C4, Duration.CreateTuplet(DurationType.Quarter, 3, 2))]));

    [Fact]
    public void TupletInsideBar_Fits() =>
        Measure.Split(TimeSignature.CommonTime, [N(C4, Duration.CreateTuplet(DurationType.Quarter, 3, 2))]).Count.ShouldBe(1);

    [Fact]
    public void EmptyInput_GivesNoMeasures() =>
        Measure.Split(TimeSignature.CommonTime, []).ShouldBeEmpty();

    [Fact]
    public void OriginalTie_StaysOnLastPart() =>
        Shape(Measure.Split(new TimeSignature(2, 4), [N(C4, D(DurationType.Quarter)), N(C4, D(DurationType.Half), true)])).ShouldBe("1/4 1/4~|1/4~");

    [Fact]
    public void MultiPartRemainder_IsTiedToo() =>
        Shape(Measure.Split(new TimeSignature(5, 8), [N(C4, D(DurationType.Whole, 1))])).ShouldBe("1/2~ 1/8~|1/2~ 1/8~|1/4");

    [Fact]
    public void ExactFit_AddsNoEmptyBar() =>
        Measure.Split(TimeSignature.CommonTime, [N(C4, D(DurationType.Whole))]).Count.ShouldBe(1);

    [Fact]
    public void NullArguments_Throw()
    {
        Should.Throw<ArgumentNullException>(() => Measure.Split(null!, []));
        Should.Throw<ArgumentNullException>(() => Measure.Split(TimeSignature.CommonTime, null!));
        Should.Throw<ArgumentNullException>(() => Measure.Split(TimeSignature.CommonTime, [null!]));
    }
}
