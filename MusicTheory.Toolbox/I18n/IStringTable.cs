namespace MusicTheory.Toolbox.I18n;

/// <summary>One tool's (or the shell's) copy, in both languages. The parity test keeps the key sets equal.</summary>
public interface IStringTable
{
    IReadOnlyDictionary<string, string> En { get; }
    IReadOnlyDictionary<string, string> Fr { get; }
}
