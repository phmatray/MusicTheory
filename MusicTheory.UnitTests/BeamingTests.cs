namespace MusicTheory.UnitTests;

public class BeamingTests
{
    private static readonly Note C4 = Note.Parse("C4");

    private static RhythmEvent N(DurationType t) => new([C4], new Duration(t));
    private static RhythmEvent R(DurationType t) => RhythmEvent.Rest(new Duration(t));
    private static RhythmEvent[] Eighths(int count) => [.. Enumerable.Repeat(N(DurationType.Eighth), count)];
    private static string Show(Measure m) => string.Join(" ", Beaming.Group(m).Select(g => string.Join("-", g)));

    [Fact]
    public void SixEighths_InSixEight_BeamInThrees() =>
        Show(new Measure(new TimeSignature(6, 8), Eighths(6))).ShouldBe("0-1-2 3-4-5");

    [Fact]
    public void SixEighths_InThreeFour_BeamInPairs() =>
        Show(new Measure(new TimeSignature(3, 4), Eighths(6))).ShouldBe("0-1 2-3 4-5");

    [Fact]
    public void Rest_BreaksTheBeamAndLeavesLoneNotesOut()
    {
        RhythmEvent[] events = [N(DurationType.Eighth), R(DurationType.Eighth), N(DurationType.Eighth), .. Eighths(3)];
        Show(new Measure(new TimeSignature(6, 8), events)).ShouldBe("3-4-5");
    }

    [Fact]
    public void Quarter_BreaksTheBeam()
    {
        RhythmEvent[] events = [.. Eighths(2), N(DurationType.Quarter), .. Eighths(2)];
        Show(new Measure(new TimeSignature(3, 4), events)).ShouldBe("0-1 3-4");
    }

    [Fact]
    public void Sixteenths_BeamWithinTheirBeat()
    {
        RhythmEvent[] events = [.. Enumerable.Repeat(N(DurationType.Sixteenth), 4), .. Eighths(2)];
        Show(new Measure(new TimeSignature(2, 4), events)).ShouldBe("0-1-2-3 4-5");
    }

    [Fact]
    public void SevenEighths_FollowTwoTwoThree() =>
        Show(new Measure(new TimeSignature(7, 8), Eighths(7))).ShouldBe("0-1 2-3 4-5-6");

    [Fact]
    public void RunEndingTheBar_IsClosed() =>
        Show(new Measure(new TimeSignature(2, 4), [N(DurationType.Quarter), .. Eighths(2)])).ShouldBe("1-2");

    [Fact]
    public void EmptyMeasure_HasNoGroups() =>
        Beaming.Group(new Measure(TimeSignature.CommonTime, [])).ShouldBeEmpty();

    [Fact]
    public void NullMeasure_Throws() =>
        Should.Throw<ArgumentNullException>(() => Beaming.Group(null!));
}
