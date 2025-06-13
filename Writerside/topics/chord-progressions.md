# Building Chord Progressions

The `ChordProgression` class helps you work with sequences of chords in the context of a key, including Roman numeral analysis and common progression patterns.

## Understanding Chord Progressions

A chord progression is:

<deflist>
    <def title="Sequence of Chords">
        A series of chords played in succession that creates harmonic movement
    </def>
    <def title="Key Context">
        Progressions exist within a key, giving each chord a function
    </def>
    <def title="Roman Numerals">
        Chords are numbered based on their scale degree (I, ii, iii, IV, V, vi, vii°)
    </def>
    <def title="Chord Functions">
        Tonic (I), Subdominant (IV), Dominant (V) create tension and resolution
    </def>
</deflist>

## Creating Chord Progressions

### Basic Setup

```C#
// Define the key
var cMajor = new KeySignature(new Note(NoteName.C), KeyMode.Major);
var aMinor = new KeySignature(new Note(NoteName.A), KeyMode.Minor);

// Create progression objects
var majorProgression = new ChordProgression(cMajor);
var minorProgression = new ChordProgression(aMinor);
```

### Diatonic Chords

Get all chords that naturally occur in a key:

<tabs>
    <tab title="Major Keys">
        <code-block lang="C#"><![CDATA[
var cMajorKey = new KeySignature(new Note(NoteName.C), KeyMode.Major);
var progression = new ChordProgression(cMajorKey);

// Get diatonic triads
var chords = progression.GetDiatonicChords().ToList();

// In C major:
// I    - C major  (C-E-G)
// ii   - D minor  (D-F-A)
// iii  - E minor  (E-G-B)
// IV   - F major  (F-A-C)
// V    - G major  (G-B-D)
// vi   - A minor  (A-C-E)
// vii° - B diminished (B-D-F)

// Uppercase = major, lowercase = minor, degree symbol = diminished
]]></code-block>
    </tab>
    <tab title="Minor Keys">
        <code-block lang="C#"><![CDATA[
var aMinorKey = new KeySignature(new Note(NoteName.A), KeyMode.Minor);
var progression = new ChordProgression(aMinorKey);

// Get diatonic triads
var chords = progression.GetDiatonicChords().ToList();

// In A natural minor:
// i    - A minor  (A-C-E)
// ii°  - B diminished (B-D-F)
// III  - C major  (C-E-G)
// iv   - D minor  (D-F-A)
// v    - E minor  (E-G-B)
// VI   - F major  (F-A-C)
// VII  - G major  (G-B-D)

// Note: Harmonic minor would have V (E major) instead of v
]]></code-block>
    </tab>
</tabs>

## Roman Numeral Analysis

### Getting Roman Numerals

```C#
var gMajor = new KeySignature(new Note(NoteName.G), KeyMode.Major);
var progression = new ChordProgression(gMajor);

// Get Roman numeral for each degree
for (int degree = 1; degree <= 7; degree++)
{
    string roman = progression.GetRomanNumeral(degree);
    var chord = progression.GetChordByDegree(degree);
    Console.WriteLine($"{roman}: {chord.GetSymbol()}");
}

// Output:
// I: G
// ii: Am
// iii: Bm
// IV: C
// V: D
// vi: Em
// vii°: F#°
```

### Parsing Roman Numerals

```C#
var progression = new ChordProgression(
    new KeySignature(new Note(NoteName.C), KeyMode.Major)
);

// Parse a progression string
var chords = progression.ParseProgression("I - vi - IV - V").ToList();

// Results in: C major, A minor, F major, G major

// With seventh chords
var jazzProgression = progression.ParseProgression("IMaj7 - vi7 - ii7 - V7");
// CMaj7, Am7, Dm7, G7

// Parse individual symbols
var twoChord = progression.ParseChordSymbol("ii");    // D minor
var five7 = progression.ParseChordSymbol("V7");       // G7
```

## Common Progressions

### Popular Music Progressions

```C#
var key = new KeySignature(new Note(NoteName.C), KeyMode.Major);
var prog = new ChordProgression(key);

// I-V-vi-IV (Pop progression)
var popProg = prog.ParseProgression("I - V - vi - IV");
// C - G - Am - F

// I-vi-IV-V (50s progression)
var doowop = prog.ParseProgression("I - vi - IV - V");
// C - Am - F - G

// vi-IV-I-V (Alternative pop)
var altPop = prog.ParseProgression("vi - IV - I - V");
// Am - F - C - G

// I-IV-I-V (12-bar blues foundation)
var blues = prog.ParseProgression("I - I - I - I - IV - IV - I - I - V - IV - I - V");
```

### Jazz Progressions

```C#
// ii-V-I (Most important jazz progression)
var twoFiveOne = prog.ParseProgression("ii7 - V7 - IMaj7");
// Dm7 - G7 - CMaj7

// I-vi-ii-V (Rhythm changes A section)
var rhythmChanges = prog.ParseProgression("IMaj7 - vi7 - ii7 - V7");
// CMaj7 - Am7 - Dm7 - G7

// iii-vi-ii-V (Extended turnaround)
var turnaround = prog.ParseProgression("iii7 - vi7 - ii7 - V7");
// Em7 - Am7 - Dm7 - G7

// Modal interchange (borrowing from parallel minor)
// Would need custom implementation for bVII, bIII, etc.
```

### Classical Progressions

