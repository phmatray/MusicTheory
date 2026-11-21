using MusicTheory.Toolbox.I18n;

namespace MusicTheory.Toolbox.Tools.Analysis;

/// <summary>One chord of the progression, read in the key. <see cref="Cadence"/> is set when a cadence arrives on it. Indexes are 1-based.</summary>
public sealed record Row(int Index, string Chord, string Numeral, string Function, string? Cadence);

/// <summary>A cadence, at the (1-based) index of its arrival chord.</summary>
public sealed record CadenceMark(int Index, string Label);

/// <summary>A stretch of chords in one key (1-based, inclusive); <see cref="PivotIndex"/> is the chord shared with the previous key.</summary>
public sealed record RegionRow(string Key, int Start, int End, int? PivotIndex);

/// <summary>A detected key, with its confidence as a whole percentage.</summary>
public sealed record KeyChoice(string Name, int Percent);

/// <summary>What <see cref="AnalysisModel.Run"/> returns; <see cref="Error"/> is set (and the lists empty) when the input is unusable.</summary>
public sealed record AnalysisResult(
    IReadOnlyList<Row> Rows,
    IReadOnlyList<CadenceMark> Cadences,
    IReadOnlyList<RegionRow> Regions,
    IReadOnlyList<KeyChoice> Candidates,
    string? UsedKey,
    bool Detected,
    string? Error);

public static class AnalysisModel
{
    /// <summary>Analyses <paramref name="chordsText"/> (tokens split on whitespace and <c>|</c>) in <paramref name="key"/>, or in the detected key when it is null or blank. Never throws.</summary>
    public static AnalysisResult Run(string chordsText, string? key, Lang lang)
    {
        var chords = new List<ChordSymbol>();
        var tokens = (chordsText ?? "").Split([' ', '\t', '\r', '\n', '|'], StringSplitOptions.RemoveEmptyEntries);
        foreach (var token in tokens)
        {
            if (!ChordSymbol.TryParse(token, ChordSymbolParseOptions.Lenient, out var chord))
                return Fail(Strings.T(lang, "analysis.badtoken", token));
            chords.Add(chord);
        }
        if (chords.Count == 0) return new AnalysisResult([], [], [], [], null, false, null);

        var detected = string.IsNullOrWhiteSpace(key);
        KeySignature? used;
        List<KeyChoice> candidates = [];
        if (detected)
        {
            var ranked = KeyDetector.Infer(chords);
            candidates = ranked.Select(c => new KeyChoice(KeyName(c.Key, lang), (int)Math.Round(c.Confidence * 100))).ToList();
            used = ranked[0].Key;
        }
        else if (!KeySignature.TryParse(key!.Trim(), out used))
            return Fail(Strings.T(lang, "analysis.badkey", key));

        var numerals = HarmonicAnalyzer.Analyze(chords, used);
        var cadences = CadenceDetector.Detect(chords, used)
            .Select(c => new CadenceMark(c.Index + 1, Strings.T(lang, "analysis.cad." + c.Type))).ToList();
        var rows = numerals.Select((n, i) => new Row(i + 1, tokens[i], n.ToString(),
            Strings.T(lang, "analysis.fn." + n.Function), cadences.FirstOrDefault(c => c.Index == i + 1)?.Label)).ToList();
        var regions = ModulationDetector.Detect(chords)
            .Select(r => new RegionRow(KeyName(r.Key, lang), r.Start + 1, r.End + 1, r.PivotIndex + 1)).ToList();
        return new AnalysisResult(rows, cadences, regions, candidates, KeyName(used, lang), detected, null);
    }

    private static AnalysisResult Fail(string error) => new([], [], [], [], null, false, error);

    private static string KeyName(KeySignature key, Lang lang) =>
        Strings.T(lang, key.Mode == KeyMode.Minor ? "analysis.minor" : "analysis.major", NoteNames.Format(key.Tonic, lang));
}
