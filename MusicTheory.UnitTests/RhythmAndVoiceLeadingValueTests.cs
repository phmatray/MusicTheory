namespace MusicTheory.UnitTests;

/// <summary>Version 2 behaviour of durations, time signatures and voice-leading checks.</summary>
public class RhythmAndVoiceLeadingValueTests
{
    [Theory]
    [InlineData(DurationType.Whole, "whole", "𝅝", 1.0)]
    [InlineData(DurationType.Half, "half", "𝅗𝅥", 0.5)]
    [InlineData(DurationType.Quarter, "quarter", "𝅘𝅥", 0.25)]
    [InlineData(DurationType.Eighth, "eighth", "𝅘𝅥𝅮", 0.125)]
    [InlineData(DurationType.Sixteenth, "16th", "𝅘𝅥𝅯", 0.0625)]
    [InlineData(DurationType.ThirtySecond, "32nd", "𝅘𝅥𝅰", 0.03125)]
    [InlineData(DurationType.SixtyFourth, "64th", "𝅘𝅥𝅱", 0.015625)]
    public void Duration_NamesSymbolsAndValues(DurationType type, string name, string symbol, double value)
    {
        var d = new Duration(type);
        d.ToString().ShouldBe(name);
        d.GetSymbol().ShouldBe(symbol);
        d.GetValueInWholeNotes().ShouldBe(value);
        d.IsTuplet.ShouldBeFalse();
    }

    [Theory]
    [InlineData(1, "dotted quarter", 0.375)]
    [InlineData(2, "double dotted quarter", 0.4375)]
    [InlineData(3, "triple dotted quarter", 0.46875)]
    public void Duration_Dots(int dots, string name, double value)
    {
        var d = new Duration(DurationType.Quarter, dots);
        d.ToString().ShouldBe(name);
        d.GetValueInWholeNotes().ShouldBe(value);
        d.GetSymbol().ShouldBe("𝅘𝅥" + new string('.', dots));
    }

    [Fact]
    public void Duration_Tuplet()
    {
        var triplet = Duration.CreateTuplet(DurationType.Eighth, 3, 2);
        triplet.IsTuplet.ShouldBeTrue();
        triplet.ToString().ShouldBe("3:2 eighth");
        triplet.GetValueInWholeNotes().ShouldBe(0.125 * 2 / 3);
        triplet.ShouldBe(Duration.CreateTuplet(DurationType.Eighth, 3, 2));
        triplet.ShouldNotBe(new Duration(DurationType.Eighth));
    }

    [Fact]
    public void Duration_Add_IsASumThatOnlyEqualsAnotherSum()
    {
        var sum = new Duration(DurationType.Half).Add(new Duration(DurationType.Half));
        sum.GetValueInWholeNotes().ShouldBe(1.0);
        sum.ToString().ShouldBe("1 whole notes");
        sum.ShouldNotBe(new Duration(DurationType.Whole));        // version 1 said these were equal
        new Duration(DurationType.Whole).ShouldNotBe(sum);
        sum.ShouldBe(new Duration(DurationType.Quarter, 1).Add(new Duration(DurationType.Quarter, 1)).Add(new Duration(DurationType.Quarter)));
        new Duration(DurationType.Quarter, 1).Add(new Duration(DurationType.Eighth, 1)).ToString().ShouldBe("0.5625 whole notes");
        sum.GetHashCode().ShouldBe(new Duration(DurationType.Half).Add(new Duration(DurationType.Half)).GetHashCode());
        sum.ShouldNotBe(new Duration(DurationType.Half).Add(new Duration(DurationType.Quarter)));
    }

    [Fact]
    public void Duration_RejectsBadArguments()
    {
        var q = new Duration(DurationType.Quarter);
        Should.Throw<ArgumentOutOfRangeException>(() => q.GetTimeInSeconds(0)).ParamName.ShouldBe("bpm");
        Should.Throw<ArgumentNullException>(() => q.GetValueInMeasures(null!));
        Should.Throw<ArgumentNullException>(() => q.Divide(null!));
        Should.Throw<ArgumentNullException>(() => q.Add(null!));
        q.GetTimeInSeconds(60).ShouldBe(1.0);
        q.GetValueInMeasures(TimeSignature.CommonTime).ShouldBe(0.25);
        new Duration(DurationType.Half).Divide(q).ShouldBe(2);
        q.Equals((object)"quarter").ShouldBeFalse();
    }

