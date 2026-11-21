using MusicTheory.Toolbox.I18n;

namespace MusicTheory.Toolbox.Tools.Analysis;

public sealed class AnalysisStrings : IStringTable
{
    public IReadOnlyDictionary<string, string> En { get; } = new Dictionary<string, string>
    {
        ["analysis.title"] = "Progression analysis",
        ["analysis.blurb"] = "Type a chord progression to get its Roman numerals, cadences and key changes.",
        ["analysis.chords"] = "Chords (separated by spaces, commas or |)",
        ["analysis.key"] = "Key (empty to detect it)",
        ["analysis.apply"] = "Analyse",
        ["analysis.usedkey"] = "Key used",
        ["analysis.detected"] = "detected",
        ["analysis.candidates"] = "Key candidates",
        ["analysis.confidence"] = "{0}%",
        ["analysis.major"] = "{0} major",
        ["analysis.minor"] = "{0} minor",
        ["analysis.col.index"] = "#",
        ["analysis.col.chord"] = "Chord",
        ["analysis.col.numeral"] = "Numeral",
        ["analysis.col.function"] = "Function",
        ["analysis.col.cadence"] = "Cadence",
        ["analysis.regions"] = "Key regions",
        ["analysis.region"] = "{0}: chords {1} to {2}",
        ["analysis.pivot"] = "pivot: chord {0}",
        ["analysis.badtoken"] = "Not a chord: \"{0}\".",
        ["analysis.badkey"] = "Unknown key \"{0}\" (try C, Am, F#).",
        ["analysis.toomany"] = "Too many chords: the limit is {0}.",
        ["analysis.fn.Diatonic"] = "Diatonic",
        ["analysis.fn.Borrowed"] = "Borrowed chord",
        ["analysis.fn.SecondaryDominant"] = "Secondary dominant",
        ["analysis.fn.Chromatic"] = "Chromatic",
        ["analysis.fn.Neapolitan"] = "Neapolitan sixth",
        ["analysis.fn.AugmentedSixth"] = "Augmented sixth",
        ["analysis.cad.Authentic"] = "Authentic cadence",
        ["analysis.cad.Plagal"] = "Plagal cadence",
        ["analysis.cad.Half"] = "Half cadence",
        ["analysis.cad.Deceptive"] = "Deceptive cadence",
        ["analysis.understand"] = "Each chord is read against a key as a Roman numeral: capitals for major chords, lower case for minor ones. "
            + "Chords outside the key are borrowed from the parallel mode, or are secondary dominants such as V7/vi. "
            + "A cadence marks how a phrase arrives: V to I is authentic, IV to I plagal, a final V is a half cadence and V to vi is deceptive. "
            + "When the key changes, a pivot chord belongs to both keys.",
    };

    public IReadOnlyDictionary<string, string> Fr { get; } = new Dictionary<string, string>
    {
        ["analysis.title"] = "Analyse de progression",
        ["analysis.blurb"] = "Saisissez une grille d'accords pour obtenir ses chiffrages romains, ses cadences et ses changements de tonalité.",
        ["analysis.chords"] = "Accords (séparés par des espaces, des virgules ou |)",
        ["analysis.key"] = "Tonalité (vide pour la détecter)",
        ["analysis.apply"] = "Analyser",
        ["analysis.usedkey"] = "Tonalité retenue",
        ["analysis.detected"] = "détectée",
        ["analysis.candidates"] = "Tonalités candidates",
        ["analysis.confidence"] = "{0} %",
        ["analysis.major"] = "{0} majeur",
        ["analysis.minor"] = "{0} mineur",
        ["analysis.col.index"] = "n°",
        ["analysis.col.chord"] = "Accord",
        ["analysis.col.numeral"] = "Chiffrage",
        ["analysis.col.function"] = "Fonction",
        ["analysis.col.cadence"] = "Cadence",
        ["analysis.regions"] = "Régions tonales",
        ["analysis.region"] = "{0} : accords {1} à {2}",
        ["analysis.pivot"] = "pivot : accord {0}",
        ["analysis.badtoken"] = "Accord non reconnu : \"{0}\".",
        ["analysis.badkey"] = "Tonalité inconnue \"{0}\" (essayez C, Am, F#).",
        ["analysis.toomany"] = "Trop d'accords : la limite est {0}.",
        ["analysis.fn.Diatonic"] = "Diatonique",
        ["analysis.fn.Borrowed"] = "Accord emprunté",
        ["analysis.fn.SecondaryDominant"] = "Dominante secondaire",
        ["analysis.fn.Chromatic"] = "Chromatique",
        ["analysis.fn.Neapolitan"] = "Sixte napolitaine",
        ["analysis.fn.AugmentedSixth"] = "Sixte augmentée",
        ["analysis.cad.Authentic"] = "Cadence parfaite (authentique)",
        ["analysis.cad.Plagal"] = "Cadence plagale",
        ["analysis.cad.Half"] = "Demi-cadence",
        ["analysis.cad.Deceptive"] = "Cadence rompue",
        ["analysis.understand"] = "Chaque accord est lu dans une tonalité sous forme de chiffre romain : majuscule pour un accord majeur, minuscule pour un accord mineur. "
            + "Un accord hors tonalité est emprunté au mode parallèle, ou c'est une dominante secondaire comme V7/vi. "
            + "Une cadence marque l'arrivée d'une phrase : V vers I est parfaite, IV vers I plagale, un V final est une demi-cadence et V vers vi est rompue. "
            + "Quand la tonalité change, un accord pivot appartient aux deux tonalités.",
    };
}
