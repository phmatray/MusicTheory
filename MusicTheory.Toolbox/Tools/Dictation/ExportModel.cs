using MusicTheory.Midi;
using MusicTheory.MusicXml;
using MusicTheory.Toolbox.Rendering;
using MusicTheory.Training;

namespace MusicTheory.Toolbox.Tools.Dictation;

/// <summary>Turns the expected melody of a dictation into file contents, built in the browser.</summary>
public static class ExportModel
{
    public static byte[] Midi(Question<IReadOnlyList<Note>> q)
    {
        using var ms = new MemoryStream();
        MidiWriter.Write(ms, MelodyMeasures.FromNotes(q.Answer), new TempoMap(90), q.Key);
        return ms.ToArray();
    }

    public static string MusicXml(Question<IReadOnlyList<Note>> q) =>
        MusicXmlWriter.Write(MelodyMeasures.FromNotes(q.Answer), q.Key).ToString();

    /// <summary>File name from the seed and the melody's place in the exercise, e.g. <c>dictation-42-1.mid</c>.</summary>
    public static string FileName(ulong seed, int number, string extension) => $"dictation-{seed}-{number}.{extension}";
}
