namespace MusicTheory.UnitTests;

/// <summary>Checks every non-empty pitch-class set against music21 (Reference/pcsets.csv).</summary>
public class PitchClassSetReferenceTests
{
    private static string Join(IEnumerable<int> xs) => string.Join(" ", xs);

    [Fact]
    public void Forms_MatchMusic21_ForAll4095Sets()
    {
        var rows = File.ReadLines(Path.Combine(AppContext.BaseDirectory, "Reference", "pcsets.csv")).Skip(1).Select(l => l.Split(',')).ToList();
        var mismatches = new List<string>();
        foreach (var r in rows)
        {
            var mask = int.Parse(r[0]);
            var set = PitchClassSet.Of([.. Enumerable.Range(0, 12).Where(pc => (mask >> pc & 1) == 1)]);
            var actual = $"{Join(set.NormalForm)}|{Join(set.PrimeForm)}|{Join(set.IntervalClassVector)}";
            if (actual != $"{r[1]}|{r[2]}|{r[3]}")
                mismatches.Add($"{set}: expected {r[1]}|{r[2]}|{r[3]}, got {actual}");
        }
        rows.Count.ShouldBe(4095);
        mismatches.ShouldBeEmpty($"{mismatches.Count} sets disagree with music21:\n{string.Join("\n", mismatches.Take(20))}");
    }
}
