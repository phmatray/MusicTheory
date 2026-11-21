namespace MusicTheory.UnitTests;

/// <summary>Checks <see cref="TempoMap.SecondsAt"/> against music21's secondsMap (tools/reference/generate.py).</summary>
public class TempoMapReferenceTests
{
    private static Rational Fraction(string s)
    {
        var p = s.Split('/');
        return Rational.Of(long.Parse(p[0]), long.Parse(p[1]));
    }

    [Fact]
    public void SecondsAt_MatchesMusic21_ForEveryRow()
    {
        var rows = File.ReadLines(Path.Combine(AppContext.BaseDirectory, "Reference", "tempo.csv")).Skip(1).Select(l => l.Split(',')).ToList();
        rows.Count.ShouldBeGreaterThan(0);
        foreach (var r in rows)
        {
            var pairs = r[0].Split(' ').Select(c => c.Split('@')).ToList();
            var map = new TempoMap(double.Parse(pairs[0][1], System.Globalization.CultureInfo.InvariantCulture));
            foreach (var c in pairs.Skip(1))
                map = map.WithChange(Fraction(c[0]), double.Parse(c[1], System.Globalization.CultureInfo.InvariantCulture));

            map.SecondsAt(Fraction(r[1])).ShouldBe(double.Parse(r[2], System.Globalization.CultureInfo.InvariantCulture), 1e-9, r[0] + " @ " + r[1]);
        }
    }
}
