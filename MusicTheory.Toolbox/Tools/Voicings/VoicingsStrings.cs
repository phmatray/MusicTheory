using MusicTheory.Toolbox.I18n;

namespace MusicTheory.Toolbox.Tools.Voicings;

public sealed class VoicingsStrings : IStringTable
{
    public IReadOnlyDictionary<string, string> En { get; } = new Dictionary<string, string>
    {
        ["voicings.title"] = "Chord voicings",
        ["voicings.blurb"] = "Playable shapes for a chord on guitar, ukulele and other tunings, easiest first.",
        ["voicings.chord"] = "Chord",
        ["voicings.tuning"] = "Tuning",
        ["voicings.play"] = "Play",
        ["voicings.empty"] = "The library has no playable voicing for this chord on this tuning.",
        ["voicings.badtuning"] = "Unknown tuning. Standard is shown instead.",
        ["voicings.diagram"] = "Chord diagram, strings low to high: {0}",
        ["voicings.tab"] = "Tab",
        ["voicings.barre.detail"] = "barre at fret {0}",
        ["voicings.open"] = "open string",
        ["voicings.muted"] = "muted string",
        ["voicings.fromfret"] = "from fret {0}",
        ["voicings.shape.open"] = "Open shape",
        ["voicings.shape.movable"] = "{0}-form shape",
        ["voicings.shape.generated"] = "Found shape",
        ["voicings.understand"] = "A voicing is one way to play a chord: which fret each string takes, which strings stay open (o) or muted (x), and which finger presses where. "
            + "A barre is one finger pressing several strings at the same fret. The library checks that every voicing sounds the chord's tones, then lists the easiest first. "
            + "Numbers on the dots are fingers: 1 is the index, 4 the little finger. A number beside the diagram says which fret the grid starts at.",
    };

    public IReadOnlyDictionary<string, string> Fr { get; } = new Dictionary<string, string>
    {
        ["voicings.title"] = "Positions d'accord",
        ["voicings.blurb"] = "Positions jouables d'un accord à la guitare, au ukulélé et dans d'autres accordages, des plus faciles aux plus difficiles.",
        ["voicings.chord"] = "Accord",
        ["voicings.tuning"] = "Accordage",
        ["voicings.play"] = "Écouter",
        ["voicings.empty"] = "La bibliothèque ne propose aucune position jouable de cet accord dans cet accordage.",
        ["voicings.badtuning"] = "Accordage inconnu. Standard est affiché à la place.",
        ["voicings.diagram"] = "Diagramme d'accord, cordes du grave à l'aigu : {0}",
        ["voicings.tab"] = "Tablature",
        ["voicings.barre.detail"] = "barré à la case {0}",
        ["voicings.open"] = "corde à vide",
        ["voicings.muted"] = "corde étouffée",
        ["voicings.fromfret"] = "à partir de la case {0}",
        ["voicings.shape.open"] = "Position ouverte",
        ["voicings.shape.movable"] = "Position en forme de {0}",
        ["voicings.shape.generated"] = "Position trouvée",
        ["voicings.understand"] = "Une position est une façon de jouer un accord : quelle case prend chaque corde, quelles cordes restent à vide (o) ou étouffées (x), et quel doigt appuie où. "
            + "Un barré est un doigt qui appuie sur plusieurs cordes à la même case. La bibliothèque vérifie que chaque position sonne les notes de l'accord, puis les classe des plus faciles aux plus difficiles. "
            + "Les numéros sur les points sont les doigts : 1 est l'index, 4 l'auriculaire. Un numéro à côté du diagramme dit à quelle case la grille commence.",
    };
}
