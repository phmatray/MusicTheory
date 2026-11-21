namespace MusicTheory.UnitTests;

public class SwingTests
{
    private static readonly Rational Eighth = Rational.Of(1, 8);

    [Fact]
    public void OffBeatEighth_LandsTwoThirdsThroughTheBeat()
    {
        Swing.Apply(Eighth, Eighth, 2.0 / 3.0).ShouldBe(1.0 / 6, 1e-12);
    }

    [Fact]
    public void OnBeat_DoesNotMove()
    {
        Swing.Apply(Rational.Of(1, 4), Eighth, 2.0 / 3.0).ShouldBe(0.25);
        Swing.Apply(Rational.Zero, Eighth, 2.0 / 3.0).ShouldBe(0.0);
    }

    [Fact]
    public void StraightRatio_ReturnsEveryPositionUnchanged()
    {
        for (int i = 0; i < 40; i++)
            Swing.Apply(Rational.Of(i, 16), Eighth, 0.5).ShouldBe(i / 16.0, 1e-12);
    }

    [Fact]
    public void Sixteenths_MoveOneSixteenthToOneTwelfth()
    {
        Swing.Apply(Rational.Of(1, 16), Rational.Of(1, 16), 2.0 / 3.0).ShouldBe(1.0 / 12, 1e-12);
    }

    [Fact]
    public void SecondBar_RepeatsFirstBarOffsets()
    {
        Swing.Apply(Rational.One + Eighth, Eighth, 2.0 / 3.0).ShouldBe(1 + 1.0 / 6, 1e-12);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(1.0)]
    [InlineData(double.NaN)]
    public void InvalidRatio_Throws(double ratio)
    {
        Should.Throw<ArgumentOutOfRangeException>(() => Swing.Apply(Eighth, Eighth, ratio)).ParamName.ShouldBe("ratio");
    }

    [Fact]
    public void InvalidSubdivisionOrPosition_Throws()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => Swing.Apply(Eighth, Rational.Zero, 0.6)).ParamName.ShouldBe("subdivision");
        Should.Throw<ArgumentOutOfRangeException>(() => Swing.Apply(Rational.Of(-1, 8), Eighth, 0.6)).ParamName.ShouldBe("position");
    }
}
