using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace MusicTheory.Guitar.UnitTests;

/// <summary>
/// Approval tests: the full, ordered output of the voicing provider and the diagram layout for a fixed set of
/// chords, options and tunings, compared line by line with Golden/*.txt. Ranking, difficulty, fingering and
/// geometry are pinned, so any change to them shows up as a diff in review. Run with UPDATE_GOLDEN=1 to
/// accept an intended change.
/// </summary>
public class GoldenTests
{
    private static readonly GuitarVoicingProvider Provider = new();

    private static readonly (string Chord, VoicingOptions? Options, string Label)[] Cases =
    [
        ("C", null, "default"), ("Am", null, "default"), ("F", null, "default"), ("G7", null, "default"),
        ("Bb7", null, "default"), ("E7#9", null, "default"), ("C/G", null, "default"), ("Cmaj13", null, "default"),
        ("C5", null, "default"), ("C7alt", null, "default"), ("Dm7b5", null, "default"), ("Eb6/9", null, "default"),
        ("F#m", null, "default"), ("Gsus4", null, "default"), ("Ab", null, "default"), ("Cm11", null, "default"),
        ("C", new VoicingOptions(Capo: 3), "capo 3"), ("C", new VoicingOptions(MaxFret: 7), "max fret 7"),
        ("C", new VoicingOptions(MaxStretch: 3), "stretch 3"), ("C", new VoicingOptions(Max: 3), "max 3"),
        ("C/E", new VoicingOptions(IncludeSlashBass: false), "no slash bass"),
        ("D", new VoicingOptions(Tuning: Tuning.DropD), "drop D"), ("Dsus4", new VoicingOptions(Tuning: Tuning.DADGAD), "DADGAD"),
        ("G", new VoicingOptions(Tuning: Tuning.OpenG), "open G"), ("E", new VoicingOptions(Tuning: Tuning.HalfStepDown), "half step down"),
        ("C", new VoicingOptions(Tuning: new Tuning("Ukulele", new[] { "G4", "C4", "E4", "A4" }.Select(Note.Parse).ToArray())), "ukulele"),
        ("C/Bb", null, "bass outside the chord"), ("D/C", null, "bass outside the chord"),
        ("C7", new VoicingOptions(MaxFret: 8, Max: 50), "max fret 8, all"), ("G", new VoicingOptions(MaxFret: 5, Max: 50), "max fret 5, all"),
        ("E", new VoicingOptions(MaxStretch: 2, Max: 50), "stretch 2, all"), ("Bb", new VoicingOptions(MaxFret: 6, Max: 50), "max fret 6, all"),
    ];

    private static readonly ChordType[] CommonTypes =
    [
        ChordType.Major, ChordType.Minor, ChordType.Dominant7, ChordType.Major7, ChordType.Minor7, ChordType.Sus2, ChordType.Sus4,
        ChordType.Diminished, ChordType.Augmented, ChordType.Major6, ChordType.Minor6, ChordType.MajorAdd9, ChordType.Dominant9,
        ChordType.Dominant7Sus4, ChordType.Power5, ChordType.HalfDiminished7, ChordType.Diminished7,
    ];

    // The three easiest voicings of every common chord on a natural root: every curated open shape shows up here.
    [Fact]
    public void Catalog() => Approve("catalog.txt", sb =>
    {
        foreach (var root in Enum.GetValues<NoteName>())
            foreach (var type in CommonTypes)
            {
                var chord = new ChordSymbol(new Note(root), type);
                var voicings = Provider.GetVoicings(chord, new VoicingOptions(Max: 3));
                sb.AppendLine(Invariant($"{chord,-8} {string.Join("  ", voicings.Select(v => Invariant($"{v.ToTab()} {v.ShapeName ?? "gen"} {v.Difficulty}")))}"));
            }
    });

