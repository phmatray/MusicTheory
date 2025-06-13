# Understanding Intervals

An `Interval` represents the distance between two musical pitches. Understanding intervals is fundamental to music theory as they form the basis of scales, chords, and melodies.

## What is an Interval?

An interval consists of two components:

<deflist>
    <def title="Interval Number">
        The numeric distance between notes (1 = unison, 2 = second, 3 = third, etc.)
    </def>
    <def title="Interval Quality">
        The specific type of interval (Perfect, Major, Minor, Augmented, Diminished)
    </def>
</deflist>

## Interval Qualities

Different intervals have different available qualities:

<tabs>
    <tab title="Perfect Intervals">
        <p>Unison (1), Fourth (4), Fifth (5), and Octave (8) can be:</p>
        - **Diminished**: One semitone smaller than perfect
        - **Perfect**: The standard interval
        - **Augmented**: One semitone larger than perfect
        <code-block lang="C#"><![CDATA[
// Perfect intervals
var perfectUnison = new Interval(IntervalQuality.Perfect, 1);
var perfectFourth = new Interval(IntervalQuality.Perfect, 4);
var perfectFifth = new Interval(IntervalQuality.Perfect, 5);
var perfectOctave = new Interval(IntervalQuality.Perfect, 8);

// Altered perfect intervals
var augmentedFourth = new Interval(IntervalQuality.Augmented, 4); // Tritone
var diminishedFifth = new Interval(IntervalQuality.Diminished, 5); // Tritone
]]></code-block>
    </tab>
    <tab title="Major/Minor Intervals">
        <p>Second (2), Third (3), Sixth (6), and Seventh (7) can be:</p>
        - **Diminished**: One semitone smaller than minor
        - **Minor**: One semitone smaller than major
        - **Major**: The standard interval
        - **Augmented**: One semitone larger than major
        <code-block lang="C#"><![CDATA[
// Major intervals
var majorSecond = new Interval(IntervalQuality.Major, 2);
var majorThird = new Interval(IntervalQuality.Major, 3);
var majorSixth = new Interval(IntervalQuality.Major, 6);
var majorSeventh = new Interval(IntervalQuality.Major, 7);

// Minor intervals
var minorSecond = new Interval(IntervalQuality.Minor, 2);
var minorThird = new Interval(IntervalQuality.Minor, 3);
var minorSixth = new Interval(IntervalQuality.Minor, 6);
var minorSeventh = new Interval(IntervalQuality.Minor, 7);
]]></code-block>
    </tab>
</tabs>

## Creating Intervals

### Direct Construction

Create intervals by specifying quality and number:

```C#
// Common intervals
var majorThird = new Interval(IntervalQuality.Major, 3);
var perfectFifth = new Interval(IntervalQuality.Perfect, 5);
var minorSeventh = new Interval(IntervalQuality.Minor, 7);

// Extended intervals
var majorNinth = new Interval(IntervalQuality.Major, 9);
var perfect11th = new Interval(IntervalQuality.Perfect, 11);
var major13th = new Interval(IntervalQuality.Major, 13);
```

### Calculating Between Notes

Calculate the interval between two notes:

```C#
// Create notes
var c4 = new Note(NoteName.C, Alteration.Natural, 4);
var e4 = new Note(NoteName.E, Alteration.Natural, 4);
var g4 = new Note(NoteName.G, Alteration.Natural, 4);

// Calculate intervals
var third = Interval.Between(c4, e4);  // Major Third
var fifth = Interval.Between(c4, g4);  // Perfect Fifth

// Works across octaves
var c5 = new Note(NoteName.C, Alteration.Natural, 5);
var octave = Interval.Between(c4, c5); // Perfect Octave

// Handles altered notes
var eb4 = new Note(NoteName.E, Alteration.Flat, 4);
var minorThird = Interval.Between(c4, eb4); // Minor Third
```

## Interval Properties

### Basic Properties

```C#
var interval = new Interval(IntervalQuality.Major, 6);

// Access properties
IntervalQuality quality = interval.Quality; // Major
int number = interval.Number;               // 6
int semitones = interval.Semitones;        // 9

// Check interval type
bool isPerfectInterval = (number == 1 || number == 4 || 
                         number == 5 || number == 8);
```

### Semitone Calculation

The library automatically calculates semitones based on quality and number:

<table>
<tr>
    <th>Interval</th>
    <th>Semitones</th>
    <th>Example (from C)</th>
</tr>
<tr>
    <td>Perfect Unison</td>
    <td>0</td>
    <td>C → C</td>
</tr>
<tr>
    <td>Minor Second</td>
    <td>1</td>
    <td>C → Db</td>
