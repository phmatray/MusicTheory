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
            offences.AddRange(Forbidden.Where(t => text.Contains(t, StringComparison.OrdinalIgnoreCase)).Select(t => $"{Path.GetFileName(file)}: {t}"));
            offences.AddRange(Url().Matches(text).Select(m => m.Value)
                .Where(u => !IsAllowed(u))
                .Select(u => $"{Path.GetFileName(file)}: {u}"));
        }
        offences.ShouldBeEmpty();
    }

    private static bool IsAllowed(string url) =>
        Uri.TryCreate(url, UriKind.Absolute, out var uri)
        && AllowedHosts.Any(h =>
        {
            var slash = h.IndexOf('/');
            var host = slash < 0 ? h : h[..slash];
            return uri.Host == host && (slash < 0 || uri.AbsolutePath.StartsWith(h[slash..], StringComparison.Ordinal));
        });

    [Fact]
    public void The_allowlist_is_not_fooled_by_lookalikes()
    {
        IsAllowed("http://www.w3.org/2000/svg").ShouldBeTrue();
        IsAllowed("https://github.com/phmatray/MusicTheory").ShouldBeTrue();
        IsAllowed("https://www.w3.org.evil.com/").ShouldBeFalse();
        IsAllowed("https://evil.com/?r=//github.com/phmatray").ShouldBeFalse();
        IsAllowed("https://github.com/other").ShouldBeFalse();
    }

    [Fact]
    public void The_url_pattern_catches_a_foreign_origin() =>
        Url().IsMatch("fetch('https://evil.example/x')").ShouldBeTrue();

    [GeneratedRegex(@"https?://[^\s""'<>)]+")]
    private static partial Regex Url();
}