```C#
// Authentic cadence (V-I)
var authentic = prog.ParseProgression("V - I");
// G - C

// Plagal cadence (IV-I)
var plagal = prog.ParseProgression("IV - I");
// F - C

// Deceptive cadence (V-vi)
var deceptive = prog.ParseProgression("V - vi");
// G - Am

// Circle progression
var circle = prog.ParseProgression("I - IV - vii° - iii - vi - ii - V - I");
// C - F - B° - Em - Am - Dm - G - C
```

## Chord Functions

Understanding the role of each chord:

```C#
var cMajor = new KeySignature(new Note(NoteName.C), KeyMode.Major);
var prog = new ChordProgression(cMajor);

// Tonic function (stability)
var tonic = prog.GetChordByDegree(1);        // I (C)
var tonicSub = prog.GetChordByDegree(6);     // vi (Am)

// Subdominant function (departure)
var subdominant = prog.GetChordByDegree(4);  // IV (F)
var subdominantSub = prog.GetChordByDegree(2); // ii (Dm)

// Dominant function (tension)
var dominant = prog.GetChordByDegree(5);      // V (G)
var leadingTone = prog.GetChordByDegree(7);  // vii° (B°)

// Common function progressions:
// Tonic → Subdominant → Dominant → Tonic
// I → IV → V → I
```

## Secondary Dominants

Chords that temporarily tonicize other degrees:

```C#
var prog = new ChordProgression(
    new KeySignature(new Note(NoteName.C), KeyMode.Major)
);

// V/V (five of five) - D major in key of C
var secondaryDom = prog.GetSecondaryDominant(5);
Console.WriteLine(secondaryDom.GetSymbol()); // D

// Common secondary dominants:
// V/ii → ii (A7 → Dm)
// V/iii → iii (B7 → Em)
// V/IV → IV (C7 → F)
// V/V → V (D7 → G)
// V/vi → vi (E7 → Am)
```

## Voice Leading

Creating smooth progressions:

```C#
// Good voice leading example
var smoothProg = prog.ParseProgression("I - vi - ii - V");

// Each chord shares common tones with the next:
// C (C-E-G) → Am (A-C-E) - Common tones: C, E
// Am (A-C-E) → Dm (D-F-A) - Common tone: A
// Dm (D-F-A) → G (G-B-D) - Common tone: D

// Consider inversions for smoother voice leading
var c = new Chord(new Note(NoteName.C, 4), ChordType.Major);
var am = new Chord(new Note(NoteName.A, 3), ChordType.Minor)
    .WithInversion(ChordInversion.First); // C in bass
var dm = new Chord(new Note(NoteName.D, 4), ChordType.Minor);
var g = new Chord(new Note(NoteName.G, 3), ChordType.Major);
```

## Modulation

Changing keys within a progression:

```C#
// Pivot chord modulation
var cMajorProg = new ChordProgression(
    new KeySignature(new Note(NoteName.C), KeyMode.Major)
);
var gMajorProg = new ChordProgression(
    new KeySignature(new Note(NoteName.G), KeyMode.Major)
);

// Am is vi in C major and ii in G major (pivot chord)
// C: I - vi - ? 
// G: ? - ii - V - I

// Direct modulation (up a half step)
var cSharpMajorProg = new ChordProgression(
    new KeySignature(new Note(NoteName.C, Alteration.Sharp), KeyMode.Major)
);
```

## Analyzing Existing Music

```C#
// Given a sequence of chords, analyze the progression
var chordSequence = new[]
{
    new Chord(new Note(NoteName.F), ChordType.Major),
    new Chord(new Note(NoteName.G), ChordType.Major),
    new Chord(new Note(NoteName.C), ChordType.Major)
};

// In C major: IV - V - I (plagal-authentic cadence)
// In F major: I - II - V (less common)
// Context determines the analysis
```

## Best Practices

- **Start simple**: Master I-IV-V-I before complex progressions
- **Consider voice leading**: Smooth movement between chords sounds better
- **Understand function**: Know why each chord works in the progression
- **Study genres**: Different styles favor different progressions
- **Experiment with inversions**: They can dramatically change the sound

## Common Progression Patterns

### By Genre

<tabs>
    <tab title="Pop/Rock">
        - I-V-vi-IV (Pop progression)
        - I-IV-V (Basic rock)
        - vi-IV-I-V (Alternative)
        - I-bVII-IV-I (Rock with modal interchange)
    </tab>
    <tab title="Jazz">
        - ii-V-I (Fundamental)
        - I-vi-ii-V (Turnaround)
        - iii-vi-ii-V (Extended turnaround)
        - I-VI-ii-V (With secondary dominant)
    </tab>
    <tab title="Blues">
        - I7-I7-I7-I7 (Bars 1-4)
        - IV7-IV7-I7-I7 (Bars 5-8)
        - V7-IV7-I7-V7 (Bars 9-12)
        - Quick change: I7-IV7-I7-I7 (Bars 1-4)
    </tab>
    <tab title="Classical">
        - I-V-I (Authentic cadence)
        - I-IV-I (Plagal cadence)
        - I-IV-V-I (Standard progression)
        - I-vi-IV-ii-V-I (Circle of fifths)
    </tab>
</tabs>

## See Also

<seealso>
    <category ref="related">
        <a href="chords.md">Understanding Chords</a>
        <a href="key-signatures.md">Working with Keys</a>
        <a href="analyzing-progressions.md">Progression Analysis Tutorial</a>
    </category>
    <category ref="api">
        <a href="api-overview.md">ChordProgression API Reference</a>
    </category>
</seealso>