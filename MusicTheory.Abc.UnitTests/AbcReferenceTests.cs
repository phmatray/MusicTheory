namespace MusicTheory.Abc.UnitTests;

/// <summary>Pins <see cref="AbcTune.Parse"/> to the notes music21 reads from the same folk-tune fixtures.</summary>
public class AbcReferenceTests
{
    private static readonly string Dir = Path.Combine(AppContext.BaseDirectory, "Reference");

    public static TheoryData<string> Files()
    {
        var data = new TheoryData<string>();
        foreach (var row in Rows())
            data.Add(row[0]);
        return data;
    }

    private static IEnumerable<string[]> Rows() =>
        File.ReadLines(Path.Combine(Dir, "abc.csv")).Skip(1).Select(l => l.Split(',', 4));

    [Theory]
    [MemberData(nameof(Files))]
    public void Parse_matches_music21(string file)
    {
        var row = Rows().Single(r => r[0] == file);
        var tune = AbcTune.Parse(File.ReadAllText(Path.Combine(Dir, file)));

        tune.TimeSignature.ToString().ShouldBe(row[1]);
        tune.Key.AccidentalCount.ShouldBe(int.Parse(row[2]));
        var notes = tune.Measures.SelectMany(m => m.Events).Select(e =>
            (e.IsRest ? "r" : string.Join("+", e.Pitches)) + $"/{e.Duration.Value.Numerator}/{e.Duration.Value.Denominator}" + (e.TiedToNext ? "~" : ""));
        string.Join(" ", notes).ShouldBe(row[3]);
    }
}
