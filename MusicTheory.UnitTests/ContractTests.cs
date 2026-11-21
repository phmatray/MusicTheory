namespace MusicTheory.UnitTests;

/// <summary>
/// Error messages are part of the contract: they are what a caller reads when they pass something wrong.
/// Each one is pinned here, along with the boundary cases mutation testing found unguarded.
/// </summary>
public class ContractTests
{
    private static string Message(Action act) => Record.Exception(act)!.Message;

    [Fact]
    public void ArgumentMessages()
    {
        Message(() => new Note((NoteName)7)).ShouldStartWith("Undefined note name.");
        Message(() => new Note(NoteName.C, (Alteration)3)).ShouldStartWith("Undefined alteration.");
        Message(() => new Note(NoteName.C, Alteration.Natural, 10)).ShouldStartWith("Octave must be between -1 and 9.");
        Message(() => new Note(NoteName.A).GetFrequency(0)).ShouldStartWith("A4 must be a positive, finite frequency.");
        Message(() => new Note(NoteName.C).Respell((NoteName)9)).ShouldStartWith("Undefined note name.");
        Message(() => new Interval((IntervalQuality)99, 3)).ShouldStartWith("Undefined interval quality.");
        Message(() => new Interval(IntervalQuality.Major, 3, (Direction)7)).ShouldStartWith("Undefined direction.");
        Message(() => new Interval(IntervalQuality.Perfect, 1, Direction.Down)).ShouldStartWith("A unison has no direction; use d1 to lower a note and A1 to raise it.");
        Message(() => new Chord(new Note(NoteName.C), (ChordQuality)99)).ShouldStartWith("Undefined chord quality.");
        Message(() => new Chord(new Note(NoteName.C), (ChordType)99)).ShouldStartWith("Undefined chord type.");
        Message(() => new Chord(new Note(NoteName.C), ChordType.Major).WithInversion((ChordInversion)9)).ShouldStartWith("Undefined inversion.");
        Message(() => new Scale(new Note(NoteName.C), (ScaleType)99)).ShouldStartWith("Undefined scale type.");
        Message(() => new KeySignature(new Note(NoteName.C), (KeyMode)5)).ShouldStartWith("Undefined key mode.");
        Message(() => new Duration((DurationType)99)).ShouldStartWith("Undefined duration type.");
        Message(() => Duration.CreateTuplet((DurationType)99, 3, 2)).ShouldStartWith("Undefined duration type.");
        Message(() => new Duration(DurationType.Quarter, 4)).ShouldStartWith("Dots must be between 0 and 3.");
        Message(() => new TimeSignature(0, 4)).ShouldStartWith("Numerator must be positive.");
        Message(() => new TimeSignature(4, 0)).ShouldStartWith("Denominator must be positive.");
        Message(() => new TimeSignature(4, 6)).ShouldStartWith("Denominator must be a power of 2.");
        Message(() => ChordSymbol.Parse("H7")).ShouldBe("Not a chord symbol: 'H7'.");
    }

    [Theory]
    [InlineData(-1, 60)]
    [InlineData(60, 128)]
    [InlineData(61, 60)]
    public void PianoLayout_RejectsBadRanges(int low, int high) =>
        Should.Throw<ArgumentOutOfRangeException>(() => PianoLayout.Keys(low, high));

    [Fact]
    public void PianoLayout_AcceptsTheFullMidiRangeAndASingleKey()
    {
        PianoLayout.Keys(0, 127).Count.ShouldBe(128);
        PianoLayout.Keys(60, 60).ShouldHaveSingleItem().IsBlack.ShouldBeFalse();
    }

    [Fact]
    public void CircleOfFifths_ReachesTheSevenAccidentalKeysBeforeWrapping()
    {
        KeySignature.Parse("F#").NextInCircle().ShouldBe(KeySignature.Parse("C#"));
        KeySignature.Parse("Gb").PreviousInCircle().ShouldBe(KeySignature.Parse("Cb"));
        KeySignature.Parse("D#m").NextInCircle().ShouldBe(KeySignature.Parse("A#m"));
        KeySignature.Parse("A#m").NextInCircle().ShouldBe(KeySignature.Parse("Fm"));
    }

    [Fact]
    public void Respell_ReachesTheLowestOctave() =>
        Note.Parse("C-1").Respell(NoteName.D).ShouldBe(Note.Parse("Dbb-1"));

    [Theory]
    [InlineData(1, false)] [InlineData(3, false)] [InlineData(4, false)] [InlineData(5, true)] [InlineData(9, false)] [InlineData(11, true)]
    public void TimeSignature_IsComplex_OnlyForOddNonCompoundMetersAboveThree(int numerator, bool complex) =>
        new TimeSignature(numerator, 8).IsComplex.ShouldBe(complex);

