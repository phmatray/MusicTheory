using MusicTheory.Abc;

namespace MusicTheory.Toolbox.Rendering;

/// <summary>One voice of quarter notes in 4/4, split into measures; shared by the staff drawing and the file exports.</summary>
public static class MelodyMeasures
{
    public static IReadOnlyList<Measure> FromNotes(IReadOnlyList<Note> notes)
    {
        var time = new TimeSignature(4, 4);
        return Measure.Split(time, notes.Select(n => new RhythmEvent([n], new Duration(DurationType.Quarter))));
    }
}
