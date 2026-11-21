using MusicTheory.Toolbox.I18n;

namespace MusicTheory.Toolbox.Drills;

/// <summary>
/// The copy every drill page shares (controls, progress, score), under the tool's own prefix, plus what is specific
/// to the tool. The player and the Share button keep using the <c>intervals.*</c> keys.
/// </summary>
public abstract class DrillStringTable(string prefix) : IStringTable
{
    private static readonly string[] Keys = ["difficulty", "d.beginner", "d.intermediate", "d.advanced", "count", "new", "new.blurb", "generate",
        "question", "choices", "right", "wrong", "next", "finish", "score", "restart", "teacher"];

    private static readonly string[] EnCommon = ["Difficulty", "Beginner", "Intermediate", "Advanced", "Questions", "New exercise",
        "This link has no valid exercise number yet. Choose a difficulty and generate one.", "Generate",
        "Question {0} of {1}", "Answers", "Right: {0}.", "Not quite. It was {0}.", "Next", "Finish", "Score: {0} out of {1}.",
        "Start again", "Answer key"];

    private static readonly string[] FrCommon = ["Difficulté", "Débutant", "Intermédiaire", "Avancé", "Questions", "Nouvel exercice",
        "Ce lien n'a pas encore de numéro d'exercice valide. Choisissez une difficulté et générez-en un.", "Générer",
        "Question {0} sur {1}", "Réponses", "Juste : {0}.", "Pas tout à fait. C'était {0}.", "Suivante", "Terminer", "Score : {0} sur {1}.",
        "Recommencer", "Corrigé"];

    protected abstract IReadOnlyDictionary<string, string> EnOwn { get; }
    protected abstract IReadOnlyDictionary<string, string> FrOwn { get; }

    public IReadOnlyDictionary<string, string> En => Build(EnCommon, EnOwn);
    public IReadOnlyDictionary<string, string> Fr => Build(FrCommon, FrOwn);

    private Dictionary<string, string> Build(string[] common, IReadOnlyDictionary<string, string> own)
    {
        var d = Keys.Zip(common).ToDictionary(p => prefix + "." + p.First, p => p.Second);
        foreach (var (k, v) in own) d[prefix + "." + k] = v;
        return d;
    }
}
