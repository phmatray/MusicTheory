namespace MusicTheory.MusicXml;

/// <summary>A single-part score read from MusicXML.</summary>
/// <param name="Title">The work title, or <see langword="null"/> when the file has none.</param>
/// <param name="Key">The key of the first measure, or <see langword="null"/> when the file has none.</param>
/// <param name="TimeSignature">The meter of the first measure.</param>
/// <param name="Measures">The measures of the first part, in order. Each carries the meter in force.</param>
public sealed record MusicXmlScore(string? Title, KeySignature? Key, TimeSignature TimeSignature,
    IReadOnlyList<Measure> Measures)
{
    /// <summary>Two scores are equal when their header fields and measures are equal.</summary>
    public bool Equals(MusicXmlScore? other) =>
        other is not null && Title == other.Title && Key == other.Key && TimeSignature == other.TimeSignature
        && Measures.SequenceEqual(other.Measures);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Title);
        hash.Add(Key);
        hash.Add(TimeSignature);
        foreach (var m in Measures)
            hash.Add(m);
        return hash.ToHashCode();
    }
}
