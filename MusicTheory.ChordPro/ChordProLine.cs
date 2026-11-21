namespace MusicTheory.ChordPro;

/// <summary>One line of a ChordPro document. <see cref="Raw"/> is the line as written, without its <c>\n</c>.</summary>
/// <param name="Raw">The line text verbatim, a trailing <c>\r</c> included.</param>
public abstract record ChordProLine(string Raw);

/// <summary>A <c>{name}</c>, <c>{name: value}</c> or <c>{name value}</c> directive.</summary>
/// <param name="Raw">The line text verbatim.</param>
/// <param name="Name">The directive name in long form (<c>t</c> becomes <c>title</c>, <c>soc</c> becomes <c>start_of_chorus</c>).</param>
/// <param name="Value">The trimmed value, or <see langword="null"/> when there is none.</param>
public sealed record DirectiveLine(string Raw, string Name, string? Value) : ChordProLine(Raw);

/// <summary>A lyric line split into chord segments. Also the fallback for any line that is nothing else.</summary>
/// <param name="Raw">The line text verbatim.</param>
/// <param name="Segments">The text split at each <c>[chord]</c>.</param>
public sealed record LyricLine(string Raw, IReadOnlyList<ChordSegment> Segments) : ChordProLine(Raw);

/// <summary>A line starting with <c>#</c>.</summary>
/// <param name="Raw">The line text verbatim.</param>
public sealed record CommentLine(string Raw) : ChordProLine(Raw);

/// <summary>A chord and the lyric that follows it up to the next chord.</summary>
/// <param name="Chord">The text inside <c>[ ]</c>, verbatim, or <see langword="null"/> for text before the first chord.</param>
/// <param name="Lyric">The lyric text.</param>
public sealed record ChordSegment(string? Chord, string Lyric);
