![MusicTheory banner](.github/banner.png)

# MusicTheory

<!-- portfolio-toc:start -->

## Table of Contents

- [✨ Features](#-features)
- [📦 Installation](#-installation)
- [🚀 Quick Start](#-quick-start)
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

Music theory for .NET that spells every note correctly. Notes, intervals, scales, chords, keys and harmony
are immutable values built the way they are written on the staff, so F♯ major has an E♯ and C up to F𝄪 is a
doubly augmented fourth.

> 📦 **Install:** `dotnet add package MusicTheory` · **Docs:** <https://phmatray.github.io/MusicTheory/>

## ✨ Features

- **Notes**: letter, accidental and octave; English and German names; MIDI numbers; frequencies at any
  concert pitch (`GetFrequency(415)`); enharmonic respelling.
- **Intervals**: every quality from quintuply diminished to quintuply augmented, direction, compound
  intervals, inversion, music21 names (`M3`, `M-3`, `dd2`) with `Parse`.
- **Chords**: 42 chord types, extensions, inversions, and lead-sheet symbols with tensions (`C7b9#11`).
- **Scales**: major, three minors, seven modes, pentatonics, blues, whole tone and chromatic, spelled by interval.
  Also the melodic-minor and harmonic-minor modes (`ScaleType.Altered`, `LydianDominant`, `PhrygianDominant`),
  bebop scales (`BebopDominant`) and world scales (`HungarianMinor`, `Hirajoshi`), 22 in all.
- **Keys**: signatures including theoretical keys, the circle of fifths, relative and parallel keys.
- **Harmony**: diatonic triads and sevenths, Roman-numeral analysis, key detection, voice-leading checks.
- **Rhythm**: durations with dots and tuplets, time signatures.
- **Guitar** (`MusicTheory.Guitar`): voicings for every chord type in any tuning, capo, chord-diagram geometry.
- **Blazor** (`MusicTheory.Guitar.Blazor`): chord playback in the browser.

## 📦 Installation

```bash
dotnet add package MusicTheory
dotnet add package MusicTheory.Guitar          # optional
dotnet add package MusicTheory.Guitar.Blazor   # optional
```

Requires .NET 11.

## 🚀 Quick Start

```csharp
using MusicTheory;

Interval.Between(Note.Parse("C4"), Note.Parse("F##4"));          // AA4
Note.Parse("C4").Transpose(Interval.Parse("M-3"));                // Ab3
new Scale(Note.Parse("F#4"), ScaleType.Major).GetNotes();         // F#4 G#4 A#4 B4 C#5 D#5 E#5 F#5
ChordSymbol.Parse("C7b9#11").ToChord().GetNotes();                // C4 E4 G4 Bb4 Db5 F#5
KeySignature.Parse("Eb").AccidentalCount;                         // -3
Note.Parse("A4").GetFrequency(442);                               // 442

HarmonicAnalyzer.Analyze(
    new[] { "Dm7", "G7", "Cmaj7" }.Select(ChordSymbol.Parse),
    KeySignature.Parse("C"));                                     // ii7 V7 Imaj7

CadenceDetector.Detect(
    new[] { "C", "F", "G", "Am" }.Select(ChordSymbol.Parse).ToList(),
    KeySignature.Parse("C"));                                     // Cadence(Deceptive, 3)
```

## 🎯 How it is checked

- **Against music21.** `tools/reference/generate.py` computes 3,669 intervals, 2,240 transpositions,
  315 scales and 70 key signatures with [music21](https://web.mit.edu/music21/); the tests replay every row.
- **100% coverage.** The three packages are covered at 100% of lines and branches; CI fails below that
  (`tools/coverage/check.py`).
- **Mutation testing.** Stryker.NET runs on demand (Actions, *Mutation testing*), with break thresholds in each test
  project's `stryker-config.json`.
- **Docs that run.** Every C# block of `docs/` is compiled, run and has its printed output compared by
  `MusicTheory.DocTests`.

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

<!-- portfolio-techstack:end -->

## 🗺️ Roadmap

The next features are tracked as issues, one family each: tuning and temperaments (#118), the mode
catalogue (#121), piano voicings (#125), advanced harmony (#128), exact rhythm (#133), pitch-class set
theory (#138), MusicXML, MIDI, ChordPro and ABC (#142), more instruments (#147), solfège (#117) and ear
training (#152).

## 📖 Documentation

The site at <https://phmatray.github.io/MusicTheory/> is built from `docs/` and the XML documentation by
`tools/SiteGen`: guides, tutorials, and an API reference generated from the code.

## 📄 License

MIT. See [LICENSE](LICENSE).
