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
}
