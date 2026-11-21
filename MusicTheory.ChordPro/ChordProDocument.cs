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

    /// <summary>Transposes every chord and the <c>{key}</c> directive by <paramref name="semitones"/>. Chords are spelled in the new key when <see cref="Key"/> is known.</summary>
    /// <param name="semitones">Semitones up (positive) or down (negative).</param>
    /// <param name="suffix">With <see cref="SuffixHandling.KeepUnparsed"/>, bracket text that is not a chord but starts with a root moves its root and bass too.</param>
    public ChordProDocument Transpose(int semitones, SuffixHandling suffix = SuffixHandling.Canonical)
    {
        if (semitones == 0)
            return this;
        var target = Key?.TransposeBySemitones(semitones);
        return Map(c => ChordTransposer.Transpose(c, semitones, target, suffix: suffix), target);
    }

    /// <summary>Transposes from <see cref="Key"/> to <paramref name="to"/>, spelling every chord in <paramref name="to"/>.</summary>
    /// <param name="to">The target key.</param>
    /// <param name="suffix">See <see cref="Transpose(int, SuffixHandling)"/>.</param>
    /// <exception cref="InvalidOperationException">The document has no parseable <c>{key}</c>.</exception>
    public ChordProDocument Transpose(KeySignature to, SuffixHandling suffix = SuffixHandling.Canonical)
    {
        var from = Key ?? throw new InvalidOperationException("Transposing to a key needs a {key:} directive the document can parse.");
        return Map(c => ChordTransposer.Transpose(c, from, to, suffix: suffix), to);
    }

    // Rebuilds only the lines that change; annotations ([*Riff]) are not chords.
    private ChordProDocument Map(Func<string, string> chord, KeySignature? newKey) =>
        new(Lines.Select(line => line switch
        {
            LyricLine l => MapLyric(l, chord),
            DirectiveLine { Name: "key" } d when newKey is not null => d with { Raw = $"{{key: {newKey}}}" + Tail(d.Raw), Value = newKey.ToString() },
            _ => line,
        }).ToList());

    private static ChordProLine MapLyric(LyricLine line, Func<string, string> chord)
    {
        var segments = line.Segments.Select(s => s.Chord is null || s.Chord.StartsWith('*') ? s : s with { Chord = chord(s.Chord) }).ToList();
        if (segments.SequenceEqual(line.Segments))
        {
            return line;
        }

        var text = string.Concat(segments.Select(s => s.Chord is null ? s.Lyric : $"[{s.Chord}]{s.Lyric}"));
        return new LyricLine(text + Tail(line.Raw), segments);
    }

    private static string Tail(string raw) => raw[raw.TrimEnd('\r').Length..]; // the trailing \r of a CRLF line

    private string? Directive(string name) =>
        Lines.OfType<DirectiveLine>().FirstOrDefault(d => d.Name == name)?.Value;

    private static DirectiveLine ParseDirective(string raw, string inner)
    {
        var end = inner.IndexOfAny([':', ' ', '\t']);
        var name = end < 0 ? inner : inner[..end];
        var rest = end < 0 ? "" : inner[end..].TrimStart(' ', '\t');
        var value = (rest.StartsWith(':') ? rest[1..] : rest).Trim(); // only the first separator colon is dropped
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
            if (close >= 0)
            {
                open = s.LastIndexOf('[', close, close - pos + 1); // the nearest '[': an earlier unclosed one is lyric text
            }

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
