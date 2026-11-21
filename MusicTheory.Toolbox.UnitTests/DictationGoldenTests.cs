using MusicTheory.Training;

namespace MusicTheory.Toolbox.UnitTests;

/// <summary>Pins the melodies of seed 42: a library change that alters them would silently change every shared dictation link.</summary>
public class DictationGoldenTests
{
    private static string Actual() => string.Join('\n', Enum.GetValues<Difficulty>().Select(d =>
    {
        var drill = new MelodyDrill(MelodyDrillOptions.For(d), 42);
        var melodies = Enumerable.Range(0, 3).Select(_ => string.Join(' ', drill.Next().Answer.Select(n => n.ToString())));
        return $"{d.ToString().ToLowerInvariant()}: {string.Join(" | ", melodies)}";
    })) + "\n";

    [Fact]
    public void The_first_three_melodies_of_seed_42_match_the_golden_file()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (!File.Exists(Path.Combine(dir.FullName, "MusicTheory.sln"))) dir = dir.Parent!;
        var path = Path.Combine(dir.FullName, "MusicTheory.Toolbox.UnitTests", "Golden", "dictation-seed42.txt");
        if (Environment.GetEnvironmentVariable("UPDATE_GOLDEN") == "1") File.WriteAllText(path, Actual());
        Actual().ShouldBe(File.ReadAllText(path).ReplaceLineEndings("\n")); // git may check the file out with CRLF on Windows
    }
}
