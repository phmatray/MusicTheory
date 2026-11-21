using Shouldly;

namespace MusicTheory.UnitTests;

public class RationalTests
{
    [Fact]
    public void Of_ReducesToLowestTerms() => Rational.Of(2, 4).ShouldBe(Rational.Of(1, 2));

    [Fact]
    public void Of_CarriesTheSignOnTheNumerator()
    {
        var r = Rational.Of(1, -2);
        r.Numerator.ShouldBe(-1);
        r.Denominator.ShouldBe(2);
        r.ToString().ShouldBe("-1/2");
        Rational.Of(-3, -6).ToString().ShouldBe("1/2");
    }

    [Fact]
    public void ToString_OmitsUnitDenominator() => Rational.Of(3, 1).ToString().ShouldBe("3");

    [Fact]
    public void ToString_ShowsFraction() => Rational.Of(3, 8).ToString().ShouldBe("3/8");

    [Fact]
    public void Zero_IsTheDefaultValue()
    {
        default(Rational).ShouldBe(Rational.Zero);
        Rational.Of(0, 5).ShouldBe(Rational.Zero);
        Rational.Zero.Denominator.ShouldBe(1);
        Rational.One.ShouldBe(Rational.Of(5, 5));
    }

    [Fact]
    public void Arithmetic_IsExact()
    {
        (Rational.Of(1, 3) + Rational.Of(1, 6)).ShouldBe(Rational.Of(1, 2));
        (Rational.Of(1, 2) - Rational.Of(1, 3)).ShouldBe(Rational.Of(1, 6));
        (Rational.Of(2, 3) * Rational.Of(3, 4)).ShouldBe(Rational.Of(1, 2));
        (Rational.Of(1, 2) / Rational.Of(1, 4)).ShouldBe(Rational.Of(2));
        (Rational.Of(1, 2) / Rational.Of(-1, 4)).ShouldBe(Rational.Of(-2));
    }

    [Fact]
    public void Comparison_Orders()
    {
        (Rational.Of(1, 3) < Rational.Of(1, 2)).ShouldBeTrue();
        (Rational.Of(1, 2) < Rational.Of(1, 3)).ShouldBeFalse();
        (Rational.Of(1, 2) > Rational.Of(1, 3)).ShouldBeTrue();
        (Rational.Of(1, 3) > Rational.Of(1, 2)).ShouldBeFalse();
        (Rational.Of(1, 2) <= Rational.Of(2, 4)).ShouldBeTrue();
        (Rational.Of(1, 2) <= Rational.Of(1, 3)).ShouldBeFalse();
        (Rational.Of(1, 2) >= Rational.Of(2, 4)).ShouldBeTrue();
        (Rational.Of(1, 3) >= Rational.Of(1, 2)).ShouldBeFalse();
        Rational.Of(1, 3).CompareTo(Rational.Of(1, 2)).ShouldBeLessThan(0);
        Rational.Of(-1, 2).CompareTo(Rational.Of(1, 3)).ShouldBeLessThan(0);
    }

    [Fact]
    public void ToDouble_IsTheNearestDouble() => Rational.Of(3, 8).ToDouble().ShouldBe(0.375);

    [Fact]
    public void ZeroDenominator_Throws()
    {
        Should.Throw<DivideByZeroException>(() => Rational.Of(1, 0));
        Should.Throw<DivideByZeroException>(() => Rational.One / Rational.Zero);
    }

    [Fact]
    public void Overflow_Throws()
    {
        Should.Throw<OverflowException>(() => Rational.Of(long.MaxValue) + Rational.One);
        Should.Throw<OverflowException>(() => Rational.Of(long.MaxValue) - Rational.Of(-1));
        Should.Throw<OverflowException>(() => Rational.Of(long.MaxValue) * Rational.Of(2));
        Should.Throw<OverflowException>(() => Rational.Of(long.MaxValue).CompareTo(Rational.Of(1, 2)));
        Should.Throw<OverflowException>(() => Rational.Of(1, long.MinValue));
    }
}