    [Theory]
    [InlineData(4, 4, "Common Time", "C", false, false)]
    [InlineData(2, 2, "Cut Time", "¢", false, false)]
    [InlineData(3, 4, "Waltz Time", "3/4", false, false)]
    [InlineData(6, 8, "Compound Duple", "6/8", true, false)]
    [InlineData(9, 8, "Compound Triple", "9/8", true, false)]
    [InlineData(12, 8, "Compound Quadruple", "12/8", true, false)]
    [InlineData(5, 4, null, "5/4", false, true)]
    [InlineData(7, 8, null, "7/8", false, true)]
    [InlineData(13, 16, null, "13/16", false, true)]
    [InlineData(4, 2, null, "4/2", false, false)]
    [InlineData(3, 8, null, "3/8", false, false)]
    public void TimeSignature_NamesSymbolsAndKinds(int top, int bottom, string? name, string symbol, bool compound, bool complex)
    {
        var t = new TimeSignature(top, bottom);
        t.GetCommonName().ShouldBe(name);
        t.GetSymbol().ShouldBe(symbol);
        t.IsCompound.ShouldBe(compound);
        t.IsComplex.ShouldBe(complex);
        t.IsSimple.ShouldBe(!compound && !complex);
    }

    private static Note N(string text) => Note.Parse(text);

    private static void Has(IReadOnlyList<VoiceLeadingViolation> violations, VoiceLeadingViolationType type, (int, int) voices) =>
        violations.Any(v => v.Type == type && v.VoiceIndices == voices).ShouldBeTrue($"expected {type} between voices {voices}");

    [Fact]
    public void Validate_FindsEveryKindOfViolation()
    {
        // bass C3→D3, tenor G3→A3 (parallel fifths with the bass), alto C4→D4 (octaves with the bass),
        // soprano C4→D4 (unisons with the alto).
        var v = VoiceLeadingRules.ValidateVoiceLeading([N("C3"), N("G3"), N("C4"), N("C4")], [N("D3"), N("A3"), N("D4"), N("D4")]);
        Has(v, VoiceLeadingViolationType.ParallelFifths, (0, 1));
        Has(v, VoiceLeadingViolationType.ParallelOctaves, (0, 2));
        Has(v, VoiceLeadingViolationType.ParallelUnisons, (2, 3));
        v.ShouldContain(x => x.Description == "Parallel fifths between voice 0 and voice 1");

        var leaps = VoiceLeadingRules.ValidateVoiceLeading([N("C3"), N("F4")], [N("C#5"), N("B4")]);
        leaps.Any(x => x.Type == VoiceLeadingViolationType.LargeMelodicLeap && x.Description == "Large melodic leap in voice 0" && x.VoiceIndices is null).ShouldBeTrue();
        leaps.ShouldContain(x => x.Type == VoiceLeadingViolationType.AugmentedInterval && x.Description == "Augmented interval in voice 1");
        leaps.ShouldContain(x => x.Type == VoiceLeadingViolationType.VoiceCrossing);
        leaps.ShouldContain(x => x.Type == VoiceLeadingViolationType.VoiceOverlap);

        var hidden = VoiceLeadingRules.ValidateVoiceLeading([N("C3"), N("E4")], [N("D3"), N("A4")]);
        Has(hidden, VoiceLeadingViolationType.HiddenFifths, (0, 1));

        VoiceLeadingRules.ValidateVoiceLeading([N("C4")], [N("D4")]).ShouldBeEmpty();
        VoiceLeadingRules.ValidateVoiceLeading([], []).ShouldBeEmpty();
    }

