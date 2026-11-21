using MusicTheory.Abc;
using MusicTheory.Toolbox.Rendering;

namespace MusicTheory.Toolbox.UnitTests;

public class StaffAbcTests
{
    private static Note[] Notes(string text) => text.Split(' ').Select(Note.Parse).ToArray();

    private static Note[] RoundTrip(string abc) =>
        AbcTune.Parse(abc).Measures.SelectMany(m => m.Events).Select(e => e.Pitches.Single()).ToArray();

    [Fact]
    public void G_major_scale_round_trips()
    {
        var notes = Notes("G4 A4 B4 C5 D5 E5 F#5 G5");
        var abc = StaffAbc.FromNotes(new KeySignature(Note.Parse("G4"), KeyMode.Major), notes);

        abc.ShouldContain("K:G");
        RoundTrip(abc).ShouldBe(notes);
    }

    [Fact]
    public void A_note_outside_the_key_keeps_its_accidental()
    {
        var notes = Notes("C4 F#4 Bb4 C5");
        var abc = StaffAbc.FromNotes(new KeySignature(Note.Parse("C4"), KeyMode.Major), notes);

        RoundTrip(abc).ShouldBe(notes);
    }
}
