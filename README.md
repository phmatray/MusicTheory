# MusicTheory

[![NuGet](https://img.shields.io/nuget/v/MusicTheory.svg)](https://www.nuget.org/packages/MusicTheory/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/MusicTheory.svg)](https://www.nuget.org/packages/MusicTheory/)
[![.NET](https://img.shields.io/badge/.NET-9.0-blue)](https://dotnet.microsoft.com/)
[![Build](https://github.com/phmatray/MusicTheory/actions/workflows/dotnet.yml/badge.svg)](https://github.com/phmatray/MusicTheory/actions/workflows/dotnet.yml)
[![Tests](https://img.shields.io/badge/tests-504%20passing-brightgreen)](#testing)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

A comprehensive C# library for music theory concepts, providing immutable domain objects for notes, intervals, scales, chords, and more. Built with modern .NET practices and extensive test coverage.

> 📦 **Install:** `dotnet add package MusicTheory` | [NuGet Gallery](https://www.nuget.org/packages/MusicTheory/)

## ✨ Features

### 🎵 Core Music Theory
- **Notes**: Create and manipulate musical notes with support for all alterations
- **Intervals**: Calculate musical intervals with proper quality handling
- **Scales**: Generate scales with 15+ scale types including modal and exotic scales
- **Chords**: Build chords with 40+ types including triads, seventh chords, extended, altered, and suspended chords
- **Key Signatures**: Handle key signatures with circle of fifths navigation

### 🔄 Advanced Functionality
- **Transposition**: Transpose notes, chords, and scales by intervals
- **Chord Progressions**: Roman numeral analysis and common progressions
- **MIDI Integration**: Convert between notes and MIDI numbers
- **Enharmonic Equivalence**: Handle enharmonic relationships (C# ↔ Db)
- **Time & Rhythm**: Time signatures and note duration calculations

### 🏗️ Design Principles
- **Immutable Objects**: Thread-safe, predictable behavior
- **Fluent API**: Chainable method calls for readable code
- **Type Safety**: Strong typing prevents invalid music theory constructs
- **Performance**: Lazy evaluation and calculated properties

---

## 🎯 Why MusicTheory?

| Feature | MusicTheory | Other Libraries |
|---------|-------------|-----------------|
| **Immutability** | ✅ All objects immutable | ❌ Often mutable |
| **Type Safety** | ✅ Compile-time validation | ⚠️ Runtime errors |
| **Chord Types** | ✅ 40+ types (jazz, altered, exotic) | ⚠️ 10-20 basic types |
| **Scale Types** | ✅ 15+ (modal, pentatonic, exotic) | ⚠️ 5-10 types |
| **MIDI Support** | ✅ Bidirectional conversion | ⚠️ Limited or none |
| **Enharmonics** | ✅ Full support (C# ↔ Db) | ⚠️ Partial or none |
| **Progressions** | ✅ Roman numeral + common patterns | ❌ Not supported |
| **Test Coverage** | ✅ 504 tests (comprehensive) | ⚠️ Varies |
| **.NET Version** | ✅ .NET 9.0 (modern) | ⚠️ Often older frameworks |
| **Documentation** | ✅ XML docs + examples | ⚠️ Varies |

**Perfect for:**
- 🎼 Music composition tools and DAWs
- 🎓 Educational software and theory trainers
- 🎸 Guitar/piano chord generators
- 🎹 MIDI processors and synthesizers
- 📊 Music analysis applications

---

## 📦 Installation

### Via NuGet Package Manager

```bash
dotnet add package MusicTheory
```

### Via Package Manager Console

```powershell
Install-Package MusicTheory
```

### Via .csproj

```xml
<ItemGroup>
  <PackageReference Include="MusicTheory" Version="1.5.1" />
</ItemGroup>
```

## 🚀 Quick Start

### Basic Usage

```csharp
using MusicTheory;

// Create notes
var c4 = new Note(NoteName.C, Alteration.Natural, 4);
var cSharp = new Note(NoteName.C, Alteration.Sharp, 4);
var fSharp = Note.Parse("F#5");

// Calculate frequency (A4 = 440Hz)
Console.WriteLine(c4.Frequency); // 261.63 Hz

// Create intervals
var perfectFifth = new Interval(IntervalQuality.Perfect, 5);
var intervalBetween = Interval.Between(c4, new Note(NoteName.G, 4));

// Invert intervals
var majorThird = new Interval(IntervalQuality.Major, 3);
var minorSixth = majorThird.Invert(); // Minor 6th

// Transpose notes
var g4 = c4.Transpose(perfectFifth, Direction.Up);
var transposedDown = cSharp.TransposeBySemitones(-3);

// Build chords
var cMajor = new Chord(c4, ChordType.Major);
var cMaj7 = new Chord(c4, ChordType.Major7);
var cSus4 = new Chord(c4, ChordType.Sus4);
var c7b9 = new Chord(c4, ChordType.Dominant7Flat9);
var firstInversion = cMajor.WithInversion(ChordInversion.First);

// Get chord notes and symbols
var notes = cMaj7.GetNotes(); // C, E, G, B
var bassNote = firstInversion.GetBassNote(); // E
var symbol = cMaj7.GetSymbol(); // "Cmaj7"
```

### Scales and Modes

```csharp
// Create scales
var cMajor = new Scale(new Note(NoteName.C), ScaleType.Major);
var aMinor = new Scale(new Note(NoteName.A), ScaleType.NaturalMinor);
var dDorian = new Scale(new Note(NoteName.D), ScaleType.Dorian);

// Generate scale notes
var majorScaleNotes = cMajor.GetNotes(); // C, D, E, F, G, A, B, C
var pentatonicNotes = new Scale(c4, ScaleType.PentatonicMajor).GetNotes();

// Scale operations
var nextNote = cMajor.GetNextNoteInScale(new Note(NoteName.E)); // F
var degree = cMajor.GetDegree(new Note(NoteName.G)); // 5
var contains = cMajor.Contains(new Note(NoteName.F, Alteration.Sharp)); // false

// Transpose scales
var dMajor = cMajor.Transpose(new Interval(IntervalQuality.Major, 2));
```

### Key Signatures and Progressions

```csharp
// Key signatures
var cMajorKey = new KeySignature(new Note(NoteName.C), KeyMode.Major);
var fSharpMinorKey = new KeySignature(new Note(NoteName.F, Alteration.Sharp), KeyMode.Minor);

// Circle of fifths navigation
var nextKey = cMajorKey.NextInCircle(); // G major
var relative = cMajorKey.GetRelative(); // A minor
var parallel = cMajorKey.GetParallel(); // C minor

// Chord progressions
var progression = new ChordProgression(cMajorKey);
var diatonicChords = progression.GetDiatonicChords(); // I, ii, iii, IV, V, vi, vii°

// Parse Roman numeral progressions
var chords = progression.ParseProgression("I - vi - IV - V"); // C - Am - F - G
var romanNumeral = progression.GetRomanNumeral(5); // "V"
```

### Advanced Chord Examples

```csharp
// Jazz chords
var maj7 = new Chord(c4, ChordType.Major7);
var min9 = new Chord(new Note(NoteName.D), ChordType.Minor9);
var dom7b9 = new Chord(new Note(NoteName.G), ChordType.Dominant7Flat9);
var halfDim = new Chord(new Note(NoteName.B), ChordType.HalfDiminished7);

// Suspended and altered chords
var sus2 = new Chord(c4, ChordType.Sus2);
var alt = new Chord(new Note(NoteName.G), ChordType.Dominant7Alt);

// Get chord symbols
Console.WriteLine(dom7b9.GetSymbol()); // "G7♭9"
Console.WriteLine(halfDim.GetSymbol()); // "Bø7"
```

### Time Signatures and Durations

```csharp
// Time signatures
var fourFour = new TimeSignature(4, 4);
var sixEight = new TimeSignature(6, 8);
var commonTime = TimeSignature.CommonTime; // 4/4

// Duration calculations
var quarter = new Duration(DurationType.Quarter);
var dottedHalf = new Duration(DurationType.Half, 1); // dotted
var triplet = Duration.CreateTuplet(DurationType.Eighth, 3, 2);

// Time calculations
var timeInSeconds = quarter.GetTimeInSeconds(120); // 0.5s at 120 BPM
var measures = quarter.GetValueInMeasures(fourFour); // 0.25 measures
var symbol = quarter.GetSymbol(); // "♩"
```

### MIDI Integration

```csharp
// MIDI conversion
var middleC = Note.FromMidiNumber(60); // C4
var midiNumber = new Note(NoteName.A, 4).MidiNumber; // 69

// Prefer flats for black keys
var dFlat = Note.FromMidiNumber(61, preferFlats: true); // Db4 instead of C#4
```

## 📚 Core Classes

### Note
Represents a musical note with name, alteration, and octave.

```csharp
public class Note
{
    public NoteName Name { get; }           // C, D, E, F, G, A, B
    public Alteration Alteration { get; }   // DoubleFlat to DoubleSharp
    public int Octave { get; }              // Scientific pitch notation
    public double Frequency { get; }        // Calculated frequency in Hz
}
```

### Interval
Represents the distance between two notes.

```csharp
public class Interval
{
    public IntervalQuality Quality { get; }  // Perfect, Major, Minor, etc.
    public int Number { get; }               // 1-8 (unison to octave)
    public int Semitones { get; }            // Total semitones
    
    public Interval Invert();                // Returns the inverted interval
}
```

### Chord
Represents a chord with root, quality, extensions, and inversions.

```csharp
public class Chord
{
    public Note Root { get; }
    public ChordType Type { get; }           // 40+ types: Major7, Dom7b9, Sus4, etc.
    public ChordInversion Inversion { get; } // Root, First, Second, Third
    
    public string GetSymbol();               // Returns chord symbol (e.g., "Cmaj7")
    public Chord AddExtension(int number, IntervalQuality quality);
    public Chord WithInversion(ChordInversion inversion);
}
```

### Scale
Generates notes following interval patterns.

```csharp
public class Scale
{
    public Note Root { get; }
    public ScaleType Type { get; }           // Major, Minor, Modal, Exotic
    
    public IEnumerable<Note> GetNotes();
    public Note GetNextNoteInScale(Note note);
    public bool Contains(Note note);
}
```

## 🎼 Supported Scale Types

| Category | Scale Types |
|----------|-------------|
| **Traditional** | Major, Natural Minor, Harmonic Minor, Melodic Minor |
| **Modal** | Ionian, Dorian, Phrygian, Lydian, Mixolydian, Aeolian, Locrian |
| **Pentatonic** | Pentatonic Major, Pentatonic Minor |
| **Exotic** | Blues, Chromatic, Whole Tone |

## 🎸 Supported Chord Types

| Category | Chord Types |
|----------|-------------|
| **Triads** | Major, Minor, Diminished, Augmented |
| **Seventh** | Major7, Minor7, Dominant7, MinorMajor7, HalfDiminished7, Diminished7, Augmented7, AugmentedMajor7 |
| **Extended** | Major9, Minor9, Dominant9, Major11, Minor11, Dominant11, Major13, Minor13, Dominant13 |
| **Altered** | Dom7b5, Dom7#5, Dom7b9, Dom7#9, Dom7b5b9, Dom7b5#9, Dom7#5b9, Dom7#5#9, Dom7Alt |
| **Suspended** | Sus2, Sus4, Sus2Sus4, Dom7Sus4 |
| **Sixth** | Major6, Minor6, Major6Add9 |
| **Add** | MajorAdd9, MinorAdd9, MajorAdd11, MinorAdd11 |
| **Power** | Power5 |

## 🧪 Testing

The library includes comprehensive test coverage with **504 passing tests** using xUnit and Shouldly.

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run specific test class
dotnet test --filter "ClassName=NoteTests"

# Generate code coverage
dotnet run --project MusicTheory.UnitTests -- --coverage
```

### Test Categories
- **Unit Tests**: Core functionality for each class
- **Integration Tests**: Cross-class interactions
- **Edge Cases**: Boundary conditions and error handling
- **Performance Tests**: Ensuring efficient calculations

## 🏛️ Architecture

### Design Patterns
- **Immutable Value Objects**: All domain objects are immutable
- **Factory Methods**: `Interval.Between()`, `Note.Parse()`, etc.
- **Fluent Interface**: Chainable method calls
- **Strategy Pattern**: Different scale generation strategies
- **Calculated Properties**: Lazy evaluation for performance

### Domain Model

```
┌─────────────────────────────────────────────────────────────┐
│                     MusicTheory Library                      │
└─────────────────────────────────────────────────────────────┘

┌──────────┐      ┌──────────┐      ┌──────────┐
│   Note   │──┬──▶│ Interval │──┬──▶│  Scale   │
│          │  │   │          │  │   │          │
│ • Name   │  │   │ • Quality│  │   │ • Type   │
│ • Alter  │  │   │ • Number │  │   │ • Degree │
│ • Octave │  │   │ • Invert │  │   │ • Notes  │
│ • MIDI   │  │   └──────────┘  │   └──────────┘
└──────────┘  │                 │
              │   ┌──────────┐  │   ┌──────────────────┐
              ├──▶│  Chord   │──┴──▶│ ChordProgression │
              │   │          │      │                  │
              │   │ • Root   │      │ • Diatonic       │
              │   │ • Type   │      │ • Roman Num.     │
              │   │ • Invert │      │ • Common Prog.   │
              │   │ • Symbol │      └──────────────────┘
              │   └──────────┘
              │
              │   ┌──────────────┐
              └──▶│ KeySignature │
                  │              │
                  │ • Key        │
                  │ • Mode       │
                  │ • Sharps/♭s  │
                  │ • Circle 5th │
                  └──────────────┘

┌──────────────┐      ┌──────────┐
│ TimeSignature│─────▶│ Duration │
│              │      │          │
│ • Numerator  │      │ • Type   │
│ • Denominator│      │ • Dots   │
│ • Beat/Measure│      │ • Tuplet │
└──────────────┘      └──────────┘
```

**Core Relationships:**
- **Note** → **Interval**: Calculate distance between notes
- **Note** + **Interval** → **Note**: Transpose notes
- **Note** + **Chord Type** → **Chord**: Build chords from root notes
- **Note** + **Scale Type** → **Scale**: Generate scales from root notes
- **KeySignature** → **ChordProgression**: Generate diatonic chords and progressions
- **TimeSignature** + **Duration** → Calculate time in seconds, measures

## 💡 Real-World Examples

### Example 1: Chord Progression Generator

```csharp
var key = new KeySignature(new Note(NoteName.C), KeyMode.Major);
var progression = new ChordProgression(key);

// Generate a I-V-vi-IV progression (very common in pop music)
var chords = progression.ParseProgression("I - V - vi - IV");
// Result: C major - G major - A minor - F major

foreach (var chord in chords)
{
    Console.WriteLine($"{chord.GetSymbol()}: {string.Join(", ", chord.GetNotes().Select(n => n.Name))}");
}
// Output:
// C: C, E, G
// G: G, B, D
// Am: A, C, E
// F: F, A, C
```

### Example 2: MIDI Note Converter

```csharp
// Convert MIDI note numbers to musical notes
var midiNotes = new[] { 60, 64, 67 }; // C major triad
var notes = midiNotes.Select(midi => Note.FromMidiNumber(midi));

Console.WriteLine(string.Join(" - ", notes.Select(n => $"{n.Name}{n.Octave}")));
// Output: C4 - E4 - G4

// Convert back to MIDI
var midiNumbers = notes.Select(n => n.MidiNumber);
```

### Example 3: Scale Explorer

```csharp
var root = new Note(NoteName.D);

// Compare major and minor scales
var major = new Scale(root, ScaleType.Major);
var minor = new Scale(root, ScaleType.NaturalMinor);

Console.WriteLine("D Major: " + string.Join(", ", major.GetNotes().Select(n => n.Name)));
Console.WriteLine("D Minor: " + string.Join(", ", minor.GetNotes().Select(n => n.Name)));

// Output:
// D Major: D, E, F♯, G, A, B, C♯, D
// D Minor: D, E, F, G, A, B♭, C, D
```

### Example 4: Frequency Calculator

```csharp
// Calculate frequencies for equal temperament tuning (A4 = 440 Hz)
var a4 = new Note(NoteName.A, Alteration.Natural, 4);
var c5 = new Note(NoteName.C, Alteration.Natural, 5);

Console.WriteLine($"A4 frequency: {a4.Frequency:F2} Hz");  // 440.00 Hz
Console.WriteLine($"C5 frequency: {c5.Frequency:F2} Hz");  // 523.25 Hz

// Perfect for synthesizers and audio applications
```

---

## 🔧 Development

### Prerequisites
- .NET 9.0 SDK
- Any IDE with C# support (Visual Studio, Rider, VS Code)

### Building
```bash
git clone https://github.com/phmatray/MusicTheory.git
cd MusicTheory
dotnet restore
dotnet build
```

### Contributing
1. Fork the repository
2. Create a feature branch: `git checkout -b feature/new-feature`
3. Write tests for your changes
4. Implement the feature
5. Ensure all tests pass: `dotnet test`
6. Commit with conventional commits: `feat(scales): add modal scales`
7. Push and create a Pull Request

### Code Style
- Follow conventional commits: `feat(domain): description`
- Write tests first (TDD approach)
- Use descriptive test names
- Maintain immutability in domain objects
- Add XML documentation for public APIs

## 📊 Performance

The library is optimized for performance:
- **Lazy Evaluation**: Expensive calculations are cached
- **Immutable Objects**: Thread-safe and optimizable
- **Efficient Algorithms**: Optimized semitone calculations
- **Memory Efficient**: Minimal object allocation

## 🗺️ Roadmap

### Completed ✅
- Core note and interval system
- Scale generation with 15+ types
- Chord construction with extensions and inversions
- Key signatures and circle of fifths
- MIDI integration
- Time signatures and durations
- Comprehensive test coverage

### Planned 🚧
- Advanced chord progressions and voice leading
- Audio synthesis integration
- Music notation rendering
- Import/export from music formats (MusicXML, MIDI files)
- Advanced rhythm and meter analysis
- Microtonal support

## 📖 Documentation

### Quick Reference

| Resource | Description | Link |
|----------|-------------|------|
| **API Reference** | Auto-generated XML docs | IntelliSense in your IDE |
| **NuGet Package** | Package page with README | [nuget.org/packages/MusicTheory](https://www.nuget.org/packages/MusicTheory/) |
| **Source Code** | Explore implementation | [GitHub Repository](https://github.com/phmatray/MusicTheory) |
| **Examples** | Code samples | See README sections above |
| **Tests** | 504 test cases as documentation | [MusicTheory.UnitTests](https://github.com/phmatray/MusicTheory/tree/main/MusicTheory.UnitTests) |

### Documentation Topics

The library includes comprehensive XML documentation for IntelliSense:

- **Getting Started**: Installation and basic usage
- **Core Concepts**: Notes, Intervals, Scales, Chords
- **Advanced Topics**: Transposition, MIDI, Enharmonics, Progressions
- **Best Practices**: Immutability, fluent APIs, performance
- **Examples**: Real-world use cases for each component

### Building Documentation Locally

To build comprehensive HTML documentation:
1. Install [Writerside](https://www.jetbrains.com/writerside/)
2. Open the project in Writerside
3. Build the documentation

> 📚 **GitHub Pages deployment coming soon** at [phmatray.github.io/MusicTheory](https://phmatray.github.io/MusicTheory/)

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🤝 Acknowledgments

- Built with modern C# and .NET practices
- Inspired by music theory principles and mathematical foundations
- Comprehensive test coverage ensures reliability
- Designed for both educational and professional use

---

**MusicTheory** - Bringing the beauty of music theory to code 🎵