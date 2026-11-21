namespace MusicTheory.UnitTests;

/// <summary>
/// Checks the library against tables computed by music21, the reference implementation of spelled music
/// theory (regenerate them with tools/reference/generate.py). A "!" marks a result music21 spells with
/// triple accidentals, which this library refuses.
/// </summary>
public class ReferenceTests
{
    private static IEnumerable<string[]> Rows(string file) =>
        File.ReadLines(Path.Combine(AppContext.BaseDirectory, "Reference", file)).Skip(1).Select(l => l.Split(','));

    private static void NoMismatches(List<string> mismatches, int checkedRows)
    {
        checkedRows.ShouldBeGreaterThan(0);
        mismatches.ShouldBeEmpty($"{mismatches.Count} of {checkedRows} rows disagree with music21:\n{string.Join("\n", mismatches.Take(25))}");
    }

    [Fact]
    public void Between_MatchesMusic21_ForEverySpelledPair()
    {
        var mismatches = new List<string>();
        var rows = Rows("intervals.csv").ToList();
        foreach (var r in rows)
        {
            var interval = Interval.Between(Note.Parse(r[0]), Note.Parse(r[1]));
            var actual = $"{interval} {interval.DirectedSemitones}";
            if (actual != $"{r[2]} {r[3]}")
                mismatches.Add($"{r[0]} -> {r[1]}: expected {r[2]} ({r[3]}), got {actual}");
        }
        NoMismatches(mismatches, rows.Count);
    }

    [Fact]
    public void Between_ThenTranspose_RoundTrips_ForEverySpelledPair()
    {
        var mismatches = new List<string>();
        var rows = Rows("intervals.csv").ToList();
        foreach (var r in rows)
        {
            var from = Note.Parse(r[0]);
            var to = Note.Parse(r[1]);
            var back = from.Transpose(Interval.Parse(r[2]));
            if (back != to)
                mismatches.Add($"{r[0]} + {r[2]}: expected {r[1]}, got {back}");
        }
        NoMismatches(mismatches, rows.Count);
    }

    [Fact]
    public void Transpose_MatchesMusic21()
    {
        var mismatches = new List<string>();
        var rows = Rows("transpositions.csv").ToList();
        foreach (var r in rows)
        {
            var note = Note.Parse(r[0]);
            var interval = Interval.Parse(r[1]);
            string actual;
            try
            {
                actual = note.Transpose(interval).ToString();
            }
            catch (InvalidOperationException)
            {
                actual = "!";
            }
            if (actual != r[2])
                mismatches.Add($"{r[0]} + {r[1]}: expected {r[2]}, got {actual}");
        }
        NoMismatches(mismatches, rows.Count);
    }

    [Fact]
    public void Scales_MatchMusic21()
    {
        var mismatches = new List<string>();
        var rows = Rows("scales.csv").ToList();
        foreach (var r in rows)
        {
            var scale = new Scale(Note.Parse(r[0]), Enum.Parse<ScaleType>(r[1]));
            string actual;
            try
            {
                actual = string.Join(" ", scale.GetNotes());
            }
            catch (InvalidOperationException)
            {
                actual = "!";
            }
            var expected = r[2].Contains('!') ? "!" : r[2];
            if (actual != expected)
                mismatches.Add($"{r[0]} {r[1]}: expected {expected}, got {actual}");
        }
        NoMismatches(mismatches, rows.Count);
    }

    [Fact]
    public void KeySignatures_MatchMusic21()
    {
        var mismatches = new List<string>();
        var rows = Rows("keys.csv").ToList();
        foreach (var r in rows)
        {
            var tonic = Note.Parse(r[0]);
            var mode = r[1] == "major" ? KeyMode.Major : KeyMode.Minor;
            int sharps = int.Parse(r[2], System.Globalization.CultureInfo.InvariantCulture);
            if (r[3] == "!")
            {
                if (Record.Exception(() => new KeySignature(tonic, mode)) is not ArgumentOutOfRangeException)
                    mismatches.Add($"{r[0]} {r[1]}: needs triple accidentals, expected a refusal");
                continue;
            }

            var key = new KeySignature(tonic, mode);
            if (key.AccidentalCount != sharps)
                mismatches.Add($"{r[0]} {r[1]}: expected {sharps} accidentals, got {key.AccidentalCount}");

            // music21 lists every step of a double accidental (Bb then Bbb); the furthest one is the key's.
            var altered = r[3].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(Note.Parse)
                .GroupBy(n => n.Name)
                .ToDictionary(g => g.Key, g => g.MaxBy(n => Math.Abs((int)n.Alteration))!.Alteration);
            foreach (var letter in Enum.GetValues<NoteName>())
            {
                var expected = altered.GetValueOrDefault(letter, Alteration.Natural);
                if (key.GetAlteration(letter) != expected)
                    mismatches.Add($"{r[0]} {r[1]}: {letter} should be {expected}, got {key.GetAlteration(letter)}");
            }
        }
        NoMismatches(mismatches, rows.Count);
    }
}
