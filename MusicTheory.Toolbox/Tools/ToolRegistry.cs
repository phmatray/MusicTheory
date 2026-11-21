using MusicTheory.Toolbox.Tools.Chords;
using MusicTheory.Toolbox.Tools.Analysis;
using MusicTheory.Toolbox.Tools.Circle;
using MusicTheory.Toolbox.Tools.Dictation;
using MusicTheory.Toolbox.Tools.Finder;
using MusicTheory.Toolbox.Tools.Help;
using MusicTheory.Toolbox.Tools.Intervals;
using MusicTheory.Toolbox.Tools.Progressions;
using MusicTheory.Toolbox.Tools.Scales;
using MusicTheory.Toolbox.Tools.Transposer;

namespace MusicTheory.Toolbox.Tools;

/// <summary>A tool the shell can show: its URL id, the string key of its title, its component and the URL parameters it reads (besides <c>t</c>, <c>lang</c>, <c>v</c>).</summary>
public sealed record ToolInfo(string Id, string TitleKey, Type Component, IReadOnlyList<string> Parameters);

public static class ToolRegistry
{
    /// <summary>Every tool; a new tool adds itself here and in <c>Strings.Tables</c>.</summary>
    public static IReadOnlyList<ToolInfo> All { get; } =
    [
        new("circle", "circle.title", typeof(CircleTool), ["key"]),
        new("chords", "chords.title", typeof(ChordDrillTool), ["d", "n", "seed", "key"]),
        new("progressions", "progressions.title", typeof(ProgressionDrillTool), ["d", "n", "seed", "key"]),
        new("scales", "scales.title", typeof(ScaleDrillTool), ["d", "n", "seed", "key"]),
        new("dictation", "dictation.title", typeof(DictationTool), ["d", "n", "seed", "key"]),
        new("intervals", "intervals.title", typeof(IntervalDrillTool), ["d", "n", "seed", "key"]),
        new("finder", "finder.title", typeof(FinderTool), ["pcs", "chord", "scale"]),
        new("transpose", "transpose.title", typeof(TransposerTool), ["inst", "dir", "key", "chords"]),
        new("fretboard", "fretboard.title", typeof(Fretboard.FretboardTool), ["tuning", "show", "root", "type", "frets"]),
        new("analysis", "analysis.title", typeof(AnalysisTool), ["chords", "key"]),
        new("voicings", "voicings.title", typeof(Voicings.VoicingsTool), ["chord", "tuning"]),
        new("piano", "piano.title", typeof(Piano.PianoTool), ["root", "type", "hand", "oct", "triad"]),
        new("help", "help.title", typeof(HelpTool), []),
    ];

    public static ToolInfo? Find(string id) => All.FirstOrDefault(t => t.Id == id);
}
