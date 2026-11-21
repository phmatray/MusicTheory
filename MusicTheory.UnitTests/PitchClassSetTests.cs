namespace MusicTheory.UnitTests;

public class PitchClassSetTests
{
    [Fact]
    public void Of_IgnoresDuplicatesAndOrder()
    {
        PitchClassSet.Of(4, 0, 8, 4).ShouldBe(PitchClassSet.Of(0, 4, 8));
        PitchClassSet.Of(4, 0, 8, 4).Cardinality.ShouldBe(3);
    }

    [Fact]
    public void Of_ReducesModulo12() => PitchClassSet.Of(-1, 12).PitchClasses.ShouldBe([0, 11]);

    [Fact]
    public void FromNotes_UsesPitchClass() =>
        PitchClassSet.FromNotes([Note.Parse("C4"), Note.Parse("E4"), Note.Parse("G#4")]).ShouldBe(PitchClassSet.Of(0, 4, 8));

    [Fact]
    public void Contains_ReducesModulo12()
    {
        PitchClassSet.Of(0, 4).Contains(16).ShouldBeTrue();
        PitchClassSet.Of(0, 4).Contains(5).ShouldBeFalse();
    }

    [Fact]
    public void Transpose_MovesEveryMember()
    {
        var major = PitchClassSet.Of(0, 4, 7);
        major.Transpose(2).ShouldBe(PitchClassSet.Of(2, 6, 9));
        major.Transpose(12).ShouldBe(major);
        major.Transpose(-1).ShouldBe(PitchClassSet.Of(11, 3, 6));
    }

    [Fact]
    public void Invert_ReflectsAroundN()
    {
        var major = PitchClassSet.Of(0, 4, 7);
        major.Invert().ShouldBe(PitchClassSet.Of(0, 5, 8));
        major.Invert(7).ShouldBe(PitchClassSet.Of(0, 3, 7));
        major.Invert().Invert().ShouldBe(major);
    }

    [Fact]
    public void Complement_HoldsTheOtherPitchClasses()
    {
        var major = PitchClassSet.Of(0, 4, 7);
        major.Complement.Cardinality.ShouldBe(9);
        major.Complement.Complement.ShouldBe(major);
        PitchClassSet.Empty.Complement.ShouldBe(PitchClassSet.Chromatic);
    }

    [Fact]
    public void IntervalClassVector_CountsPairs()
    {
        PitchClassSet.Of(0, 4, 7).IntervalClassVector.ShouldBe([0, 0, 1, 1, 1, 0]);
        PitchClassSet.Chromatic.IntervalClassVector.ShouldBe([12, 12, 12, 12, 12, 6]);
        PitchClassSet.Empty.IntervalClassVector.ShouldBe([0, 0, 0, 0, 0, 0]);
    }

    [Fact]
    public void ToString_ListsMembers() => PitchClassSet.Of(7, 0, 4).ToString().ShouldBe("{0,4,7}");

    [Fact]
    public void NormalForm_PacksToTheLeft()
    {
        PitchClassSet.Of(0, 1, 4).NormalForm.ShouldBe([0, 1, 4]);
        PitchClassSet.Of(8, 0, 4).NormalForm.ShouldBe([0, 4, 8]);
        PitchClassSet.Of(11, 0, 3).NormalForm.ShouldBe([11, 0, 3]);
        PitchClassSet.Empty.NormalForm.ShouldBeEmpty();
    }

    [Fact]
    public void PrimeForm_StartsAtZero()
    {
        PitchClassSet.Of(11, 0, 3).PrimeForm.ShouldBe([0, 1, 4]);
        PitchClassSet.Of(0, 4, 7).PrimeForm.ShouldBe([0, 3, 7]);
        PitchClassSet.Of(0, 3, 7).PrimeForm.ShouldBe([0, 3, 7]);
        PitchClassSet.Empty.PrimeForm.ShouldBeEmpty();
        PitchClassSet.Of(5).PrimeForm.ShouldBe([0]);
    }

    [Fact]
    public void Equivalence()
    {
        var major = PitchClassSet.Of(0, 4, 7);
        major.IsTranspositionOf(PitchClassSet.Of(2, 6, 9)).ShouldBeTrue();
        major.IsTranspositionOf(PitchClassSet.Of(0, 3, 7)).ShouldBeFalse();
        major.IsTranspositionOrInversionOf(PitchClassSet.Of(0, 3, 7)).ShouldBeTrue();
        major.IsTranspositionOrInversionOf(PitchClassSet.Of(0, 1, 2)).ShouldBeFalse();
    }
}
