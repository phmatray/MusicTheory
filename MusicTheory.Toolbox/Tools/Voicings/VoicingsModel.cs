using MusicTheory.Guitar;
using MusicTheory.Toolbox.I18n;
using MusicTheory.Toolbox.Tools.Fretboard;

namespace MusicTheory.Toolbox.Tools.Voicings;

/// <summary>One voicing ready to draw: the library voicing, its diagram geometry, its tab and a localised shape name.</summary>
public sealed record VoicingCard(GuitarVoicing Voicing, ChordDiagramLayout Layout, string Tab, string Shape);

/// <summary>The cards for a chord and tuning, or an error (the library's message) when the chord does not parse.</summary>
public sealed record VoicingsView(string ChordText, TuningPreset Tuning, IReadOnlyList<VoicingCard> Cards, string? Error, string? Notice);

public static class VoicingsModel
{
    public const string DefaultChord = "C";

    /// <summary>Empty chord means <see cref="DefaultChord"/> and an unknown tuning means Standard (the latter with a notice).</summary>
    public static VoicingsView For(string? chordText, string? tuningId, Lang lang)
    {
        var text = string.IsNullOrWhiteSpace(chordText) ? DefaultChord : chordText.Trim();
        var preset = TuningPresets.Find(tuningId);
        string? notice = null;
        if (preset is null)
        {
            preset = TuningPresets.All[0];
            if (!string.IsNullOrEmpty(tuningId)) notice = Strings.T(lang, "voicings.badtuning");
        }

        ChordSymbol chord;
        try { chord = ChordSymbol.Parse(text); }
        catch (FormatException e) { return new VoicingsView(text, preset, [], e.Message, notice); }

        IReadOnlyList<GuitarVoicing> voicings;
        try { voicings = new GuitarVoicingProvider().GetVoicings(chord, new VoicingOptions(Tuning: preset.Tuning)); }
        catch (Exception e) when (e is InvalidOperationException or ArgumentException)
        {
            // A chord the library cannot spell or place on this tuning: the empty state, not a crash.
            voicings = [];
        }

        var cards = voicings
            .Select(v => new VoicingCard(v, ChordDiagramLayout.For(v, tuning: preset.Tuning), v.ToTab(), ShapeLabel(v, lang)))
            .ToList();
        return new VoicingsView(text, preset, cards, null, notice);
    }

    private static string ShapeLabel(GuitarVoicing v, Lang lang) => v.ShapeName switch
    {
        "open" => Strings.T(lang, "voicings.shape.open"),
        "E-form" or "A-form" or "D-form" => Strings.T(lang, "voicings.shape.movable", v.ShapeName[0]),
        _ => Strings.T(lang, "voicings.shape.generated"),
    };
}
