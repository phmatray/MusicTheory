namespace MusicTheory.UnitTests;

/// <summary>
/// Checks <see cref="Duration.Value"/> against the durations music21 computes
/// (regenerate with tools/reference/generate.py).
/// </summary>
public class DurationReferenceTests
{
    [Fact]
    public void Value_MatchesMusic21_ForEveryTypeDotAndTuplet()
    {
        var rows = File.ReadLines(Path.Combine(AppContext.BaseDirectory, "Reference", "durations.csv"))
            .Skip(1).Select(l => l.Split(',')).ToList();
        var mismatches = new List<string>();
        foreach (var r in rows)
        {
            var type = Enum.Parse<DurationType>(r[0]);
            var duration = r[2] == ""
                ? new Duration(type, int.Parse(r[1]))
                : Duration.CreateTuplet(type, int.Parse(r[2]), int.Parse(r[3]));
            if (duration.Value.ToString() != r[4])
                mismatches.Add($"{r[0]} dots={r[1]} {r[2]}:{r[3]}: expected {r[4]}, got {duration.Value}");
        }
        rows.Count.ShouldBeGreaterThan(0);
        mismatches.ShouldBeEmpty($"{mismatches.Count} of {rows.Count} rows disagree with music21:\n{string.Join("\n", mismatches)}");
    }
}
