namespace MusicTheory.Toolbox.Drills;

/// <summary>Turns a question's sounds into MIDI numbers for <c>ChordAudio</c>.</summary>
public static class DrillAudio
{
    /// <summary>One entry per step; notes inside an entry sound together. A melodic interval gives two entries.</summary>
    public static IReadOnlyList<IReadOnlyList<int>> ToSchedule(IReadOnlyList<IReadOnlyList<Note>> sounds) =>
        sounds.Select(step => (IReadOnlyList<int>)step.Select(n => n.MidiNumber).ToArray()).ToArray();
}
