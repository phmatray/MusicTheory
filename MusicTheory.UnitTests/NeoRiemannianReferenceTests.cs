namespace MusicTheory.UnitTests;

/// <summary>Replays music21's analysis.neoRiemannian P, L and R (Reference/neoriemannian.csv).</summary>
public class NeoRiemannianReferenceTests
{
    public static IEnumerable<string[]> Rows() =>
        File.ReadLines(Path.Combine(AppContext.BaseDirectory, "Reference", "neoriemannian.csv"))
            .Skip(1).Select(l => l.Split(','));

    [Theory]
    [MemberData(nameof(Rows))]
    public void Move_MatchesMusic21(string root, string type, string move, string resultRoot, string resultType)
    {
        var triad = new Chord(Note.Parse(root + "4"), Enum.Parse<ChordType>(type, true));
        var result = NeoRiemannian.Apply(triad, move);
        result.Root.GetName().ShouldBe(resultRoot);
        result.Type.ShouldBe(Enum.Parse<ChordType>(resultType, true));
    }
}
