namespace MusicTheory.Jazz;

/// <summary>
/// Provides jazz chord voicing functionality.
/// Jazz voicings arrange chord tones in specific ways to achieve
/// smooth voice leading and characteristic jazz sonorities.
/// </summary>
public static class JazzVoicing
{
    /// <summary>
    /// Gets a shell voicing (root, 3rd, 7th) for a chord.
    /// Shell voicings are economical 3-note voicings used in comping.
    /// </summary>
    /// <param name="chord">The chord to voice.</param>
    /// <returns>A list of notes forming the shell voicing.</returns>
    public static IReadOnlyList<Note> GetShellVoicing(Chord chord)
    {
        var chordNotes = chord.GetNotes().ToList();
        var voicing = new List<Note>();

        // Root
        voicing.Add(chordNotes[0]);

        // 3rd (index 1)
        if (chordNotes.Count > 1)
            voicing.Add(chordNotes[1]);

        // 7th (index 3 for seventh chords)
        if (chordNotes.Count > 3)
            voicing.Add(chordNotes[3]);

        return voicing;
    }

    /// <summary>
    /// Gets the guide tones (3rd and 7th) for a chord.
    /// Guide tones define the chord quality and are essential for voice leading.
    /// </summary>
    /// <param name="chord">The chord.</param>
    /// <returns>A list containing the 3rd and 7th of the chord.</returns>
    public static IReadOnlyList<Note> GetGuideTones(Chord chord)
    {
        var chordNotes = chord.GetNotes().ToList();
        var guideTones = new List<Note>();

        // 3rd (index 1)
        if (chordNotes.Count > 1)
            guideTones.Add(chordNotes[1]);

        // 7th (index 3 for seventh chords)
        if (chordNotes.Count > 3)
            guideTones.Add(chordNotes[3]);

        return guideTones;
    }

    /// <summary>
    /// Gets a rootless Type A voicing (3-5-7-9).
    /// Bill Evans-style left hand voicing, commonly used for ii and I chords.
    /// </summary>
    /// <param name="chord">The chord to voice.</param>
    /// <returns>A list of notes forming the rootless Type A voicing.</returns>
    public static IReadOnlyList<Note> GetRootlessTypeA(Chord chord)
    {
        var chordNotes = chord.GetNotes().ToList();
        var voicing = new List<Note>();

        // 3rd
        if (chordNotes.Count > 1)
            voicing.Add(chordNotes[1]);

        // 5th
        if (chordNotes.Count > 2)
            voicing.Add(chordNotes[2]);

        // 7th
        if (chordNotes.Count > 3)
            voicing.Add(chordNotes[3]);

        // 9th (2nd above root)
        var ninth = GetNinth(chord);
        voicing.Add(ninth);

        return voicing;
    }

    /// <summary>
    /// Gets a rootless Type B voicing (7-9-3-5).
    /// Bill Evans-style left hand voicing, commonly used for V chords.
    /// </summary>
    /// <param name="chord">The chord to voice.</param>
    /// <returns>A list of notes forming the rootless Type B voicing.</returns>
    public static IReadOnlyList<Note> GetRootlessTypeB(Chord chord)
    {
        var chordNotes = chord.GetNotes().ToList();
        var voicing = new List<Note>();

        // 7th
        if (chordNotes.Count > 3)
            voicing.Add(chordNotes[3]);

        // 9th
        var ninth = GetNinth(chord);
        voicing.Add(ninth);

        // 3rd
        if (chordNotes.Count > 1)
            voicing.Add(chordNotes[1]);

        // 5th
        if (chordNotes.Count > 2)
            voicing.Add(chordNotes[2]);

        return voicing;
    }

    /// <summary>
    /// Gets a Drop 2 voicing for a chord.
    /// The 2nd note from the top of a close position voicing is dropped an octave.
    /// </summary>
    /// <param name="chord">The chord to voice.</param>
    /// <returns>A list of notes forming the Drop 2 voicing.</returns>
    public static IReadOnlyList<Note> GetDrop2Voicing(Chord chord)
    {
        var chordNotes = chord.GetNotes().ToList();

        if (chordNotes.Count < 4)
            return chordNotes; // Can't do Drop 2 with less than 4 notes

        var voicing = new List<Note>();

        // Close position: 1, 3, 5, 7 (root, 3rd, 5th, 7th)
        // Drop 2: The 2nd from top (5th) goes down an octave
        // Result: 5 (low), 1, 3, 7

        // Add the dropped note (5th) an octave lower
        var droppedNote = chordNotes[2]; // 5th is at index 2
        var lowDroppedNote = new Note(droppedNote.Name, droppedNote.Alteration, droppedNote.Octave - 1);
        voicing.Add(lowDroppedNote);

        // Add root
        voicing.Add(chordNotes[0]);

        // Add 3rd
        voicing.Add(chordNotes[1]);

        // Add 7th
        voicing.Add(chordNotes[3]);

        return voicing;
    }

