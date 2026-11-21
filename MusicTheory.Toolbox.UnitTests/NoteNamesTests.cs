using MusicTheory.Toolbox.I18n;

namespace MusicTheory.Toolbox.UnitTests;

public class NoteNamesTests
{
    [Fact]
    public void French_is_fixed_do_and_english_is_letters()
    {
        var note = Note.Parse("F#4");
        NoteNames.Format(note, Lang.Fr).ShouldBe(Solfege.FixedDo(note));
        NoteNames.Format(note, Lang.Fr).ShouldBe("Fa dièse");
        NoteNames.Format(note, Lang.En).ShouldBe("F#");
    }
}
