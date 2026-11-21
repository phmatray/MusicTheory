namespace MusicTheory;

/// <summary>What <see cref="ChordTransposer"/> does with the suffix text of a chord.</summary>
public enum SuffixHandling
{
    /// <summary>Today's behaviour: parseable chords are rewritten canonically, anything else is returned unchanged.</summary>
    Canonical,

    /// <summary>
    /// Parseable chords are rewritten canonically; text that is not a chord symbol but starts with a root
    /// (<c>Cfoo/E</c>) moves its root and bass and keeps the rest as written.
    /// </summary>
    KeepUnparsed,

    /// <summary>
    /// Root and bass move, the suffix text is kept exactly as written, even when the chord parses
    /// (<c>C-7</c> up 2 is <c>D-7</c>, <c>CM7</c> is <c>DM7</c>, <c>CΔ7</c> is <c>DΔ7</c>).
    /// </summary>
    Verbatim,
}
