using System.Text.RegularExpressions;

namespace MusicTheory.Toolbox.UnitTests;

/// <summary>The app is client-side only: no storage, no cookies, no other origin.</summary>
public partial class PrivacyGuardTests
{
    private static readonly string[] Forbidden = ["localStorage", "sessionStorage", "indexedDB", "document.cookie"];
    private static readonly string[] AllowedHosts = ["www.w3.org", "github.com/phmatray"];

    private static string ToolboxDir()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "MusicTheory.sln")))
            dir = dir.Parent;
        return Path.Combine((dir ?? throw new InvalidOperationException("MusicTheory.sln not found above the test binary.")).FullName, "MusicTheory.Toolbox");
    }

    private static IEnumerable<string> Sources()
    {
        var root = ToolboxDir();
        var sep = Path.DirectorySeparatorChar;
        return Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
            .Where(f => f.EndsWith(".cs") || f.EndsWith(".razor") || f.EndsWith(".js") || f.EndsWith(".html"))
            .Where(f => !f.Contains($"{sep}obj{sep}") && !f.Contains($"{sep}bin{sep}") && !f.Contains($"{sep}wwwroot{sep}lib{sep}"));
    }

    [Fact]
    public void The_guard_actually_reads_source_files() => Sources().Count().ShouldBeGreaterThan(10);

    [Fact]
    public void No_storage_cookie_or_foreign_origin_in_the_toolbox_source()
    {
        var offences = new List<string>();
        foreach (var file in Sources())
        {
            var text = File.ReadAllText(file);
            offences.AddRange(Forbidden.Where(text.Contains).Select(t => $"{Path.GetFileName(file)}: {t}"));
            offences.AddRange(Url().Matches(text).Select(m => m.Value)
                .Where(u => !AllowedHosts.Any(h => u.Contains("//" + h)))
                .Select(u => $"{Path.GetFileName(file)}: {u}"));
        }
        offences.ShouldBeEmpty();
    }

    [Fact]
    public void The_url_pattern_catches_a_foreign_origin() =>
        Url().IsMatch("fetch('https://evil.example/x')").ShouldBeTrue();

    [GeneratedRegex(@"https?://[^\s""'<>)]+")]
    private static partial Regex Url();
}
