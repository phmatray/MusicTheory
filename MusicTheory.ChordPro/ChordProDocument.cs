namespace MusicTheory.ChordPro;

/// <summary>A parsed ChordPro chord sheet. <see cref="ToString"/> reproduces the parsed text byte for byte.</summary>
/// <param name="Lines">The lines of the document.</param>
public sealed record ChordProDocument(IReadOnlyList<ChordProLine> Lines)
{
    /// <summary>The <c>{title}</c> (or <c>{t}</c>) value.</summary>
    public string? Title => Directive("title");

    /// <summary>The <c>{subtitle}</c> (or <c>{st}</c>) value.</summary>
    public string? Subtitle => Directive("subtitle");

    /// <summary>The <c>{key}</c> value, or <see langword="null"/> when absent or not a key MusicTheory can parse.</summary>
    public KeySignature? Key => KeySignature.TryParse(Directive("key"), out var key) ? key : null;

    /// <summary>The <c>{capo}</c> value, or <see langword="null"/> when absent or not a number.</summary>
    public int? Capo => int.TryParse(Directive("capo"), out var capo) ? capo : null;

    /// <summary>The distinct chord texts, in first-appearance order.</summary>
    public IReadOnlyList<string> Chords =>
        Lines.OfType<LyricLine>().SelectMany(l => l.Segments).Select(s => s.Chord).OfType<string>().Distinct().ToList();

    /// <summary>Parses a ChordPro document. Never throws on content: a line that is nothing else is a lyric line.</summary>
    /// <param name="text">The document text, split on <c>\n</c>.</param>
    public static ChordProDocument Parse(string text)
    {
        var lines = new List<ChordProLine>();
        var inTab = false;
        foreach (var raw in text.Split('\n'))
        {
            var content = raw.TrimEnd('\r');
            var trimmed = content.Trim();
            ChordProLine line;
            if (trimmed.StartsWith('#'))
            {
                line = new CommentLine(raw);
            }
            else if (trimmed.StartsWith('{') && trimmed.EndsWith('}'))
            {
                var directive = ParseDirective(raw, trimmed[1..^1].Trim());
                inTab = directive.Name == "start_of_tab" || (inTab && directive.Name != "end_of_tab");
                line = directive;
            }
            else
            {
                line = new LyricLine(raw, inTab ? [new ChordSegment(null, content)] : Segments(content));
            }

            lines.Add(line);
        }

        return new ChordProDocument(lines);
    }

    /// <summary>The document text, with <c>\n</c> between lines.</summary>
    public override string ToString() => string.Join('\n', Lines.Select(l => l.Raw));

    private string? Directive(string name) =>
        Lines.OfType<DirectiveLine>().FirstOrDefault(d => d.Name == name)?.Value;

    private static DirectiveLine ParseDirective(string raw, string inner)
    {
        var end = inner.IndexOfAny([':', ' ', '\t']);
        var name = end < 0 ? inner : inner[..end];
        var value = end < 0 ? "" : inner[end..].TrimStart(' ', '\t', ':');
        return new DirectiveLine(raw, LongName(name.ToLowerInvariant()), value.Length == 0 ? null : value);
    }

    // A method, not a static readonly table: Stryker cannot mutate static initialisers.
    private static string LongName(string name)
    {
        string[] pairs =
        [
            "t", "title", "st", "subtitle", "c", "comment",
            "soc", "start_of_chorus", "eoc", "end_of_chorus", "sov", "start_of_verse", "eov", "end_of_verse",
            "sob", "start_of_bridge", "eob", "end_of_bridge", "sot", "start_of_tab", "eot", "end_of_tab",
        ];
        var i = Array.IndexOf(pairs, name);
        return i is >= 0 && i % 2 == 0 ? pairs[i + 1] : name;
    }

    private static List<ChordSegment> Segments(string s)
    {
        var segments = new List<ChordSegment>();
        string? chord = null;
        var pos = 0;
        while (true)
        {
            var open = s.IndexOf('[', pos);
            var close = open < 0 ? -1 : s.IndexOf(']', open + 1);
            if (close < 0)
            {
                if (chord is not null || pos < s.Length)
                {
                    segments.Add(new ChordSegment(chord, s[pos..]));
                }

                return segments;
            }

            if (chord is not null || open > pos)
            {
                segments.Add(new ChordSegment(chord, s[pos..open]));
            }

            chord = s[(open + 1)..close];
            pos = close + 1;
        }
    }
}
