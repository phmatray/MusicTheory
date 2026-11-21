using MusicTheory.Toolbox.I18n;

namespace MusicTheory.Toolbox.Tools.Dictation;

public sealed class DictationStrings : IStringTable
{
    public IReadOnlyDictionary<string, string> En { get; } = new Dictionary<string, string>
    {
        ["dictation.title"] = "Melodic dictation",
        ["dictation.blurb"] = "Listen to a short melody and enter it on the keyboard. Share the link and the whole class gets the same melodies.",
        ["dictation.difficulty"] = "Difficulty",
        ["dictation.d.beginner"] = "Beginner",
        ["dictation.d.intermediate"] = "Intermediate",
        ["dictation.d.advanced"] = "Advanced",
        ["dictation.count"] = "Melodies",
        ["dictation.generate"] = "Generate",
        ["dictation.new"] = "New exercise",
        ["dictation.new.blurb"] = "This link has no valid exercise number yet. Choose a difficulty and generate one.",
        ["dictation.question"] = "Melody {0} of {1}",
        ["dictation.key"] = "Key: {0}",
        ["dictation.keyboard"] = "Keyboard",
        ["dictation.entered"] = "Your notes: {0}",
        ["dictation.none"] = "none yet",
        ["dictation.undo"] = "Undo",
        ["dictation.submit"] = "Check",
        ["dictation.expected"] = "Expected melody",
        ["dictation.given"] = "Your melody",
        ["dictation.staff.alt"] = "Melody on a staff: {0}",
        ["dictation.note.right"] = "Note {0}: right ({1})",
        ["dictation.note.wrong"] = "Note {0}: wrong, you wrote {1}, expected {2}",
        ["dictation.note.missing"] = "Note {0}: missing, expected {1}",
        ["dictation.result"] = "Score for this melody: {0} out of {1} notes.",
        ["dictation.next"] = "Next melody",
        ["dictation.finish"] = "Finish",
        ["dictation.score"] = "Total score: {0} out of {1}.",
        ["dictation.restart"] = "Start again",
        ["dictation.teacher"] = "Answer key",
        ["dictation.understand"] = "Each exercise is drawn from a number called a seed: the same seed, difficulty and count always give the same melodies, in any browser. "
            + "The link is enough to share an exercise, and the answer key is the same link with the key view switched on. "
            + "Each note is scored by pitch, so an enharmonic spelling such as Db for C# counts as right, but the octave matters. "
            + "Nothing is stored or sent. The seed is in the address, so this is a practice tool, not an exam.",
    };

    public IReadOnlyDictionary<string, string> Fr { get; } = new Dictionary<string, string>
    {
        ["dictation.title"] = "Dictée mélodique",
        ["dictation.blurb"] = "Écoutez une courte mélodie et saisissez-la au clavier. Partagez le lien et toute la classe reçoit les mêmes mélodies.",
        ["dictation.difficulty"] = "Difficulté",
        ["dictation.d.beginner"] = "Débutant",
        ["dictation.d.intermediate"] = "Intermédiaire",
        ["dictation.d.advanced"] = "Avancé",
        ["dictation.count"] = "Mélodies",
        ["dictation.generate"] = "Générer",
        ["dictation.new"] = "Nouvel exercice",
        ["dictation.new.blurb"] = "Ce lien n'a pas encore de numéro d'exercice valide. Choisissez une difficulté et générez-en un.",
        ["dictation.question"] = "Mélodie {0} sur {1}",
        ["dictation.key"] = "Tonalité : {0}",
        ["dictation.keyboard"] = "Clavier",
        ["dictation.entered"] = "Vos notes : {0}",
        ["dictation.none"] = "aucune pour l'instant",
        ["dictation.undo"] = "Annuler",
        ["dictation.submit"] = "Vérifier",
        ["dictation.expected"] = "Mélodie attendue",
        ["dictation.given"] = "Votre mélodie",
        ["dictation.staff.alt"] = "Mélodie sur une portée : {0}",
        ["dictation.note.right"] = "Note {0} : juste ({1})",
        ["dictation.note.wrong"] = "Note {0} : fausse, vous avez écrit {1}, attendu {2}",
        ["dictation.note.missing"] = "Note {0} : manquante, attendu {1}",
        ["dictation.result"] = "Score de cette mélodie : {0} notes justes sur {1}.",
        ["dictation.next"] = "Mélodie suivante",
        ["dictation.finish"] = "Terminer",
        ["dictation.score"] = "Score total : {0} sur {1}.",
        ["dictation.restart"] = "Recommencer",
        ["dictation.teacher"] = "Corrigé",
        ["dictation.understand"] = "Chaque exercice est tiré d'un nombre appelé graine : la même graine, la même difficulté et le même nombre de mélodies donnent toujours les mêmes mélodies, dans n'importe quel navigateur. "
            + "Le lien suffit à partager un exercice, et le corrigé est le même lien avec la vue enseignant activée. "
            + "Chaque note est notée selon sa hauteur : une enharmonie comme ré bémol pour do dièse est juste, mais l'octave compte. "
            + "Rien n'est stocké ni envoyé. La graine est dans l'adresse : c'est un outil d'entraînement, pas un examen.",
    };
}
