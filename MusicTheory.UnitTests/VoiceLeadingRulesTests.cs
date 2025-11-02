namespace MusicTheory.UnitTests;

public class VoiceLeadingRulesTests
{
    [Fact]
    public void ParallelFifths_ShouldBeDetected_BetweenTwoVoices()
    {
        // Arrange
        var voice1Note1 = new Note(NoteName.C, Alteration.Natural, 4);
        var voice1Note2 = new Note(NoteName.D, Alteration.Natural, 4);
        var voice2Note1 = new Note(NoteName.G, Alteration.Natural, 4);
        var voice2Note2 = new Note(NoteName.A, Alteration.Natural, 4);

        // Act
        var hasParallelFifths = VoiceLeadingRules.HasParallelFifths(voice1Note1, voice1Note2, voice2Note1, voice2Note2);

        // Assert
        hasParallelFifths.ShouldBeTrue();
    }

    [Fact]
    public void ParallelFifths_ShouldNotBeDetected_WithContraryMotion()
    {
        // Arrange
        var voice1Note1 = new Note(NoteName.C, Alteration.Natural, 4);
        var voice1Note2 = new Note(NoteName.D, Alteration.Natural, 4);
        var voice2Note1 = new Note(NoteName.G, Alteration.Natural, 4);
        var voice2Note2 = new Note(NoteName.F, Alteration.Natural, 4);

        // Act
        var hasParallelFifths = VoiceLeadingRules.HasParallelFifths(voice1Note1, voice1Note2, voice2Note1, voice2Note2);

        // Assert
        hasParallelFifths.ShouldBeFalse();
    }

    [Fact]
    public void ParallelOctaves_ShouldBeDetected_BetweenTwoVoices()
    {
        // Arrange
        var voice1Note1 = new Note(NoteName.C, Alteration.Natural, 4);
        var voice1Note2 = new Note(NoteName.D, Alteration.Natural, 4);
        var voice2Note1 = new Note(NoteName.C, Alteration.Natural, 5);
        var voice2Note2 = new Note(NoteName.D, Alteration.Natural, 5);

        // Act
        var hasParallelOctaves = VoiceLeadingRules.HasParallelOctaves(voice1Note1, voice1Note2, voice2Note1, voice2Note2);

        // Assert
        hasParallelOctaves.ShouldBeTrue();
    }

    [Fact]
    public void ParallelUnisons_ShouldBeDetected_BetweenTwoVoices()
    {
        // Arrange
        var voice1Note1 = new Note(NoteName.C, Alteration.Natural, 4);
        var voice1Note2 = new Note(NoteName.D, Alteration.Natural, 4);
        var voice2Note1 = new Note(NoteName.C, Alteration.Natural, 4);
        var voice2Note2 = new Note(NoteName.D, Alteration.Natural, 4);

        // Act
        var hasParallelUnisons = VoiceLeadingRules.HasParallelUnisons(voice1Note1, voice1Note2, voice2Note1, voice2Note2);

        // Assert
        hasParallelUnisons.ShouldBeTrue();
    }

    [Fact]
    public void HiddenFifths_ShouldBeDetected_WithSimilarMotionToOuterVoices()
    {
        // Arrange - Both voices move up to a perfect 5th
        var bassNote1 = new Note(NoteName.C, Alteration.Natural, 3);
        var bassNote2 = new Note(NoteName.D, Alteration.Natural, 3);
        var sopranoNote1 = new Note(NoteName.E, Alteration.Natural, 4);
        var sopranoNote2 = new Note(NoteName.A, Alteration.Natural, 4); // D to A is a perfect 5th

        // Act
        var hasHiddenFifths = VoiceLeadingRules.HasHiddenFifths(bassNote1, bassNote2, sopranoNote1, sopranoNote2);

        // Assert
        hasHiddenFifths.ShouldBeTrue();
    }

    [Fact]
    public void VoiceCrossing_ShouldBeDetected_WhenLowerVoiceGoesAboveHigherVoice()
    {
        // Arrange
        var lowerVoiceNote = new Note(NoteName.G, Alteration.Natural, 4);
        var higherVoiceNote = new Note(NoteName.E, Alteration.Natural, 4);

        // Act
        var hasVoiceCrossing = VoiceLeadingRules.HasVoiceCrossing(lowerVoiceNote, higherVoiceNote);

        // Assert
        hasVoiceCrossing.ShouldBeTrue();
    }

    [Fact]
    public void VoiceOverlap_ShouldBeDetected_WhenVoiceMovesIntoRangeOfOtherVoice()
    {
        // Arrange
        var voice1Note1 = new Note(NoteName.C, Alteration.Natural, 4);
        var voice1Note2 = new Note(NoteName.G, Alteration.Natural, 4);
        var voice2Note1 = new Note(NoteName.E, Alteration.Natural, 4);
        var voice2Note2 = new Note(NoteName.D, Alteration.Natural, 4);

        // Act
        var hasVoiceOverlap = VoiceLeadingRules.HasVoiceOverlap(voice1Note1, voice1Note2, voice2Note1, voice2Note2);

        // Assert
        hasVoiceOverlap.ShouldBeTrue();
    }

    [Fact]
    public void AugmentedInterval_ShouldBeDetected_InMelodicLine()
    {
        // Arrange
        var note1 = new Note(NoteName.F, Alteration.Natural, 4);
        var note2 = new Note(NoteName.B, Alteration.Natural, 4);

        // Act
        var hasAugmentedInterval = VoiceLeadingRules.HasAugmentedInterval(note1, note2);

        // Assert
        hasAugmentedInterval.ShouldBeTrue();
    }

