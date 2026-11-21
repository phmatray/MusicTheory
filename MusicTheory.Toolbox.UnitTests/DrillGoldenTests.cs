using MusicTheory.Training;

namespace MusicTheory.Toolbox.UnitTests;

/// <summary>Pins the draw order of the chord, scale and progression drills: a library change that alters seed 42 would silently change every shared link.</summary>
public class DrillGoldenTests
{
    private static string Actual<T>(Func<Difficulty, Func<Question<T>>> next, Func<T, string> name) =>
        string.Join('\n', Enum.GetValues<Difficulty>().Select(d =>
        {
            var draw = next(d);
            return $"{d.ToString().ToLowerInvariant()}: {string.Join(' ', Enumerable.Range(0, 10).Select(_ => name(draw().Answer)))}";
        })) + "\n";

    private static void Pin(string file, string actual)
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (!File.Exists(Path.Combine(dir.FullName, "MusicTheory.sln"))) dir = dir.Parent!;
        var path = Path.Combine(dir.FullName, "MusicTheory.Toolbox.UnitTests", "Golden", file);
        if (Environment.GetEnvironmentVariable("UPDATE_GOLDEN") == "1") File.WriteAllText(path, actual);
        actual.ShouldBe(File.ReadAllText(path).ReplaceLineEndings("\n")); // git may check the file out with CRLF on Windows
    }

    [Fact]
    public void Chords_seed_42() =>
        Pin("chords-seed42.txt", Actual<ChordType>(d => new ChordDrill(ChordDrillOptions.For(d), 42).Next, t => t.ToString()));

    [Fact]
    public void Scales_seed_42() =>
        Pin("scales-seed42.txt", Actual<ScaleType>(d => new ScaleDrill(ScaleDrillOptions.For(d), 42).Next, t => t.ToString()));

    [Fact]
    public void Progressions_seed_42() =>
        Pin("progressions-seed42.txt", Actual<IReadOnlyList<int>>(d => new ProgressionDrill(ProgressionDrillOptions.For(d), 42).Next, a => string.Concat(a)));
}
