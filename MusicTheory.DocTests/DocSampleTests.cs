using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace MusicTheory.DocTests;

/// <summary>
/// Every <c>```csharp</c> block of every page in docs/ is compiled and run against the library, in page order,
/// like cells of a notebook: a block sees what earlier blocks on the same page declared. When every
/// <c>Console.WriteLine</c> of a block ends with a <c>// comment</c>, the comments are the expected output and
/// are compared line by line. A page whose code no longer compiles, throws, or prints something else fails.
/// The pages run one after another: they are rows of one test in one class, which xUnit never parallelises,
/// and that matters because each row redirects Console.Out.
/// </summary>
public partial class DocSampleTests
{
    private static readonly string Docs = Path.Combine(AppContext.BaseDirectory, "docs");

    public static TheoryData<string> Pages() => new(Directory.GetFiles(Docs, "*.md").Select(Path.GetFileName).Order()!);

    [GeneratedRegex(@"^```csharp[ \t]*\n(.*?)^```[ \t]*$", RegexOptions.Singleline | RegexOptions.Multiline)]
    private static partial Regex CSharpBlock();

    [GeneratedRegex(@"Console\.WriteLine\(.*\);\s*//\s?(.*)$")]
    private static partial Regex Expectation();

    [Theory]
    [MemberData(nameof(Pages))]
    public async Task EveryCodeBlockCompilesRunsAndPrintsWhatItSays(string page)
    {
        var options = ScriptOptions.Default
            .AddReferences(typeof(Note).Assembly)
            .AddImports("System", "System.Linq", "System.Collections.Generic", "MusicTheory");
        var text = (await File.ReadAllTextAsync(Path.Combine(Docs, page), TestContext.Current.CancellationToken)).ReplaceLineEndings("\n");
        var blocks = CSharpBlock().Matches(text).Select(m => m.Groups[1].Value).ToList();
        blocks.ShouldNotBeEmpty($"{page} has no C# block; a docs page without code is not tested here.");

        ScriptState<object>? state = null;
        var original = Console.Out;
        // The pages print numbers the way readers expect them, with a decimal point.
        System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        try
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                var output = new StringWriter();
                Console.SetOut(output);
                try
                {
                    state = state is null
                        ? await CSharpScript.RunAsync(blocks[i], options, cancellationToken: TestContext.Current.CancellationToken)
                        : await state.ContinueWithAsync(blocks[i], options, cancellationToken: TestContext.Current.CancellationToken);
                }
                catch (CompilationErrorException e)
                {
                    Assert.Fail($"{page}, block {i + 1} does not compile:\n{string.Join("\n", e.Diagnostics.Take(5))}\n---\n{blocks[i]}");
                }
                catch (Exception e)
                {
                    Assert.Fail($"{page}, block {i + 1} throws {e.GetType().Name}: {e.Message}\n---\n{blocks[i]}");
                }

                var lines = blocks[i].Split('\n');
                int writes = lines.Count(l => l.Contains("Console.WriteLine("));
                var expected = lines.Select(l => Expectation().Match(l)).Where(m => m.Success).Select(m => m.Groups[1].Value.TrimEnd()).ToList();
                if (writes > 0 && expected.Count == writes)
                {
                    var printed = output.ToString().ReplaceLineEndings("\n").TrimEnd('\n').Split('\n');
                    printed.ShouldBe(expected.ToArray(), $"{page}, block {i + 1} prints something other than its comments say:\n{blocks[i]}");
                }
            }
        }
        finally
        {
            Console.SetOut(original);
        }
    }
}
