namespace MusicTheory.UnitTests;

/// <summary>Checks Beaming.Group and TimeSignature.BeatGroups against music21's makeBeams() (Reference/beams.csv, beatgroups.csv).</summary>
public class BeamingReferenceTests
{
    private static IEnumerable<string[]> Rows(string file) =>
        File.ReadLines(Path.Combine(AppContext.BaseDirectory, "Reference", file)).Skip(1).Select(l => l.Split(','));

    public static IEnumerable<TheoryDataRow<string, string, string>> BeamCases() =>
        Rows("beams.csv").Select(r => new TheoryDataRow<string, string, string>(r[1], r[2], r[3]) { TestDisplayName = r[0] });

    public static IEnumerable<TheoryDataRow<string, string>> GroupCases() =>
        Rows("beatgroups.csv").Select(r => new TheoryDataRow<string, string>(r[0], r[1]) { TestDisplayName = r[0] });

    private static Rational Parse(string s)
    {
        var p = s.Split('/');
        return Rational.Of(long.Parse(p[0]), p.Length == 1 ? 1 : long.Parse(p[1]));
    }

    private static TimeSignature Meter(string meter)
    {
        var m = meter.Split('/');
        return new TimeSignature(int.Parse(m[0]), int.Parse(m[1]));
    }

    [Theory]
    [MemberData(nameof(BeamCases))]
    public void Group_MatchesMusic21(string meter, string input, string expected)
    {
        var events = input.Split(' ').Select(tok =>
        {
            Duration.TryFromValue(Parse(tok.TrimStart('r')), out var d).ShouldBeTrue();
            return new RhythmEvent(tok.StartsWith('r') ? [] : [Note.Parse("C4")], d!);
        }).ToList();

        var actual = string.Join(" ", Beaming.Group(new Measure(Meter(meter), events)).Select(g => string.Join("-", g)));

        actual.ShouldBe(expected);
    }

    [Theory]
    [MemberData(nameof(GroupCases))]
    public void BeatGroups_MatchMusic21(string meter, string expected)
    {
        var ts = Meter(meter);

        var actual = string.Join(" ", ts.BeatGroups.Select(g => (g / Rational.Of(1, ts.Denominator)).ToString()));

        actual.ShouldBe(expected);
    }
}