</tr>
<tr>
    <td>Major Second</td>
    <td>2</td>
    <td>C → D</td>
</tr>
<tr>
    <td>Minor Third</td>
    <td>3</td>
    <td>C → Eb</td>
</tr>
<tr>
    <td>Major Third</td>
    <td>4</td>
    <td>C → E</td>
</tr>
<tr>
    <td>Perfect Fourth</td>
    <td>5</td>
    <td>C → F</td>
</tr>
<tr>
    <td>Tritone</td>
    <td>6</td>
    <td>C → F#/Gb</td>
</tr>
<tr>
    <td>Perfect Fifth</td>
    <td>7</td>
    <td>C → G</td>
</tr>
<tr>
    <td>Minor Sixth</td>
    <td>8</td>
    <td>C → Ab</td>
</tr>
<tr>
    <td>Major Sixth</td>
    <td>9</td>
    <td>C → A</td>
</tr>
<tr>
    <td>Minor Seventh</td>
    <td>10</td>
    <td>C → Bb</td>
</tr>
<tr>
    <td>Major Seventh</td>
    <td>11</td>
    <td>C → B</td>
</tr>
<tr>
    <td>Perfect Octave</td>
    <td>12</td>
    <td>C → C</td>
</tr>
</table>

## Using Intervals

### Transposing Notes

Use intervals to transpose notes up or down:

```C#
var c4 = new Note(NoteName.C, Alteration.Natural, 4);

// Transpose up
var e4 = c4.Transpose(new Interval(IntervalQuality.Major, 3), Direction.Up);
var g4 = c4.Transpose(new Interval(IntervalQuality.Perfect, 5)); // Up is default

// Transpose down
var a3 = c4.Transpose(new Interval(IntervalQuality.Major, 3), Direction.Down);
var f3 = c4.Transpose(new Interval(IntervalQuality.Perfect, 5), Direction.Down);

// Chain transpositions
var d5 = c4
    .Transpose(new Interval(IntervalQuality.Perfect, 5), Direction.Up)  // → G4
    .Transpose(new Interval(IntervalQuality.Perfect, 5), Direction.Up); // → D5
```

### Building Scales

Intervals define scale patterns:

```C#
// Major scale intervals from root
var majorScaleIntervals = new[]
{
    new Interval(IntervalQuality.Perfect, 1),  // Root
    new Interval(IntervalQuality.Major, 2),    // Major 2nd
    new Interval(IntervalQuality.Major, 3),    // Major 3rd
    new Interval(IntervalQuality.Perfect, 4),  // Perfect 4th
    new Interval(IntervalQuality.Perfect, 5),  // Perfect 5th
    new Interval(IntervalQuality.Major, 6),    // Major 6th
    new Interval(IntervalQuality.Major, 7),    // Major 7th
    new Interval(IntervalQuality.Perfect, 8)   // Octave
};

// Build C major scale
var c = new Note(NoteName.C, Alteration.Natural, 4);
var cMajorScale = majorScaleIntervals
    .Select(interval => c.Transpose(interval))
    .ToList();
```

### Building Chords

Intervals define chord structures:

```C#
// Major triad: Root, Major 3rd, Perfect 5th
var root = new Note(NoteName.C, Alteration.Natural, 4);
var third = root.Transpose(new Interval(IntervalQuality.Major, 3));
var fifth = root.Transpose(new Interval(IntervalQuality.Perfect, 5));

// Minor 7th chord: Root, Minor 3rd, Perfect 5th, Minor 7th
var minorThird = root.Transpose(new Interval(IntervalQuality.Minor, 3));
var minorSeventh = root.Transpose(new Interval(IntervalQuality.Minor, 7));
```

## Interval Inversions

Invert intervals using the `Invert()` method:

```C#
// Basic inversions
var majorThird = new Interval(IntervalQuality.Major, 3);
var minorSixth = majorThird.Invert(); // Minor 6th

var perfectFifth = new Interval(IntervalQuality.Perfect, 5);
var perfectFourth = perfectFifth.Invert(); // Perfect 4th

var minorSecond = new Interval(IntervalQuality.Minor, 2);
var majorSeventh = minorSecond.Invert(); // Major 7th

// Quality inversion rules:
// - Perfect remains Perfect
// - Major becomes Minor
// - Minor becomes Major
// - Augmented becomes Diminished
// - Diminished becomes Augmented

// Number rule: original + inverted = 9
var unison = new Interval(IntervalQuality.Perfect, 1);
var octave = unison.Invert(); // Perfect 8th (1 + 8 = 9)

// Compound intervals are reduced before inversion
var majorNinth = new Interval(IntervalQuality.Major, 9);
var minorSeventh = majorNinth.Invert(); // Minor 7th (not 0!)

// Tritone inverts to tritone
var augFourth = new Interval(IntervalQuality.Augmented, 4);
var dimFifth = augFourth.Invert(); // Diminished 5th
```

