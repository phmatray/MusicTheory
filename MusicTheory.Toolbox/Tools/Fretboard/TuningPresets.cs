using MusicTheory.Guitar;

namespace MusicTheory.Toolbox.Tools.Fretboard;

/// <summary>A library tuning with its URL id and its English and French labels.</summary>
public sealed record TuningPreset(string Id, Tuning Tuning, string En, string Fr);

public static class TuningPresets
{
    // Display names are partly proper names (Drop D, DADGAD) and partly words, so the labels live here.
    public static IReadOnlyList<TuningPreset> All { get; } =
    [
        new("standard", Tuning.Standard, "Standard", "Standard"),
        new("dropd", Tuning.DropD, "Drop D", "Drop D"),
        new("dadgad", Tuning.DADGAD, "DADGAD", "DADGAD"),
        new("openg", Tuning.OpenG, "Open G", "Sol ouvert"),
        new("halfstepdown", Tuning.HalfStepDown, "Half step down", "Demi-ton plus bas"),
        new("ukulele", Tuning.Ukulele, "Ukulele", "Ukulélé"),
        new("ukulelelowg", Tuning.UkuleleLowG, "Ukulele (low G)", "Ukulélé (sol grave)"),
        new("bass", Tuning.Bass, "Bass", "Basse"),
        new("mandolin", Tuning.Mandolin, "Mandolin", "Mandoline"),
        new("sevenstring", Tuning.SevenString, "7-string", "7 cordes"),
    ];

    public static TuningPreset? Find(string? id) => All.FirstOrDefault(p => p.Id == id?.ToLowerInvariant());
}
