// Builds the MusicTheory site into a static folder.
//
//   dotnet run --project tools/SiteGen -- <repo root> <output dir>
//
// 1. site/ is copied as is, except files starting with "_" (templates).
// 2. docs/*.md become docs/<slug>.html through site/_page.html.
// 3. The public types of MusicTheory and MusicTheory.Guitar become api/<Type>.html, from their XML docs.
// 4. site/index.html: every <figure class="sample" data-eval> runs its C# expression against the library and
//    shows the result; {{circle:json}} and {{circle:table}} carry the circle of fifths the library computes.
// Any sample that fails to compile or throws fails the build, so the site cannot show a wrong output.

using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using SiteGen;

var root = Path.GetFullPath(args.Length > 0 ? args[0] : ".");
var output = Path.GetFullPath(args.Length > 1 ? args[1] : Path.Combine(root, "_site"));
var site = Path.Combine(root, "site");

if (Directory.Exists(output))
    Directory.Delete(output, recursive: true);
foreach (var file in Directory.EnumerateFiles(site, "*", SearchOption.AllDirectories))
{
    var relative = Path.GetRelativePath(site, file);
    if (Path.GetFileName(relative).StartsWith('_'))
        continue;
    var target = Path.Combine(output, relative);
    Directory.CreateDirectory(Path.GetDirectoryName(target)!);
    File.Copy(file, target);
}

var template = File.ReadAllText(Path.Combine(site, "_page.html"));
var api = new ApiReference();
var docs = new Docs(Path.Combine(root, "docs"), api);

foreach (var page in docs.Pages)
    Write(Path.Combine(output, "docs", page.Slug + ".html"), Page.Render(template, page.Title, page.Description, "../", docs.Nav(page.Slug, "../"), page.Html, page.Toc));
foreach (var type in api.Types)
    Write(Path.Combine(output, "api", ApiReference.FileName(type) + ".html"), Page.Render(template, type.Name, api.Summary(type), "../", docs.Nav("api", "../"), api.TypePage(type), []));
Write(Path.Combine(output, "api", "index.html"), Page.Render(template, "API reference", "Every public type of MusicTheory and MusicTheory.Guitar.", "../", docs.Nav("api", "../"), api.IndexPage(), []));

var index = File.ReadAllText(Path.Combine(site, "index.html"));
index = await Samples.EvaluateAsync(index);
index = index.Replace("{{circle:json}}", Circle.Json()).Replace("{{circle:table}}", Circle.Table());
var leftover = Regex.Match(index, @"\{\{[a-z]+:[^}]*\}\}");
if (leftover.Success)
    throw new InvalidOperationException($"site/index.html has an unknown token {leftover.Value}.");
Write(Path.Combine(output, "index.html"), index);

Console.WriteLine($"{docs.Pages.Count} docs pages, {api.Types.Count} API pages, home page with {Samples.Count} evaluated samples -> {output}");

static void Write(string path, string html)
{
    Directory.CreateDirectory(Path.GetDirectoryName(path)!);
    File.WriteAllText(path, html, new UTF8Encoding(false));
}

namespace SiteGen
{
    internal static class Page
    {
        public static string Render(string template, string title, string description, string root, string nav, string content, IReadOnlyList<(string Id, string Text)> toc) =>
            template
                .Replace("{{title}}", WebUtility.HtmlEncode(title))
                .Replace("{{description}}", WebUtility.HtmlEncode(description))
                .Replace("{{root}}", root)
                .Replace("{{nav}}", nav)
                .Replace("{{toc}}", toc.Count == 0 ? "" :
                    "<nav class=\"toc\" aria-label=\"On this page\"><p>On this page</p><ol>" +
                    string.Concat(toc.Select(t => $"<li><a href=\"#{t.Id}\">{t.Text}</a></li>")) + "</ol></nav>")
                .Replace("{{content}}", content);
    }
}