    [Fact]
    public void LargeMelodicLeap_ShouldBeDetected_WhenGreaterThanOctave()
    {
        // Arrange
        var note1 = new Note(NoteName.C, Alteration.Natural, 4);
        var note2 = new Note(NoteName.E, Alteration.Natural, 5);

        // Act
        var hasLargeLeap = VoiceLeadingRules.HasLargeMelodicLeap(note1, note2);

        // Assert
        hasLargeLeap.ShouldBeTrue();
    }

    [Fact]
    public void LargeMelodicLeap_ShouldNotBeDetected_ForSmallIntervals()
    {
        // Arrange
        var note1 = new Note(NoteName.C, Alteration.Natural, 4);
        var note2 = new Note(NoteName.E, Alteration.Natural, 4);

        // Act
        var hasLargeLeap = VoiceLeadingRules.HasLargeMelodicLeap(note1, note2);

        // Assert
        hasLargeLeap.ShouldBeFalse();
    }

    [Fact]
    public void ValidateVoiceLeading_ShouldReturnAllViolations_ForPoorProgression()
    {
        // Arrange
        var chord1 = new[]
        {
            new Note(NoteName.C, Alteration.Natural, 3), // Bass
            new Note(NoteName.E, Alteration.Natural, 3), // Tenor
            new Note(NoteName.G, Alteration.Natural, 3), // Alto
            new Note(NoteName.C, Alteration.Natural, 4)  // Soprano
        };

        var chord2 = new[]
        {
            new Note(NoteName.D, Alteration.Natural, 3), // Bass
            new Note(NoteName.F, Alteration.Sharp, 3),   // Tenor
            new Note(NoteName.A, Alteration.Natural, 3), // Alto
            new Note(NoteName.D, Alteration.Natural, 4)  // Soprano
        };

        // Act
        var violations = VoiceLeadingRules.ValidateVoiceLeading(chord1, chord2);

        // Assert
        violations.ShouldContain(v => v.Type == VoiceLeadingViolationType.ParallelFifths);
    }

    [Fact]
    public void ValidateVoiceLeading_ShouldReturnNoViolations_ForGoodProgression()
    {
        // Arrange - C major to F major with smooth voice leading
        var chord1 = new[]
        {
            new Note(NoteName.C, Alteration.Natural, 3), // Bass
            new Note(NoteName.E, Alteration.Natural, 3), // Tenor
            new Note(NoteName.G, Alteration.Natural, 3), // Alto
            new Note(NoteName.C, Alteration.Natural, 4)  // Soprano
        };

        var chord2 = new[]
        {
            new Note(NoteName.F, Alteration.Natural, 2), // Bass - down to avoid overlap
            new Note(NoteName.F, Alteration.Natural, 3), // Tenor
            new Note(NoteName.A, Alteration.Natural, 3), // Alto
            new Note(NoteName.C, Alteration.Natural, 4)  // Soprano (common tone)
        };

        // Act
        var violations = VoiceLeadingRules.ValidateVoiceLeading(chord1, chord2);

        // Assert
        violations.ShouldBeEmpty();
    }

    [Fact]
    public void MotionType_ShouldBeParallel_WhenBothVoicesMoveInSameDirection()
    {
        // Arrange
        var voice1Note1 = new Note(NoteName.C, Alteration.Natural, 4);
        var voice1Note2 = new Note(NoteName.D, Alteration.Natural, 4);
        var voice2Note1 = new Note(NoteName.E, Alteration.Natural, 4);
        var voice2Note2 = new Note(NoteName.F, Alteration.Sharp, 4); // Both move up 2 semitones

        // Act
        var motionType = VoiceLeadingRules.GetMotionType(voice1Note1, voice1Note2, voice2Note1, voice2Note2);

        // Assert
        motionType.ShouldBe(MotionType.Parallel);
    }

    [Fact]
    public void MotionType_ShouldBeContrary_WhenVoicesMoveInOppositeDirections()
    {
        // Arrange
        var voice1Note1 = new Note(NoteName.C, Alteration.Natural, 4);
        var voice1Note2 = new Note(NoteName.D, Alteration.Natural, 4);
        var voice2Note1 = new Note(NoteName.E, Alteration.Natural, 4);
        var voice2Note2 = new Note(NoteName.D, Alteration.Natural, 4);

        // Act
        var motionType = VoiceLeadingRules.GetMotionType(voice1Note1, voice1Note2, voice2Note1, voice2Note2);

        // Assert
        motionType.ShouldBe(MotionType.Contrary);
    }

    [Fact]
    public void MotionType_ShouldBeOblique_WhenOneVoiceStays()
    {
        // Arrange
        var voice1Note1 = new Note(NoteName.C, Alteration.Natural, 4);
        var voice1Note2 = new Note(NoteName.C, Alteration.Natural, 4);
        var voice2Note1 = new Note(NoteName.E, Alteration.Natural, 4);
        var voice2Note2 = new Note(NoteName.F, Alteration.Natural, 4);

        // Act
        var motionType = VoiceLeadingRules.GetMotionType(voice1Note1, voice1Note2, voice2Note1, voice2Note2);

        // Assert
        motionType.ShouldBe(MotionType.Oblique);
    }
}