## Enharmonic Intervals

Different spellings of the same sonic interval:

```C#
// Augmented 4th and Diminished 5th (both 6 semitones)
var augFourth = new Interval(IntervalQuality.Augmented, 4);
var dimFifth = new Interval(IntervalQuality.Diminished, 5);

bool sameSound = augFourth.Semitones == dimFifth.Semitones; // true
bool areEnharmonic = augFourth.IsEnharmonicWith(dimFifth);   // true

// Example from C
var c = new Note(NoteName.C, Alteration.Natural, 4);
var fSharp = c.Transpose(augFourth);  // F#4
var gFlat = c.Transpose(dimFifth);    // Gb4
// Same pitch, different spelling
```

## Common Interval Patterns

### Consonant and Dissonant Intervals

```C#
// Consonant intervals
var consonantIntervals = new[]
{
    new Interval(IntervalQuality.Perfect, 1),   // Unison
    new Interval(IntervalQuality.Major, 3),     // Major 3rd
    new Interval(IntervalQuality.Minor, 3),     // Minor 3rd
    new Interval(IntervalQuality.Perfect, 4),   // Perfect 4th
    new Interval(IntervalQuality.Perfect, 5),   // Perfect 5th
    new Interval(IntervalQuality.Major, 6),     // Major 6th
    new Interval(IntervalQuality.Minor, 6),     // Minor 6th
    new Interval(IntervalQuality.Perfect, 8)    // Octave
};

// Dissonant intervals
var dissonantIntervals = new[]
{
    new Interval(IntervalQuality.Minor, 2),     // Minor 2nd
    new Interval(IntervalQuality.Major, 2),     // Major 2nd
    new Interval(IntervalQuality.Augmented, 4), // Tritone
    new Interval(IntervalQuality.Diminished, 5),// Tritone
    new Interval(IntervalQuality.Major, 7),     // Major 7th
    new Interval(IntervalQuality.Minor, 7)      // Minor 7th
};
```

### Circle of Fifths Navigation

```C#
var c = new Note(NoteName.C, Alteration.Natural, 4);
var fifth = new Interval(IntervalQuality.Perfect, 5);

// Move clockwise (adding sharps)
var g = c.Transpose(fifth);                    // G
var d = g.Transpose(fifth);                    // D
var a = d.Transpose(fifth);                    // A

// Move counter-clockwise (adding flats)
var f = c.Transpose(fifth, Direction.Down);    // F
var bb = f.Transpose(fifth, Direction.Down);   // Bb
var eb = bb.Transpose(fifth, Direction.Down);  // Eb
```

## Advanced Topics

### Compound Intervals

Intervals larger than an octave:

```C#
// Compound intervals (> octave)
var ninth = new Interval(IntervalQuality.Major, 9);      // Octave + 2nd
var eleventh = new Interval(IntervalQuality.Perfect, 11); // Octave + 4th
var thirteenth = new Interval(IntervalQuality.Major, 13); // Octave + 6th

// Reduce to simple interval
int simpleIntervalNumber = ((ninth.Number - 1) % 7) + 1; // 2
```

### Interval Addition

Combining intervals:

```C#
// Adding intervals (conceptually)
var majorThird = new Interval(IntervalQuality.Major, 3);    // 4 semitones
var minorThird = new Interval(IntervalQuality.Minor, 3);    // 3 semitones
// Together they make a perfect fifth (7 semitones)

var c = new Note(NoteName.C, Alteration.Natural, 4);
var e = c.Transpose(majorThird);      // E
var g = e.Transpose(minorThird);      // G
// C to G is a perfect fifth
```

## Best Practices

- **Use appropriate quality**: Perfect for 1, 4, 5, 8; Major/Minor for 2, 3, 6, 7
- **Consider enharmonic context**: Aug 4th vs Dim 5th based on musical context
- **Validate interval creation**: Some quality/number combinations are invalid
- **Think musically**: Choose intervals that make sense in the harmonic context

## See Also

<seealso>
    <category ref="related">
        <a href="notes.md">Working with Notes</a>
        <a href="scales.md">Intervals in Scales</a>
        <a href="chords.md">Intervals in Chords</a>
        <a href="transposition.md">Transposition Guide</a>
    </category>
    <category ref="api">
        <a href="api-overview.md">Interval API Reference</a>
        <a href="enums.md">IntervalQuality Enum</a>
    </category>
</seealso>