namespace MusicTheory.UnitTests;

public class VoiceLeaderTests
{
    private static ChordSymbol[] Parse(params string[] s) => [.. s.Select(ChordSymbol.Parse)];

    private static int Motion(IReadOnlyList<Note> a, IReadOnlyList<Note> b) =>
        a.Select((n, i) => Math.Abs(n.MidiNumber - b[i].MidiNumber)).Sum();

    private static bool Parallel(IReadOnlyList<Note> a, IReadOnlyList<Note> b) =>
        VoiceLeadingRules.ValidateVoiceLeading([.. a], [.. b]).Any(x =>
            x.Type is VoiceLeadingViolationType.ParallelFifths or VoiceLeadingViolationType.ParallelOctaves);

    [Fact]
    public void RootlessIiVI_AlternatesAAndB()
    {
        var voicings = VoiceLeader.Lead(Parse("Dm7", "G7", "Cmaj7"), PianoVoicingStyle.RootlessA);
        voicings.Select(v => string.Join(" ", v)).ShouldBe(["F4 A4 C5 E5", "F4 A4 B4 D5", "E4 G4 B4 D5"]);
    }

    [Fact]
    public void Drop2_NeverMovesMoreThanFixedVoicing()
    {
        var chords = Parse("Cmaj7", "Am7", "Dm7", "G7");
        var led = VoiceLeader.Lead(chords, PianoVoicingStyle.Drop2);
        var fixedV = chords.Select(c => PianoVoicer.Voice(c, PianoVoicingStyle.Drop2, 4, 0)).ToList();
        for (int i = 1; i < chords.Length; i++)
            Motion(led[i - 1], led[i]).ShouldBeLessThanOrEqualTo(Motion(fixedV[i - 1], fixedV[i]));
    }

    [Fact]
    public void Drop2_AvoidsParallelsWhenPossible()
    {
        var chords = new ChordProgression(KeySignature.Parse("C")).GetDiatonicSeventhChords().Select(c => c.ToChordSymbol()).ToArray();
        int checkedPairs = 0;
        foreach (var seq in Sequences(chords, 4))
        {
            var led = VoiceLeader.Lead(seq, PianoVoicingStyle.Drop2);
            for (int i = 1; i < seq.Length; i++)
            {
                var clean = Enumerable.Range(0, 4).SelectMany(inv => Enumerable.Range(1, 7).Select(o => (inv, o)))
                    .Select(t => PianoVoicer.Voice(seq[i], PianoVoicingStyle.Drop2, t.o, t.inv))
                    .Where(v => v.All(n => n.MidiNumber is >= 40 and <= 84))
                    .Any(v => !Parallel(led[i - 1], v));
                if (clean)
                {
                    Parallel(led[i - 1], led[i]).ShouldBeFalse();
                    checkedPairs++;
                }
            }
        }
        checkedPairs.ShouldBeGreaterThan(0);
    }

    private static IEnumerable<ChordSymbol[]> Sequences(ChordSymbol[] pool, int length) =>
        length == 0
            ? [[]]
            : Sequences(pool, length - 1).SelectMany(s => pool.Select(c => (ChordSymbol[])[.. s, c]));

    [Fact]
    public void EmptyAndInvalidInput()
    {
        VoiceLeader.Lead([], PianoVoicingStyle.Drop2).ShouldBeEmpty();
        Should.Throw<ArgumentNullException>(() => VoiceLeader.Lead(null!, PianoVoicingStyle.Drop2));
        Should.Throw<ArgumentException>(() => VoiceLeader.Lead(Parse("C", "F"), PianoVoicingStyle.Drop2))
            .Message.ShouldContain("Chord 0");
        Should.Throw<ArgumentException>(() => VoiceLeader.Lead(Parse("Cmaj7", "F"), PianoVoicingStyle.Drop2))
            .Message.ShouldContain("Chord 1");
    }

    [Fact]
    public void LaterVoicingsStayInRange()
    {
        var led = VoiceLeader.Lead(Parse("Cmaj7", "Fmaj7", "G7", "Cmaj7"), PianoVoicingStyle.Shell);
        led.Skip(1).SelectMany(v => v).ShouldAllBe(n => n.MidiNumber >= 40 && n.MidiNumber <= 84);
    }

    [Fact]
    public void ChordsOfDifferentSizeUseNearestNote()
    {
        var led = VoiceLeader.Lead(Parse("Cmaj7", "F"), PianoVoicingStyle.Close);
        led.Count.ShouldBe(2);
    }
}
