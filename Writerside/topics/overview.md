# MusicTheory Documentation

Welcome to the **MusicTheory** library documentation. This comprehensive C# library provides immutable domain objects for music theory concepts, built with modern .NET practices and extensive test coverage.

## What is MusicTheory?

MusicTheory is a powerful .NET library that models music theory concepts as code. Whether you're building a music education app, a composition tool, or analyzing musical structures, this library provides the fundamental building blocks you need.

## Key Features

<tabs>
    <tab title="Core Components">
        - **Notes**: Create and manipulate musical notes with support for all alterations
        - **Intervals**: Calculate musical intervals with proper quality handling
        - **Scales**: Generate scales with 15+ scale types including modal and exotic scales
        - **Chords**: Build chords with 40+ types including triads, seventh chords, extended, altered, and suspended chords
        - **Key Signatures**: Handle key signatures with circle of fifths navigation
    </tab>
    <tab title="Advanced Features">
        - **Transposition**: Transpose notes, chords, and scales by intervals
        - **Chord Progressions**: Roman numeral analysis and common progressions
        - **MIDI Integration**: Convert between notes and MIDI numbers
        - **Enharmonic Equivalence**: Handle enharmonic relationships (C# ↔ Db)
        - **Time & Rhythm**: Time signatures and note duration calculations
    </tab>
    <tab title="Design Principles">
        - **Immutable Objects**: Thread-safe, predictable behavior
        - **Fluent API**: Chainable method calls for readable code
        - **Type Safety**: Strong typing prevents invalid music theory constructs
        - **Performance**: Lazy evaluation and calculated properties
        - **Comprehensive Testing**: 479+ unit tests ensure reliability
    </tab>
</tabs>

## Quick Example

Here's a simple example demonstrating the library's capabilities:

```C#
using MusicTheory;

// Create a note
var c4 = new Note(NoteName.C, Alteration.Natural, 4);

// Build a chord
var cMaj7 = new Chord(c4, ChordType.Major7);

// Get chord notes
var notes = cMaj7.GetNotes(); // C, E, G, B

// Get chord symbol
var symbol = cMaj7.GetSymbol(); // "Cmaj7"

// Create a scale
var cMajorScale = new Scale(c4, ScaleType.Major);
var scaleNotes = cMajorScale.GetNotes(); // C, D, E, F, G, A, B, C
```

## Installation

<procedure title="Getting Started" id="getting-started">
    <step>
        <p>Clone the repository:</p>
        <code-block lang="bash">
            git clone https://github.com/phmatray/MusicTheory.git
            cd MusicTheory
        </code-block>
    </step>
    <step>
        <p>Build the solution:</p>
        <code-block lang="bash">
            dotnet build
        </code-block>
    </step>
    <step>
        <p>Run tests to verify everything is working:</p>
        <code-block lang="bash">
            dotnet test
        </code-block>
    </step>
</procedure>

## Where to Start?

- [Getting Started Guide](getting-started.md) - Learn the basics of the library
- [Working with Notes](notes.md) - Understanding the Note class
- [Building Chords](chords.md) - Create and manipulate chords
- [Scale Generation](scales.md) - Generate and work with scales
- [API Reference](api-overview.md) - Complete API documentation

## Requirements

- .NET 9.0 or later
- Any IDE with C# support (Visual Studio, Rider, VS Code)

## Support

- [Report Issues](https://github.com/phmatray/MusicTheory/issues)
- [GitHub Repository](https://github.com/phmatray/MusicTheory)

<seealso>
    <category ref="getting-started">
        <a href="getting-started.md">Getting Started</a>
        <a href="examples.md">Code Examples</a>
    </category>
    <category ref="core-concepts">
        <a href="notes.md">Notes</a>
        <a href="intervals.md">Intervals</a>
        <a href="chords.md">Chords</a>
        <a href="scales.md">Scales</a>
    </category>
</seealso>