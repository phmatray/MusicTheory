using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Markdig;

namespace SiteGen;

/// <summary>A docs page: front matter, rendered body and its h2 outline.</summary>
internal sealed record DocPage(string Slug, string Title, string Description, string Section, int Order, string Html, IReadOnlyList<(string Id, string Text)> Toc);

/// <summary>docs/*.md rendered to HTML, with the sidebar navigation that ties them together.</summary>
internal sealed partial class Docs
{
    // Sidebar sections, in order. A page names its section in front matter.
    private static readonly string[] Sections = ["Start", "Concepts", "Topics", "Tutorials", "Help"];

    private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

    private readonly ApiReference _api;

    public IReadOnlyList<DocPage> Pages { get; }

    public Docs(string folder, ApiReference api)
    {
        _api = api;
        Pages = Directory.GetFiles(folder, "*.md").Select(Load).OrderBy(p => Array.IndexOf(Sections, p.Section)).ThenBy(p => p.Order).ToList();
    }

    private DocPage Load(string path)
    {
        var text = File.ReadAllText(path);
        var front = FrontMatter().Match(text);
        if (!front.Success)
            throw new InvalidOperationException($"{path} has no front matter.");
        var fields = front.Groups[1].Value.Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(l => l.Split(':', 2)).ToDictionary(p => p[0].Trim(), p => p[1].Trim().Trim('"'));
        string Field(string name) => fields.TryGetValue(name, out var v) ? v : throw new InvalidOperationException($"{path} has no '{name}' in its front matter.");

        var section = Field("section");
        if (!Sections.Contains(section))
            throw new InvalidOperationException($"{path}: unknown section '{section}'.");
        var html = Markdown.ToHtml(text[front.Length..], Pipeline);
        html = MdLink().Replace(html, "href=\"$1.html$2\"");
        html = CodeBlock().Replace(html, m => $"<pre class=\"code\"><code>{Highlight.CSharp(WebUtility.HtmlDecode(m.Groups[1].Value))}</code></pre>");
        html = _api.LinkTypes(html);
        var toc = H2().Matches(html).Select(m => (m.Groups[1].Value, Regex.Replace(m.Groups[2].Value, "<[^>]+>", ""))).ToList();
        html = $"<h1>{WebUtility.HtmlEncode(Field("title"))}</h1>" + html;
        return new DocPage(Path.GetFileNameWithoutExtension(path), Field("title"), Field("description"), section, int.Parse(Field("order")), html, toc);
    }

    /// <summary>The sidebar, with <paramref name="current"/> marked.</summary>
    public string Nav(string current, string root)
    {
        var sb = new StringBuilder("<nav class=\"docs-nav\" aria-label=\"Documentation\">");
        foreach (var section in Sections)
        {
            sb.Append($"<p>{section}</p><ul>");
            foreach (var page in Pages.Where(p => p.Section == section))
                sb.Append(Item($"{root}docs/{page.Slug}.html", page.Title, page.Slug == current));
            sb.Append("</ul>");
        }
        sb.Append("<p>Reference</p><ul>").Append(Item($"{root}api/index.html", "API reference", current == "api")).Append("</ul></nav>");
        return sb.ToString();
    }

    private static string Item(string href, string text, bool current) =>
        $"<li><a href=\"{href}\"{(current ? " aria-current=\"page\"" : "")}>{WebUtility.HtmlEncode(text)}</a></li>";

    [GeneratedRegex(@"\A---\n(.*?)\n---\n", RegexOptions.Singleline)]
    private static partial Regex FrontMatter();

    [GeneratedRegex(@"href=""(?!https?:)([^""#]+)\.md(#[^""]*)?""")]
    private static partial Regex MdLink();

    [GeneratedRegex(@"<pre><code class=""language-csharp"">(.*?)</code></pre>", RegexOptions.Singleline)]
    private static partial Regex CodeBlock();

    [GeneratedRegex(@"<h2 id=""([^""]+)"">(.*?)</h2>")]
    private static partial Regex H2();
}
