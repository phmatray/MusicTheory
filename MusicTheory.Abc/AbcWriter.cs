using System.Globalization;
using System.Text;

namespace MusicTheory.Abc;

/// <summary>Writes the canonical ABC of a tune: the inverse of <see cref="AbcParser"/>.</summary>
internal static class AbcWriter
{
    internal static string Write(AbcTune tune)
    {
        var sb = new StringBuilder();
        sb.Append("X:").Append(Int(tune.ReferenceNumber)).Append('\n');
        if (tune.Title is not null)
            sb.Append("T:").Append(tune.Title).Append('\n');
        sb.Append("M:").Append(Int(tune.TimeSignature.Numerator)).Append('/').Append(Int(tune.TimeSignature.Denominator)).Append('\n');
        sb.Append("L:").Append(Fraction(tune.UnitLength)).Append('\n');
        if (tune.Tempo is not null)
        {
            var change = tune.Tempo.Changes[0];
            var beat = (change.BeatUnit ?? new Duration(DurationType.Quarter)).Value;
            sb.Append("Q:").Append(Fraction(beat)).Append('=').Append(change.Bpm.ToString(CultureInfo.InvariantCulture)).Append('\n');
        }

        sb.Append("K:").Append(tune.Key.Tonic.GetName()).Append(tune.Key.Mode == KeyMode.Minor ? "m" : "");
        if (tune.Measures.Count > 0)
            sb.Append('\n').AppendJoin('|', tune.Measures.Select(m => WriteBar(m, tune.Key, tune.UnitLength))).Append("|]");
        return sb.ToString();
    }

    private static string Int(long n) => n.ToString(CultureInfo.InvariantCulture);

    private static string Fraction(Rational r) => $"{Int(r.Numerator)}/{Int(r.Denominator)}";

    private static string WriteBar(Measure measure, KeySignature key, Rational unit)
    {
        var accidentals = new Dictionary<(NoteName, int), Alteration>();
        var words = new List<string>();
        var pending = 0; // events left in an open triplet
        foreach (var e in measure.Events)
        {
            var sb = new StringBuilder();
            var value = e.Duration.Value;
            if (e.Duration.Tuplet is var (actual, normal))
            {
                if (pending == 0)
                {
                    sb.Append("(3");
                    pending = 3;
                }

                pending--;
                value *= Rational.Of(actual, normal);
            }

            if (e.IsRest)
                sb.Append('z');
            else if (e.Pitches.Count == 1)
                sb.Append(WriteNote(e.Pitches[0], key, accidentals));
            else
                sb.Append('[').AppendJoin("", e.Pitches.Select(p => WriteNote(p, key, accidentals))).Append(']');
            sb.Append(Length(value / unit));
            if (e.TiedToNext)
                sb.Append('-');
            words.Add(sb.ToString());
        }

        return string.Join(' ', words);
    }

    private static string Length(Rational factor)
    {
        var numerator = factor.Numerator == 1 ? "" : Int(factor.Numerator);
        return factor.Denominator == 1 ? numerator : numerator + "/" + Int(factor.Denominator);
    }

    private static string WriteNote(Note note, KeySignature key, Dictionary<(NoteName, int), Alteration> accidentals)
    {
        var current = accidentals.TryGetValue((note.Name, note.Octave), out var seen) ? seen : key.GetAlteration(note.Name);
        var prefix = "";
        if (note.Alteration != current)
        {
            var n = (int)note.Alteration;
            prefix = n == 0 ? "=" : new string(n > 0 ? '^' : '_', Math.Abs(n));
            accidentals[(note.Name, note.Octave)] = note.Alteration;
        }

        var letter = note.Name.ToString();
        return note.Octave >= 5
            ? prefix + letter.ToLowerInvariant() + new string('\'', note.Octave - 5)
            : prefix + letter + new string(',', 4 - note.Octave);
    }
}
