# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

### Build and Test
```bash
# Build the solution
dotnet build

# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run a specific test
dotnet test --filter "FullyQualifiedName~ScaleTests.Scale_ShouldGenerateCorrectNotes_ForGMajor"

# Run tests for a specific class
dotnet test --filter "ClassName=NoteTests"
```

### Development Workflow
1. Write failing test in `MusicTheory.UnitTests/`
2. Implement minimal code in `MusicTheory/`
3. Run `dotnet test` to verify
4. Refactor if needed
5. Commit using conventional commits: `feat(domain): description`

## Architecture

### Core Domain Model
The library models music theory concepts as immutable domain objects:

- **Note**: Musical note with `NoteName`, `Alteration`, `Octave`. Calculates frequency using A4=440Hz.
- **Interval**: Distance between notes with `IntervalQuality` and numeric size. Includes static factory `Interval.Between()`.
- **Scale**: Generates notes following interval patterns (W-W-H-W-W-W-H for major).
- **Chord**: Built from root note and `ChordQuality`, supports extensions via fluent API.

### Key Design Patterns
- **Immutable Objects**: All properties are read-only, set via constructor
- **Calculated Properties**: `Note.Frequency`, `Interval.Semitones` computed on-demand
- **Fluent Interface**: `Chord.AddExtension()` returns self for chaining
- **Factory Methods**: `Interval.Between()` creates intervals from two notes

### Testing Strategy
- **Framework**: xUnit with FluentAssertions
- **Structure**: One test file per domain class
- **Pattern**: Arrange-Act-Assert with descriptive test names
- **Global Usings**: FluentAssertions available in all tests via project file

### Enums and Constants
- `NoteName`: C, D, E, F, G, A, B (0-6)
- `Alteration`: DoubleFlat=-2 to DoubleSharp=2
- `IntervalQuality`: Diminished, Minor, Major, Perfect, Augmented
- `ChordQuality`: Major, Minor, Diminished, Augmented
- `ScaleType`: Major, NaturalMinor, HarmonicMinor, MelodicMinor

### Semitone Calculations
- Notes use array `[0,2,4,5,7,9,11]` for C to B semitones
- Total semitones = `Octave * 12 + semitonesFromC[NoteName] + Alteration`
- Intervals handle perfect (1,4,5,8) vs major/minor (2,3,6,7) distinctions

## Next Features to Implement

### High Priority
1. **Key Signature class** - Track sharps/flats for each key
2. **Chord Progressions** - Roman numeral analysis and common progressions  
3. **Transposition methods** - Transpose notes, intervals, and chords
4. **Chord Inversions** - First, second, third inversions

### Medium Priority
5. **Modal Scales** - Dorian, Phrygian, Lydian, Mixolydian, Aeolian, Locrian
6. **Circle of Fifths** - Navigate key relationships
7. **MIDI conversion** - Convert notes to/from MIDI numbers
8. **Enharmonic equivalence** - Handle C# = Db relationships
9. **Time Signature class** - Represent meter
10. **Rhythm/Duration classes** - Note values and timing