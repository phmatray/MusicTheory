using Microsoft.JSInterop;

namespace MusicTheory.Guitar.Blazor;

/// <summary>
/// Plays voicings in the browser through <c>_content/MusicTheory.Guitar.Blazor/chord-audio.js</c>.
/// Call <see cref="PlayAsync(GuitarVoicing, Tuning?, int)"/> from a user gesture (a click), as browsers only
/// start audio after one.
/// </summary>
public sealed class ChordAudio(IJSRuntime js) : IAsyncDisposable
{
    private Task<IJSObjectReference>? _module;

    private Task<IJSObjectReference> Module =>
        _module ??= js.InvokeAsync<IJSObjectReference>("import", "./_content/MusicTheory.Guitar.Blazor/chord-audio.js").AsTask();

    /// <summary>Strums a guitar voicing (muted strings skipped).</summary>
    public ValueTask PlayAsync(GuitarVoicing voicing, Tuning? tuning = null, int capo = 0) =>
        PlayAsync(voicing.ToMidi(tuning, capo));

    /// <summary>Strums MIDI notes low to high, e.g. a piano voicing.</summary>
    public async ValueTask PlayAsync(IReadOnlyList<int> midiNotes, int strumMs = 25, int durationMs = 1800, double volume = 0.6) =>
        await (await Module).InvokeVoidAsync("playVoicing", midiNotes, new { strumMs, durationMs, volume });

    /// <summary>Silences the current chord.</summary>
    public async ValueTask StopAsync() => await (await Module).InvokeVoidAsync("stop");

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_module is null)
            return;
        try
        {
            await (await _module).DisposeAsync();
        }
        catch (JSDisconnectedException)
        {
            // The circuit is gone; there is nothing left to release.
        }
    }
}
