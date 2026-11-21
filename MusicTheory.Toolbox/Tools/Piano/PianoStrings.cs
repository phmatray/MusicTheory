using MusicTheory.Toolbox.I18n;

namespace MusicTheory.Toolbox.Tools.Piano;

public sealed class PianoStrings : IStringTable
{
    public IReadOnlyDictionary<string, string> En { get; } = new Dictionary<string, string>
    {
        ["piano.title"] = "Piano fingering",
        ["piano.blurb"] = "The standard fingering of a scale or a triad, hand by hand, on the keyboard.",
        ["piano.root"] = "Root",
        ["piano.type"] = "Scale",
        ["piano.hand"] = "Hand",
        ["piano.hand.rh"] = "Right hand",
        ["piano.hand.lh"] = "Left hand",
        ["piano.oct"] = "Octaves",
        ["piano.triad"] = "Triad only",
        ["piano.keyboard"] = "Keyboard with {0} marked keys",
        ["piano.invalid"] = "Unknown value for: {0}. A default is shown instead.",
        ["piano.scale.Major"] = "Major",
        ["piano.scale.NaturalMinor"] = "Natural minor",
        ["piano.scale.HarmonicMinor"] = "Harmonic minor",
        ["piano.scale.MelodicMinor"] = "Melodic minor",
        ["piano.scale.Dorian"] = "Dorian",
        ["piano.scale.Phrygian"] = "Phrygian",
        ["piano.scale.Lydian"] = "Lydian",
        ["piano.scale.Mixolydian"] = "Mixolydian",
        ["piano.scale.Locrian"] = "Locrian",
        ["piano.understand"] = "Piano fingering numbers the fingers from 1 (thumb) to 5 (little finger). The thumb passes under the hand, or the hand over the thumb, "
            + "so that the long fingers land on black keys and the hand never has to leap. The library picks the standard fingering for each scale and hand. "
            + "Each marked key shows its finger number and its note name. For a triad, the three notes are the first, third and fifth of the scale.",
    };

    public IReadOnlyDictionary<string, string> Fr { get; } = new Dictionary<string, string>
    {
        ["piano.title"] = "Doigté au piano",
        ["piano.blurb"] = "Le doigté standard d'une gamme ou d'un accord de trois sons, main par main, sur le clavier.",
        ["piano.root"] = "Fondamentale",
        ["piano.type"] = "Gamme",
        ["piano.hand"] = "Main",
        ["piano.hand.rh"] = "Main droite",
        ["piano.hand.lh"] = "Main gauche",
        ["piano.oct"] = "Octaves",
        ["piano.triad"] = "Accord de trois sons seulement",
        ["piano.keyboard"] = "Clavier avec {0} touches marquées",
        ["piano.invalid"] = "Valeur inconnue pour : {0}. Une valeur par défaut est affichée.",
        ["piano.scale.Major"] = "Majeure",
        ["piano.scale.NaturalMinor"] = "Mineure naturelle",
        ["piano.scale.HarmonicMinor"] = "Mineure harmonique",
        ["piano.scale.MelodicMinor"] = "Mineure mélodique",
        ["piano.scale.Dorian"] = "Dorien",
        ["piano.scale.Phrygian"] = "Phrygien",
        ["piano.scale.Lydian"] = "Lydien",
        ["piano.scale.Mixolydian"] = "Mixolydien",
        ["piano.scale.Locrian"] = "Locrien",
        ["piano.understand"] = "Le doigté numérote les doigts de 1 (pouce) à 5 (auriculaire). Le pouce passe sous la main, ou la main passe par-dessus le pouce, "
            + "pour que les doigts longs tombent sur les touches noires et que la main ne saute jamais. La bibliothèque choisit le doigté standard de chaque gamme et de chaque main. "
            + "Chaque touche marquée montre son numéro de doigt et le nom de la note. Pour un accord de trois sons, les trois notes sont la première, la tierce et la quinte de la gamme.",
    };
}
