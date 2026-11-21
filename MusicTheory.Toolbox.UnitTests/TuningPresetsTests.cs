using MusicTheory.Guitar;
using MusicTheory.Toolbox.Tools.Fretboard;

namespace MusicTheory.Toolbox.UnitTests;

public class TuningPresetsTests
{
    [Fact]
    public void Every_library_preset_has_an_entry_with_both_labels()
    {
        var library = typeof(Tuning).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(p => p.PropertyType == typeof(Tuning))
            .Select(p => (Tuning)p.GetValue(null)!).ToList();
        library.Count.ShouldBeGreaterThanOrEqualTo(10);
        foreach (var t in library)
            TuningPresets.All.Count(p => p.Tuning == t).ShouldBe(1, t.Name);
        TuningPresets.All.Where(p => string.IsNullOrWhiteSpace(p.En) || string.IsNullOrWhiteSpace(p.Fr)).ShouldBeEmpty();
        TuningPresets.All.Select(p => p.Id).Distinct().Count().ShouldBe(TuningPresets.All.Count);
    }
}
