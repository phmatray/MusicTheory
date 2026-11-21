using MusicTheory.Training;
using Shouldly;

namespace MusicTheory.UnitTests.Training;

public class SeededRandomTests
{
    [Fact]
    public void SplitMix64_MatchesPublishedVectors()
    {
        var r = new SeededRandom(0);
        r.NextUInt64().ShouldBe(0xE220A8397B1DCDAFUL);
        r.NextUInt64().ShouldBe(0x6E789E6AA1B965F4UL);
        r.NextUInt64().ShouldBe(0x06C45D188009454FUL);
    }

    [Fact]
    public void Next_StaysInRange()
    {
        var r = new SeededRandom(1);
        for (var i = 0; i < 10_000; i++)
        {
            r.Next(7).ShouldBeInRange(0, 6);
        }
    }

    [Fact]
    public void Next_RejectsNonPositiveBound() =>
        Should.Throw<ArgumentOutOfRangeException>(() => new SeededRandom(1).Next(0));

    [Fact]
    public void Pick_RejectsEmptyList() =>
        Should.Throw<ArgumentException>(() => new SeededRandom(1).Pick(Array.Empty<int>()));

    [Fact]
    public void Pick_RejectsNull() =>
        Should.Throw<ArgumentNullException>(() => new SeededRandom(1).Pick<int>(null!));

    [Fact]
    public void Pick_ReturnsAnElement() =>
        new SeededRandom(1).Pick(new[] { 5 }).ShouldBe(5);
}