    [Fact]
    public void Sound_SpellsANonChordToneOnTheChordsSide()
    {
        // A hand-written voicing with a C#/Db that is not in the chord: flat roots spell it Db, others C#.
        var frets = new int?[] { null, 4, 3, 3, null, null };
        var flat = new GuitarVoicing(ChordSymbol.Parse("Bb"), frets, [null, 3, 1, 2, null, null], null, 1, VoicingSource.Generated, null, 0);
        var sharp = flat with { Chord = ChordSymbol.Parse("B") };
        string.Join(" ", flat.Sound(Tuning.Standard).Select(n => n?.ToString() ?? "x")).ShouldBe("x Db3 F3 Bb3 x x");
        string.Join(" ", sharp.Sound(Tuning.Standard).Select(n => n?.ToString() ?? "x")).ShouldBe("x C#3 F3 Bb3 x x");
        // A slash bass that doubles a chord tone takes the chord tone's spelling: C/Fb sounds an E in the bass.
        Provider.GetVoicings(ChordSymbol.Parse("C/Fb"))[0].Sound(Tuning.Standard).OfType<Note>().First().ShouldBe(Note.Parse("E3"));
    }

    [Fact]
    public void Voicings() => Approve("voicings.txt", sb =>
    {
        foreach (var (chord, options, label) in Cases)
        {
            sb.AppendLine($"## {chord} ({label})");
            var tuning = options?.Tuning ?? Tuning.Standard;
            foreach (var v in Provider.GetVoicings(ChordSymbol.Parse(chord), options))
            {
                var fingers = string.Concat(v.Fingers.Select(f => f?.ToString() ?? "-"));
                var barre = v.Barre is null ? "" : $" barre {v.Barre.Fret}:{v.Barre.LowString}-{v.Barre.HighString}";
                var sound = string.Join(" ", v.Sound(tuning, options?.Capo ?? 0).Select(n => n?.ToString() ?? "x"));
                sb.AppendLine(Invariant($"{v.ToTab(),-18} {v.Source,-12} {v.ShapeName ?? "-",-7} base {v.BaseFret,2}  difficulty {v.Difficulty,5}  fingers {fingers}{barre}  sounds {sound}  midi {string.Join(",", v.ToMidi(tuning, options?.Capo ?? 0))}"));
            }
        }
    });

    [Fact]
    public void Diagrams() => Approve("diagrams.txt", sb =>
    {
        void Layout(string chord, string tab, DiagramOptions? options, string label)
        {
            var v = Provider.GetVoicings(ChordSymbol.Parse(chord), new VoicingOptions(Max: 50)).Single(x => x.ToTab() == tab);
            var d = ChordDiagramLayout.For(v, options);
            sb.AppendLine($"## {chord} {tab} ({label})");
            sb.AppendLine(Invariant($"size {d.Width}x{d.Height} nut {d.ShowNut} position {d.PositionLabel?.ToString() ?? "-"}"));
            sb.AppendLine("strings " + string.Join(" ", d.Strings.Select(l => Invariant($"({l.X1},{l.Y1})-({l.X2},{l.Y2})"))));
            sb.AppendLine("frets " + string.Join(" ", d.FretLines.Select(l => Invariant($"({l.X1},{l.Y1})-({l.X2},{l.Y2})"))));
            sb.AppendLine("markers " + string.Join(" ", d.Markers.Select(m => Invariant($"{m.String}:{m.Kind}@({m.X},{m.Y})"))));
            sb.AppendLine("dots " + string.Join(" ", d.Dots.Select(p => Invariant($"{p.String}/{p.Fret}:{p.Finger?.ToString() ?? "-"}:{p.NoteName}@({p.X},{p.Y})"))));
            sb.AppendLine("barre " + (d.Barre is { } b ? Invariant($"{b.Fret}@({b.X1}-{b.X2},{b.Y})") : "-"));
            sb.AppendLine("inlays " + string.Join(" ", d.InlayDots.Select(p => Invariant($"({p.X},{p.Y})"))));
        }
        Layout("C", "x32010", null, "default");
        Layout("F", "133211", new DiagramOptions(Frets: 5, StringSpacing: 10, FretSpacing: 12), "spaced, 5 frets");
        Layout("F", "x 8 10 10 10 8", new DiagramOptions(LeftHanded: true), "left-handed, position 8");
        Layout("Ab", "466544", new DiagramOptions(Frets: 4), "barre at 4");
        Layout("Ab", "x 11 13 13 13 11", new DiagramOptions(Frets: 4), "position 11, octave inlays");
        // A voicing that frets the low string, the one string drop D retunes.
        var d = Provider.GetVoicings(ChordSymbol.Parse("G"), new VoicingOptions(Tuning: Tuning.DropD, Max: 50)).First(v => v.Frets[0] > 0);
        var dropD = ChordDiagramLayout.For(d, tuning: Tuning.DropD);
        sb.AppendLine($"## G {d.ToTab()} (drop D tuning)");
        sb.AppendLine("dots " + string.Join(" ", dropD.Dots.Select(p => $"{p.String}/{p.Fret}:{p.NoteName}")));
    });

