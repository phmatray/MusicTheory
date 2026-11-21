namespace MusicTheory.Abc.UnitTests;

public class AbcRoundTripTests
{
    private static string Bar(Measure m) =>
        string.Join(" ", m.Events.Select(e =>
            (e.IsRest ? "z" : string.Join("+", e.Pitches)) + "/" + e.Duration + (e.TiedToNext ? "~" : "")));

    private static void RoundTrips(string text)
    {
        var tune = AbcTune.Parse(text);
        var back = AbcTune.Parse(tune.ToString());
        back.Measures.Count.ShouldBe(tune.Measures.Count);
        for (var i = 0; i < tune.Measures.Count; i++)
            Bar(back.Measures[i]).ShouldBe(Bar(tune.Measures[i]));
        back.Key.ShouldBe(tune.Key);
        back.TimeSignature.ShouldBe(tune.TimeSignature);
        back.UnitLength.ShouldBe(tune.UnitLength);
        // A bare Q:120 is written as 1/4=120, so compare the timing, not the beat unit.
        back.Tempo?.SecondsAt(Rational.One).ShouldBe(tune.Tempo!.SecondsAt(Rational.One));
        back.Title.ShouldBe(tune.Title);
        back.ReferenceNumber.ShouldBe(tune.ReferenceNumber);
    }

    [Fact]
    public void SpeedThePloughIsCanonical()
    {
        var tune = AbcTune.Parse("X:1\nT:Speed the Plough\nM:4/4\nL:1/8\nQ:1/4=120\nK:G\nGABc dedB|dedB dedB|c2ec B2dB|c2A2 A2BA|");
        tune.ToString().ShouldBe(
            "X:1\nT:Speed the Plough\nM:4/4\nL:1/8\nQ:1/4=120\nK:G\n" +
            "G A B c d e d B|d e d B d e d B|c2 e c B2 d B|c2 A2 A2 B A|]");
        RoundTrips(tune.ToString());
    }

    [Fact]
    public void NaturalIsWrittenOnceThenBare()
    {
        AbcTune.Parse("X:1\nL:1/4\nK:G\nF=FF F|F F|").ToString()
            .ShouldEndWith("K:G\nF =F F F|F F|]");
    }

    [Theory]
    [InlineData("X:2\nM:3/4\nK:Dm\nD2 ^F=F^^G __B,,|z2 [CEG]2 [A,^C]-|")]
    [InlineData("X:3\nM:6/8\nQ:120\nK:F#\nc2-c2 z2|(3abc A3/2 B/|")]
    [InlineData("X:4\nM:4/4\nL:1/8\nK:Ador\nE2 A2|c2 B2 A2 G2|")]
    [InlineData("X:5\nK:C")]
    public void Fixtures(string text) => RoundTrips(text);

    [Fact]
    public void EmptyAndBareHeaders()
    {
        AbcTune.Parse("X:5\nK:C").ToString().ShouldBe("X:5\nM:4/4\nL:1/8\nK:C");
        AbcTune.Parse("X:5\nQ:90\nK:Em\nE|").ToString().ShouldContain("Q:1/4=90\nK:Em\nE|]");
    }
}
