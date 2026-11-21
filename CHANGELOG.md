# Changelog

Entries are written by release-please from the Conventional Commits merged into `dev`.

## [2.1.0](https://github.com/phmatray/MusicTheory/compare/v2.0.0...v2.1.0) (2026-11-21)


### Features

* **analysis:** detect cadences in a chord sequence ([#130](https://github.com/phmatray/MusicTheory/issues/130)) ([#165](https://github.com/phmatray/MusicTheory/issues/165)) ([ba27fc5](https://github.com/phmatray/MusicTheory/commit/ba27fc52728ce952d2cd41ca557ce60cde7189e3))
* **training:** seeded interval drill with difficulty levels ([#166](https://github.com/phmatray/MusicTheory/issues/166)) ([855e06c](https://github.com/phmatray/MusicTheory/commit/855e06cf5c3805b946851cd31301fb02d4d819cb))


### Bug Fixes

* **site:** circle of fifths labels, staff placement and site links in the top bar ([#163](https://github.com/phmatray/MusicTheory/issues/163)) ([77267f9](https://github.com/phmatray/MusicTheory/commit/77267f95079acf1e4a42d40c1e16f88dc726a341))

## [2.0.0](https://github.com/phmatray/MusicTheory/compare/v1.7.0...v2.0.0) (2026-11-21)


### ⚠ BREAKING CHANGES

* MusicTheory 2.0 on .NET 11 ([#157](https://github.com/phmatray/MusicTheory/issues/157))

### Features

* MusicTheory 2.0 on .NET 11 ([#157](https://github.com/phmatray/MusicTheory/issues/157)) ([c4775fb](https://github.com/phmatray/MusicTheory/commit/c4775fb7c4699df1fb5152096d7cdf56207b9482))

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
