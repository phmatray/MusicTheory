using MusicTheory.Toolbox.I18n;

namespace MusicTheory.Toolbox.Tools.Finder;

public sealed class FinderStrings : IStringTable
{
    public IReadOnlyDictionary<string, string> En { get; } = new Dictionary<string, string>
    {
        ["finder.title"] = "Chord and scale finder",
        ["finder.blurb"] = "Switch on the notes you hear to name the chord, or type a chord to see which scales fit it.",
        ["finder.identify"] = "Which chord is this?",
        ["finder.keys"] = "The twelve notes",
        ["finder.hint"] = "Switch on at least three notes to see matching chords.",
        ["finder.none"] = "No chord matches these notes.",
        ["finder.extra"] = "{0} extra note(s)",
        ["finder.exact"] = "exact match",
        ["finder.scales"] = "Which scales fit this chord?",
        ["finder.chord"] = "Chord symbol",
        ["finder.show"] = "Show scales",
        ["finder.chordhint"] = "Type a chord such as Cmaj7, Dm7 or G7 (letters, not syllables).",
        ["finder.error"] = "Cannot read this chord: {0}",
        ["finder.noscales"] = "No scale on this root holds every note of the chord.",
        ["finder.modes"] = "Modes of {0}",
        ["finder.nomodes"] = "This scale has no named modes.",
        ["finder.degree"] = "Degree {0}",
        ["finder.understand"] = "A chord fits a scale when every chord note belongs to it, so one chord usually has several scales. "
            + "Scales are listed with the fewest avoid notes first: an avoid note is a scale note a semitone above a chord note, which sounds harsh when held. "
            + "A scale's modes start the same notes on each of its degrees. The library does the matching; the keys use pitch classes, so enharmonic spellings never confuse the result.",
    };

    public IReadOnlyDictionary<string, string> Fr { get; } = new Dictionary<string, string>
    {
        ["finder.title"] = "Chercheur d'accords et de gammes",
        ["finder.blurb"] = "Activez les notes entendues pour nommer l'accord, ou saisissez un accord pour voir quelles gammes lui conviennent.",
        ["finder.identify"] = "Quel est cet accord ?",
        ["finder.keys"] = "Les douze notes",
        ["finder.hint"] = "Activez au moins trois notes pour voir les accords correspondants.",
        ["finder.none"] = "Aucun accord ne correspond à ces notes.",
        ["finder.extra"] = "{0} note(s) en plus",
        ["finder.exact"] = "correspondance exacte",
        ["finder.scales"] = "Quelles gammes conviennent à cet accord ?",
        ["finder.chord"] = "Symbole d'accord",
        ["finder.show"] = "Voir les gammes",
        ["finder.chordhint"] = "Saisissez un accord comme Cmaj7, Dm7 ou G7 (lettres, pas syllabes).",
        ["finder.error"] = "Accord illisible : {0}",
        ["finder.noscales"] = "Aucune gamme sur cette fondamentale ne contient toutes les notes de l'accord.",
        ["finder.modes"] = "Modes de {0}",
        ["finder.nomodes"] = "Cette gamme n'a pas de modes nommés.",
        ["finder.degree"] = "Degré {0}",
        ["finder.understand"] = "Un accord convient à une gamme quand chacune de ses notes en fait partie : un accord a donc souvent plusieurs gammes. "
            + "Les gammes sont classées avec le moins de notes à éviter d'abord : une note à éviter est une note de la gamme située un demi-ton au-dessus d'une note de l'accord, ce qui sonne dur si on la tient. "
            + "Les modes d'une gamme reprennent les mêmes notes à partir de chaque degré. La bibliothèque fait la correspondance ; les touches utilisent des classes de hauteur, donc les enharmonies ne brouillent jamais le résultat.",
    };
}