    [Fact]
    public void SpokenNamesAndTunings() => Approve("spoken.txt", sb =>
    {
        foreach (var type in Enum.GetValues<ChordType>())
            sb.AppendLine($"{type}: {VoicingDescriber.ChordName(new ChordSymbol(Note.Parse("Bb"), type, Note.Parse("F##")))}");
        foreach (var alteration in Enum.GetValues<Alteration>())
            sb.AppendLine($"{alteration}: {VoicingDescriber.ChordName(new ChordSymbol(new Note(NoteName.E, alteration), ChordType.Major))}");
        foreach (var tuning in new[] { Tuning.Standard, Tuning.DropD, Tuning.DADGAD, Tuning.OpenG, Tuning.HalfStepDown })
            sb.AppendLine($"{tuning.Name}: {string.Join(" ", tuning.Strings)}");
    });

    [Fact]
    public void SpokenNamesFrench() => Approve("spoken.fr.txt", sb =>
    {
        var fr = new CultureInfo("fr");
        foreach (var type in Enum.GetValues<ChordType>())
            sb.AppendLine($"{type}: {VoicingDescriber.ChordName(new ChordSymbol(Note.Parse("Bb"), type, Note.Parse("F##")), fr)}");
        foreach (var alteration in Enum.GetValues<Alteration>())
            sb.AppendLine($"{alteration}: {VoicingDescriber.ChordName(new ChordSymbol(new Note(NoteName.E, alteration), ChordType.Major), fr)}");
    });

    [Fact]
    public void Tuning_IsAValue()
    {
        Tuning.Standard.ShouldBe(Tuning.Standard);
        Tuning.Standard.GetHashCode().ShouldBe(Tuning.Standard.GetHashCode());
        Tuning.Standard.ShouldNotBe(Tuning.DropD);
        Tuning.Standard.ShouldNotBe(Tuning.Standard with { Name = "Mine" });
        Tuning.Standard.Equals(null).ShouldBeFalse();
        Should.Throw<ArgumentException>(() => new Tuning("x", [Note.Parse("E2"), null!, Note.Parse("E4")]))
            .Message.ShouldStartWith("A tuning needs at least one string and no null notes.");
    }

    private static string Invariant(FormattableString s) => s.ToString(CultureInfo.InvariantCulture);

    private static void Approve(string file, Action<StringBuilder> write, [CallerFilePath] string source = "")
    {
        var sb = new StringBuilder();
        write(sb);
        var actual = sb.ToString().ReplaceLineEndings("\n");
        // The golden files are copied next to the binary: CI builds map source paths (/_/...), so the
        // caller's file path only serves UPDATE_GOLDEN, which runs on a developer machine.
        if (Environment.GetEnvironmentVariable("UPDATE_GOLDEN") == "1")
            File.WriteAllText(Path.Combine(Path.GetDirectoryName(source)!, "Golden", file), actual);
        var expected = Environment.GetEnvironmentVariable("UPDATE_GOLDEN") == "1"
            ? actual
            : File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Golden", file)).ReplaceLineEndings("\n");
        var diff = expected.Split('\n').Zip(actual.Split('\n')).Select((p, i) => (p, i)).FirstOrDefault(x => x.p.First != x.p.Second);
        actual.ShouldBe(expected, $"{file} differs at line {diff.i + 1}:\n- {diff.p.First}\n+ {diff.p.Second}\nRun with UPDATE_GOLDEN=1 if the change is intended.");
    }
}
