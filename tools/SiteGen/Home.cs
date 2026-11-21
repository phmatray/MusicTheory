using System.Collections;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using MusicTheory;

namespace SiteGen;

/// <summary>
/// Runs the code samples of the home page. Each <c>&lt;figure class="sample" data-eval&gt;</c> holds one C#
/// expression; its <c>&lt;output&gt;</c> receives what the library returns. The page cannot show an output
/// the library does not produce.
/// </summary>
internal static partial class Samples
{
    public static int Count { get; private set; }

    public static async Task<string> EvaluateAsync(string html)
    {
        var options = ScriptOptions.Default.AddReferences(typeof(Note).Assembly).AddImports("System", "System.Linq", "MusicTheory");
        var results = new Dictionary<string, string>();
        foreach (Match m in Sample().Matches(html))
        {
            var code = WebUtility.HtmlDecode(m.Groups["code"].Value).Trim();
            object? value;
            try
            {
                value = await CSharpScript.EvaluateAsync<object?>(code, options);
            }
            catch (Exception e) when (e is CompilationErrorException or InvalidOperationException or ArgumentException or FormatException)
            {
                throw new InvalidOperationException($"Home page sample failed:\n{code}\n{e.Message}", e);
            }
            results[m.Value] = $"<figure class=\"sample\"><pre><code>{Highlight.CSharp(code)}</code></pre><output>{WebUtility.HtmlEncode(Format(value))}</output></figure>";
        }
        Count = results.Count;
        return Sample().Replace(html, m => results[m.Value]);
    }

    private static string Format(object? value) => value switch
    {
        null => "null",
        string s => s,
        double d => d.ToString("0.###", CultureInfo.InvariantCulture),
        IEnumerable items => string.Join("  ", items.Cast<object>().Select(Format)),
        _ => value.ToString()!,
    };

    [GeneratedRegex(@"<figure class=""sample"" data-eval>\s*<pre><code>(?<code>.*?)</code></pre>\s*<output></output>\s*</figure>", RegexOptions.Singleline)]
    private static partial Regex Sample();
}

/// <summary>The circle of fifths as the library computes it: fifteen major keys from Cb to C#, with their minors.</summary>
internal static class Circle
{
    private static IEnumerable<(KeySignature Major, KeySignature Minor)> Keys()
    {
        // Walk the line of fifths from 7 flats to 7 sharps.
        var key = KeySignature.Parse("Cb");
        for (int i = 0; i < 15; i++)
        {
            yield return (key, key.GetRelative());
            var next = key.Tonic.Transpose(Interval.Parse("P5"));
            key = new KeySignature(new Note(next.Name, next.Alteration), KeyMode.Major); // a key has no octave

        }
    }

    private static string Name(KeySignature key) => key.ToString(AccidentalStyle.Unicode);

    private static string Signature(KeySignature key) =>
        key.AccidentalCount == 0 ? "no sharps or flats" :
        $"{Math.Abs(key.AccidentalCount)} {(key.AccidentalCount > 0 ? "sharp" : "flat")}{(Math.Abs(key.AccidentalCount) > 1 ? "s" : "")}";

    private static string Scale(KeySignature key, ScaleType type) =>
        string.Join(" ", new Scale(key.Tonic, type).GetNotes().Take(7).Select(n => n.GetName(AccidentalStyle.Unicode)));

    public static string Json() => JsonSerializer.Serialize(Keys().Select(k => new
    {
        major = Name(k.Major),
        minor = Name(k.Minor),
        accidentals = k.Major.AccidentalCount,
        signature = Signature(k.Major),
        altered = k.Major.AlteredNotes.Select(n => new Note(n, k.Major.GetAlteration(n)).GetName(AccidentalStyle.Unicode)),
        majorScale = Scale(k.Major, ScaleType.Major),
        minorScale = Scale(k.Minor, ScaleType.NaturalMinor),
        pitchClass = k.Major.Tonic.PitchClass,
        enharmonic = k.Major.GetEnharmonicEquivalents().Select(Name).FirstOrDefault(),
    }));

    public static string Table()
    {
        var sb = new StringBuilder("<table class=\"circle-table\"><thead><tr><th scope=\"col\">Major</th><th scope=\"col\">Minor</th><th scope=\"col\">Signature</th><th scope=\"col\">Major scale</th></tr></thead><tbody>");
        foreach (var (major, minor) in Keys())
            sb.Append($"<tr><th scope=\"row\">{Name(major)}</th><td>{Name(minor)}</td><td>{Signature(major)}</td><td>{Scale(major, ScaleType.Major)}</td></tr>");
        return sb.Append("</tbody></table>").ToString();
    }
}
