using MusicTheory.Toolbox.Drills;

namespace MusicTheory.Toolbox.Tools.Chords;

public sealed class ChordStrings() : DrillStringTable("chords")
{
    protected override IReadOnlyDictionary<string, string> EnOwn { get; } = new Dictionary<string, string>
    {
        ["title"] = "Chord drill",
        ["blurb"] = "Listen to a chord and name its quality. Share the link and the whole class gets the same exercise.",
        ["understand"] = "Each exercise is drawn from a number called a seed: the same seed, difficulty and count always give the same chords, in any browser. "
            + "That is why the link is enough to share an exercise, and why the answer key can be checked with the same link. "
            + "Nothing is stored or sent, and no answer travels with the link, but the seed is in the address: this is a practice tool, not an exam. "
            + "At the advanced level, chords may be played in inversion; the quality you name stays the same.",
    };

    protected override IReadOnlyDictionary<string, string> FrOwn { get; } = new Dictionary<string, string>
    {
        ["title"] = "Exercice d'accords",
        ["blurb"] = "Écoutez un accord et nommez sa qualité. Partagez le lien et toute la classe reçoit le même exercice.",
        ["understand"] = "Chaque exercice est tiré d'un nombre appelé graine : la même graine, la même difficulté et le même nombre de questions donnent toujours les mêmes accords, dans n'importe quel navigateur. "
            + "C'est pourquoi le lien suffit à partager un exercice, et pourquoi le corrigé se vérifie avec le même lien. "
            + "Rien n'est stocké ni envoyé, et aucune réponse ne voyage avec le lien, mais la graine est dans l'adresse : c'est un outil d'entraînement, pas un examen. "
            + "Au niveau avancé, les accords peuvent être joués renversés ; la qualité à nommer reste la même.",
    };
}