    [Fact]
    public void VoiceLeading_Boundaries()
    {
        static Note N(string s) => Note.Parse(s);
        // A perfect 8ve into an augmented 7th of the same size is not parallel octaves.
        VoiceLeadingRules.HasParallelOctaves(N("C3"), N("D3"), N("C4"), N("C##4")).ShouldBeFalse();
        // A unison into a diminished 2nd of the same size is not parallel unisons.
        VoiceLeadingRules.HasParallelUnisons(N("C4"), N("D4"), N("C4"), N("Ebb4")).ShouldBeFalse();
        VoiceLeadingRules.HasParallelUnisons(N("C4"), N("D4"), N("E4"), N("D4")).ShouldBeFalse();
        // Parallel perfect 4ths are not parallel fifths.
        VoiceLeadingRules.HasParallelFifths(N("C4"), N("D4"), N("F4"), N("G4")).ShouldBeFalse();
        // Voices on the same pitch neither cross nor overlap.
        VoiceLeadingRules.HasVoiceCrossing(N("C4"), N("C4")).ShouldBeFalse();
        VoiceLeadingRules.HasVoiceOverlap(N("C4"), N("D4"), N("C4"), N("E4")).ShouldBeFalse();
        VoiceLeadingRules.HasVoiceOverlap(N("C4"), N("G4"), N("G4"), N("A4")).ShouldBeFalse();
        VoiceLeadingRules.HasVoiceOverlap(N("C4"), N("B3"), N("G4"), N("C4")).ShouldBeFalse();
        // A single voice is its own bass and soprano and never forms hidden fifths.
        VoiceLeadingRules.ValidateVoiceLeading([N("C4")], [N("G4")]).ShouldBeEmpty();
    }

    [Fact]
    public void ValidateVoiceLeading_DescribesEachViolation()
    {
        static Note N(string s) => Note.Parse(s);
        var v = VoiceLeadingRules.ValidateVoiceLeading([N("C3"), N("C4"), N("C4"), N("E5")], [N("D3"), N("D4"), N("D4"), N("A5")]);
        v.Select(x => x.Description).ShouldBe([
            "Parallel octaves between voice 0 and voice 1",
            "Parallel octaves between voice 0 and voice 2",
            "Parallel unisons between voice 1 and voice 2",
            "Hidden fifths in outer voices",
        ], ignoreOrder: true);
        VoiceLeadingRules.ValidateVoiceLeading([N("E4"), N("C4")], [N("E4"), N("D4")])
            .Select(x => x.Description).ShouldBe(["Voice crossing between voice 0 and voice 1"]);
        VoiceLeadingRules.ValidateVoiceLeading([N("C4"), N("G4")], [N("A4"), N("B4")])
            .Select(x => x.Description).ShouldContain("Voice overlap between voice 0 and voice 1");
    }
}

public class DiatonicTriadTests
{
    [Theory]
    [InlineData("C", KeyMode.Major, "C Dm Em F G Am Bdim")]
    [InlineData("A", KeyMode.Minor, "Am Bdim C Dm Em F G")]
    [InlineData("Eb", KeyMode.Major, "Eb Fm Gm Ab Bb Cm Ddim")]
    public void GetDiatonicChords_AreTriads(string tonic, KeyMode mode, string expected) =>
        string.Join(" ", new ChordProgression(new KeySignature(Note.Parse(tonic), mode)).GetDiatonicChords()).ShouldBe(expected);
}

public class KeyDetectorScoreTests
{
    [Fact]
    public void Infer_ScoresATonicChordAboveTheSameChordElsewhere()
    {
        // C alone: C major scores 1 (diatonic) + 0.5 (tonic) + 1 (opens on it) + 1.5 (ends on it) = 4, weight 5.
        // F and G major only count it as diatonic: score 1, weight 2. Majors before minors on a tie.
        var keys = KeyDetector.Infer([ChordSymbol.Parse("C")]);
        keys.Select(k => k.Key.ToString()).ShouldBe(["C", "F", "G"]);
        keys.Select(k => k.Confidence).ShouldBe([5 / 9.0, 2 / 9.0, 2 / 9.0], tolerance: 1e-12);
        keys[0].DiatonicHits.ShouldBe(1);
        keys[0].Total.ShouldBe(1);
    }
}

public class MutationGapTests
{
    [Theory]
    [InlineData("M0")] [InlineData("m0")] [InlineData("A0")]
    public void Interval_TryParse_RejectsANumberBelowOne_WithoutThrowing(string text) =>
        Interval.TryParse(text, out _).ShouldBeFalse();

    [Fact]
    public void KeySignature_Spell_UsesTheDoubleSharpsOfATheoreticalKey()
    {
        // G# major has F##: the seventh degree is spelled from the key, not from the black-key rule.
        new KeySignature(Note.Parse("G#"), KeyMode.Major).Spell(7).GetName().ShouldBe("F##");
        new KeySignature(Note.Parse("Fb"), KeyMode.Major).Spell(9).GetName().ShouldBe("Bbb");
    }

    [Fact]
    public void Respell_ReachesTheHighestOctave() =>
        Note.Parse("B8").Respell(NoteName.C).ShouldBe(Note.Parse("Cb9"));
}
