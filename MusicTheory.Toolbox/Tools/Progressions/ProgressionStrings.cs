using MusicTheory.Toolbox.Drills;

namespace MusicTheory.Toolbox.Tools.Progressions;

public sealed class ProgressionStrings() : DrillStringTable("progressions")
{
    protected override IReadOnlyDictionary<string, string> EnOwn { get; } = new Dictionary<string, string>
    {
        ["title"] = "Progression drill",
        ["blurb"] = "Listen to a chord progression and build its Roman numerals in order. Share the link and the whole class gets the same exercise.",
        ["key"] = "Key: {0}",
        ["undo"] = "Undo",
        ["check"] = "Check",
        ["yours"] = "Your answer",
        ["partial"] = "{0} of {1} right. It was {2}.",
        ["understand"] = "Each exercise is drawn from a number called a seed: the same seed, difficulty and count always give the same progressions, in any browser. "
            + "The first chord is always I (or i) and the last is I or V. Tap the degrees in the order you hear them: each correct position earns a share of the point. "
            + "Nothing is stored or sent, and no answer travels with the link, but the seed is in the address: this is a practice tool, not an exam.",
    };

    protected override IReadOnlyDictionary<string, string> FrOwn { get; } = new Dictionary<string, string>
    {
        ["title"] = "Exercice de progressions",
        ["blurb"] = "Écoutez une progression d'accords et construisez ses chiffrages romains dans l'ordre. Partagez le lien et toute la classe reçoit le même exercice.",
        ["key"] = "Tonalité : {0}",
        ["undo"] = "Annuler",
        ["check"] = "Valider",
        ["yours"] = "Votre réponse",
        ["partial"] = "{0} sur {1} justes. C'était {2}.",
        ["understand"] = "Chaque exercice est tiré d'un nombre appelé graine : la même graine, la même difficulté et le même nombre de questions donnent toujours les mêmes progressions, dans n'importe quel navigateur. "
            + "Le premier accord est toujours I (ou i) et le dernier I ou V. Touchez les degrés dans l'ordre où vous les entendez : chaque position juste rapporte une part du point. "
            + "Rien n'est stocké ni envoyé, et aucune réponse ne voyage avec le lien, mais la graine est dans l'adresse : c'est un outil d'entraînement, pas un examen.",
    };
}