    /// <summary>
    /// Gets a quartal voicing (stacked 4ths) starting from a chord root.
    /// </summary>
    /// <param name="chord">The chord to base the voicing on.</param>
    /// <param name="noteCount">Number of notes in the voicing (default 4).</param>
    /// <returns>A list of notes forming the quartal voicing.</returns>
    public static IReadOnlyList<Note> GetQuartalVoicing(Chord chord, int noteCount = 4)
    {
        return GetQuartalVoicing(chord.Root, noteCount);
    }

    /// <summary>
    /// Gets a quartal voicing (stacked 4ths) starting from a root note.
    /// </summary>
    /// <param name="root">The starting note.</param>
    /// <param name="noteCount">Number of notes in the voicing (default 4).</param>
    /// <returns>A list of notes forming the quartal voicing.</returns>
    public static IReadOnlyList<Note> GetQuartalVoicing(Note root, int noteCount = 4)
    {
        var voicing = new List<Note> { root };
        var currentNote = root;

        for (int i = 1; i < noteCount; i++)
        {
            // Add a perfect 4th (5 semitones)
            var perfectFourth = new Interval(IntervalQuality.Perfect, 4);
            currentNote = currentNote.Transpose(perfectFourth);
            voicing.Add(currentNote);
        }

        return voicing;
    }

    /// <summary>
    /// Gets a Drop 3 voicing for a chord.
    /// The 3rd note from the top of a close position voicing is dropped an octave.
    /// </summary>
    /// <param name="chord">The chord to voice.</param>
    /// <returns>A list of notes forming the Drop 3 voicing.</returns>
    public static IReadOnlyList<Note> GetDrop3Voicing(Chord chord)
    {
        var chordNotes = chord.GetNotes().ToList();

        if (chordNotes.Count < 4)
            return chordNotes;

        var voicing = new List<Note>();

        // Close position: 1, 3, 5, 7
        // Drop 3: The 3rd from top (3rd degree) goes down an octave
        // Result: 3 (low), 1, 5, 7

        var droppedNote = chordNotes[1]; // 3rd is at index 1
        var lowDroppedNote = new Note(droppedNote.Name, droppedNote.Alteration, droppedNote.Octave - 1);
        voicing.Add(lowDroppedNote);

        // Add root
        voicing.Add(chordNotes[0]);

        // Add 5th
        voicing.Add(chordNotes[2]);

        // Add 7th
        voicing.Add(chordNotes[3]);

        return voicing;
    }

    /// <summary>
    /// Gets a spread voicing (open position) for a chord.
    /// Notes are spread across more than an octave for a more open sound.
    /// </summary>
    /// <param name="chord">The chord to voice.</param>
    /// <returns>A list of notes forming the spread voicing.</returns>
    public static IReadOnlyList<Note> GetSpreadVoicing(Chord chord)
    {
        var chordNotes = chord.GetNotes().ToList();

        if (chordNotes.Count < 4)
            return chordNotes;

        var voicing = new List<Note>();

        // Spread: Root (low), 5th, 3rd (up), 7th
        // Creates wider intervals

        // Root in bass
        voicing.Add(chordNotes[0]);

        // 5th
        voicing.Add(chordNotes[2]);

        // 3rd up an octave
        var third = chordNotes[1];
        var highThird = new Note(third.Name, third.Alteration, third.Octave + 1);
        voicing.Add(highThird);

        // 7th
        var seventh = chordNotes[3];
        var highSeventh = new Note(seventh.Name, seventh.Alteration, seventh.Octave + 1);
        voicing.Add(highSeventh);

        return voicing;
    }

    private static Note GetNinth(Chord chord)
    {
        // The 9th is a major 2nd above the root
        var root = chord.Root;
        var majorSecond = new Interval(IntervalQuality.Major, 2);
        return root.Transpose(majorSecond);
    }
}
