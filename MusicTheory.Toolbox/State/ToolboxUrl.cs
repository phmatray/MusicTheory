using MusicTheory.Toolbox.I18n;

namespace MusicTheory.Toolbox.State;

/// <summary>Everything a shared link carries: the tool, the language and the tool's own parameters.</summary>
public sealed record ToolState(string Tool, Lang Lang, IReadOnlyDictionary<string, string> Params);

/// <summary>Reads and writes the query string that holds the whole app state (<c>?t=&amp;lang=&amp;v=1&amp;...</c>).</summary>
public static class ToolboxUrl
{
    /// <summary>Parses any URI; never throws. A missing or unknown <c>lang</c> follows <paramref name="browserLang"/>.</summary>
    public static ToolState Parse(Uri uri, string? browserLang)
    {
        var tool = "";
        Lang? lang = null;
        var rest = new SortedDictionary<string, string>(StringComparer.Ordinal);
        var text = uri.OriginalString;
        var q = text.IndexOf('?');
        var hash = text.IndexOf('#');
        if (hash >= 0 && hash < q) q = -1; // a '?' inside the fragment is not a query
        var query = q < 0 ? "" : text[(q + 1)..(hash > q ? hash : text.Length)];
        foreach (var pair in query.Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var eq = pair.IndexOf('=');
            var key = Decode(eq < 0 ? pair : pair[..eq]).ToLowerInvariant();
            var value = eq < 0 ? "" : Decode(pair[(eq + 1)..]);
            if (key.Length == 0) continue;
            switch (key)
            {
                case "t": tool = value.ToLowerInvariant(); break;
                case "lang": lang = ParseLang(value, exact: true) ?? lang; break;
                case "v": break; // only version 1 exists: every value reads as 1
                default: rest[key] = value; break;
            }
        }
        return new ToolState(tool, lang ?? ParseLang(browserLang, exact: false) ?? Lang.En, rest);
    }

    /// <summary>The canonical query string, leading <c>?</c> included: <c>t</c>, <c>lang</c>, <c>v=1</c>, then the other keys sorted.</summary>
    public static string Build(ToolState state)
    {
        var parts = new List<string>();
        if (state.Tool.Length > 0) parts.Add("t=" + Uri.EscapeDataString(state.Tool));
        parts.Add("lang=" + (state.Lang == Lang.Fr ? "fr" : "en"));
        parts.Add("v=1");
        foreach (var (k, v) in state.Params.OrderBy(p => p.Key, StringComparer.Ordinal))
            parts.Add(Uri.EscapeDataString(k) + "=" + Uri.EscapeDataString(v));
        return "?" + string.Join('&', parts);
    }

    // The link says exactly fr or en; a browser says fr-BE, en-US...
    private static Lang? ParseLang(string? s, bool exact)
    {
        if (s is null) return null;
        var code = exact ? s : s.Split('-', '_')[0];
        return code.Equals("fr", StringComparison.OrdinalIgnoreCase) ? Lang.Fr
            : code.Equals("en", StringComparison.OrdinalIgnoreCase) ? Lang.En
            : null;
    }

    private static string Decode(string s)
    {
        try { return Uri.UnescapeDataString(s.Replace('+', ' ')).Trim(); }
        catch (UriFormatException) { return s.Trim(); }
    }
}
