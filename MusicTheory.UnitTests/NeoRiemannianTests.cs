namespace MusicTheory.UnitTests;

public class NeoRiemannianTests
{
    private static Chord Triad(string root, ChordType type) => new(Note.Parse(root), type);

    private static void Is(Chord actual, string root, ChordType type)
    {
        actual.Root.GetName().ShouldBe(root);
        actual.Type.ShouldBe(type);
    }

    [Fact]
    public void PRL_OfCMajor_GiveCMinor_AMinor_EMinor()
    {
        var c = Triad("C4", ChordType.Major);
        Is(NeoRiemannian.P(c), "C", ChordType.Minor);
        Is(NeoRiemannian.R(c), "A", ChordType.Minor);
        Is(NeoRiemannian.L(c), "E", ChordType.Minor);
    }

    [Fact]
    public void MinorTriads_MoveBackToMajor()
    {
        var a = Triad("A4", ChordType.Minor);
        Is(NeoRiemannian.P(a), "A", ChordType.Major);
        Is(NeoRiemannian.R(a), "C", ChordType.Major);
        Is(NeoRiemannian.L(Triad("E4", ChordType.Minor)), "C", ChordType.Major);
    }

    [Fact]
    public void Roots_AreSpelledNotRespelled()
    {
        Is(NeoRiemannian.R(Triad("Eb4", ChordType.Major)), "C", ChordType.Minor);
        Is(NeoRiemannian.L(Triad("F#4", ChordType.Major)), "A#", ChordType.Minor);
    }

    [Fact]
    public void Apply_ComposesLeftToRight()
    {
        var c = Triad("C4", ChordType.Major);
        Is(NeoRiemannian.Apply(c, "PLR"), "F", ChordType.Minor);
        Is(NeoRiemannian.Apply(c, "l p L"), "G#", ChordType.Minor);
        NeoRiemannian.Apply(c, "").ShouldBeSameAs(c);
    }

    [Fact]
    public void Results_AreRootPositionAtOctaveFour()
    {
        var chord = Triad("C2", ChordType.Major).WithInversion(ChordInversion.First);
        var moved = NeoRiemannian.R(chord);
        moved.Root.Octave.ShouldBe(4);
        moved.Inversion.ShouldBe(ChordInversion.Root);
    }

    [Fact]
    public void Invalid_Input_Throws()
    {
        Should.Throw<ArgumentException>(() => NeoRiemannian.P(Triad("G4", ChordType.Dominant7)));
        Should.Throw<ArgumentException>(() => NeoRiemannian.P(Triad("G4", ChordType.Major).AddExtension(9, IntervalQuality.Major)));
        Should.Throw<ArgumentException>(() => NeoRiemannian.Apply(Triad("C4", ChordType.Major), "pX")).Message.ShouldContain("X");
        Should.Throw<ArgumentNullException>(() => NeoRiemannian.Apply(Triad("C4", ChordType.Major), null!));
        Should.Throw<ArgumentNullException>(() => NeoRiemannian.P(null!));
    }

    public static IEnumerable<object[]> Spellings() =>
        from name in Enum.GetValues<NoteName>()
        from alteration in new[] { Alteration.Flat, Alteration.Natural, Alteration.Sharp }
        from type in new[] { ChordType.Major, ChordType.Minor }
        select new object[] { name, alteration, type };

    [Theory]
    [MemberData(nameof(Spellings))]
    public void EveryMove_IsAnInvolution(NoteName name, Alteration alteration, ChordType type)
    {
        var chord = new Chord(new Note(name, alteration), type);
        foreach (Func<Chord, Chord> move in new[] { NeoRiemannian.P, NeoRiemannian.L, NeoRiemannian.R })
        {
            Chord once;
            try { once = move(chord); }
            catch (InvalidOperationException) { continue; } // result needs a triple accidental
            move(once).ShouldBe(chord);
        }
    }
}
