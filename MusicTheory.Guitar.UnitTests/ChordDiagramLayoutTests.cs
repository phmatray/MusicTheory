namespace MusicTheory.Guitar.UnitTests;

public class ChordDiagramLayoutTests
{
    private static readonly GuitarVoicingProvider Provider = new();

    private static GuitarVoicing Voicing(string chord, string tab) =>
        Provider.GetVoicings(ChordSymbol.Parse(chord)).Single(v => v.ToTab() == tab);

    [Fact]
    public void OpenAm_HasNutMarkersAndThreeDots()
    {
        var layout = ChordDiagramLayout.For(Voicing("Am", "x02210"));
        layout.ShowNut.ShouldBeTrue();
        layout.PositionLabel.ShouldBeNull();
        layout.Markers.Select(m => (m.String, m.Kind)).ShouldBe([(0, MarkerKind.Muted), (1, MarkerKind.Open), (5, MarkerKind.Open)]);
        layout.Dots.Count.ShouldBe(3);
        layout.Barre.ShouldBeNull();
        layout.Dots.Select(d => d.NoteName).ShouldBe(["E", "A", "C"]);
        layout.Strings.Count.ShouldBe(6);
        layout.FretLines.Count.ShouldBe(5);
        layout.InlayDots.Count.ShouldBe(1); // fret 3
    }

    [Fact]
    public void FSharpMinor7AForm_AtNinthFret()
    {
        var layout = ChordDiagramLayout.For(Voicing("F#m7", "x 9 11 9 10 9"));
        layout.ShowNut.ShouldBeFalse();
        layout.PositionLabel.ShouldBe(9);
        layout.Barre.ShouldNotBeNull();
        layout.Barre.X1.ShouldBe(1);
        layout.Barre.X2.ShouldBe(5);
        layout.Dots.ShouldAllBe(d => d.Y >= 0 && d.Y <= layout.Height);
        layout.InlayDots.Count.ShouldBe(3); // 9, and the double dot at 12
    }

    [Fact]
    public void LeftHanded_MirrorsStrings()
    {
        var layout = ChordDiagramLayout.For(Voicing("Am", "x02210"), new DiagramOptions(LeftHanded: true));
        layout.Markers.Single(m => m.String == 0).X.ShouldBe(5);
    }

    public static TheoryData<ChordType> AllTypes() => new(Enum.GetValues<ChordType>());

    [Theory]
    [MemberData(nameof(AllTypes))]
    public void EveryVoicing_LaysOutInsideTheGrid(ChordType type)
    {
        foreach (var root in new[] { "C", "Db", "D", "Eb", "E", "F", "F#", "G", "Ab", "A", "Bb", "B" })
            foreach (var v in Provider.GetVoicings(new ChordSymbol(Note.Parse(root), type)))
            {
                var layout = ChordDiagramLayout.For(v);
                layout.Dots.ShouldAllBe(d => d.Y >= 0 && d.Y <= layout.Height && d.X >= 0 && d.X <= layout.Width, v.ToTab());
                layout.Dots.Select(d => d.String).ShouldBeUnique(v.ToTab());
            }
    }

    [Fact]
    public void AccessibleLabels()
    {
        VoicingDescriber.AccessibleLabel(Voicing("Am", "x02210")).ShouldBe("A minor, x-0-2-2-1-0");
        VoicingDescriber.AccessibleLabel(Voicing("D/F#", "2x0232")).ShouldBe("D major over F sharp, 2-x-0-2-3-2");
        VoicingDescriber.ChordName(ChordSymbol.Parse("Bbmaj7")).ShouldBe("B flat major seven");
    }
}
