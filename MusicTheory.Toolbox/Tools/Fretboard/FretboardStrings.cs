using MusicTheory.Toolbox.I18n;

namespace MusicTheory.Toolbox.Tools.Fretboard;

public sealed class FretboardStrings : IStringTable
{
    public IReadOnlyDictionary<string, string> En { get; } = new Dictionary<string, string>
    {
        ["fretboard.title"] = "Fretboard",
        ["fretboard.blurb"] = "See where a note, a scale or a chord lives on the neck, in your tuning.",
        ["fretboard.tuning"] = "Tuning",
        ["fretboard.frets"] = "Frets",
        ["fretboard.show"] = "Show",
        ["fretboard.show.note"] = "A note",
        ["fretboard.show.scale"] = "A scale",
        ["fretboard.show.chord"] = "A chord",
        ["fretboard.root"] = "Root",
        ["fretboard.type"] = "Type",
        ["fretboard.neck"] = "Fretboard with {0} strings and {1} frets",
        ["fretboard.degree"] = "degree {0}",
        ["fretboard.invalid"] = "Unknown value for: {0}. A default is shown instead.",
        ["fretboard.scale.Major"] = "Major",
        ["fretboard.scale.NaturalMinor"] = "Natural minor",
        ["fretboard.scale.HarmonicMinor"] = "Harmonic minor",
        ["fretboard.scale.MelodicMinor"] = "Melodic minor",
        ["fretboard.scale.Dorian"] = "Dorian",
        ["fretboard.scale.Phrygian"] = "Phrygian",
        ["fretboard.scale.Lydian"] = "Lydian",
        ["fretboard.scale.Mixolydian"] = "Mixolydian",
        ["fretboard.scale.Locrian"] = "Locrian",
        ["fretboard.scale.PentatonicMajor"] = "Major pentatonic",
        ["fretboard.scale.PentatonicMinor"] = "Minor pentatonic",
        ["fretboard.scale.Blues"] = "Blues",
        ["fretboard.chord.Major"] = "Major",
        ["fretboard.chord.Minor"] = "Minor",
        ["fretboard.chord.Diminished"] = "Diminished",
        ["fretboard.chord.Augmented"] = "Augmented",
        ["fretboard.chord.Major7"] = "Major 7th",
        ["fretboard.chord.Minor7"] = "Minor 7th",
        ["fretboard.chord.Dominant7"] = "Dominant 7th",
        ["fretboard.chord.Sus2"] = "Sus2",
        ["fretboard.chord.Sus4"] = "Sus4",
        ["fretboard.understand"] = "A tuning is the note each open string plays. The fretboard map lists every fret where a note, a scale tone or a chord tone sounds, "
            + "and the library computes it from the open strings. Filled dots are roots; other dots are tones of the scale or chord. "
            + "Hover a dot to read its degree, the position of the tone in the scale or chord. Frets are evenly spaced here, unlike a real neck.",
    };

    public IReadOnlyDictionary<string, string> Fr { get; } = new Dictionary<string, string>
    {
        ["fretboard.title"] = "Manche",
        ["fretboard.blurb"] = "Voyez où se trouve une note, une gamme ou un accord sur le manche, dans votre accordage.",
        ["fretboard.tuning"] = "Accordage",
        ["fretboard.frets"] = "Cases",
        ["fretboard.show"] = "Afficher",
        ["fretboard.show.note"] = "Une note",
        ["fretboard.show.scale"] = "Une gamme",
        ["fretboard.show.chord"] = "Un accord",
        ["fretboard.root"] = "Fondamentale",
        ["fretboard.type"] = "Type",
        ["fretboard.neck"] = "Manche de {0} cordes et {1} cases",
        ["fretboard.degree"] = "degré {0}",
        ["fretboard.invalid"] = "Valeur inconnue pour : {0}. Une valeur par défaut est affichée.",
        ["fretboard.scale.Major"] = "Majeure",
        ["fretboard.scale.NaturalMinor"] = "Mineure naturelle",
        ["fretboard.scale.HarmonicMinor"] = "Mineure harmonique",
        ["fretboard.scale.MelodicMinor"] = "Mineure mélodique",
        ["fretboard.scale.Dorian"] = "Dorien",
        ["fretboard.scale.Phrygian"] = "Phrygien",
        ["fretboard.scale.Lydian"] = "Lydien",
        ["fretboard.scale.Mixolydian"] = "Mixolydien",
        ["fretboard.scale.Locrian"] = "Locrien",
        ["fretboard.scale.PentatonicMajor"] = "Pentatonique majeure",
        ["fretboard.scale.PentatonicMinor"] = "Pentatonique mineure",
        ["fretboard.scale.Blues"] = "Blues",
        ["fretboard.chord.Major"] = "Majeur",
        ["fretboard.chord.Minor"] = "Mineur",
        ["fretboard.chord.Diminished"] = "Diminué",
        ["fretboard.chord.Augmented"] = "Augmenté",
        ["fretboard.chord.Major7"] = "Septième majeure",
        ["fretboard.chord.Minor7"] = "Septième mineure",
        ["fretboard.chord.Dominant7"] = "Septième de dominante",
        ["fretboard.chord.Sus2"] = "Sus2",
        ["fretboard.chord.Sus4"] = "Sus4",
        ["fretboard.understand"] = "Un accordage est la note de chaque corde à vide. La carte du manche liste chaque case où sonne une note, un degré de gamme ou une note d'accord, "
            + "calculée par la bibliothèque à partir des cordes à vide. Les points pleins sont les fondamentales ; les autres points sont les notes de la gamme ou de l'accord. "
            + "Survolez un point pour lire son degré, la place de la note dans la gamme ou l'accord. Les cases sont ici espacées régulièrement, contrairement à un vrai manche.",
    };
}
