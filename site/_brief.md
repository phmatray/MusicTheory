# Scroll brief: MusicTheory home page (site/index.html)

Status: interviewed on 2026-11-21 through four choices. Those answers are marked *owner*. Everything
else is authored from the library itself and marked *authored*.

## The eight topics

1. **Vibe** (*owner*: "Editorial / score"): cream paper, ink, one accent, music-engraving type. Dense
   and exact, like a treatise. References: an engraved Henle urtext edition, a theory textbook plate.
2. **Journey** (*owner*: "From the note to harmony"): theory built the way it is learned. A note,
   then an interval, a scale, a chord, a key, an analysed progression. Each step is a real call to
   the API with the output the library actually returns.
3. **Energy** (*authored*): quiet at the title page, steady and readable through the chapters,
   a held breath before the keys, loudest when the circle turns, calm and practical to the close.
4. **Feeling and the one moment** (*owner*: "The circle of fifths"): see the curve and the peak below.
5. **What no other site does** (*authored*): the visitor turns the circle of fifths with their
   scroll, and the key signature engraves itself on the staff, sharp by sharp, flat by flat, while
   the scale underneath respells (E♯ in F♯ major, C♭ in G♭ major).
6. **Distance from premium-minimal** (*owner*: "Editorial / score"): the editorial family,
   information-forward, paper and ink.
7. **One world or scenes** (*authored*): chapters with hard cuts, as a printed book has.
8. **Assets** (*owner*: "No generated images"): staves, the circle and every figure are SVG and
   HTML drawn from data the library computes at build time.

## Grammar: chaptered editorial

A section is a chapter of a treatise.

- **Navigation:** a folio in the margin, chapter numeral and title, updating as chapters pass.
  The sequence is information here: it is the order in which theory is learned.
- **Hero:** a title page. Type on paper, no media above the fold.
- **Ending:** a colophon. The install command set as a line of running text.
- **Bans:** the full-bleed scrub hero, pinned crossfade type acts, drift between grounds,
  a magnetic CTA, spotlight, centred hero copy, invented numbers, em dashes.

Why the others lost: filmic one-shot hides the chapter order a learner needs. Live surface would
fake an IDE. Continuous world has no geography to travel. Typographic poster cannot carry code.
Gallery fits a catalogue, not an argument. Split stage needs two sides; theory has one.
Rhythmic cutlist is the wrong pulse for a reference.

## Feeling curve

| Chapter | Feeling | Cause on screen |
|---|---|---|
| Title page | Composure | A title set like a book, the promise, the install line |
| I. The note | Recognition | A4 is 440 Hz, and 415 when you ask for baroque pitch |
| II. The interval | Surprise | C to F𝄪 is a doubly augmented fourth, not a "tritone" |
| III. The scale | Trust | F♯ major written with E♯, built by interval, matched against music21 |
| IV. The chord | Appetite | C7♭9♯11 parsed, spelled and voiced |
| Silence | Held breath | One line on its own: every key is a place on one circle |
| V. The key | Wonder (peak) | The circle turns under the scroll and the signature engraves itself |
| VI. Harmony | Clarity | ii7 V7 Imaj7 written under the chords, by pitch class |
| Colophon | Resolve | How it is made, and the one command to install it |

## The peak

"I scrolled and the circle of fifths turned, and the sharps wrote themselves onto the staff one by
one, F♯ then C♯, until C♯ major had all seven, and then it went round to the flats."

It lives in chapter V, which holds the largest span on the page. The silent line before it is authored
silence, so the harness may report that screen as sparse.

**It's the site where** your scroll turns the circle of fifths and the key signature engraves itself.

## Signature move

The circle is an SVG ring of fifteen major keys (C♭ to C♯) and their relative minors. Scroll progress
through chapter V picks a key; the ring rotates to it, the staff in the margin draws that key's
accidentals at their engraving positions, and the scale line respells. The keys, signatures and
scales are data emitted by the library at build time, not hand-written.

## Score

| Beat | Device |
|---|---|
| Title page | `flow` + `in` |
| I to IV | `flow` + `in`, a `reveal` at each chapter boundary |
| Silence | `flow`, one line |
| V. The key | `pin`, span 5, driven by the page's own circle script |
| VI. Harmony | `flow` + `in` |
| Colophon | `flow` |

## Fallbacks

Without JavaScript or under reduced motion, chapter V is a table of the fifteen keys with their
signatures and scales, computed by the same build step. Nothing is hidden.
