namespace MusicTheory.FiguredBass;

/// <summary>
/// Provides functionality to realize figured bass notation into actual chords.
/// </summary>
public static class FiguredBassRealization
{
    /// <summary>
    /// Realizes a figured bass notation into a chord.
    /// </summary>
    /// <param name="bassNote">The bass note.</param>
    /// <param name="notation">The figured bass notation.</param>
    /// <param name="key">The key signature for determining chord qualities.</param>
    /// <returns>The realized chord.</returns>
    public static Chord Realize(Note bassNote, FiguredBassNotation notation, KeySignature key)
    {
        // Determine the chord root based on the bass note and inversion
        var root = DetermineRoot(bassNote, notation);

        // Determine the chord quality based on the key and scale degree
        var quality = DetermineQuality(root, key, notation.IsSeventhChord);

        // Create the chord
        if (notation.IsSeventhChord)
        {
            var chordType = GetSeventhChordType(root, key, quality);
            return new Chord(root, chordType);
        }
        else
        {
            return new Chord(root, quality);
        }
    }

    /// <summary>
    /// Realizes a figured bass notation into chord notes (with specific voicing).
    /// </summary>
    /// <param name="bassNote">The bass note.</param>
    /// <param name="notation">The figured bass notation.</param>
    /// <param name="key">The key signature.</param>
    /// <returns>The realized notes of the chord.</returns>
    public static IReadOnlyList<Note> RealizeNotes(Note bassNote, FiguredBassNotation notation, KeySignature key)
    {
        var notes = new List<Note> { bassNote };
        var bassPitchClass = GetPitchClass(bassNote);

        foreach (var figure in notation.Figures)
        {
            // Calculate the interval above the bass
            var interval = GetIntervalFromFigure(figure);

            // Get the note at that interval
            var note = bassNote.TransposeBySemitones(interval);

            // Apply any alterations
            if (notation.HasAlteration(figure))
            {
                var alteration = notation.GetAlteration(figure);
                note = new Note(note.Name, alteration, note.Octave);
            }

            notes.Add(note);
        }

        return notes;
    }

    private static Note DetermineRoot(Note bassNote, FiguredBassNotation notation)
    {
        // The root depends on the inversion
        // In root position, bass = root
        // In first inversion (6), root is a 6th below bass (or 3rd above)
        // In second inversion (6/4), root is a 4th below bass (or 5th above)
        // In third inversion (4/2), root is a 2nd below bass (or 7th above)

        return notation.Inversion switch
        {
            ChordInversion.Root => bassNote,
            ChordInversion.First => TransposeDown(bassNote, 4), // Down a 3rd interval = 4 semitones for major 3rd
            ChordInversion.Second => TransposeDown(bassNote, 7), // Down a 5th = 7 semitones
            ChordInversion.Third => TransposeDown(bassNote, 2), // Down a major 2nd = 2 semitones
            _ => bassNote
        };
    }

    private static Note TransposeDown(Note note, int semitones)
    {
        // Calculate new pitch class
        var currentPitchClass = GetPitchClass(note);
        var newPitchClass = ((currentPitchClass - semitones) % 12 + 12) % 12;

        return PitchClassToNote(newPitchClass, note.Octave);
    }

    private static ChordQuality DetermineQuality(Note root, KeySignature key, bool isSeventh)
    {
        // Determine the scale degree of the root
        var degree = GetScaleDegree(root, key);

        if (key.Mode == KeyMode.Major)
        {
            return degree switch
            {
                1 => ChordQuality.Major,      // I
                2 => ChordQuality.Minor,      // ii
                3 => ChordQuality.Minor,      // iii
                4 => ChordQuality.Major,      // IV
                5 => ChordQuality.Major,      // V
                6 => ChordQuality.Minor,      // vi
                7 => ChordQuality.Diminished, // vii°
                _ => ChordQuality.Major
            };
        }
        else // Minor
        {
            return degree switch
            {
                1 => ChordQuality.Minor,      // i
                2 => ChordQuality.Diminished, // ii°
                3 => ChordQuality.Major,      // III
                4 => ChordQuality.Minor,      // iv
                5 => ChordQuality.Major,      // V (harmonic minor)
                6 => ChordQuality.Major,      // VI
                7 => ChordQuality.Diminished, // vii°
                _ => ChordQuality.Minor
            };
        }
    }

