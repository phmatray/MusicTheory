using System.Xml.Linq;
using MusicTheory.Midi;
using MusicTheory.MusicXml;
using MusicTheory.Toolbox.Tools.Dictation;
using MusicTheory.Training;

namespace MusicTheory.Toolbox.UnitTests;

public class ExportModelTests
{
    private static Question<IReadOnlyList<Note>> Melody(Difficulty d) => new MelodyDrill(MelodyDrillOptions.For(d), 42).Next();

    private static int[] Pitches(IEnumerable<Measure> measures) =>
        measures.SelectMany(m => m.Events).SelectMany(e => e.Pitches).Select(n => n.MidiNumber).ToArray();

    [Theory]
    [InlineData(Difficulty.Beginner)]
    [InlineData(Difficulty.Advanced)]
    public void The_midi_file_reads_back_to_the_same_notes_in_order(Difficulty d)
    {
        var q = Melody(d);
        var import = MidiReader.Read(new MemoryStream(ExportModel.Midi(q)));
        Pitches(import.Measures).ShouldBe(q.Answer.Select(n => n.MidiNumber).ToArray());
    }

    [Theory]
    [InlineData(Difficulty.Beginner)]
    [InlineData(Difficulty.Advanced)]
    public void The_musicxml_file_reads_back_to_the_same_notes_and_key(Difficulty d)
    {
        var q = Melody(d);
        var score = MusicXmlReader.Read(XDocument.Parse(ExportModel.MusicXml(q)));
        Pitches(score.Measures).ShouldBe(q.Answer.Select(n => n.MidiNumber).ToArray());
        score.Key.ShouldBe(q.Key);
    }

    [Fact]
    public void File_names_carry_the_seed() => ExportModel.FileName(42, 2, "mid").ShouldBe("dictation-42-2.mid");
}
