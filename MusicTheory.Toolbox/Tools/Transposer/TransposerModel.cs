using MusicTheory.Toolbox.I18n;

namespace MusicTheory.Toolbox.Tools.Transposer;

public enum TransposeDirection { ConcertToWritten, WrittenToConcert }

public sealed record TransposeResult(KeySignature TargetKey, IReadOnlyList<string> Chords, IReadOnlyList<string> Notices);

public static class TransposerModel
{
    /// <summary>The instrument's label; its note (what it sounds for a written C) is spelled by <see cref="NoteNames"/>.</summary>
    public static string Name(Instrument i, Lang lang) =>
        Strings.T(lang, i.NameKey, NoteNames.Format(new KeySignature(Note.Parse("C4"), KeyMode.Major).TransposeBySemitones(-i.WrittenAboveConcertSemitones).Tonic, lang));

    public static TransposeResult Run(Instrument instrument, TransposeDirection direction, KeySignature key, string chordsText, Lang lang)
    {
        var notices = new List<string>();
        var semitones = direction == TransposeDirection.ConcertToWritten ? instrument.WrittenAboveConcertSemitones : -instrument.WrittenAboveConcertSemitones;

        var from = Practical(key, lang, notices);
        var to = Practical(from.TransposeBySemitones(semitones), lang, notices);

        var chords = new List<string>();
        foreach (var token in chordsText.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries))
        {
            var moved = ChordTransposer.Transpose(token, from, to, null, SuffixHandling.Verbatim);
            // Unchanged text only means "not a chord" when the move was not a whole number of octaves.
            if (moved == token && semitones % 12 != 0 && char.IsLetter(token[0]))
                notices.Add(Strings.T(lang, "transpose.unparsed", token));
            chords.Add(moved);
        }
        return new TransposeResult(to, chords, notices);
    }

    // A key needing more than seven accidentals (A#, G#) is shown as its enharmonic equivalent, with a notice.
    private static KeySignature Practical(KeySignature key, Lang lang, List<string> notices)
    {
        if (Math.Abs(key.AccidentalCount) <= 7) return key;
        var practical = key.GetEnharmonicEquivalents().First();
        notices.Add(Strings.T(lang, "transpose.theoretical", NoteNames.Format(key.Tonic, lang), NoteNames.Format(practical.Tonic, lang)));
        return practical;
    }
}
