using MusicTheory.Guitar.Blazor;
using Microsoft.JSInterop;

namespace MusicTheory.Guitar.UnitTests;

/// <summary>Version 2 behaviour of the guitar packages: validation, other tunings, spoken names, playback.</summary>
public class GuitarValueTests
{
    private static readonly GuitarVoicingProvider Provider = new();

    [Fact]
    public void GetVoicings_Validates()
    {
        var c = ChordSymbol.Parse("C");
        Should.Throw<ArgumentNullException>(() => Provider.GetVoicings(null!));
        Should.Throw<ArgumentOutOfRangeException>(() => Provider.GetVoicings(c, new VoicingOptions(Capo: -1))).ParamName.ShouldBe("Capo");
        Should.Throw<ArgumentOutOfRangeException>(() => Provider.GetVoicings(c, new VoicingOptions(MaxFret: 0))).ParamName.ShouldBe("MaxFret");
        Should.Throw<ArgumentOutOfRangeException>(() => Provider.GetVoicings(c, new VoicingOptions(MaxStretch: 0))).ParamName.ShouldBe("MaxStretch");
        Should.Throw<ArgumentOutOfRangeException>(() => Provider.GetVoicings(c, new VoicingOptions(Max: 0))).ParamName.ShouldBe("Max");
    }

    [Fact]
    public void GetVoicings_IgnoresTheSlashBassWhenAsked()
    {
        var voicings = Provider.GetVoicings(ChordSymbol.Parse("C/G"), new VoicingOptions(IncludeSlashBass: false));
        voicings.ShouldNotBeEmpty();
        voicings.ShouldAllBe(v => v.Chord.Bass == null);
        voicings.ShouldContain(v => v.ToTab() == "x32010");
    }

    [Fact]
    public void GetVoicings_WorksOnAFourStringUkulele()
    {
        var ukulele = new Tuning("Ukulele", new[] { "G4", "C4", "E4", "A4" }.Select(Note.Parse).ToArray());
        var voicings = Provider.GetVoicings(ChordSymbol.Parse("Am"), new VoicingOptions(Tuning: ukulele));
        voicings.ShouldNotBeEmpty();
        voicings.ShouldAllBe(v => v.Source == VoicingSource.Generated && v.Frets.Count == 4);
    }

    [Fact]
    public void GetVoicings_DropsShapesWiderThanTheStretch()
    {
        var tight = Provider.GetVoicings(ChordSymbol.Parse("C"), new VoicingOptions(MaxStretch: 2, Max: 50));
        tight.ShouldNotBeEmpty();
        tight.ShouldAllBe(v => v.Frets.Where(f => f > 0).DefaultIfEmpty(0).Max() - v.Frets.Where(f => f > 0).DefaultIfEmpty(0).Min() < 2);
        tight.ShouldNotContain(v => v.ToTab() == "x32010");
    }

    [Fact]
    public void Tuning_Validates()
    {
        Should.Throw<ArgumentNullException>(() => new Tuning(null!, [Note.Parse("E2")]));
        Should.Throw<ArgumentNullException>(() => new Tuning("x", null!));
        Should.Throw<ArgumentException>(() => new Tuning("x", []));
        Should.Throw<ArgumentException>(() => new Tuning("x", [null!]));
        Should.Throw<ArgumentNullException>(() => Tuning.Standard with { Name = null! });
        Should.Throw<ArgumentException>(() => Tuning.Standard with { Strings = [] });
        (Tuning.Standard with { Name = "Mine" }).Name.ShouldBe("Mine");
    }

    [Fact]
    public void ChordDiagramLayout_Validates()
    {
        var v = Provider.GetVoicings(ChordSymbol.Parse("G"))[0];
        Should.Throw<ArgumentNullException>(() => ChordDiagramLayout.For(null!));
        Should.Throw<ArgumentOutOfRangeException>(() => ChordDiagramLayout.For(v, new DiagramOptions(Frets: 0)));
        Should.Throw<ArgumentOutOfRangeException>(() => ChordDiagramLayout.For(v, new DiagramOptions(StringSpacing: 0)));
        Should.Throw<ArgumentOutOfRangeException>(() => ChordDiagramLayout.For(v, new DiagramOptions(FretSpacing: -1)));
    }