    [Fact]
    public void Validate_RejectsBadArguments()
    {
        Should.Throw<ArgumentNullException>(() => VoiceLeadingRules.ValidateVoiceLeading(null!, []));
        Should.Throw<ArgumentNullException>(() => VoiceLeadingRules.ValidateVoiceLeading([], null!));
        Should.Throw<ArgumentException>(() => VoiceLeadingRules.ValidateVoiceLeading([N("C4")], []))
            .Message.ShouldStartWith("Both chords must have the same number of voices.");
        Should.Throw<ArgumentNullException>(() => VoiceLeadingRules.HasVoiceCrossing(null!, N("C4"))).ParamName.ShouldBe("lowerVoiceNote");
    }

    [Theory]
    [InlineData("C4", "C4", "E4", "G4", MotionType.Oblique)]
    [InlineData("C4", "D4", "G4", "G4", MotionType.Oblique)]
    [InlineData("C4", "C4", "G4", "G4", MotionType.Oblique)]
    [InlineData("C4", "D4", "G4", "F4", MotionType.Contrary)]
    [InlineData("D4", "C4", "F4", "G4", MotionType.Contrary)]
    [InlineData("C4", "D4", "G4", "A4", MotionType.Parallel)]
    [InlineData("C4", "D4", "G4", "B4", MotionType.Similar)]
    [InlineData("D4", "C4", "B4", "G4", MotionType.Similar)]
    public void GetMotionType(string a1, string a2, string b1, string b2, MotionType expected) =>
        VoiceLeadingRules.GetMotionType(N(a1), N(a2), N(b1), N(b2)).ShouldBe(expected);

    [Fact]
    public void ParallelChecks_AreSpellingAware()
    {
        // C–G to D–A is parallel fifths; C–G to C#–Ab (a diminished 6th) is not.
        VoiceLeadingRules.HasParallelFifths(N("C4"), N("D4"), N("G4"), N("A4")).ShouldBeTrue();
        VoiceLeadingRules.HasParallelFifths(N("C4"), N("C#4"), N("G4"), N("Ab4")).ShouldBeFalse();
        VoiceLeadingRules.HasParallelFifths(N("C4"), N("D4"), N("G4"), N("G4")).ShouldBeFalse();
        VoiceLeadingRules.HasParallelOctaves(N("C3"), N("D3"), N("C4"), N("D4")).ShouldBeTrue();
        VoiceLeadingRules.HasParallelOctaves(N("C4"), N("D4"), N("C4"), N("D4")).ShouldBeFalse();   // unisons
        VoiceLeadingRules.HasParallelOctaves(N("C3"), N("D3"), N("C4"), N("C4")).ShouldBeFalse();
        VoiceLeadingRules.HasParallelUnisons(N("C4"), N("D4"), N("C4"), N("D4")).ShouldBeTrue();
        VoiceLeadingRules.HasParallelUnisons(N("C4"), N("D4"), N("C4"), N("E4")).ShouldBeFalse();
        VoiceLeadingRules.HasHiddenFifths(N("C3"), N("D3"), N("E4"), N("A4")).ShouldBeTrue();
        VoiceLeadingRules.HasHiddenFifths(N("C3"), N("D3"), N("G4"), N("A4")).ShouldBeFalse();      // step in the soprano
        VoiceLeadingRules.HasHiddenFifths(N("C3"), N("D3"), N("B4"), N("A4")).ShouldBeFalse();      // contrary motion
        VoiceLeadingRules.HasHiddenFifths(N("C3"), N("D3"), N("E4"), N("B4")).ShouldBeFalse();      // not a fifth
        VoiceLeadingRules.HasVoiceOverlap(N("C4"), N("A4"), N("G4"), N("B4")).ShouldBeTrue();
        VoiceLeadingRules.HasVoiceOverlap(N("C4"), N("D4"), N("G4"), N("B3")).ShouldBeTrue();
        VoiceLeadingRules.HasVoiceOverlap(N("C4"), N("D4"), N("G4"), N("A4")).ShouldBeFalse();
        VoiceLeadingRules.HasVoiceOverlap(N("G4"), N("D5"), N("C4"), N("D3")).ShouldBeFalse();     // voices already crossed
        VoiceLeadingRules.HasLargeMelodicLeap(N("C4"), N("D5"), 12).ShouldBeTrue();
        VoiceLeadingRules.HasLargeMelodicLeap(N("D5"), N("C4"), 14).ShouldBeFalse();
    }
}
