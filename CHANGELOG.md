# Changelog

Entries are written by release-please from the Conventional Commits merged into `dev`.

## [3.0.0](https://github.com/phmatray/MusicTheory/compare/v2.0.0...v3.0.0) (2026-11-21)


### ⚠ BREAKING CHANGES

* **rhythm:** TimeSignature is a sealed record; GetMeasureDuration() (double) is replaced by MeasureLength (Rational).
* **rhythm:** Duration.Add, Divide, GetValueInWholeNotes and GetValueInMeasures are removed; use Duration.Value (Rational).

### Features

* **abc:** parse single-voice ABC tunes into measures ([#218](https://github.com/phmatray/MusicTheory/issues/218)) ([9cd6ab7](https://github.com/phmatray/MusicTheory/commit/9cd6ab7d49efdb21b3617253b83d646c8322ed00))
* **abc:** pin the ABC parser to music21 on folk-tune fixtures ([#182](https://github.com/phmatray/MusicTheory/issues/182)) ([#224](https://github.com/phmatray/MusicTheory/issues/224)) ([6bdb6d8](https://github.com/phmatray/MusicTheory/commit/6bdb6d8493aef1c6226ed891f8aa2caaabe7c90a))
* **abc:** write canonical ABC that round-trips ([#221](https://github.com/phmatray/MusicTheory/issues/221)) ([62cc4b5](https://github.com/phmatray/MusicTheory/commit/62cc4b509376485fcf0b11e8668f1a0c8bf38e9a))
* **analysis:** detect cadences in a chord sequence ([#130](https://github.com/phmatray/MusicTheory/issues/130)) ([#165](https://github.com/phmatray/MusicTheory/issues/165)) ([ba27fc5](https://github.com/phmatray/MusicTheory/commit/ba27fc52728ce952d2cd41ca557ce60cde7189e3))
* **analysis:** detect modulations and pivot chords in a chord sequence ([#210](https://github.com/phmatray/MusicTheory/issues/210)) ([716861f](https://github.com/phmatray/MusicTheory/commit/716861fb987a046a4137f04287204efe1bb2db45))
* **analysis:** recognise Neapolitan and augmented sixth chords ([#198](https://github.com/phmatray/MusicTheory/issues/198)) ([9c290bc](https://github.com/phmatray/MusicTheory/commit/9c290bc0e0dd9f84b66a1fe09d8d70cbe6ba38a1))
* **analysis:** Reharmonizer with tritone substitution and ii-V insertion ([#209](https://github.com/phmatray/MusicTheory/issues/209)) ([96d6999](https://github.com/phmatray/MusicTheory/commit/96d6999f13d7fd1c287d4f6ce734a2c2977999ca))
* **chordpro:** ChordPro package with a round-tripping parser ([#194](https://github.com/phmatray/MusicTheory/issues/194)) ([98ad55a](https://github.com/phmatray/MusicTheory/commit/98ad55a8a1c24eff14c382abf260af5f047a9e05))
* **chordpro:** transpose chord sheets with key-aware spelling ([#202](https://github.com/phmatray/MusicTheory/issues/202)) ([8f1f8a9](https://github.com/phmatray/MusicTheory/commit/8f1f8a9e74d53352a69846c7ea40fe97e713a4bf))
* **chords:** opt-in lenient parse for lower-case chord roots ([#103](https://github.com/phmatray/MusicTheory/issues/103)) ([#246](https://github.com/phmatray/MusicTheory/issues/246)) ([07db8d3](https://github.com/phmatray/MusicTheory/commit/07db8d3349c86276483f15f30611eb4f99c9524e))
* **chords:** parse 7sus2, m(maj9), add[#11](https://github.com/phmatray/MusicTheory/issues/11) and the mi alias ([#101](https://github.com/phmatray/MusicTheory/issues/101)) ([#228](https://github.com/phmatray/MusicTheory/issues/228)) ([7cbba2c](https://github.com/phmatray/MusicTheory/commit/7cbba2c49db77fade147a595edba5968713f19a2))
* **guitar:** curated and movable shapes on 7- and 8-string guitars ([#199](https://github.com/phmatray/MusicTheory/issues/199)) ([97c88a2](https://github.com/phmatray/MusicTheory/commit/97c88a2e2c7ea53edddc7d19759b8eeda0b431aa)), closes [#151](https://github.com/phmatray/MusicTheory/issues/151)
* **guitar:** fretboard note map for notes, scales and chords on any tuning ([#149](https://github.com/phmatray/MusicTheory/issues/149)) ([#206](https://github.com/phmatray/MusicTheory/issues/206)) ([eadfa79](https://github.com/phmatray/MusicTheory/commit/eadfa79ed8c6500c0ec5e2504805eda6a28094ec))
* **guitar:** refuse a tuning whose string count differs from the voicing ([#197](https://github.com/phmatray/MusicTheory/issues/197)) ([cbd3ab2](https://github.com/phmatray/MusicTheory/commit/cbd3ab2a4f6f91224c706e900ab4fd7617dcbf91))
* **guitar:** speak chord names and voicing labels in French ([#100](https://github.com/phmatray/MusicTheory/issues/100)) ([#259](https://github.com/phmatray/MusicTheory/issues/259)) ([5134521](https://github.com/phmatray/MusicTheory/commit/5134521d68e53c20d5d6a69fe7381b38f55db502))
* **guitar:** ukulele, bass, mandolin and 7-string tunings ([#188](https://github.com/phmatray/MusicTheory/issues/188)) ([512fc5e](https://github.com/phmatray/MusicTheory/commit/512fc5ebd437d0ac798052eba9f0a83e34f60837))
* **guitar:** voice chords on re-entrant tunings such as ukulele ([#191](https://github.com/phmatray/MusicTheory/issues/191)) ([2824753](https://github.com/phmatray/MusicTheory/commit/2824753d464198d48d87fa0a69996cbe681da93d))
* **harmony:** piano voicings, drop 2/3, shell and rootless ([#126](https://github.com/phmatray/MusicTheory/issues/126)) ([#185](https://github.com/phmatray/MusicTheory/issues/185)) ([4cdca11](https://github.com/phmatray/MusicTheory/commit/4cdca11ac42a228c52a8355a641099e190336bf3))
* **midi:** MidiReader and write/read round trip ([#223](https://github.com/phmatray/MusicTheory/issues/223)) ([e5ac96b](https://github.com/phmatray/MusicTheory/commit/e5ac96b472828b1dc15ffbfd7f56aebdb14943b9))
* **midi:** MusicTheory.Midi package skeleton and MidiWriter ([#174](https://github.com/phmatray/MusicTheory/issues/174)) ([#217](https://github.com/phmatray/MusicTheory/issues/217)) ([ff687ec](https://github.com/phmatray/MusicTheory/commit/ff687ec740fa8ec6dcfe6d836401748debd1536b))
* **midi:** pin the MIDI reader to music21-written files ([#225](https://github.com/phmatray/MusicTheory/issues/225)) ([18ba548](https://github.com/phmatray/MusicTheory/commit/18ba548c625e6269451cfde05eb5adc3ab5370ac))
* **musicxml:** MusicXML package skeleton and MusicXmlWriter ([#216](https://github.com/phmatray/MusicTheory/issues/216)) ([80bce84](https://github.com/phmatray/MusicTheory/commit/80bce847018f9e5b19a11ea4518a5ab7d266d707))
* **musicxml:** MusicXmlReader and write/read round trip ([#178](https://github.com/phmatray/MusicTheory/issues/178)) ([#220](https://github.com/phmatray/MusicTheory/issues/220)) ([64b0c19](https://github.com/phmatray/MusicTheory/commit/64b0c197c171d68c73a8c60ad294a63b0303ceab))
* **musicxml:** pin the MusicXML reader to music21-written files ([#222](https://github.com/phmatray/MusicTheory/issues/222)) ([445627c](https://github.com/phmatray/MusicTheory/commit/445627cf05411976cb13d510d5b1f9c837f5d320))
* **piano:** piano fingering for scales and triads ([#150](https://github.com/phmatray/MusicTheory/issues/150)) ([#207](https://github.com/phmatray/MusicTheory/issues/207)) ([515e2e2](https://github.com/phmatray/MusicTheory/commit/515e2e23f471d9f52829efbfdb5c89158cbe02cb))
* **rhythm:** Duration is an exact sealed record on Rational ([#193](https://github.com/phmatray/MusicTheory/issues/193)) ([92a67f9](https://github.com/phmatray/MusicTheory/commit/92a67f93cc11838ae8ced895a1b74b19ec476ef8)), closes [#169](https://github.com/phmatray/MusicTheory/issues/169)
* **rhythm:** exact Rational value type ([#189](https://github.com/phmatray/MusicTheory/issues/189)) ([c31d9d3](https://github.com/phmatray/MusicTheory/commit/c31d9d3c6f8ffd0c0e301f630b27a33580719115))
* **rhythm:** measures check capacity and tie notes across barlines ([#135](https://github.com/phmatray/MusicTheory/issues/135)) ([#214](https://github.com/phmatray/MusicTheory/issues/214)) ([115dbe2](https://github.com/phmatray/MusicTheory/commit/115dbe26404fd5f9d7052d634c5b1667dcf02d22))
* **rhythm:** pin Duration values to a music21 reference table ([#205](https://github.com/phmatray/MusicTheory/issues/205)) ([4260906](https://github.com/phmatray/MusicTheory/commit/42609065a3be4681ff4e53f70303fad2496e5741))
* **rhythm:** TempoMap and Swing for exact tempo-aware positions ([#213](https://github.com/phmatray/MusicTheory/issues/213)) ([6ce5c25](https://github.com/phmatray/MusicTheory/commit/6ce5c25430069f13b8b33836d16d309b060d57ce))
* **rhythm:** time signatures expose beat groups and measures beam per meter ([#137](https://github.com/phmatray/MusicTheory/issues/137)) ([#219](https://github.com/phmatray/MusicTheory/issues/219)) ([c5e31f2](https://github.com/phmatray/MusicTheory/commit/c5e31f209a0abf5a89617213adb607b6a3d17f21))
* **rhythm:** TimeSignature record with exact MeasureLength ([#204](https://github.com/phmatray/MusicTheory/issues/204)) ([412fd72](https://github.com/phmatray/MusicTheory/commit/412fd722683d36fb2f77a56f2e2e9a84155bf210)), closes [#170](https://github.com/phmatray/MusicTheory/issues/170)
* **scales:** add melodic/harmonic minor modes, bebop and world scales ([#167](https://github.com/phmatray/MusicTheory/issues/167)) ([5c7b2c6](https://github.com/phmatray/MusicTheory/commit/5c7b2c61deb17ca7057fb6e65e45eeca695e3af4))
* **scales:** add Scale.GetModes() ([#123](https://github.com/phmatray/MusicTheory/issues/123)) ([#192](https://github.com/phmatray/MusicTheory/issues/192)) ([184c282](https://github.com/phmatray/MusicTheory/commit/184c282e270c2e6966d23b208d5f36b4a2cd1f97))
* **scales:** match scales to chords ([#124](https://github.com/phmatray/MusicTheory/issues/124)) ([#200](https://github.com/phmatray/MusicTheory/issues/200)) ([dfabd28](https://github.com/phmatray/MusicTheory/commit/dfabd282d6b347a483a9341fa2097edd6867691f))
* **settheory:** add PitchClassSet with normal form, prime form and interval vector ([#139](https://github.com/phmatray/MusicTheory/issues/139)) ([#201](https://github.com/phmatray/MusicTheory/issues/201)) ([9978ec2](https://github.com/phmatray/MusicTheory/commit/9978ec2c50395fc0ed830f64afa057b6672b002e))
* **settheory:** neo-Riemannian P, L and R on spelled triads, pinned to music21 ([#208](https://github.com/phmatray/MusicTheory/issues/208)) ([b2447a5](https://github.com/phmatray/MusicTheory/commit/b2447a52d0787bc9b1fb1fba9121cbcd7089629b)), closes [#140](https://github.com/phmatray/MusicTheory/issues/140)
* **settheory:** PitchClassSet names its Forte set class and Z-partner ([#215](https://github.com/phmatray/MusicTheory/issues/215)) ([8e1e4d3](https://github.com/phmatray/MusicTheory/commit/8e1e4d309ff19dd4f7037eac4c99d4e973b5caea))
* **solfege:** fixed-do and movable-do note names ([#117](https://github.com/phmatray/MusicTheory/issues/117)) ([#211](https://github.com/phmatray/MusicTheory/issues/211)) ([12cb219](https://github.com/phmatray/MusicTheory/commit/12cb219b548c7a44edb771433387fdd780902d6c))
* **toolbox:** a workbook frame in the site's paper and ink ([#292](https://github.com/phmatray/MusicTheory/issues/292)) ([ae95b68](https://github.com/phmatray/MusicTheory/commit/ae95b688ab08224e7bccb05dddc744259e503cba))
* **toolbox:** Blazor WebAssembly shell published under /toolbox ([#245](https://github.com/phmatray/MusicTheory/issues/245)) ([#263](https://github.com/phmatray/MusicTheory/issues/263)) ([c9a6c69](https://github.com/phmatray/MusicTheory/commit/c9a6c69e08b0c16a7d5a74dbe2bbd013cd8f2d9e))
* **toolbox:** chord and scale finder ([#250](https://github.com/phmatray/MusicTheory/issues/250)) ([#284](https://github.com/phmatray/MusicTheory/issues/284)) ([26f6752](https://github.com/phmatray/MusicTheory/commit/26f6752cc3f6f36e4bf7e53f4088e91ae8d0b30c))
* **toolbox:** chord voicings and piano scale fingering ([#253](https://github.com/phmatray/MusicTheory/issues/253)) ([#288](https://github.com/phmatray/MusicTheory/issues/288)) ([da358a2](https://github.com/phmatray/MusicTheory/commit/da358a2da40f9afa6a31ab814a2a7ea223c66f42))
* **toolbox:** export the dictation melody as MIDI and MusicXML ([#257](https://github.com/phmatray/MusicTheory/issues/257)) ([#289](https://github.com/phmatray/MusicTheory/issues/289)) ([bf1799e](https://github.com/phmatray/MusicTheory/commit/bf1799ebb65221c21167e43b0ffbbc84177880ed))
* **toolbox:** fretboard map for notes, scales and chords ([#252](https://github.com/phmatray/MusicTheory/issues/252)) ([#282](https://github.com/phmatray/MusicTheory/issues/282)) ([ea9c913](https://github.com/phmatray/MusicTheory/commit/ea9c913d0fa0bab301a8a410e1271d0e578551ce))
* **toolbox:** progression analysis tool ([#256](https://github.com/phmatray/MusicTheory/issues/256)) ([#283](https://github.com/phmatray/MusicTheory/issues/283)) ([c4903e4](https://github.com/phmatray/MusicTheory/commit/c4903e447b6d9be4f71e34e2b8c476516602b2eb))
* **toolbox:** seeded chord, scale and progression drills ([#287](https://github.com/phmatray/MusicTheory/issues/287)) ([60d457e](https://github.com/phmatray/MusicTheory/commit/60d457e25e71dbeaca2a956473dc74bcc7903f16))
* **toolbox:** seeded interval drill with audio and shareable link ([#248](https://github.com/phmatray/MusicTheory/issues/248)) ([#281](https://github.com/phmatray/MusicTheory/issues/281)) ([6cc1457](https://github.com/phmatray/MusicTheory/commit/6cc14576481b06f11c796c8e00e7a2a5b2ccbdd4))
* **toolbox:** seeded melodic dictation with per-note scoring ([#255](https://github.com/phmatray/MusicTheory/issues/255)) ([#286](https://github.com/phmatray/MusicTheory/issues/286)) ([34a9ceb](https://github.com/phmatray/MusicTheory/commit/34a9ceb33f070c4e3d68076d15a1efb8b9195a46))
* **toolbox:** staff rendering through the ABC writer and a vendored abcjs ([#280](https://github.com/phmatray/MusicTheory/issues/280)) ([a9135a6](https://github.com/phmatray/MusicTheory/commit/a9135a6e7584505831c865772b47e04ef453a954))
* **toolbox:** teacher guide, privacy page and site links ([#258](https://github.com/phmatray/MusicTheory/issues/258)) ([#285](https://github.com/phmatray/MusicTheory/issues/285)) ([2f5d669](https://github.com/phmatray/MusicTheory/commit/2f5d669558e1686fe842c3f6dcf8567a61b1c672))
* **toolbox:** transposer for transposing instruments ([#251](https://github.com/phmatray/MusicTheory/issues/251)) ([#279](https://github.com/phmatray/MusicTheory/issues/279)) ([9de9277](https://github.com/phmatray/MusicTheory/commit/9de92772569015ee2a33fb38ea522fda61677229))
* **toolbox:** URL state, FR/EN i18n and circle of fifths tracer ([#247](https://github.com/phmatray/MusicTheory/issues/247)) ([#272](https://github.com/phmatray/MusicTheory/issues/272)) ([b416087](https://github.com/phmatray/MusicTheory/commit/b4160873c7e8c4407bad74697e4e4351dd88b186))
* **training:** seeded chord-quality and scale-identification drills ([#196](https://github.com/phmatray/MusicTheory/issues/196)) ([1736c95](https://github.com/phmatray/MusicTheory/commit/1736c95388ad4de12c8106690449c16ff2c7788b))
* **training:** seeded interval drill with difficulty levels ([#166](https://github.com/phmatray/MusicTheory/issues/166)) ([855e06c](https://github.com/phmatray/MusicTheory/commit/855e06cf5c3805b946851cd31301fb02d4d819cb))
* **training:** seeded melodic dictation with per-note scoring ([#203](https://github.com/phmatray/MusicTheory/issues/203)) ([32b9272](https://github.com/phmatray/MusicTheory/commit/32b9272defbd8ae29f57a9819a0de67739bf3d79))
* **training:** seeded Roman-numeral progression drill ([#187](https://github.com/phmatray/MusicTheory/issues/187)) ([bed8f3e](https://github.com/phmatray/MusicTheory/commit/bed8f3e6d28e4fbe4187555b905bf7014b743a7c)), closes [#155](https://github.com/phmatray/MusicTheory/issues/155)
* **transpose:** SuffixHandling.KeepUnparsed moves root and bass of unparseable chords ([#102](https://github.com/phmatray/MusicTheory/issues/102)) ([#229](https://github.com/phmatray/MusicTheory/issues/229)) ([5b510b3](https://github.com/phmatray/MusicTheory/commit/5b510b379c91291736ca5641cd88eb3abf8a27d1))
* **transpose:** SuffixHandling.Verbatim keeps the writer's chord suffix ([#104](https://github.com/phmatray/MusicTheory/issues/104)) ([#260](https://github.com/phmatray/MusicTheory/issues/260)) ([6ce124d](https://github.com/phmatray/MusicTheory/commit/6ce124d99046f06a18799b2dba147b2cacabd5f6))
* **tuning:** add Pythagorean, just and quarter-comma meantone temperaments ([#120](https://github.com/phmatray/MusicTheory/issues/120)) ([#212](https://github.com/phmatray/MusicTheory/issues/212)) ([89cfc29](https://github.com/phmatray/MusicTheory/commit/89cfc2993e7e248ee5d82624c7da014735adb348))
* **tuning:** read the nearest note and cents from a frequency ([#119](https://github.com/phmatray/MusicTheory/issues/119)) ([#190](https://github.com/phmatray/MusicTheory/issues/190)) ([e67dfcc](https://github.com/phmatray/MusicTheory/commit/e67dfcc86253f7270071e4ffd5106929f2e3ef87))
* **voicing:** voice-lead a progression with the smoothest voicings ([#195](https://github.com/phmatray/MusicTheory/issues/195)) ([4fd6598](https://github.com/phmatray/MusicTheory/commit/4fd659827f002710070deeda28c4b9a4b8900138))


### Bug Fixes

* **abc:** value equality for tunes and measures ([#291](https://github.com/phmatray/MusicTheory/issues/291)) ([7233800](https://github.com/phmatray/MusicTheory/commit/72338001ff3556b7d3d97fbbd423841af92a74a4))
* **chordpro:** transposing updates the document key ([#290](https://github.com/phmatray/MusicTheory/issues/290)) ([5ea0b4a](https://github.com/phmatray/MusicTheory/commit/5ea0b4ad96ee834315be77dccf51f75df6ca0f45))
* **guitar:** rank voicings that drop a chord tone behind complete ones ([#226](https://github.com/phmatray/MusicTheory/issues/226)) ([bb431d4](https://github.com/phmatray/MusicTheory/commit/bb431d428f3b8166a37e33ab0b6d9ee9e6742891))
* **site:** circle of fifths labels, staff placement and site links in the top bar ([#163](https://github.com/phmatray/MusicTheory/issues/163)) ([77267f9](https://github.com/phmatray/MusicTheory/commit/77267f95079acf1e4a42d40c1e16f88dc726a341))
* **transpose:** interval-based chord spelling, theoretical keys parse ([#105](https://github.com/phmatray/MusicTheory/issues/105)) ([#227](https://github.com/phmatray/MusicTheory/issues/227)) ([2933a14](https://github.com/phmatray/MusicTheory/commit/2933a14718e74779416396f7f13705aea94de3fd))

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
