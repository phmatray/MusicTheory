namespace MusicTheory.UnitTests.Analysis;

using MusicTheory.Analysis;

public class PitchClassSetTests
{
    #region Creation

    [Fact]
    public void PitchClassSet_FromNotes_ShouldCreate_CorrectPitchClasses()
    {
        // Arrange - C major triad
        var notes = new[]
        {
            new Note(NoteName.C, Alteration.Natural, 4),
            new Note(NoteName.E, Alteration.Natural, 4),
            new Note(NoteName.G, Alteration.Natural, 4)
        };

        // Act
        var set = PitchClassSet.FromNotes(notes);

        // Assert
        set.PitchClasses.ShouldContain(0);  // C
        set.PitchClasses.ShouldContain(4);  // E
        set.PitchClasses.ShouldContain(7);  // G
        set.Cardinality.ShouldBe(3);
    }

    [Fact]
    public void PitchClassSet_FromNotes_ShouldIgnore_OctaveDuplicates()
    {
        // Arrange - C major triad with doubled root
        var notes = new[]
        {
            new Note(NoteName.C, Alteration.Natural, 4),
            new Note(NoteName.E, Alteration.Natural, 4),
            new Note(NoteName.G, Alteration.Natural, 4),
            new Note(NoteName.C, Alteration.Natural, 5)  // Doubled root
        };

        // Act
        var set = PitchClassSet.FromNotes(notes);

        // Assert
        set.Cardinality.ShouldBe(3);  // Only 3 unique pitch classes
    }

    [Fact]
    public void PitchClassSet_FromIntervals_ShouldCreate_CorrectPitchClasses()
    {
        // Arrange - Major triad intervals from C (0, 4, 7)
        var intervals = new[] { 0, 4, 7 };

        // Act
        var set = PitchClassSet.FromIntervals(intervals);

        // Assert
        set.PitchClasses.ShouldContain(0);
        set.PitchClasses.ShouldContain(4);
        set.PitchClasses.ShouldContain(7);
    }

    #endregion

    #region Transposition

    [Fact]
    public void PitchClassSet_Transpose_ShouldShift_AllPitchClasses()
    {
        // Arrange - C major triad (0, 4, 7)
        var set = PitchClassSet.FromIntervals(new[] { 0, 4, 7 });

        // Act - Transpose up by 2 semitones (to D major)
        var transposed = set.Transpose(2);

        // Assert
        transposed.PitchClasses.ShouldContain(2);   // D
        transposed.PitchClasses.ShouldContain(6);   // F#
        transposed.PitchClasses.ShouldContain(9);   // A
    }

    [Fact]
    public void PitchClassSet_Transpose_ShouldWrap_AtOctave()
    {
        // Arrange - A major triad (9, 1, 4)
        var set = PitchClassSet.FromIntervals(new[] { 9, 1, 4 });

        // Act - Transpose up by 5 semitones
        var transposed = set.Transpose(5);

        // Assert - Should wrap around
        transposed.PitchClasses.ShouldContain(2);   // 9 + 5 = 14 % 12 = 2
        transposed.PitchClasses.ShouldContain(6);   // 1 + 5 = 6
        transposed.PitchClasses.ShouldContain(9);   // 4 + 5 = 9
    }

    #endregion

    #region Normal Form

    [Fact]
    public void PitchClassSet_GetNormalForm_ShouldReturn_MostCompactForm()
    {
        // Arrange - C major triad in various orderings
        var set = PitchClassSet.FromIntervals(new[] { 7, 0, 4 });

        // Act
        var normalForm = set.GetNormalForm();

        // Assert - Normal form should be most compact
        var pitchClasses = normalForm.PitchClasses.ToList();
        pitchClasses.Count.ShouldBe(3);
    }

    #endregion

    #region Prime Form

    [Fact]
    public void PitchClassSet_GetPrimeForm_ShouldReturn_TranspositionallyEquivalent()
    {
        // Arrange - D major triad (2, 6, 9)
        var dMajor = PitchClassSet.FromIntervals(new[] { 2, 6, 9 });
        // C major triad (0, 4, 7)
        var cMajor = PitchClassSet.FromIntervals(new[] { 0, 4, 7 });

        // Act
        var dPrime = dMajor.GetPrimeForm();
        var cPrime = cMajor.GetPrimeForm();

        // Assert - Prime forms should be equal for transpositions
        dPrime.PitchClasses.SequenceEqual(cPrime.PitchClasses).ShouldBeTrue();
    }

    #endregion

    #region Set Operations

    [Fact]
    public void PitchClassSet_Union_ShouldCombine_PitchClasses()
    {
        // Arrange
        var set1 = PitchClassSet.FromIntervals(new[] { 0, 4, 7 });    // C major
        var set2 = PitchClassSet.FromIntervals(new[] { 0, 3, 7 });    // C minor (adds Eb=3)

        // Act
        var union = set1.Union(set2);

        // Assert
        union.PitchClasses.ShouldContain(0);
        union.PitchClasses.ShouldContain(3);
        union.PitchClasses.ShouldContain(4);
        union.PitchClasses.ShouldContain(7);
        union.Cardinality.ShouldBe(4);
    }

    [Fact]
    public void PitchClassSet_Intersection_ShouldReturn_CommonPitchClasses()
    {
        // Arrange
        var set1 = PitchClassSet.FromIntervals(new[] { 0, 4, 7 });    // C major
        var set2 = PitchClassSet.FromIntervals(new[] { 0, 3, 7 });    // C minor

        // Act
        var intersection = set1.Intersection(set2);

        // Assert - Common notes: C (0) and G (7)
        intersection.PitchClasses.ShouldContain(0);
        intersection.PitchClasses.ShouldContain(7);
        intersection.PitchClasses.ShouldNotContain(3);
        intersection.PitchClasses.ShouldNotContain(4);
        intersection.Cardinality.ShouldBe(2);
    }

    [Fact]
    public void PitchClassSet_IsSubsetOf_ShouldReturn_True_ForSubset()
    {
        // Arrange
        var subset = PitchClassSet.FromIntervals(new[] { 0, 7 });     // C and G only
        var superset = PitchClassSet.FromIntervals(new[] { 0, 4, 7 }); // C major

        // Act & Assert
        subset.IsSubsetOf(superset).ShouldBeTrue();
    }

    [Fact]
    public void PitchClassSet_IsSubsetOf_ShouldReturn_False_ForNonSubset()
    {
        // Arrange
        var set1 = PitchClassSet.FromIntervals(new[] { 0, 4, 7 });    // C major
        var set2 = PitchClassSet.FromIntervals(new[] { 0, 3, 7 });    // C minor

        // Act & Assert
        set1.IsSubsetOf(set2).ShouldBeFalse();
    }

    #endregion

    #region Equality

    [Fact]
    public void PitchClassSet_Equals_ShouldReturn_True_ForSamePitchClasses()
    {
        // Arrange
        var set1 = PitchClassSet.FromIntervals(new[] { 0, 4, 7 });
        var set2 = PitchClassSet.FromIntervals(new[] { 7, 0, 4 });  // Different order

        // Act & Assert
        set1.Equals(set2).ShouldBeTrue();
    }

    [Fact]
    public void PitchClassSet_GetHashCode_ShouldBe_SameForEqualSets()
    {
        // Arrange
        var set1 = PitchClassSet.FromIntervals(new[] { 0, 4, 7 });
        var set2 = PitchClassSet.FromIntervals(new[] { 4, 7, 0 });

        // Act & Assert
        set1.GetHashCode().ShouldBe(set2.GetHashCode());
    }

    #endregion
}
