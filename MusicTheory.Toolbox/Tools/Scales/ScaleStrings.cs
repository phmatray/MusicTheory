using MusicTheory.Toolbox.Drills;

namespace MusicTheory.Toolbox.Tools.Scales;

public sealed class ScaleStrings() : DrillStringTable("scales")
{
    protected override IReadOnlyDictionary<string, string> EnOwn { get; } = new Dictionary<string, string>
    {
        ["title"] = "Scale drill",
        ["blurb"] = "Listen to a scale and name it. Share the link and the whole class gets the same exercise.",
        ["understand"] = "Each exercise is drawn from a number called a seed: the same seed, difficulty and count always give the same scales, in any browser. "
            + "That is why the link is enough to share an exercise, and why the answer key can be checked with the same link. "
            + "Nothing is stored or sent, and no answer travels with the link, but the seed is in the address: this is a practice tool, not an exam. "
            + "The scale is played upward from its first note. Ionian counts as major and Aeolian as natural minor.",
    };

    protected override IReadOnlyDictionary<string, string> FrOwn { get; } = new Dictionary<string, string>
    {
        ["title"] = "Exercice de gammes",
        ["blurb"] = "Écoutez une gamme et nommez-la. Partagez le lien et toute la classe reçoit le même exercice.",
        ["understand"] = "Chaque exercice est tiré d'un nombre appelé graine : la même graine, la même difficulté et le même nombre de questions donnent toujours les mêmes gammes, dans n'importe quel navigateur. "
            + "C'est pourquoi le lien suffit à partager un exercice, et pourquoi le corrigé se vérifie avec le même lien. "
            + "Rien n'est stocké ni envoyé, et aucune réponse ne voyage avec le lien, mais la graine est dans l'adresse : c'est un outil d'entraînement, pas un examen. "
            + "La gamme est jouée vers l'aigu depuis sa première note. Ionien compte pour majeur et éolien pour mineur naturel.",
    };
}
