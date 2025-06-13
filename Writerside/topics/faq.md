# Frequently Asked Questions

## General Questions

### What is MusicTheory?

MusicTheory is a comprehensive .NET library that models music theory concepts as code. It provides immutable domain objects for notes, intervals, scales, chords, and more, making it easy to work with music theory programmatically.

### Who is this library for?

The library is designed for:
- **Developers** building music applications
- **Music educators** creating teaching tools
- **Musicians** who want to analyze music programmatically
- **Students** learning music theory through code
- **Researchers** working on music analysis projects

### What .NET versions are supported?

The library targets .NET 9.0 and is compatible with:
- .NET 9.0 and later
- Any platform that supports .NET (Windows, macOS, Linux)

## Technical Questions

### Why are all objects immutable?

Immutability provides several benefits:
- **Thread safety**: Objects can be safely shared between threads
- **Predictability**: Objects never change after creation
- **Performance**: The compiler can optimize immutable objects
- **Functional programming**: Supports functional programming patterns

```C#
var c = new Note(NoteName.C, Alteration.Natural, 4);
var d = c.TransposeBySemitones(2); // Creates new Note, doesn't modify c
```

### How does the library handle enharmonic equivalents?

The library recognizes enharmonic equivalents (notes that sound the same but are spelled differently):

```C#
var cSharp = new Note(NoteName.C, Alteration.Sharp, 4);
var dFlat = new Note(NoteName.D, Alteration.Flat, 4);

bool areEnharmonic = cSharp.IsEnharmonicWith(dFlat); // true
var equivalent = cSharp.GetEnharmonicEquivalent(); // Returns Db4
```

### How accurate is the frequency calculation?

The library uses the equal temperament tuning system with A4 = 440 Hz as the reference:

```C#
var a4 = new Note(NoteName.A, Alteration.Natural, 4);
Console.WriteLine(a4.Frequency); // 440.0 Hz

var c4 = new Note(NoteName.C, Alteration.Natural, 4);
Console.WriteLine(c4.Frequency); // 261.626 Hz
```

Frequencies are calculated using the formula: `f = 440 × 2^((n-69)/12)` where n is the MIDI note number.

## Usage Questions

### How do I create a chord progression?

Use the `ChordProgression` class with a key signature:

```C#
var key = new KeySignature(new Note(NoteName.C), KeyMode.Major);
var progression = new ChordProgression(key);

// Parse Roman numerals
var chords = progression.ParseProgression("I - vi - IV - V");

// Or get individual chords
var tonic = progression.GetChordByDegree(1);
var dominant = progression.GetChordByDegree(5);
```

### How do I transpose a melody?

Transpose individual notes or entire collections:

```C#
// Transpose a single note
var c = new Note(NoteName.C, Alteration.Natural, 4);
var g = c.Transpose(new Interval(IntervalQuality.Perfect, 5));

// Transpose a melody
var melody = new[] { /* notes */ };
var transposedMelody = melody
    .Select(note => note.Transpose(interval))
    .ToArray();
```

### How do I work with different scale types?

The library supports 15+ scale types:

```C#
// Traditional scales
var major = new Scale(new Note(NoteName.C), ScaleType.Major);
var minor = new Scale(new Note(NoteName.A), ScaleType.NaturalMinor);

// Modal scales
var dorian = new Scale(new Note(NoteName.D), ScaleType.Dorian);
var lydian = new Scale(new Note(NoteName.F), ScaleType.Lydian);

// Exotic scales
var blues = new Scale(new Note(NoteName.A), ScaleType.Blues);
var wholeTone = new Scale(new Note(NoteName.C), ScaleType.WholeTone);
```

### How do I check if a note is in a scale?

Use the `Contains` method:

```C#
var cMajor = new Scale(new Note(NoteName.C), ScaleType.Major);

bool hasF = cMajor.Contains(new Note(NoteName.F)); // true
bool hasFSharp = cMajor.Contains(new Note(NoteName.F, Alteration.Sharp)); // false

// Works across octaves
bool hasHighC = cMajor.Contains(new Note(NoteName.C, 5)); // true
```

## Common Issues

### Why does my chord sound wrong?

Check these common issues:

1. **Wrong octave**: Ensure notes are in the intended octave
2. **Enharmonic spelling**: Use the correct enharmonic (C# vs Db)
3. **Chord type**: Verify you're using the right ChordType enum value

```C#
// Correct
var cMajor = new Chord(new Note(NoteName.C, 4), ChordType.Major);

// Check the notes
foreach (var note in cMajor.GetNotes())
{
    Console.WriteLine($"{note} - {note.Frequency} Hz");
}
```

### How do I handle MIDI note ranges?

MIDI notes range from 0-127. The library validates this:

```C#
try
{
    var note = new Note(NoteName.C, Alteration.Natural, 10);
    int midi = note.MidiNumber; // Throws exception - too high
}
catch (InvalidOperationException)
{
    // Handle out-of-range note
}

// Safe MIDI conversion
var midiNote = Note.FromMidiNumber(60); // C4
```

### Why are there no audio playback features?

The MusicTheory library focuses on music theory concepts and calculations. For audio playback, integrate with audio libraries like:
- NAudio
- CSCore  
- Unity Audio (for games)
- Web Audio API (for web apps)

## Best Practices

### Should I use ChordQuality or ChordType?

Use `ChordType` for specific chord types:

```C#
// Preferred - specific chord type
var cMaj7 = new Chord(root, ChordType.Major7);
var dm7b5 = new Chord(root, ChordType.HalfDiminished7);

// Legacy - requires manual extensions
var cMaj7Legacy = new Chord(root, ChordQuality.Major)
    .AddExtension(7, IntervalQuality.Major);
```

### How should I handle user input?

Parse and validate user input carefully:

```C#
// Parse note strings
if (Note.TryParse(userInput, out var note))
{
    // Valid note
}
else
{
    // Invalid input
}

// Validate ranges
if (octave >= -1 && octave <= 10)
{
    var note = new Note(noteName, alteration, octave);
}
```

### What's the best way to learn the library?

1. Start with the [Getting Started](getting-started.md) guide
2. Explore the [Examples](examples.md) page
3. Understand core concepts: [Notes](notes.md), [Intervals](intervals.md), [Chords](chords.md), [Scales](scales.md)
4. Build a small project
5. Refer to the API documentation as needed

## Contributing

### How can I contribute?

1. Fork the repository on GitHub
2. Create a feature branch
3. Write tests first (TDD approach)
4. Implement your feature
5. Ensure all tests pass
6. Submit a pull request

### Where do I report bugs?

Report issues on the [GitHub Issues](https://github.com/phmatray/MusicTheory/issues) page.

### Is there a roadmap?

Check the README for planned features. Current priorities include:
- Advanced chord progressions
- Voice leading analysis
- Music notation support
- Audio synthesis integration

## See Also

<seealso>
    <category ref="getting-started">
        <a href="getting-started.md">Getting Started Guide</a>
        <a href="overview.md">Library Overview</a>
    </category>
    <category ref="support">
        <a href="https://github.com/phmatray/MusicTheory">GitHub Repository</a>
        <a href="https://github.com/phmatray/MusicTheory/issues">Issue Tracker</a>
    </category>
</seealso>