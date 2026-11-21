![MusicTheory banner](.github/banner.png)

# MusicTheory

<!-- portfolio-toc:start -->

## Table of Contents

- [✨ Features](#-features)
- [📦 Installation](#-installation)
- [🚀 Quick Start](#-quick-start)
- [🔁 Upgrading to 3.0](#-upgrading-to-30)
- [🧰 Toolbox](#-toolbox)
- [🎯 How it is checked](#-how-it-is-checked)
- [🧪 Testing](#-testing)
- [Tech Stack](#tech-stack)
- [🗺️ Roadmap](#️-roadmap)
- [📖 Documentation](#-documentation)
- [📄 License](#-license)

<!-- portfolio-toc:end -->

[![NuGet](https://img.shields.io/nuget/v/MusicTheory.svg)](https://www.nuget.org/packages/MusicTheory/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/MusicTheory.svg)](https://www.nuget.org/packages/MusicTheory/)
[![.NET](https://img.shields.io/badge/.NET-11.0-blue)](https://dotnet.microsoft.com/)
[![Build](https://github.com/phmatray/MusicTheory/actions/workflows/dotnet.yml/badge.svg)](https://github.com/phmatray/MusicTheory/actions/workflows/dotnet.yml)
[![Mutation testing](https://github.com/phmatray/MusicTheory/actions/workflows/mutation.yml/badge.svg)](https://github.com/phmatray/MusicTheory/actions/workflows/mutation.yml)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

Music theory for .NET that spells every note correctly. Notes, intervals, scales, chords, keys, harmony and
rhythm are immutable values built the way they are written on the staff, so F♯ major has an E♯, C up to F𝄪
is a doubly augmented fourth, and a borrowed E♭ in C moves to B♭ in G, not A♯.

> 📦 **Install:** `dotnet add package MusicTheory` · **Docs:** <https://phmatray.github.io/MusicTheory/> ·
> **Toolbox:** <https://phmatray.github.io/MusicTheory/toolbox/>

## ✨ Features

**Pitch**
- **Notes**: letter, accidental and octave; English and German names; MIDI numbers; enharmonic respelling.
- **Intervals**: every quality from quintuply diminished to quintuply augmented, direction, compound
  intervals, inversion, music21 names (`M3`, `M-3`, `dd2`) with `Parse`.
- **Tuning**: frequencies at any concert pitch, the nearest note and cents from a frequency, and equal,
  Pythagorean, just and quarter-comma meantone temperaments.
- **Solfège**: fixed-do and movable-do syllables, with la-based minor.
- **Pitch-class sets**: normal and prime form, interval-class vector, the Forte catalogue, and the
  neo-Riemannian moves P, L and R.

**Scales and keys**
- **Scales**: major, the three minors, the seven modes, the melodic- and harmonic-minor modes
  (`Altered`, `LydianDominant`, `PhrygianDominant`), bebop, pentatonic, blues and world scales, 22 in all.
- **Chord-scales**: which scales fit a chord, fewest avoid notes first (`ChordScale`).
- **Keys**: signatures up to fourteen accidentals, the circle of fifths, relative and parallel keys.

**Harmony**
- **Chords**: 44 chord types, extensions, inversions, and lead-sheet symbols with tensions (`C7b9#11`),
  aliases (`C-7`, `Cmi7`, `CΔ7`) and an opt-in lenient parse for lower-case roots.
- **Analysis**: Roman numerals, key detection, Neapolitan and augmented sixths, cadences, modulations
  with pivot chords.
- **Voicing**: close, drop-2, drop-3, shell and rootless piano voicings, and voice leading between chords.
- **Reharmonisation**: tritone substitutions and ii-V insertion.
- **Transposition**: by interval between keys, with an option to keep the writer's suffix as typed.

**Rhythm**
- Exact durations on `Rational`, dots and tuplets, time signatures, measures with ties across barlines,
  beat groups and beaming, tempo maps and swing.

**Instruments and practice**
- **Fretted instruments**: guitar (6, 7 and 8 strings), ukulele, bass and mandolin tunings, a fretboard
  map, and ranked voicings for every chord type (`MusicTheory.Guitar`).
- **Piano**: scale and triad fingering.
- **Ear training**: seeded, reproducible interval, chord, scale, progression and dictation drills.

## 📦 Installation

| Package | What it adds |
|---|---|
| [`MusicTheory`](https://www.nuget.org/packages/MusicTheory/) | The theory: everything above except the packages below. |
| [`MusicTheory.Guitar`](https://www.nuget.org/packages/MusicTheory.Guitar/) | Voicings in any tuning, capo, chord-diagram geometry, spoken chord names in English and French. |
| [`MusicTheory.Guitar.Blazor`](https://www.nuget.org/packages/MusicTheory.Guitar.Blazor/) | Chord playback in the browser for Blazor apps. |
| [`MusicTheory.ChordPro`](https://www.nuget.org/packages/MusicTheory.ChordPro/) | Parse, transpose and write ChordPro chord sheets. |
| [`MusicTheory.Midi`](https://www.nuget.org/packages/MusicTheory.Midi/) | Write and read Standard MIDI Files. |
| [`MusicTheory.MusicXml`](https://www.nuget.org/packages/MusicTheory.MusicXml/) | Write and read single-part MusicXML. |
| [`MusicTheory.Abc`](https://www.nuget.org/packages/MusicTheory.Abc/) | Parse and write single-voice ABC tunes. |

```bash
dotnet add package MusicTheory
dotnet add package MusicTheory.Guitar          # optional, and so on for each package above
```

Every package shares one version and requires .NET 11.

## 🚀 Quick Start

```csharp
using MusicTheory;

Interval.Between(Note.Parse("C4"), Note.Parse("F##4"));          // AA4
Note.Parse("C4").Transpose(Interval.Parse("M-3"));                // Ab3
new Scale(Note.Parse("F#4"), ScaleType.Major).GetNotes();         // F#4 G#4 A#4 B4 C#5 D#5 E#5 F#5
ChordSymbol.Parse("C7b9#11").ToChord().GetNotes();                // C4 E4 G4 Bb4 Db5 F#5
KeySignature.Parse("Eb").AccidentalCount;                         // -3
Note.Parse("A4").GetFrequency(442);                               // 442
Note.FromFrequency(446.0);                                        // A4 +23.4¢
TunedPitch.CentsBetween(440, 880);                                // 1200
Temperament.JustIntonation.GetFrequency(Note.Parse("E4"), tonic: Note.Parse("C4"));  // 330 (pure 5/4 over a 264 C)
PitchClassSet.Of(0, 4, 7).ForteName;                              // 3-11

// Transposition follows the interval between the keys, and can keep the chart's own notation.
ChordTransposer.Transpose("Eb", KeySignature.Parse("C"), KeySignature.Parse("G"));   // Bb
ChordTransposer.Transpose("C-7", 2, suffix: SuffixHandling.Verbatim);                // D-7

// Rhythm is exact: a 4/4 bar holding one quarter note has 3/4 left.
new Measure(TimeSignature.CommonTime,
    [new RhythmEvent([Note.Parse("C4")], new Duration(DurationType.Quarter))]).Remaining;  // 3/4

HarmonicAnalyzer.Analyze(
    new[] { "Dm7", "G7", "Cmaj7" }.Select(ChordSymbol.Parse),
    KeySignature.Parse("C"));                                     // ii7 V7 Imaj7

CadenceDetector.Detect(
    new[] { "C", "F", "G", "Am" }.Select(ChordSymbol.Parse).ToList(),
    KeySignature.Parse("C"));                                     // Cadence(Deceptive, 3)

// Voice leading: each voicing moves the least from the previous one.
foreach (var v in VoiceLeader.Lead(
             new[] { "Dm7", "G7", "Cmaj7" }.Select(ChordSymbol.Parse), PianoVoicingStyle.RootlessA))
    Console.WriteLine(string.Join(" ", v));
// F4 A4 C5 E5
// F4 A4 B4 D5
// E4 G4 B4 D5

// Which scales fit a chord? Fewest avoid notes first.
ChordScale.ScalesFor(ChordSymbol.Parse("G7")).Select(s => s.Type);  // LydianDominant, ..., Mixolydian, ...
ChordScale.ScalesFor(ChordSymbol.Parse("G7alt")).Any(s => s.Type == ScaleType.Altered);  // true
```

## 🔁 Upgrading to 3.0

3.0 changes the rhythm API; everything else is additive.

- `Duration` is an exact record on `Rational`: `Duration.Add`, `Divide`, `GetValueInWholeNotes` and
  `GetValueInMeasures` are gone, use `Duration.Value`.
- `TimeSignature` is a sealed record: `GetMeasureDuration()` (a `double`) becomes `MeasureLength` (a `Rational`).
- `KeySignature.TryParse` now accepts theoretical keys such as `G#` (eight sharps).

[Measures and tempo](docs/measures-and-tempo.md) shows the new rhythm API, the [FAQ](docs/faq.md) lists
every change since 1.x, and [CHANGELOG.md](CHANGELOG.md) has the full history.

## 🧰 Toolbox

A client-side web app for music schools and their students, at <https://phmatray.github.io/MusicTheory/toolbox/>,
in French and English. It runs the library in the browser: seeded drills a teacher shares as a link (the whole
class gets the same exercise), melodic dictation with MIDI and MusicXML export, a transposer for B♭, E♭ and F
instruments, the circle of fifths, a chord and scale finder, fretboard, voicings, piano fingering and
progression analysis. Nothing is stored or sent anywhere; the
[teacher guide and privacy statement](https://phmatray.github.io/MusicTheory/toolbox/?t=help) say exactly how.

## 🎯 How it is checked

- **Against music21.** `tools/reference/generate.py` computes 3,669 intervals, 2,240 transpositions,
  315 scales and 70 key signatures with [music21](https://web.mit.edu/music21/); the tests replay every row.
  The MIDI, MusicXML and ABC readers are pinned to files music21 wrote.
- **100% coverage.** The seven packages are covered at 100% of lines and branches; CI fails below that
  (`tools/coverage/check.py`).
- **Mutation testing.** Stryker.NET runs on demand (Actions, *Mutation testing*), with break thresholds in each test
  project's `stryker-config.json`.
- **Docs that run.** Every C# block of `docs/` is compiled, run and has its printed output compared by
  `MusicTheory.DocTests`.
- **Trusted publishing.** Releases are cut by release-please and pushed to NuGet with GitHub OIDC, no stored key.

## 🧪 Testing

```bash
dotnet test                                                         # all suites
dotnet test --project MusicTheory.UnitTests --coverage --coverage-output-format cobertura --coverage-output $PWD/TestResults/core.xml
dotnet tool restore && (cd MusicTheory.UnitTests && dotnet stryker)  # mutation testing
UPDATE_GOLDEN=1 dotnet test --project MusicTheory.Guitar.UnitTests   # accept an intended change to the golden files
```

<!-- portfolio-techstack:start -->
## Tech Stack

- **C#**, .NET 11
- xunit.v3 on Microsoft.Testing.Platform, Shouldly
- Microsoft.Testing.Extensions.CodeCoverage, Stryker.NET
- music21 (reference tables), Roslyn scripting (doc tests), Markdig (site)
- Blazor WebAssembly and abcjs (toolbox)

<!-- portfolio-techstack:end -->

## 🗺️ Roadmap

3.0.0 ships everything planned so far: the feature families of #117 to #156, Documentation 2.0 (#230)
and the Toolbox (#244). New work starts as an [issue](https://github.com/phmatray/MusicTheory/issues).

## 📖 Documentation

The site at <https://phmatray.github.io/MusicTheory/> is built from `docs/` and the XML documentation by
`tools/SiteGen`: guides, tutorials, and an API reference generated from the code. Start with
[Getting started](docs/getting-started.md), then the guides for
[tuning and temperaments](docs/tuning-and-temperaments.md), [solfège](docs/solfege.md),
[modes](docs/mode-catalogue.md), [piano voicings](docs/piano-voicings.md),
[advanced harmony](docs/advanced-harmony.md), [measures and tempo](docs/measures-and-tempo.md),
[pitch-class sets](docs/pitch-class-sets.md), [instruments](docs/instruments.md),
[chord sheets and ABC](docs/chord-sheets-and-abc.md), [MIDI and MusicXML](docs/midi-and-musicxml.md) and
[ear training](docs/ear-training.md).

## 📄 License

MIT. See [LICENSE](LICENSE).
