# Changelog

Entries are written by release-please from the Conventional Commits merged into `dev`.

## [1.7.0](https://github.com/phmatray/MusicTheory/compare/v1.6.0...v1.7.0) (2026-10-17)


### Features

* **guitar:** chord playback via a dependency-free JS module ([#97](https://github.com/phmatray/MusicTheory/issues/97)) ([b25ad28](https://github.com/phmatray/MusicTheory/commit/b25ad28a79416fbfc93e051a7c10f075472e5a34))


### Refactor

* **guitarchords:** replace MudBlazor with DaisyBlazor ([#95](https://github.com/phmatray/MusicTheory/issues/95)) ([efff0ae](https://github.com/phmatray/MusicTheory/commit/efff0ae279bce69d15a1053df1fc5f26582cf1df))

## [1.6.0](https://github.com/phmatray/MusicTheory/releases/tag/v1.6.0) (2026-10-17)

### Features

* **chords:** `ChordSymbol.Parse` for lead-sheet chord symbols, with a canonical round-trip ([#92](https://github.com/phmatray/MusicTheory/pull/92))
* **transpose:** key-aware chord transposition with correct enharmonic spelling ([#86](https://github.com/phmatray/MusicTheory/pull/86))
* **analysis:** infer the key of a chord sequence ([#87](https://github.com/phmatray/MusicTheory/pull/87)) and Roman-numeral analysis ([#88](https://github.com/phmatray/MusicTheory/pull/88))
* **guitar:** new `MusicTheory.Guitar` package: voicings for every chord type ([#89](https://github.com/phmatray/MusicTheory/pull/89)), diagram geometry, accessible labels, piano data ([#90](https://github.com/phmatray/MusicTheory/pull/90)), `ToMidi` ([#91](https://github.com/phmatray/MusicTheory/pull/91))
* **chords:** `ChordFinder` identifies chords from note sets
