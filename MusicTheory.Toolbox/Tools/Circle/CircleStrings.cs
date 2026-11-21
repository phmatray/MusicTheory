using MusicTheory.Toolbox.I18n;

namespace MusicTheory.Toolbox.Tools.Circle;

public sealed class CircleStrings : IStringTable
{
    public IReadOnlyDictionary<string, string> En { get; } = new Dictionary<string, string>
    {
        ["circle.title"] = "Circle of fifths",
        ["circle.blurb"] = "Pick a key on the ring to see its signature and scale.",
        ["circle.ring"] = "The twelve major keys",
        ["circle.major"] = "{0} major",
        ["circle.minor"] = "{0} minor",
        ["circle.sig.none"] = "no sharps or flats",
        ["circle.sig.sharp.one"] = "1 sharp",
        ["circle.sig.sharp.many"] = "{0} sharps",
        ["circle.sig.flat.one"] = "1 flat",
        ["circle.sig.flat.many"] = "{0} flats",
        ["circle.signature"] = "Key signature",
        ["circle.relative"] = "Relative minor",
        ["circle.scale"] = "Scale",
        ["circle.staff.alt"] = "{0} scale on a staff: {1}",
        ["circle.invalid"] = "Unknown key \"{0}\": showing C major.",
        ["circle.understand"] = "Going up a fifth adds one sharp (or removes one flat), and going down a fifth adds one flat. "
            + "Each major key shares its notes with a relative minor key, a minor third lower, so both have the same signature. "
            + "The scale is computed by the library from the key's tonic and spelled for the language you chose.",
    };

    public IReadOnlyDictionary<string, string> Fr { get; } = new Dictionary<string, string>
    {
        ["circle.title"] = "Cercle des quintes",
        ["circle.blurb"] = "Choisissez une tonalité sur l'anneau pour voir son armure et sa gamme.",
        ["circle.ring"] = "Les douze tonalités majeures",
        ["circle.major"] = "{0} majeur",
        ["circle.minor"] = "{0} mineur",
        ["circle.sig.none"] = "aucune altération",
        ["circle.sig.sharp.one"] = "1 dièse",
        ["circle.sig.sharp.many"] = "{0} dièses",
        ["circle.sig.flat.one"] = "1 bémol",
        ["circle.sig.flat.many"] = "{0} bémols",
        ["circle.signature"] = "Armure",
        ["circle.relative"] = "Relatif mineur",
        ["circle.scale"] = "Gamme",
        ["circle.staff.alt"] = "Gamme de {0} sur une portée : {1}",
        ["circle.invalid"] = "Tonalité inconnue \"{0}\" : affichage de Do majeur.",
        ["circle.understand"] = "Monter d'une quinte ajoute un dièse (ou retire un bémol), et descendre d'une quinte ajoute un bémol. "
            + "Chaque tonalité majeure partage ses notes avec une tonalité relative mineure, une tierce mineure plus bas : les deux ont la même armure. "
            + "La gamme est calculée par la bibliothèque à partir de la tonique, puis écrite dans la langue choisie.",
    };
}
