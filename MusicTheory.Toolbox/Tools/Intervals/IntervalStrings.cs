using MusicTheory.Toolbox.I18n;

namespace MusicTheory.Toolbox.Tools.Intervals;

public sealed class IntervalStrings : IStringTable
{
    public IReadOnlyDictionary<string, string> En { get; } = new Dictionary<string, string>
    {
        ["intervals.title"] = "Interval drill",
        ["intervals.blurb"] = "Listen to two notes and name the interval. Share the link and the whole class gets the same exercise.",
        ["intervals.difficulty"] = "Difficulty",
        ["intervals.d.beginner"] = "Beginner",
        ["intervals.d.intermediate"] = "Intermediate",
        ["intervals.d.advanced"] = "Advanced",
        ["intervals.count"] = "Questions",
        ["intervals.new"] = "New exercise",
        ["intervals.new.blurb"] = "This link has no valid exercise number yet. Choose a difficulty and generate one.",
        ["intervals.generate"] = "Generate",
        ["intervals.question"] = "Question {0} of {1}",
        ["intervals.play"] = "Play",
        ["intervals.replay"] = "Play again",
        ["intervals.choices"] = "Answers",
        ["intervals.right"] = "Right: {0}.",
        ["intervals.wrong"] = "Not quite. It was {0}.",
        ["intervals.next"] = "Next",
        ["intervals.finish"] = "Finish",
        ["intervals.score"] = "Score: {0} out of {1}.",
        ["intervals.restart"] = "Start again",
        ["intervals.share"] = "Share",
        ["intervals.copied"] = "Link copied.",
        ["intervals.copyfail"] = "Copy this address from the address bar.",
        ["intervals.teacher"] = "Answer key",
        ["intervals.kind.up"] = "ascending",
        ["intervals.kind.down"] = "descending",
        ["intervals.kind.together"] = "together",
        ["intervals.understand"] = "Each exercise is drawn from a number called a seed: the same seed, difficulty and count always give the same questions, in any browser. "
            + "That is why the link is enough to share an exercise, and why the answer key can be checked with the same link. "
            + "Nothing is stored or sent, and no answer travels with the link. "
            + "The seed is in the address, so a determined student could work out the answers: this is a practice tool for the classroom, not an exam. "
            + "Notes are played with a plucked-string sound.",
    };

    public IReadOnlyDictionary<string, string> Fr { get; } = new Dictionary<string, string>
    {
        ["intervals.title"] = "Exercice d'intervalles",
        ["intervals.blurb"] = "Écoutez deux notes et nommez l'intervalle. Partagez le lien et toute la classe reçoit le même exercice.",
        ["intervals.difficulty"] = "Difficulté",
        ["intervals.d.beginner"] = "Débutant",
        ["intervals.d.intermediate"] = "Intermédiaire",
        ["intervals.d.advanced"] = "Avancé",
        ["intervals.count"] = "Questions",
        ["intervals.new"] = "Nouvel exercice",
        ["intervals.new.blurb"] = "Ce lien n'a pas encore de numéro d'exercice valide. Choisissez une difficulté et générez-en un.",
        ["intervals.generate"] = "Générer",
        ["intervals.question"] = "Question {0} sur {1}",
        ["intervals.play"] = "Écouter",
        ["intervals.replay"] = "Réécouter",
        ["intervals.choices"] = "Réponses",
        ["intervals.right"] = "Juste : {0}.",
        ["intervals.wrong"] = "Pas tout à fait. C'était {0}.",
        ["intervals.next"] = "Suivante",
        ["intervals.finish"] = "Terminer",
        ["intervals.score"] = "Score : {0} sur {1}.",
        ["intervals.restart"] = "Recommencer",
        ["intervals.share"] = "Partager",
        ["intervals.copied"] = "Lien copié.",
        ["intervals.copyfail"] = "Copiez l'adresse depuis la barre d'adresse.",
        ["intervals.teacher"] = "Corrigé",
        ["intervals.kind.up"] = "ascendant",
        ["intervals.kind.down"] = "descendant",
        ["intervals.kind.together"] = "simultané",
        ["intervals.understand"] = "Chaque exercice est tiré d'un nombre appelé graine : la même graine, la même difficulté et le même nombre de questions donnent toujours les mêmes questions, dans n'importe quel navigateur. "
            + "C'est pourquoi le lien suffit à partager un exercice, et pourquoi le corrigé se vérifie avec le même lien. "
            + "Rien n'est stocké ni envoyé, et aucune réponse ne voyage avec le lien. "
            + "La graine est dans l'adresse : un élève déterminé pourrait calculer les réponses. C'est un outil d'entraînement pour la classe, pas un examen. "
            + "Les notes sont jouées avec un son de corde pincée.",
    };
}
