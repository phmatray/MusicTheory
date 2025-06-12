# Contributing to MusicTheory

First off, thank you for considering contributing to MusicTheory! It's people like you that make MusicTheory such a great tool.

## Code of Conduct

This project and everyone participating in it is governed by our Code of Conduct. By participating, you are expected to uphold this code. Please report unacceptable behavior to the project maintainers.

## How Can I Contribute?

### Reporting Bugs

Before creating bug reports, please check existing issues as you might find out that you don't need to create one. When you are creating a bug report, please include as many details as possible. Fill out the required template, the information it asks for helps us resolve issues faster.

### Suggesting Enhancements

Enhancement suggestions are tracked as GitHub issues. When creating an enhancement suggestion, please include:

- A clear and descriptive title
- A detailed description of the proposed enhancement
- Examples of how the feature would be used
- Why this enhancement would be useful to most users

### Your First Code Contribution

Unsure where to begin contributing? You can start by looking through these `beginner` and `help-wanted` issues:

- [Beginner issues](https://github.com/phmatray/MusicTheory/labels/good%20first%20issue) - issues which should only require a few lines of code
- [Help wanted issues](https://github.com/phmatray/MusicTheory/labels/help%20wanted) - issues which should be a bit more involved than beginner issues

### Pull Requests

1. Fork the repo and create your branch from `main`
2. If you've added code that should be tested, add tests
3. Ensure the test suite passes
4. Make sure your code follows the existing code style
5. Issue that pull request!

## Development Process

1. **Fork and Clone**
   ```bash
   git clone https://github.com/phmatray/MusicTheory.git
   cd MusicTheory
   ```

2. **Create a Feature Branch**
   ```bash
   git checkout -b feature/amazing-feature
   ```

3. **Write Tests First (TDD)**
   ```csharp
   [Fact]
   public void NewFeature_ShouldBehaveCorrectly()
   {
       // Arrange
       var input = new Note(NoteName.C);
       
       // Act
       var result = input.NewMethod();
       
       // Assert
       result.ShouldBe(expected);
   }
   ```

4. **Implement Your Feature**
   - Keep methods small and focused
   - Maintain immutability in domain objects
   - Use meaningful names
   - Add XML documentation

5. **Run Tests**
   ```bash
   dotnet test
   ```

6. **Commit Your Changes**
   ```bash
   git commit -m 'feat(domain): add amazing feature'
   ```

## Coding Conventions

### C# Style Guide

- Use PascalCase for public members
- Use camelCase for private fields and parameters
- Prefix interfaces with 'I'
- Use meaningful names (no abbreviations)
- Keep line length under 120 characters

### Example:
```csharp
public class Scale
{
    private readonly Note root;
    
    public Note Root => root;
    public ScaleType Type { get; }
    
    public Scale(Note root, ScaleType type)
    {
        this.root = root ?? throw new ArgumentNullException(nameof(root));
        Type = type;
    }
    
    /// <summary>
    /// Gets the notes in the scale.
    /// </summary>
    /// <returns>An enumerable of notes in the scale.</returns>
    public IEnumerable<Note> GetNotes()
    {
        // Implementation
    }
}
```

### Test Conventions

- One test class per production class
- Use descriptive test names: `MethodName_StateUnderTest_ExpectedBehavior`
- Follow AAA pattern: Arrange, Act, Assert
- Use Shouldly for assertions

### Commit Messages

Follow conventional commits:

- `feat`: A new feature
- `fix`: A bug fix
- `docs`: Documentation only changes
- `style`: Changes that don't affect code meaning
- `refactor`: Code change that neither fixes a bug nor adds a feature
- `perf`: Code change that improves performance
- `test`: Adding missing tests
- `chore`: Changes to the build process or auxiliary tools

Examples:
```
feat(scale): add harmonic major scale type
fix(chord): correct enharmonic calculation for augmented chords
docs(readme): add examples for chord progressions
test(interval): add edge cases for compound intervals
```

## Project Structure

```
MusicTheory/
├── MusicTheory/              # Core library
│   ├── Note.cs              # Note domain object
│   ├── Interval.cs          # Interval calculations
│   ├── Scale.cs             # Scale generation
│   ├── Chord.cs             # Chord construction
│   └── ...                  # Other domain objects
├── MusicTheory.UnitTests/    # Test project
│   ├── NoteTests.cs         # Note tests
│   ├── IntervalTests.cs     # Interval tests
│   └── ...                  # Other test files
└── README.md                 # Project documentation
```

## Testing Guidelines

- Aim for 100% code coverage on public APIs
- Test edge cases and error conditions
- Use theory tests for multiple inputs
- Mock external dependencies (if any)

### Test Example:
```csharp
[Theory]
[InlineData(NoteName.C, Alteration.Sharp, "C#")]
[InlineData(NoteName.D, Alteration.Flat, "Db")]
[InlineData(NoteName.E, Alteration.Natural, "E")]
public void Note_ToString_ShouldFormatCorrectly(
    NoteName name, Alteration alteration, string expected)
{
    // Arrange
    var note = new Note(name, alteration);
    
    // Act
    var result = note.ToString();
    
    // Assert
    result.ShouldBe(expected);
}
```

## Documentation

- Add XML documentation to all public members
- Update README.md for new features
- Include code examples in documentation
- Keep CHANGELOG.md updated

## Questions?

Feel free to open an issue with your question or reach out to the maintainers.

Thank you for contributing! 🎵