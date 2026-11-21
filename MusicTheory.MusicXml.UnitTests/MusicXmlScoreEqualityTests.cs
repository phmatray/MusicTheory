namespace MusicTheory.MusicXml.UnitTests;

public class MusicXmlScoreEqualityTests
{
    private static MusicXmlScore Score(DurationType t) => new("t", null, TimeSignature.CommonTime,
        [new Measure(TimeSignature.CommonTime, [RhythmEvent.Rest(new Duration(t))])]);

    [Fact]
    public void ScoresCompareByContent()
    {
        Score(DurationType.Half).ShouldBe(Score(DurationType.Half));
        Score(DurationType.Half).GetHashCode().ShouldBe(Score(DurationType.Half).GetHashCode());
        Score(DurationType.Half).ShouldNotBe(Score(DurationType.Quarter));
        Score(DurationType.Half).Equals(null).ShouldBeFalse();
    }
}
