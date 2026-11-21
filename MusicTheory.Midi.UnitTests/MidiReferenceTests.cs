namespace MusicTheory.Midi.UnitTests;

/// <summary>Reads music21-written .mid files and compares with what music21 itself parses back (Reference/midi.csv).</summary>
public class MidiReferenceTests
{
    public static IEnumerable<TheoryDataRow<string, string, string, string>> Rows() =>
        File.ReadLines(Path.Combine(AppContext.BaseDirectory, "Reference", "midi.csv")).Skip(1)
            .Select(l => l.Split(','))
            .Select(c => new TheoryDataRow<string, string, string, string>(c[0], c[1], c[2], c[3]));

    private static string Show(RhythmEvent e) =>
        (e.IsRest ? "r" : string.Join("+", e.Pitches)) + ":" + e.Duration.Value + (e.TiedToNext ? "~" : "");

    [Theory]
    [MemberData(nameof(Rows))]
    public void Reader_matches_music21(string file, string meter, string key, string notes)
    {
        using var input = File.OpenRead(Path.Combine(AppContext.BaseDirectory, "Reference", file));
        var import = MidiReader.Read(input);

        import.TimeSignature.ToString().ShouldBe(meter);
        (import.Key?.ToString() ?? "").ShouldBe(key);
        string.Join(" ", import.Measures.SelectMany(m => m.Events).Select(Show)).ShouldBe(notes);
    }

    [Fact]
    public void Reader_keeps_the_tempo_change_music21_wrote()
    {
        using var input = File.OpenRead(Path.Combine(AppContext.BaseDirectory, "Reference", "tempo-change.mid"));
        var changes = MidiReader.Read(input).Tempo.Changes;

        changes.Select(c => c.Bpm).ShouldBe([80.0, 60.0]);
        changes[1].Position.ToString().ShouldBe("1/2");
    }
}
