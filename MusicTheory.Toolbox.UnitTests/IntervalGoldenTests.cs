using MusicTheory.Training;

namespace MusicTheory.Toolbox.UnitTests;

/// <summary>Pins the draw order: a library change that alters the questions of seed 42 would silently change every shared link.</summary>
public class IntervalGoldenTests
{
    private static string Actual() => string.Join('\n', Enum.GetValues<Difficulty>().Select(d =>
    {
        var drill = new IntervalDrill(IntervalDrillOptions.For(d), 42);
        var answers = Enumerable.Range(0, 10).Select(_ => drill.Next().Answer.Name);
        return $"{d.ToString().ToLowerInvariant()}: {string.Join(' ', answers)}";
    })) + "\n";

    [Fact]
    public void The_first_ten_answers_of_seed_42_match_the_golden_file()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (!File.Exists(Path.Combine(dir.FullName, "MusicTheory.sln"))) dir = dir.Parent!;
        var path = Path.Combine(dir.FullName, "MusicTheory.Toolbox.UnitTests", "Golden", "intervals-seed42.txt");
        if (Environment.GetEnvironmentVariable("UPDATE_GOLDEN") == "1") File.WriteAllText(path, Actual());
        Actual().ShouldBe(File.ReadAllText(path));
    }
}
