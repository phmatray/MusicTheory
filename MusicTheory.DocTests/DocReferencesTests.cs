using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace MusicTheory.DocTests;

/// <summary>
/// Pins that doc blocks can use the Guitar package and the four I/O packages: one scripted line per package,
/// run through the same options as the page rows.
/// </summary>
public class DocReferencesTests
{
    public static TheoryData<string, object> Lines() => new()
    {
        { "using MusicTheory.Guitar; Tuning.Standard.Strings.Count", 6 },
        { "using MusicTheory.ChordPro; ChordProDocument.Parse(\"{title: T}\").Title", "T" },
        { "using MusicTheory.Midi; new MidiWriteOptions().TicksPerQuarter", 480 },
        { "using MusicTheory.MusicXml; MusicXmlWriter.Write(new[] { new Measure(TimeSignature.CommonTime, new List<RhythmEvent>()) }).Root!.Name.LocalName", "score-partwise" },
        { "using MusicTheory.Abc; AbcTune.TryParse(\"X:1\\nK:C\\nCDEF|\", out _)", true },
    };

    [Theory]
    [MemberData(nameof(Lines))]
    public async Task OneLinePerPackageCompilesAndRuns(string script, object expected)
    {
        var value = await CSharpScript.EvaluateAsync(script, DocSampleTests.Options(), cancellationToken: TestContext.Current.CancellationToken);
        value.ShouldBe(expected);
    }
}
