namespace MusicTheory.UnitTests;

public class PianoFingeringTests
{
    [Theory]
    [InlineData("C", Hand.Right, new[] { 1, 2, 3, 1, 2, 3, 4, 5 })]
    [InlineData("C", Hand.Left, new[] { 5, 4, 3, 2, 1, 3, 2, 1 })]
    [InlineData("G", Hand.Right, new[] { 1, 2, 3, 1, 2, 3, 4, 5 })]
    [InlineData("G", Hand.Left, new[] { 5, 4, 3, 2, 1, 3, 2, 1 })]
    [InlineData("D", Hand.Right, new[] { 1, 2, 3, 1, 2, 3, 4, 5 })]
    [InlineData("D", Hand.Left, new[] { 5, 4, 3, 2, 1, 3, 2, 1 })]
    [InlineData("A", Hand.Right, new[] { 1, 2, 3, 1, 2, 3, 4, 5 })]
    [InlineData("A", Hand.Left, new[] { 5, 4, 3, 2, 1, 3, 2, 1 })]
    [InlineData("E", Hand.Right, new[] { 1, 2, 3, 1, 2, 3, 4, 5 })]
    [InlineData("E", Hand.Left, new[] { 5, 4, 3, 2, 1, 3, 2, 1 })]
    [InlineData("B", Hand.Right, new[] { 1, 2, 3, 1, 2, 3, 4, 5 })]
    [InlineData("B", Hand.Left, new[] { 4, 3, 2, 1, 4, 3, 2, 1 })]
    [InlineData("F#", Hand.Right, new[] { 2, 3, 4, 1, 2, 3, 1, 2 })]
    [InlineData("F#", Hand.Left, new[] { 4, 3, 2, 1, 3, 2, 1, 4 })]
    [InlineData("Db", Hand.Right, new[] { 2, 3, 1, 2, 3, 4, 1, 2 })]
    [InlineData("Db", Hand.Left, new[] { 3, 2, 1, 4, 3, 2, 1, 3 })]
    [InlineData("Ab", Hand.Right, new[] { 3, 4, 1, 2, 3, 1, 2, 3 })]
    [InlineData("Ab", Hand.Left, new[] { 3, 2, 1, 4, 3, 2, 1, 3 })]
    [InlineData("Eb", Hand.Right, new[] { 3, 1, 2, 3, 4, 1, 2, 3 })]
    [InlineData("Eb", Hand.Left, new[] { 3, 2, 1, 4, 3, 2, 1, 3 })]
    [InlineData("Bb", Hand.Right, new[] { 4, 1, 2, 3, 1, 2, 3, 4 })]
    [InlineData("Bb", Hand.Left, new[] { 3, 2, 1, 4, 3, 2, 1, 3 })]
    [InlineData("F", Hand.Right, new[] { 1, 2, 3, 4, 1, 2, 3, 4 })]
    [InlineData("F", Hand.Left, new[] { 5, 4, 3, 2, 1, 3, 2, 1 })]
    public void ForScale_MajorMatchesPublishedTable(string key, Hand hand, int[] fingers)
    {
        PianoFingering.ForScale(new Scale(Note.Parse(key), ScaleType.Major), hand).ShouldBe(fingers);
    }

    [Fact]
    public void ForScale_HarmonicMinor()
    {
        PianoFingering.ForScale(new Scale(Note.Parse("A"), ScaleType.HarmonicMinor), Hand.Right)
            .ShouldBe([1, 2, 3, 1, 2, 3, 4, 5]);
    }

    [Fact]
    public void ForScale_TwoOctaves()
    {
        PianoFingering.ForScale(new Scale(Note.Parse("C"), ScaleType.Major), Hand.Right, octaves: 2)
            .ShouldBe([1, 2, 3, 1, 2, 3, 4, 1, 2, 3, 1, 2, 3, 4, 5]);
    }

    [Fact]
    public void ForScale_RejectsBadInput()
    {
        Should.Throw<ArgumentException>(() => PianoFingering.ForScale(new Scale(Note.Parse("C"), ScaleType.PentatonicMajor), Hand.Right));
        Should.Throw<ArgumentNullException>(() => PianoFingering.ForScale(null!, Hand.Right));
        Should.Throw<ArgumentOutOfRangeException>(() => PianoFingering.ForScale(new Scale(Note.Parse("C"), ScaleType.Major), Hand.Right, 0));
    }

    [Fact]
    public void ForScale_EveryHeptatonicTypeGivesFingers()
    {
        foreach (var type in Enum.GetValues<ScaleType>())
        {
            var scale = new Scale(Note.Parse("C"), type);
            if (scale.GetNotes().Count() != 8) continue;
            foreach (var hand in Enum.GetValues<Hand>())
                PianoFingering.ForScale(scale, hand).Count.ShouldBe(8);
        }
    }
}
