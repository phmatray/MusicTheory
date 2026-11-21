# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
dotnet build                                   # .NET 11 SDK (global.json)
dotnet test                                    # all suites: core, guitar, docs
dotnet test --project MusicTheory.UnitTests --filter-method "*Interval*"   # MTP filter syntax

# Coverage gate (CI fails below 100% lines and branches on the three libraries)
dotnet test --project MusicTheory.UnitTests --coverage --coverage-output-format cobertura --coverage-output $PWD/TestResults/core.xml
dotnet test --project MusicTheory.Guitar.UnitTests --coverage --coverage-output-format cobertura --coverage-output $PWD/TestResults/guitar.xml
python3 tools/coverage/check.py TestResults/*.xml

# Mutation testing (run from the test project folder; thresholds in its stryker-config.json)
dotnet tool restore && (cd MusicTheory.UnitTests && dotnet stryker)

# Site: docs/*.md + API reference + evaluated home page -> _site/
dotnet run --project tools/SiteGen -- . _site

# music21 reference tables (only when changing what is checked)
python3 -m venv .venv && .venv/bin/pip install -r tools/reference/requirements.txt && .venv/bin/python tools/reference/generate.py
```

On macOS 27 the DaisyBlazor tailwind binary cached under `~/Library/Application Support/daisyblazor` has an
invalid signature and is killed at launch, which breaks the GuitarChords build. Point
`DaisyBlazorTailwindPath` at an ad-hoc signed copy (`codesign --force -s -`).

## Architecture

Packages: `MusicTheory` (the theory), `MusicTheory.Guitar` (voicings, tunings, diagram geometry),
`MusicTheory.Guitar.Blazor` (playback). `GuitarChords` is a demo app, not packed.

- **Spelled values.** `Note`, `Interval`, `Chord`, `Scale`, `KeySignature`, `ChordSymbol` are sealed records
  with value equality and constructor validation. Everything is built by interval from a root, never by
  semitone arithmetic, so spelling is exact.
- **`Interval`** carries quality (13 values, quintuply diminished to quintuply augmented), number and
  direction; names follow music21 (`M3`, `M-3`, `dd2`). `Between` follows the staff and never throws.
- **`Note.Transpose`** moves the letter by the interval number and throws `InvalidOperationException` for
  results needing more than two accidentals. Never clamp or respell silently.
- **One chord-type table**: `ChordTypes` in `Chord.cs` (intervals + ASCII suffix). `ChordFinder` patterns are
  derived from it. Spoken names live in `MusicTheory.Guitar/VoicingDescriber.cs`.
- Every `Parse` throws `FormatException`; every `TryParse` never throws.
- `MusicTheoryUtilities` and `MusicTheoryConstants` are internal.

## Tests

- `MusicTheory.UnitTests`: xUnit v3 + Shouldly. `ReferenceTests` replays the music21 tables in
  `Reference/*.csv`. `ContractTests` pins error messages (they are part of the API).
- `MusicTheory.Guitar.UnitTests`: `GoldenTests` compare the provider and diagram output with
  `Golden/*.txt`. `UPDATE_GOLDEN=1 dotnet test ...` accepts an intended change; review the diff.
- `MusicTheory.DocTests`: runs every `csharp` block of `docs/*.md` in page order (blocks share state like
  notebook cells). When every `Console.WriteLine` of a block has a trailing `// comment`, the comments are
  the expected output. Write expected output that way in docs.
- Coverage must stay at 100%. Prefer deleting an unreachable branch to testing around it.
- Stryker cannot activate mutants inside `static readonly` initialisers; keep tables that need testing in
  methods. Equivalent mutants are marked `// Stryker disable once <Mutator>: <why>`.

## Site

`site/` holds the hand-authored home page (scroll-craft engine, vendored, never edited: `scrollcraft.js`,
`scrollcraft.css`), tokens, fonts (OFL) and the docs template `_page.html`. `site/_brief.md` is the
scroll-craft brief. Home-page samples are `<figure class="sample" data-eval>` blocks whose output SiteGen
computes; never type an output by hand. Visible copy uses no em dashes.

## Roadmap

Feature families are tracked as GitHub issues #117 to #156 (parents: #118, #121, #125, #128, #133, #138,
#142, #147, #152; solfège is #117).
