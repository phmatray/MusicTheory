using MusicTheory.Toolbox.Drills;
using MusicTheory.Toolbox.I18n;
using MusicTheory.Training;

namespace MusicTheory.Toolbox.UnitTests;

public class TypeNamesTests
{
    public static TheoryData<Difficulty> Levels() => new(Enum.GetValues<Difficulty>());

    [Theory]
    [MemberData(nameof(Levels))]
    public void Every_chord_of_the_pool_has_a_distinct_label_in_both_languages(Difficulty d)
    {
        foreach (var lang in Enum.GetValues<Lang>())
        {
            var pool = ChordDrillOptions.For(d).Pool;
            var labels = pool.Select(t => TypeNames.Label(t, lang)).ToList();
            labels.Zip(pool).ShouldAllBe(p => p.First != p.Second.ToString());
            labels.Distinct().Count().ShouldBe(labels.Count);
        }
    }

    [Theory]
    [MemberData(nameof(Levels))]
    public void Every_scale_of_the_pool_has_a_distinct_label_in_both_languages(Difficulty d)
    {
        foreach (var lang in Enum.GetValues<Lang>())
        {
            var pool = ScaleDrillOptions.For(d).Pool;
            var labels = pool.Select(t => TypeNames.Label(t, lang)).ToList();
            if (lang == Lang.Fr) labels.Zip(pool).ShouldAllBe(p => p.First != p.Second.ToString()); // Dorian is spelled the same in English
            labels.Distinct().Count().ShouldBe(labels.Count);
        }
    }

    [Theory]
    [MemberData(nameof(Levels))]
    public void Every_degree_of_the_pool_has_a_numeral_in_major_and_minor(Difficulty d)
    {
        foreach (var degree in ProgressionDrillOptions.For(d).Degrees)
            foreach (var minor in new[] { false, true })
                foreach (var lang in Enum.GetValues<Lang>())
                    TypeNames.Degree(degree, minor, lang).ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Spot_checks()
    {
        TypeNames.Label(ChordType.Dominant7, Lang.Fr).ShouldBe("septième de dominante");
        TypeNames.Label(ChordType.Minor, Lang.En).ShouldBe("minor");
        TypeNames.Label(ScaleType.Dorian, Lang.Fr).ShouldBe("dorien");
        TypeNames.Degree(5, false, Lang.En).ShouldBe("V");
        TypeNames.Degree(4, true, Lang.Fr).ShouldBe("iv");
        TypeNames.Degree(7, false, Lang.En).ShouldBe("vii°");
        TypeNames.Key(new KeySignature(Note.Parse("A4"), KeyMode.Minor), Lang.Fr).ShouldBe("La mineur");
    }

    [Fact]
    public void A_type_outside_the_pools_shows_its_name() =>
        TypeNames.Label(ChordType.Sus4, Lang.En).ShouldBe("Sus4");
}
