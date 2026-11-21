namespace MusicTheory.Toolbox.I18n;

/// <summary>Copy of the shell: title, nav, language names, the Comprendre label.</summary>
public sealed class ShellStrings : IStringTable
{
    public IReadOnlyDictionary<string, string> En { get; } = new Dictionary<string, string>
    {
        ["shell.title"] = "MusicTheory Toolbox",
        ["shell.intro"] = "Music theory tools that run entirely in your browser. Nothing is sent anywhere.",
        ["shell.nav"] = "Tools",
        ["shell.lang"] = "Language",
        ["shell.lang.en"] = "English",
        ["shell.lang.fr"] = "Français",
        ["shell.unknown"] = "Unknown tool.",
        ["understand.label"] = "Understand",
    };

    public IReadOnlyDictionary<string, string> Fr { get; } = new Dictionary<string, string>
    {
        ["shell.title"] = "Boîte à outils MusicTheory",
        ["shell.intro"] = "Des outils de théorie musicale qui tournent entièrement dans votre navigateur. Rien n'est envoyé ailleurs.",
        ["shell.nav"] = "Outils",
        ["shell.lang"] = "Langue",
        ["shell.lang.en"] = "English",
        ["shell.lang.fr"] = "Français",
        ["shell.unknown"] = "Outil inconnu.",
        ["understand.label"] = "Comprendre",
    };
}
