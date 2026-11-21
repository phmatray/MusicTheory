using MusicTheory.Toolbox.I18n;

namespace MusicTheory.Toolbox.Tools.Help;

public sealed class HelpStrings : IStringTable
{
    public IReadOnlyDictionary<string, string> En { get; } = new Dictionary<string, string>
    {
        ["help.title"] = "Teacher guide and privacy",
        ["help.blurb"] = "How to share an exercise with a class, and exactly what this app does with your data.",
        ["help.share.h"] = "Sharing a drill",
        ["help.share.p"] = "Open a drill (intervals, chords, scales, progressions or melodic dictation), choose a difficulty and a number of questions, then generate an exercise and use Share. "
            + "The address in your browser now holds the whole exercise: send it by message, mail or a learning platform, and every student who opens it gets the same questions.",
        ["help.seed.h"] = "What a seed is",
        ["help.seed.p"] = "The seed is a number in the address. The same seed, difficulty and count always produce the same questions, in any browser. "
            + "Generating again gives a new seed, so a new exercise with a new link.",
        ["help.key.h"] = "The teacher key",
        ["help.key.p"] = "Add key=1 to the address to display the answer key for that exercise. "
            + "Check your copy of the link before you share it: a link with key=1 shows the answers to whoever opens it.",
        ["help.ceiling.h"] = "The honest ceiling",
        ["help.ceiling.p"] = "No answer is stored or sent, but the seed is in the address, so a determined student can work the answers out. "
            + "This is a practice tool for the classroom, not an exam.",
        ["help.privacy.h"] = "Privacy statement",
        ["help.privacy.p"] = "The whole state of the app is in the address: the tool, the language and the settings of the exercise. "
            + "These are the parameters it can contain, computed from the tools themselves:",
        ["help.privacy.none"] = "There are no cookies, no local or session storage, no accounts, no analytics and no request to a third-party server: "
            + "the page, its notation library and its sounds all come from the same site, and the sounds are generated in your browser. The export of a dictation is built in your browser and saved as a file by a local download: nothing is uploaded. "
            + "The notation library we bundle contains the addresses of downloadable instrument sounds, but the app never uses that feature, so those addresses are never contacted. "
            + "Your host (GitHub Pages) sees the page request like any website does. We state technical facts only; whether this suits your school's rules is for your school to judge.",
        ["help.verify.h"] = "Check it yourself",
        ["help.verify.p"] = "Open the developer tools of your browser. In the Network tab, reload the page and play a drill: every request goes to this site. "
            + "In the Application or Storage tab, cookies and the local, session and database storage of the page stay empty.",
    };

    public IReadOnlyDictionary<string, string> Fr { get; } = new Dictionary<string, string>
    {
        ["help.title"] = "Guide enseignant et confidentialité",
        ["help.blurb"] = "Comment partager un exercice avec une classe, et ce que cette application fait exactement de vos données.",
        ["help.share.h"] = "Partager un exercice",
        ["help.share.p"] = "Ouvrez un exercice (intervalles, accords, gammes, progressions ou dictée mélodique), choisissez une difficulté et un nombre de questions, générez l'exercice puis utilisez Partager. "
            + "L'adresse de votre navigateur contient alors tout l'exercice : envoyez-la par message, par courriel ou sur une plateforme d'apprentissage, et chaque élève qui l'ouvre obtient les mêmes questions.",
        ["help.seed.h"] = "Ce qu'est une graine",
        ["help.seed.p"] = "La graine est un nombre présent dans l'adresse. La même graine, la même difficulté et le même nombre de questions donnent toujours les mêmes questions, dans tous les navigateurs. "
            + "Générer à nouveau donne une nouvelle graine, donc un nouvel exercice et un nouveau lien.",
        ["help.key.h"] = "La clé enseignant",
        ["help.key.p"] = "Ajoutez key=1 à l'adresse pour afficher le corrigé de cet exercice. "
            + "Vérifiez votre lien avant de le partager : un lien avec key=1 montre les réponses à quiconque l'ouvre.",
        ["help.ceiling.h"] = "La limite, en toute honnêteté",
        ["help.ceiling.p"] = "Aucune réponse n'est stockée ni envoyée, mais la graine figure dans l'adresse : un élève déterminé peut retrouver les réponses. "
            + "C'est un outil d'entraînement pour la classe, pas un examen.",
        ["help.privacy.h"] = "Déclaration de confidentialité",
        ["help.privacy.p"] = "Tout l'état de l'application tient dans l'adresse : l'outil, la langue et les réglages de l'exercice. "
            + "Voici les paramètres qu'elle peut contenir, calculés à partir des outils eux-mêmes :",
        ["help.privacy.none"] = "Il n'y a ni cookie, ni stockage local ou de session, ni compte, ni outil de mesure d'audience, ni requête vers un serveur tiers : "
            + "la page, sa bibliothèque de notation et ses sons viennent tous du même site, et les sons sont produits dans votre navigateur. L'export d'une dictée est construit dans votre navigateur et enregistré comme fichier par un téléchargement local : rien n'est envoyé. "
            + "La bibliothèque de notation que nous embarquons contient des adresses de sons d'instruments téléchargeables, mais l'application n'utilise jamais cette fonction : ces adresses ne sont jamais contactées. "
            + "Votre hébergeur (GitHub Pages) voit la requête de la page, comme tout site web. Nous énonçons des faits techniques uniquement : c'est à votre établissement de juger si cela convient à ses règles.",
        ["help.verify.h"] = "Vérifiez par vous-même",
        ["help.verify.p"] = "Ouvrez les outils de développement de votre navigateur. Dans l'onglet Réseau, rechargez la page et jouez un exercice : toutes les requêtes vont vers ce site. "
            + "Dans l'onglet Application ou Stockage, les cookies et les stockages local, de session et de base de données de la page restent vides.",
    };
}
