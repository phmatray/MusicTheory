using System.Globalization;
using MusicTheory.Toolbox.Tools.Circle;
using MusicTheory.Toolbox.Tools.Finder;
using MusicTheory.Toolbox.Tools.Intervals;
using MusicTheory.Toolbox.Tools.Transposer;

namespace MusicTheory.Toolbox.I18n;

/// <summary>Looks keys up across every registered table.</summary>
public static class Strings
{
    /// <summary>Every table; a new tool adds its own here.</summary>
    public static IReadOnlyList<IStringTable> Tables { get; } = [new ShellStrings(), new CircleStrings(), new FinderStrings(), new TransposerStrings(), new IntervalStrings()];

    /// <summary>The text for <paramref name="key"/>, or <c>[key]</c> so a gap is visible (the parity test fails first).</summary>
    public static string T(Lang lang, string key)
    {
        foreach (var table in Tables)
            if ((lang == Lang.Fr ? table.Fr : table.En).TryGetValue(key, out var text))
                return text;
        return $"[{key}]";
    }

    /// <summary>The text with <c>{0}</c>... filled from <paramref name="args"/>.</summary>
    public static string T(Lang lang, string key, params object[] args) =>
        string.Format(CultureInfo.InvariantCulture, T(lang, key), args);
}
