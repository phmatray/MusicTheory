namespace MusicTheory.UnitTests;

public class ModalScaleTests
{
    [Fact]
    public void Scale_ShouldCreateDorianMode()
    {
        // Arrange & Act
        var dDorian = new Scale(new Note(NoteName.D, Alteration.Natural, 4), ScaleType.Dorian);
        
        // Assert
        dDorian.Type.Should().Be(ScaleType.Dorian);
        dDorian.Root.Name.Should().Be(NoteName.D);
    }

    [Fact]
    public void Scale_ShouldGetNotes_ForDorianMode()
    {
        // Arrange
        var dDorian = new Scale(new Note(NoteName.D, Alteration.Natural, 4), ScaleType.Dorian);
        
        // Act
        var notes = dDorian.GetNotes().ToList();
        
        // Assert
        notes.Should().HaveCount(8); // Including octave
        
        // D Dorian: D E F G A B C D
        notes[0].Name.Should().Be(NoteName.D);
        notes[0].Alteration.Should().Be(Alteration.Natural);
        
        notes[1].Name.Should().Be(NoteName.E);
        notes[1].Alteration.Should().Be(Alteration.Natural);
        
        notes[2].Name.Should().Be(NoteName.F);
        notes[2].Alteration.Should().Be(Alteration.Natural);
        
        notes[3].Name.Should().Be(NoteName.G);
        notes[3].Alteration.Should().Be(Alteration.Natural);
        
        notes[4].Name.Should().Be(NoteName.A);
        notes[4].Alteration.Should().Be(Alteration.Natural);
        
        notes[5].Name.Should().Be(NoteName.B);
        notes[5].Alteration.Should().Be(Alteration.Natural);
        
        notes[6].Name.Should().Be(NoteName.C);
        notes[6].Alteration.Should().Be(Alteration.Natural);
        notes[6].Octave.Should().Be(5); // Next octave
        
        notes[7].Name.Should().Be(NoteName.D);
        notes[7].Octave.Should().Be(5); // Octave
    }

    [Theory]
    [InlineData(ScaleType.Dorian, "W-H-W-W-W-H-W")]
    [InlineData(ScaleType.Phrygian, "H-W-W-W-H-W-W")]
    [InlineData(ScaleType.Lydian, "W-W-W-H-W-W-H")]
    [InlineData(ScaleType.Mixolydian, "W-W-H-W-W-H-W")]
    [InlineData(ScaleType.Aeolian, "W-H-W-W-H-W-W")] // Same as natural minor
    [InlineData(ScaleType.Locrian, "H-W-W-H-W-W-W")]
    public void Scale_ShouldHaveCorrectIntervalPattern_ForMode(ScaleType mode, string expectedPattern)
    {
        // Arrange
        var scale = new Scale(new Note(NoteName.C, Alteration.Natural, 4), mode);
        
        // Act
        var notes = scale.GetNotes().ToList();
        
        // Assert
        var actualPattern = GetIntervalPattern(notes);
        actualPattern.Should().Be(expectedPattern);
    }

    [Fact]
    public void Scale_ShouldGetNotes_ForPhrygianMode()
    {
        // Arrange
        var ePhrygian = new Scale(new Note(NoteName.E, Alteration.Natural, 4), ScaleType.Phrygian);
        
        // Act
        var notes = ePhrygian.GetNotes().ToList();
        
        // Assert
        // E Phrygian: E F G A B C D E
        notes[0].Name.Should().Be(NoteName.E);
        notes[1].Name.Should().Be(NoteName.F);
        notes[2].Name.Should().Be(NoteName.G);
        notes[3].Name.Should().Be(NoteName.A);
        notes[4].Name.Should().Be(NoteName.B);
        notes[5].Name.Should().Be(NoteName.C);
        notes[6].Name.Should().Be(NoteName.D);
        notes[7].Name.Should().Be(NoteName.E);
        notes[7].Octave.Should().Be(5);
    }

    [Fact]
    public void Scale_ShouldGetNotes_ForLydianMode()
    {
        // Arrange
        var fLydian = new Scale(new Note(NoteName.F, Alteration.Natural, 4), ScaleType.Lydian);
        
        // Act
        var notes = fLydian.GetNotes().ToList();
        
        // Assert
        // F Lydian: F G A B C D E F
        notes[0].Name.Should().Be(NoteName.F);
        notes[1].Name.Should().Be(NoteName.G);
        notes[2].Name.Should().Be(NoteName.A);
        notes[3].Name.Should().Be(NoteName.B); // Raised 4th
        notes[3].Alteration.Should().Be(Alteration.Natural);
        notes[4].Name.Should().Be(NoteName.C);
        notes[5].Name.Should().Be(NoteName.D);
        notes[6].Name.Should().Be(NoteName.E);
        notes[7].Name.Should().Be(NoteName.F);
    }

