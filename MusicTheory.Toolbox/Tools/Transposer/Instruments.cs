namespace MusicTheory.Toolbox.Tools.Transposer;

/// <summary>A transposing instrument: its written part sits <paramref name="WrittenAboveConcertSemitones"/> above the sounding (concert) pitch.</summary>
public sealed record Instrument(string Id, string NameKey, int WrittenAboveConcertSemitones);

public static class Instruments
{
    public static IReadOnlyList<Instrument> All { get; } =
    [
        new("c", "transpose.inst.c", 0),
        new("bb", "transpose.inst.bb", 2),    // trumpet, clarinet, soprano sax: major second
        new("eb", "transpose.inst.eb", 9),    // alto, baritone sax: major sixth
        new("f", "transpose.inst.f", 7),      // horn: perfect fifth
        new("bbt", "transpose.inst.bbt", 14), // tenor sax: major ninth, same pitch class as Bb
    ];

    public static Instrument Find(string? id) => All.FirstOrDefault(i => i.Id == id) ?? All[1];
}
