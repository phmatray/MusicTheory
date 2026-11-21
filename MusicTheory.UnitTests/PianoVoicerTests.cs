namespace MusicTheory.UnitTests;

public class PianoVoicerTests
{
    private static string V(string symbol, PianoVoicingStyle style, int inversion = 0) =>
        string.Join(" ", PianoVoicer.Voice(ChordSymbol.Parse(symbol), style, 4, inversion));

    [Fact]
    public void Ac1_CloseDrop2Drop3()
    {
        V("Cmaj7", PianoVoicingStyle.Close).ShouldBe("C4 E4 G4 B4");
        V("Cmaj7", PianoVoicingStyle.Drop2).ShouldBe("G3 C4 E4 B4");
        V("Cmaj7", PianoVoicingStyle.Drop3).ShouldBe("E3 C4 G4 B4");
    }

    [Fact]
    public void Ac2_Inversions()
    {
        V("Cmaj7", PianoVoicingStyle.Close, 1).ShouldBe("E4 G4 B4 C5");
        V("Cmaj7", PianoVoicingStyle.Drop2, 1).ShouldBe("B3 E4 G4 C5");
    }

    [Fact]
    public void Ac3_Rootless()
    {
        V("Dm7", PianoVoicingStyle.RootlessA).ShouldBe("F4 A4 C5 E5");
        V("Dm7", PianoVoicingStyle.RootlessB).ShouldBe("C4 E4 F4 A4");
        V("G7", PianoVoicingStyle.RootlessB).ShouldBe("F4 A4 B4 D5");
    }

    [Fact]
    public void Ac4_Shell()
    {
        V("Cmaj7", PianoVoicingStyle.Shell).ShouldBe("C3 E3 B3");
        V("Cmaj7", PianoVoicingStyle.Shell, 1).ShouldBe("C3 B3 E4");
        V("C6", PianoVoicingStyle.Shell).ShouldBe("C3 E3 A3");
    }

    [Fact]
    public void Ac5_TensionSpelling()
    {
        V("G7b9", PianoVoicingStyle.RootlessA).ShouldBe("B4 D5 F5 Ab5");
        V("E7#9", PianoVoicingStyle.RootlessA).ShouldContain("F##");
    }

    [Fact]
    public void Ac6_Validation()
    {
        Should.Throw<ArgumentException>(() => V("C", PianoVoicingStyle.Drop2));
        Should.Throw<ArgumentOutOfRangeException>(() => V("Cmaj7", PianoVoicingStyle.Drop2, 4));
        Should.Throw<ArgumentOutOfRangeException>(() => V("Cmaj7", PianoVoicingStyle.RootlessA, 1));
        Should.Throw<ArgumentOutOfRangeException>(() => V("Cmaj7", PianoVoicingStyle.Close, -1));
        Should.Throw<ArgumentOutOfRangeException>(() => V("Cmaj7", (PianoVoicingStyle)99));
        Should.Throw<ArgumentNullException>(() => PianoVoicer.Voice(null!, PianoVoicingStyle.Close));
        V("C", PianoVoicingStyle.Close).ShouldBe("C4 E4 G4");
    }

    [Fact]
    public void SlashBassIgnored_SusThird()
    {
        V("C7/E", PianoVoicingStyle.Close).ShouldBe(V("C7", PianoVoicingStyle.Close));
        V("Csus4", PianoVoicingStyle.Close).ShouldBe("C4 F4 G4");
        V("Csus2", PianoVoicingStyle.Close).ShouldBe("C4 D4 G4");
    }
}