    private static ChordType GetSeventhChordType(Note root, KeySignature key, ChordQuality triadQuality)
    {
        var degree = GetScaleDegree(root, key);

        if (key.Mode == KeyMode.Major)
        {
            return degree switch
            {
                1 => ChordType.Major7,         // Imaj7
                2 => ChordType.Minor7,         // ii7
                3 => ChordType.Minor7,         // iii7
                4 => ChordType.Major7,         // IVmaj7
                5 => ChordType.Dominant7,      // V7
                6 => ChordType.Minor7,         // vi7
                7 => ChordType.HalfDiminished7, // viiø7
                _ => ChordType.Dominant7
            };
        }
        else // Minor
        {
            return degree switch
            {
                1 => ChordType.Minor7,          // i7
                2 => ChordType.HalfDiminished7, // iiø7
                3 => ChordType.Major7,          // IIImaj7
                4 => ChordType.Minor7,          // iv7
                5 => ChordType.Dominant7,       // V7 (harmonic minor)
                6 => ChordType.Major7,          // VImaj7
                7 => ChordType.Diminished7,     // vii°7
                _ => ChordType.Minor7
            };
        }
    }

    private static int GetScaleDegree(Note note, KeySignature key)
    {
        var tonicPitchClass = GetPitchClass(key.Tonic);
        var notePitchClass = GetPitchClass(note);
        var interval = ((notePitchClass - tonicPitchClass) + 12) % 12;

        var majorIntervals = new[] { 0, 2, 4, 5, 7, 9, 11 };

        for (int i = 0; i < majorIntervals.Length; i++)
        {
            if (majorIntervals[i] == interval)
                return i + 1;
        }

        // Non-diatonic, find closest
        for (int i = 0; i < majorIntervals.Length; i++)
        {
            if (Math.Abs(majorIntervals[i] - interval) <= 1)
                return i + 1;
        }

        return 1;
    }

    private static int GetIntervalFromFigure(int figure)
    {
        // Figured bass figures represent intervals above the bass
        // These are diatonic intervals, so we need to convert to semitones
        return figure switch
        {
            2 => 2,  // Major 2nd
            3 => 4,  // Major 3rd (can be minor = 3)
            4 => 5,  // Perfect 4th
            5 => 7,  // Perfect 5th
            6 => 9,  // Major 6th (can be minor = 8)
            7 => 11, // Major 7th (can be minor = 10)
            8 => 12, // Octave
            _ => 0
        };
    }

    private static int GetPitchClass(Note note)
    {
        var semitones = MusicTheoryConstants.SemitonesFromC[(int)note.Name] + (int)note.Alteration;
        return ((semitones % 12) + 12) % 12;
    }

    private static Note PitchClassToNote(int pitchClass, int octave)
    {
        return pitchClass switch
        {
            0 => new Note(NoteName.C, Alteration.Natural, octave),
            1 => new Note(NoteName.D, Alteration.Flat, octave),
            2 => new Note(NoteName.D, Alteration.Natural, octave),
            3 => new Note(NoteName.E, Alteration.Flat, octave),
            4 => new Note(NoteName.E, Alteration.Natural, octave),
            5 => new Note(NoteName.F, Alteration.Natural, octave),
            6 => new Note(NoteName.G, Alteration.Flat, octave),
            7 => new Note(NoteName.G, Alteration.Natural, octave),
            8 => new Note(NoteName.A, Alteration.Flat, octave),
            9 => new Note(NoteName.A, Alteration.Natural, octave),
            10 => new Note(NoteName.B, Alteration.Flat, octave),
            11 => new Note(NoteName.B, Alteration.Natural, octave),
            _ => new Note(NoteName.C, Alteration.Natural, octave)
        };
    }
}
