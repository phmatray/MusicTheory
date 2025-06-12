# MusicTheory Library - Coverage Analysis

## Overall Coverage Summary
- **Line Coverage**: 81.2%
- **Branch Coverage**: 61.3%

## Priority 1: Methods with 0% Coverage (HIGHEST PRIORITY)
These methods have absolutely no test coverage and should be addressed first:

### Duration Class
1. **`Equals(object obj)`** - Standard equality comparison
2. **`GetHashCode()`** - Hash code generation for collections

### TimeSignature Class  
1. **`get_CommonTime`** - Static property for 4/4 time
2. **`get_CutTime`** - Static property for 2/2 time
3. **`Equals(object obj)`** - Standard equality comparison
4. **`GetHashCode()`** - Hash code generation

## Priority 2: Classes Below 80% Coverage

### 1. KeySignature (72.6% coverage) - LOWEST COVERAGE
Missing coverage for:
- **`GetEnharmonicEquivalents()`** (51.4% coverage) - Lines 285-295
- **`GetKeyFromPosition()`** (60.5% coverage) - Lines 346-370
- **`GetCircleOfFifthsPosition()`** (67.2% coverage) - Lines 153-185

### 2. Note (78.4% coverage)
Missing coverage for:
- **`SimplifyEnharmonic()`** (46.7% coverage) - Enharmonic simplification logic
- **`GetEnharmonicEquivalent()`** (60.0% coverage) - Lines 419-428
- **`Parse(string noteString)`** (69.8% coverage) - String parsing logic
- **`FromMidiNumber(int midiNumber, bool preferFlats)`** (72.1% coverage) - Lines 305-317
- Complex enharmonic handling in lines 476-487, 499-529

### 3. Scale (78.0% coverage)
Missing coverage for:
- **`GetNextNoteInScale()`** (69.8% coverage) - Lines 169-177
- **`GetIntervalPattern()`** (78.4% coverage) - Lines 96-105 (modal scales patterns)
- Modal scale interval patterns not fully tested

### 4. Chord (79.2% coverage)
Missing coverage for:
- **`GetNotesInInversion()`** (35.3% coverage) - Lines 213-236
  - Second inversion logic (lines 222-224)
  - Third inversion logic (lines 227-233)
- **`GetNoteAtInterval()`** (68.0% coverage) - Lines 138-146
  - Octave wrapping logic for alterations

## Priority 3: Specific Uncovered Scenarios

### ChordProgression (85.2% coverage)
- **`ParseChordSymbol()`** - Lines 158-161 (secondary dominant parsing)
- **`GetDegreeFromRomanNumeral()`** - Lines 179-184 (edge cases)

### Interval (92.3% coverage)
- **`DetermineIntervalFromSemitonesAndNumber()`** - Lines 103-107 (edge cases)

### Duration (88.4% coverage)
- **`ToString()`** - Lines 218-224 (tuplet string representation)
- **`GetSymbol()`** - Lines 263-274 (special symbol cases)

## Priority 4: Branch Coverage Improvements

### Areas with Low Branch Coverage:
1. **Note class** - Enharmonic equivalence branches
2. **Chord class** - Inversion handling branches
3. **KeySignature** - Circle of fifths navigation branches
4. **Scale class** - Modal scale type branches

## Recommended Test Implementation Order

1. **Immediate (0% coverage methods):**
   - [ ] Duration.Equals() and GetHashCode()
   - [ ] TimeSignature static properties (CommonTime, CutTime)
   - [ ] TimeSignature.Equals() and GetHashCode()

2. **High Priority (Low coverage critical methods):**
   - [ ] Chord.GetNotesInInversion() - especially 2nd and 3rd inversions
   - [ ] Note.SimplifyEnharmonic() - complex enharmonic logic
   - [ ] KeySignature.GetEnharmonicEquivalents()
   - [ ] Scale modal patterns in GetIntervalPattern()

3. **Medium Priority (Edge cases):**
   - [ ] Note.FromMidiNumber() with preferFlats=true
   - [ ] Note.Parse() error cases and complex formats
   - [ ] Chord.GetNoteAtInterval() with octave wrapping
   - [ ] ChordProgression.ParseChordSymbol() secondary dominants

4. **Low Priority (Minor gaps):**
   - [ ] Duration.ToString() for tuplets
   - [ ] Various edge cases in high-coverage methods

## Specific Test Cases Needed

### Note Class
```csharp
// Test FromMidiNumber with preferFlats=true for all black keys
[Theory]
[InlineData(61, "Db4")] // C#4
[InlineData(63, "Eb4")] // D#4
[InlineData(66, "Gb4")] // F#4
[InlineData(68, "Ab4")] // G#4
[InlineData(70, "Bb4")] // A#4]

// Test SimplifyEnharmonic for double sharps/flats
[Theory]
[InlineData("C##", "D")]
[InlineData("Dbb", "C")]
[InlineData("E##", "F#")]
[InlineData("Fbb", "Eb")]
```

### Chord Class
```csharp
// Test GetNotesInInversion for all inversions
[Theory]
[InlineData(ChordInversion.Second)]
[InlineData(ChordInversion.Third)]

// Test GetNoteAtInterval with octave wrapping
[Theory]
[InlineData("B", 2, "C#5")] // Wraps to next octave
[InlineData("C", -2, "Bb3")] // Wraps to previous octave
```

### TimeSignature Class
```csharp
// Test static properties
[Fact]
public void CommonTime_ShouldReturn4_4()

[Fact] 
public void CutTime_ShouldReturn2_2()

// Test Equals and GetHashCode
[Fact]
public void Equals_WithSameValues_ShouldReturnTrue()

[Fact]
public void GetHashCode_WithSameValues_ShouldReturnSameHash()
```

### Duration Class
```csharp
// Test Equals and GetHashCode implementation
[Fact]
public void Equals_WithObject_ShouldWorkCorrectly()

[Fact]
public void GetHashCode_ShouldBeConsistent()
```

## Summary
To reach 100% coverage, approximately **126 uncovered lines** need tests. The highest priority should be given to methods with 0% coverage and critical functionality like chord inversions and enharmonic handling. This will require approximately 30-40 new test methods.