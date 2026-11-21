using MusicTheory.Guitar;
using MusicTheory.Toolbox.I18n;

namespace MusicTheory.Toolbox.Tools.Fretboard;

/// <summary>One dot: string index as listed in the tuning (0 = first listed), fret, note label, scale or chord degree.</summary>
public sealed record FretDot(int String, int Fret, string Label, int? Degree, bool IsRoot);

/// <summary>Everything the neck needs: open-string names, fret count, dots, and a notice when an input fell back.</summary>
public sealed record FretboardView(IReadOnlyList<string> Strings, int Frets, IReadOnlyList<FretDot> Dots, string? Notice);

public static class FretboardModel
{
    public const int MaxFrets = 24;
    public const int DefaultFrets = 12;

    public static readonly IReadOnlyList<ScaleType> ScaleTypes =
    [
        ScaleType.Major, ScaleType.NaturalMinor, ScaleType.HarmonicMinor, ScaleType.MelodicMinor, ScaleType.Dorian,
        ScaleType.Phrygian, ScaleType.Lydian, ScaleType.Mixolydian, ScaleType.Locrian,
        ScaleType.PentatonicMajor, ScaleType.PentatonicMinor, ScaleType.Blues,
    ];

    public static readonly IReadOnlyList<ChordType> ChordTypeList =
    [
        ChordType.Major, ChordType.Minor, ChordType.Diminished, ChordType.Augmented, ChordType.Major7,
        ChordType.Minor7, ChordType.Dominant7, ChordType.Sus2, ChordType.Sus4,
    ];

    /// <summary>Builds the view. Anything invalid falls back (Standard, 12 frets, C, first type, show=note) and sets the notice; empty means default, silently.</summary>
    public static FretboardView Build(string? tuningId, int frets, string? show, string? root, string? type, Lang lang)
    {
        var bad = new List<string>();
        var preset = TuningPresets.Find(tuningId);
        if (preset is null) { preset = TuningPresets.All[0]; if (!string.IsNullOrEmpty(tuningId)) bad.Add("tuning"); }
        if (frets < 1 || frets > MaxFrets) { frets = DefaultFrets; bad.Add("frets"); }
        var mode = show?.ToLowerInvariant();
        if (mode is not ("note" or "scale" or "chord")) { if (!string.IsNullOrEmpty(show)) bad.Add("show"); mode = "note"; }
        var tonic = Note.TryParse(root + "4", out var parsed) ? parsed : Note.Parse("C4");
        if (parsed is null && !string.IsNullOrEmpty(root)) bad.Add("root");

        var board = new Guitar.Fretboard(preset.Tuning, frets);
        IReadOnlyList<FretPosition> positions;
        if (mode == "scale")
            positions = board.Positions(new Scale(tonic, Pick(ScaleTypes, type, bad)));
        else if (mode == "chord")
            positions = board.Positions(new Chord(tonic, Pick(ChordTypeList, type, bad)));
        else
            positions = board.Positions(tonic);

        var dots = positions
            .Select(p => new FretDot(p.String, p.Fret, NoteNames.Format(p.Note, lang), p.Degree, p.Degree is null or 1))
            .ToList();
        var strings = preset.Tuning.Strings.Select(n => NoteNames.Format(n, lang)).ToList();
        var notice = bad.Count == 0 ? null : Strings.T(lang, "fretboard.invalid", string.Join(", ", bad));
        return new FretboardView(strings, frets, dots, notice);
    }

    /// <summary>The type ids valid for a mode (scale or chord), in display order.</summary>
    public static IReadOnlyList<string> TypeIds(string mode) =>
        mode == "scale" ? ScaleTypes.Select(t => t.ToString()).ToList()
        : mode == "chord" ? ChordTypeList.Select(t => t.ToString()).ToList()
        : [];

    private static T Pick<T>(IReadOnlyList<T> allowed, string? type, List<string> bad) where T : struct, Enum
    {
        foreach (var a in allowed)
            if (string.Equals(a.ToString(), type, StringComparison.OrdinalIgnoreCase)) return a;
        if (!string.IsNullOrEmpty(type)) bad.Add("type");
        return allowed[0];
    }
}