    [Fact]
    public void Scale_ShouldGetNotes_ForMixolydianMode()
    {
        // Arrange
        var gMixolydian = new Scale(new Note(NoteName.G, Alteration.Natural, 4), ScaleType.Mixolydian);
        
        // Act
        var notes = gMixolydian.GetNotes().ToList();
        
        // Assert
        // G Mixolydian: G A B C D E F G
        notes[0].Name.Should().Be(NoteName.G);
        notes[1].Name.Should().Be(NoteName.A);
        notes[2].Name.Should().Be(NoteName.B);
        notes[3].Name.Should().Be(NoteName.C);
        notes[4].Name.Should().Be(NoteName.D);
        notes[5].Name.Should().Be(NoteName.E);
        notes[6].Name.Should().Be(NoteName.F); // Lowered 7th
        notes[6].Alteration.Should().Be(Alteration.Natural);
        notes[7].Name.Should().Be(NoteName.G);
    }

    [Fact]
    public void Scale_ShouldGetNotes_ForLocrianMode()
    {
        // Arrange
        var bLocrian = new Scale(new Note(NoteName.B, Alteration.Natural, 4), ScaleType.Locrian);
        
        // Act
        var notes = bLocrian.GetNotes().ToList();
        
        // Assert
        // B Locrian: B C D E F G A B
        notes[0].Name.Should().Be(NoteName.B);
        notes[1].Name.Should().Be(NoteName.C);
        notes[2].Name.Should().Be(NoteName.D);
        notes[3].Name.Should().Be(NoteName.E);
        notes[4].Name.Should().Be(NoteName.F); // Lowered 5th
        notes[5].Name.Should().Be(NoteName.G);
        notes[6].Name.Should().Be(NoteName.A);
        notes[7].Name.Should().Be(NoteName.B);
    }

    [Fact]
    public void Scale_ShouldTranspose_ModalScale()
    {
        // Arrange
        var dDorian = new Scale(new Note(NoteName.D, Alteration.Natural, 4), ScaleType.Dorian);
        var majorSecond = new Interval(IntervalQuality.Major, 2);
        
        // Act
        var eDorian = dDorian.Transpose(majorSecond);
        
        // Assert
        eDorian.Root.Name.Should().Be(NoteName.E);
        eDorian.Type.Should().Be(ScaleType.Dorian);
        
        var notes = eDorian.GetNotes().ToList();
        // E Dorian: E F# G A B C# D E
        notes[1].Name.Should().Be(NoteName.F);
        notes[1].Alteration.Should().Be(Alteration.Sharp);
        notes[6].Name.Should().Be(NoteName.D);
        notes[6].Alteration.Should().Be(Alteration.Natural);
    }

    [Theory]
    [InlineData(NoteName.C, ScaleType.Ionian)]  // C Ionian = C Major
    [InlineData(NoteName.D, ScaleType.Dorian)]
    [InlineData(NoteName.E, ScaleType.Phrygian)]
    [InlineData(NoteName.F, ScaleType.Lydian)]
    [InlineData(NoteName.G, ScaleType.Mixolydian)]
    [InlineData(NoteName.A, ScaleType.Aeolian)]  // A Aeolian = A Natural Minor
    [InlineData(NoteName.B, ScaleType.Locrian)]
    public void Scale_ModesOfCMajor_ShouldHaveSameNotes(NoteName root, ScaleType mode)
    {
        // Arrange
        var scale = new Scale(new Note(root, Alteration.Natural, 4), mode);
        
        // Act
        var notes = scale.GetNotes().Take(7).Select(n => (n.Name, n.Alteration)).ToList();
        
        // Assert
        // All modes of C major should contain only natural notes
        notes.Should().OnlyContain(n => n.Alteration == Alteration.Natural);
    }

    private string GetIntervalPattern(List<Note> notes)
    {
        var intervals = new List<string>();
        
        for (int i = 0; i < notes.Count - 1; i++)
        {
            var semitones = GetSemitoneDifference(notes[i], notes[i + 1]);
            intervals.Add(semitones switch
            {
                1 => "H",
                2 => "W",
                3 => "WH",
                _ => "?"
            });
        }
        
        return string.Join("-", intervals);
    }

    private int GetSemitoneDifference(Note note1, Note note2)
    {
        int[] semitonesFromC = { 0, 2, 4, 5, 7, 9, 11 };
        
        var semitones1 = note1.Octave * 12 + semitonesFromC[(int)note1.Name] + (int)note1.Alteration;
        var semitones2 = note2.Octave * 12 + semitonesFromC[(int)note2.Name] + (int)note2.Alteration;
        
        return semitones2 - semitones1;
    }
}