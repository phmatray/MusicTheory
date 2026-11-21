namespace MusicTheory.UnitTests;

/// <summary>Checks Measure.Split against music21's makeMeasures() + makeTies() (Reference/measures.csv).</summary>
public class MeasureReferenceTests
{
    public static IEnumerable<TheoryDataRow<string, string, string>> Cases() =>
        File.ReadLines(Path.Combine(AppContext.BaseDirectory, "Reference", "measures.csv")).Skip(1)
            .Select(l => l.Split(','))
            .Select(r => new TheoryDataRow<string, string, string>(r[1], r[2], r[3]) { TestDisplayName = r[0] });

    private static Rational Parse(string s)
    {
        var p = s.Split('/');
        return Rational.Of(long.Parse(p[0]), p.Length == 1 ? 1 : long.Parse(p[1]));
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void Split_MatchesMusic21(string meter, string input, string expected)
    {
        var m = meter.Split('/');
        var events = input.Split(' ').Select(tok =>
        {
            var rest = tok.StartsWith('r');
            Duration.TryFromValue(Parse(tok.TrimStart('r')), out var d).ShouldBeTrue();
            return new RhythmEvent(rest ? [] : [Note.Parse("C4")], d!);
        });

        var bars = Measure.Split(new TimeSignature(int.Parse(m[0]), int.Parse(m[1])), events);

        var actual = string.Join("|", bars.Select(b => string.Join(" ", b.Events.Select(e => e.Duration.Value + (e.TiedToNext ? "~" : "")))));
        actual.ShouldBe(expected);
    }
}
