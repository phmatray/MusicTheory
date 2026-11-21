namespace MusicTheory.MusicXml;

/// <summary>A single-part score read from MusicXML.</summary>
/// <param name="Title">The work title, or <see langword="null"/> when the file has none.</param>
/// <param name="Key">The key of the first measure, or <see langword="null"/> when the file has none.</param>
/// <param name="TimeSignature">The meter of the first measure.</param>
/// <param name="Measures">The measures of the first part, in order. Each carries the meter in force.</param>
public sealed record MusicXmlScore(string? Title, KeySignature? Key, TimeSignature TimeSignature,
    IReadOnlyList<Measure> Measures);
