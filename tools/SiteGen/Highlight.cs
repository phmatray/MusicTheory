using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace SiteGen;

/// <summary>Build-time C# highlighting: comments, strings, keywords and type names, as spans. No script on the page.</summary>
internal static partial class Highlight
{
    private static readonly HashSet<string> Keywords =
    [
        "var", "new", "using", "return", "if", "else", "foreach", "for", "in", "while", "switch", "case", "break", "true", "false",
        "null", "public", "private", "static", "class", "record", "struct", "void", "int", "double", "string", "bool", "out", "is",
        "not", "and", "or", "try", "catch", "throw", "await", "async", "readonly", "sealed", "this", "default", "with", "char",
    ];

    public static string CSharp(string code)
    {
        var sb = new StringBuilder();
        foreach (Match m in Token().Matches(code))
        {
            var text = WebUtility.HtmlEncode(m.Value);
            if (m.Groups["comment"].Success) sb.Append($"<span class=\"c\">{text}</span>");
            else if (m.Groups["string"].Success) sb.Append($"<span class=\"s\">{text}</span>");
            else if (m.Groups["word"].Success && Keywords.Contains(m.Value)) sb.Append($"<span class=\"k\">{text}</span>");
            else if (m.Groups["word"].Success && char.IsUpper(m.Value[0])) sb.Append($"<span class=\"t\">{text}</span>");
            else if (m.Groups["number"].Success) sb.Append($"<span class=\"n\">{text}</span>");
            else sb.Append(text);
        }
        return sb.ToString();
    }

    [GeneratedRegex(@"(?<comment>//[^\n]*)|(?<string>\$?@?""(?:[^""\\]|\\.)*"")|(?<string>'(?:[^'\\]|\\.)')|(?<word>[A-Za-z_][A-Za-z0-9_]*)|(?<number>\d+(?:\.\d+)?)|(?<other>[\s\S])")]
    private static partial Regex Token();
}
