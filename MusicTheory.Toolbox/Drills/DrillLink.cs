using System.Globalization;
using MusicTheory.Toolbox.I18n;
using MusicTheory.Toolbox.State;
using MusicTheory.Training;

namespace MusicTheory.Toolbox.Drills;

/// <summary>
/// The shareable identity of a drill: <c>?t=&lt;tool&gt;&amp;d=&amp;seed=&amp;n=&amp;key=1</c>. Never throws on bad input:
/// an unknown difficulty reads as beginner, a count is clamped to 1..50 and a missing or invalid seed is <c>null</c>
/// (the page then offers a Generate button).
/// </summary>
public sealed record DrillLink(string Tool, Difficulty Difficulty, ulong? Seed, int Count, bool TeacherKey)
{
    public const int DefaultCount = 10;
    public const int MaxCount = 50;

    public static DrillLink FromState(ToolState state)
    {
        string Get(string k) => state.Params.TryGetValue(k, out var v) ? v : "";
        var difficulty = Get("d").ToLowerInvariant() switch
        {
            "intermediate" => Difficulty.Intermediate,
            "advanced" => Difficulty.Advanced,
            _ => Difficulty.Beginner,
        };
        ulong? seed = ulong.TryParse(Get("seed"), NumberStyles.None, CultureInfo.InvariantCulture, out var s) ? s : null;
        // A huge count overflows int.TryParse: that is invalid input too, so it falls back to the default.
        var count = int.TryParse(Get("n"), NumberStyles.None, CultureInfo.InvariantCulture, out var n)
            ? Math.Clamp(n, 1, MaxCount)
            : DefaultCount;
        return new DrillLink(state.Tool, difficulty, seed, count, Get("key") == "1");
    }

    public ToolState ToState(Lang lang)
    {
        var p = new Dictionary<string, string>
        {
            ["d"] = Difficulty.ToString().ToLowerInvariant(),
            ["n"] = Count.ToString(CultureInfo.InvariantCulture),
        };
        if (Seed is { } s) p["seed"] = s.ToString(CultureInfo.InvariantCulture);
        if (TeacherKey) p["key"] = "1";
        return new ToolState(Tool, lang, p);
    }
}
