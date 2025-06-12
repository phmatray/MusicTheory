## Description

Please include a summary of the changes and which issue is fixed. Include relevant motivation and context.

Fixes # (issue)

## Type of change

Please delete options that are not relevant.

- [ ] Bug fix (non-breaking change which fixes an issue)
- [ ] New feature (non-breaking change which adds functionality)
- [ ] Breaking change (fix or feature that would cause existing functionality to not work as expected)
- [ ] Documentation update
- [ ] Performance improvement
- [ ] Code refactoring

## How Has This Been Tested?

Please describe the tests that you ran to verify your changes. Provide instructions so we can reproduce.

- [ ] Unit tests pass
- [ ] Integration tests pass
- [ ] Manual testing completed

**Test Configuration**:
* OS: [e.g. Windows 11, macOS 14]
* .NET Version: [e.g. 9.0]

## Checklist:

- [ ] My code follows the style guidelines of this project
- [ ] I have performed a self-review of my own code
- [ ] I have commented my code, particularly in hard-to-understand areas
- [ ] I have made corresponding changes to the documentation
- [ ] My changes generate no new warnings
- [ ] I have added tests that prove my fix is effective or that my feature works
- [ ] New and existing unit tests pass locally with my changes
- [ ] Any dependent changes have been merged and published in downstream modules
- [ ] I have used conventional commit messages (e.g., `feat:`, `fix:`, `docs:`)

## Code Examples

```csharp
// If applicable, show example usage of your changes
var chord = new Chord(root, ChordQuality.Major);
var altered = chord.AddAlteration(5, Alteration.Flat);
```