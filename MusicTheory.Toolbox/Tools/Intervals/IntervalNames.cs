using MusicTheory.Toolbox.I18n;

namespace MusicTheory.Toolbox.Tools.Intervals;

/// <summary>FR and EN label of an interval, keyed by its library name (<c>m3</c>, <c>A4</c>...).</summary>
public static class IntervalNames
{
    private static readonly Dictionary<string, (string En, string Fr)> Names = new()
    {
        ["m2"] = ("minor second", "seconde mineure"),
        ["M2"] = ("major second", "seconde majeure"),
        ["m3"] = ("minor third", "tierce mineure"),
        ["M3"] = ("major third", "tierce majeure"),
        ["P4"] = ("perfect fourth", "quarte juste"),
        ["A4"] = ("augmented fourth", "quarte augmentée"),
        ["P5"] = ("perfect fifth", "quinte juste"),
        ["m6"] = ("minor sixth", "sixte mineure"),
        ["M6"] = ("major sixth", "sixte majeure"),
        ["m7"] = ("minor seventh", "septième mineure"),
        ["M7"] = ("major seventh", "septième majeure"),
        ["P8"] = ("octave", "octave"),
    };

    /// <summary>The label, or the bare library name when the interval has no label (a test keeps that from happening).</summary>
    public static string Label(Interval interval, Lang lang) =>
        Names.TryGetValue(interval.Name, out var n) ? (lang == Lang.Fr ? n.Fr : n.En) : interval.Name;
}