    [Fact]
    public void VoicingDescriber_NamesEveryChordType()
    {
        foreach (var type in Enum.GetValues<ChordType>())
            Should.NotThrow(() => VoicingDescriber.ChordName(new ChordSymbol(new Note(NoteName.C), type)));
    }

    [Theory]
    [InlineData("Am", "A minor")]
    [InlineData("D/F#", "D major over F sharp")]
    [InlineData("Bbmaj7", "B flat major seven")]
    [InlineData("Ebbdim7", "E double flat diminished seven")]
    [InlineData("F##7alt", "F double sharp seven altered")]
    [InlineData("C5", "C five")]
    public void VoicingDescriber_ChordName(string symbol, string spoken) =>
        VoicingDescriber.ChordName(ChordSymbol.Parse(symbol)).ShouldBe(spoken);

    [Fact]
    public void VoicingDescriber_Validates()
    {
        Should.Throw<ArgumentNullException>(() => VoicingDescriber.ChordName(null!));
        Should.Throw<ArgumentNullException>(() => VoicingDescriber.AccessibleLabel(null!));
        VoicingDescriber.AccessibleLabel(Provider.GetVoicings(ChordSymbol.Parse("Am"))[0]).ShouldBe("A minor, x-0-2-2-1-0");
    }

    // ── MusicTheory.Guitar.Blazor ─────────────────────────────────────────────

    private sealed class FakeModule(Exception? onDispose = null) : IJSObjectReference
    {
        public List<(string Identifier, object?[]? Args)> Calls { get; } = [];
        public bool Disposed { get; private set; }

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
        {
            Calls.Add((identifier, args));
            return ValueTask.FromResult(default(TValue)!);
        }

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args) =>
            InvokeAsync<TValue>(identifier, args);

        public ValueTask DisposeAsync()
        {
            Disposed = true;
            return onDispose is null ? ValueTask.CompletedTask : ValueTask.FromException(onDispose);
        }
    }

    private sealed class FakeRuntime(FakeModule module) : IJSRuntime
    {
        public List<object?[]?> Imports { get; } = [];

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
        {
            identifier.ShouldBe("import");
            Imports.Add(args);
            return ValueTask.FromResult((TValue)(object)module);
        }

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args) =>
            InvokeAsync<TValue>(identifier, args);
    }

    [Fact]
    public async Task ChordAudio_ImportsTheModuleOnce_AndPlaysMidi()
    {
        var module = new FakeModule();
        var js = new FakeRuntime(module);
        await using (var audio = new ChordAudio(js))
        {
            await audio.PlayAsync(Provider.GetVoicings(ChordSymbol.Parse("C"))[0]);
            await audio.PlayAsync([60, 64, 67], strumMs: 10, durationMs: 500, volume: 0.3);
            await audio.StopAsync();
        }

        js.Imports.Count.ShouldBe(1);
        js.Imports[0]![0].ShouldBe("./_content/MusicTheory.Guitar.Blazor/chord-audio.js");
        module.Calls.Select(c => c.Identifier).ShouldBe(["playVoicing", "playVoicing", "stop"]);
        module.Calls[0].Args![0].ShouldBe(new[] { 48, 52, 55, 60, 64 });
        module.Calls[1].Args![0].ShouldBe(new[] { 60, 64, 67 });
        module.Disposed.ShouldBeTrue();
    }

    [Fact]
    public async Task ChordAudio_DisposeBeforeUse_DoesNothing_AndToleratesALostCircuit()
    {
        var unused = new FakeModule();
        await new ChordAudio(new FakeRuntime(unused)).DisposeAsync();
        unused.Disposed.ShouldBeFalse();

        var lost = new FakeModule(new JSDisconnectedException("gone"));
        var audio = new ChordAudio(new FakeRuntime(lost));
        await audio.StopAsync();
        await Should.NotThrowAsync(async () => await audio.DisposeAsync());
        lost.Disposed.ShouldBeTrue();
    }
}
