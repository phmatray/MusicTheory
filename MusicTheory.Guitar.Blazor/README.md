# MusicTheory.Guitar.Blazor

Optional Blazor add-on for [MusicTheory.Guitar](https://www.nuget.org/packages/MusicTheory.Guitar): light
chord playback. Plucked strings are synthesised in plain Web Audio (no WASM, no dependencies) from MIDI
numbers, so it follows any tuning or capo and also plays piano voicings.

```csharp
builder.Services.AddScoped<ChordAudio>();
```

```razor
@inject ChordAudio Audio

<button @onclick="() => Audio.PlayAsync(voicing)">Play</button>
<button @onclick="() => Audio.StopAsync()">Stop</button>
```

Without the C# wrapper, import the module yourself:

```js
const audio = await import('./_content/MusicTheory.Guitar.Blazor/chord-audio.js');
await audio.playVoicing([48, 52, 55, 60, 64], { strumMs: 25, durationMs: 1800, volume: 0.6 });
audio.stop();
```

Source and issues: https://github.com/phmatray/MusicTheory
