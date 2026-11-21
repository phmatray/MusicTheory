namespace MusicTheory.Midi;

/// <summary>What <see cref="MidiReader"/> recovers from a Standard MIDI File.</summary>
/// <param name="TimeSignature">The first time signature event, or 4/4 when the file has none.</param>
/// <param name="Key">The first key signature event, or null when the file has none.</param>
/// <param name="Tempo">Every Set Tempo event, in quarter-note beats; 120 bpm from the start when the file has none.</param>
/// <param name="Measures">The notes as measures of <paramref name="TimeSignature"/>, ties restored across barlines.</param>
public sealed record MidiImport(TimeSignature TimeSignature, KeySignature? Key, TempoMap Tempo,
    IReadOnlyList<Measure> Measures);
