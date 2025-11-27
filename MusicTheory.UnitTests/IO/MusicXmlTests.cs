namespace MusicTheory.UnitTests.IO;

using MusicTheory.IO;

public class MusicXmlTests
{
    #region Note Serialization

    [Fact]
    public void MusicXml_SerializeNote_C4_ShouldCreateValidXml()
    {
        // Arrange
        var note = new Note(NoteName.C, Alteration.Natural, 4);

        // Act
        var xml = MusicXmlWriter.NoteToXml(note);

        // Assert
        xml.ShouldContain("<step>C</step>");
        xml.ShouldContain("<octave>4</octave>");
    }

    [Fact]
    public void MusicXml_SerializeNote_FSharp5_ShouldIncludeAlter()
    {
        // Arrange
        var note = new Note(NoteName.F, Alteration.Sharp, 5);

        // Act
        var xml = MusicXmlWriter.NoteToXml(note);

        // Assert
        xml.ShouldContain("<step>F</step>");
        xml.ShouldContain("<alter>1</alter>");
        xml.ShouldContain("<octave>5</octave>");
    }

    [Fact]
    public void MusicXml_SerializeNote_BFlat3_ShouldIncludeNegativeAlter()
    {
        // Arrange
        var note = new Note(NoteName.B, Alteration.Flat, 3);

        // Act
        var xml = MusicXmlWriter.NoteToXml(note);

        // Assert
        xml.ShouldContain("<step>B</step>");
        xml.ShouldContain("<alter>-1</alter>");
        xml.ShouldContain("<octave>3</octave>");
    }

    #endregion

    #region Note Deserialization

    [Fact]
    public void MusicXml_DeserializeNote_C4_ShouldParseCorrectly()
    {
        // Arrange
        var xml = @"<pitch>
            <step>C</step>
            <octave>4</octave>
        </pitch>";

        // Act
        var note = MusicXmlReader.ParsePitch(xml);

        // Assert
        note.Name.ShouldBe(NoteName.C);
        note.Alteration.ShouldBe(Alteration.Natural);
        note.Octave.ShouldBe(4);
    }

    [Fact]
    public void MusicXml_DeserializeNote_FSharp5_ShouldParseAlter()
    {
        // Arrange
        var xml = @"<pitch>
            <step>F</step>
            <alter>1</alter>
            <octave>5</octave>
        </pitch>";

        // Act
        var note = MusicXmlReader.ParsePitch(xml);

        // Assert
        note.Name.ShouldBe(NoteName.F);
        note.Alteration.ShouldBe(Alteration.Sharp);
        note.Octave.ShouldBe(5);
    }

    [Fact]
    public void MusicXml_DeserializeNote_BFlat3_ShouldParseNegativeAlter()
    {
        // Arrange
        var xml = @"<pitch>
            <step>B</step>
            <alter>-1</alter>
            <octave>3</octave>
        </pitch>";

        // Act
        var note = MusicXmlReader.ParsePitch(xml);

        // Assert
        note.Name.ShouldBe(NoteName.B);
        note.Alteration.ShouldBe(Alteration.Flat);
        note.Octave.ShouldBe(3);
    }

    #endregion

    #region Key Signature Serialization

    [Fact]
    public void MusicXml_SerializeKey_CMajor_ShouldHaveZeroFifths()
    {
        // Arrange
        var key = new KeySignature(new Note(NoteName.C), KeyMode.Major);

        // Act
        var xml = MusicXmlWriter.KeyToXml(key);

        // Assert
        xml.ShouldContain("<fifths>0</fifths>");
        xml.ShouldContain("<mode>major</mode>");
    }

    [Fact]
    public void MusicXml_SerializeKey_GMajor_ShouldHaveOneFifth()
    {
        // Arrange
        var key = new KeySignature(new Note(NoteName.G), KeyMode.Major);

        // Act
        var xml = MusicXmlWriter.KeyToXml(key);

        // Assert
        xml.ShouldContain("<fifths>1</fifths>");
        xml.ShouldContain("<mode>major</mode>");
    }

    [Fact]
    public void MusicXml_SerializeKey_FMajor_ShouldHaveNegativeOneFifth()
    {
        // Arrange
        var key = new KeySignature(new Note(NoteName.F), KeyMode.Major);

        // Act
        var xml = MusicXmlWriter.KeyToXml(key);

        // Assert
        xml.ShouldContain("<fifths>-1</fifths>");
        xml.ShouldContain("<mode>major</mode>");
    }

    [Fact]
    public void MusicXml_SerializeKey_AMinor_ShouldHaveZeroFifthsMinorMode()
    {
        // Arrange
        var key = new KeySignature(new Note(NoteName.A), KeyMode.Minor);

        // Act
        var xml = MusicXmlWriter.KeyToXml(key);

        // Assert
        xml.ShouldContain("<fifths>0</fifths>");
        xml.ShouldContain("<mode>minor</mode>");
    }

    #endregion

    #region Key Signature Deserialization

    [Fact]
    public void MusicXml_DeserializeKey_CMajor_ShouldParseCorrectly()
    {
        // Arrange
        var xml = @"<key>
            <fifths>0</fifths>
            <mode>major</mode>
        </key>";

        // Act
        var key = MusicXmlReader.ParseKey(xml);

        // Assert
        key.Tonic.Name.ShouldBe(NoteName.C);
        key.Mode.ShouldBe(KeyMode.Major);
    }

    [Fact]
    public void MusicXml_DeserializeKey_DMajor_ShouldParseTwoSharps()
    {
        // Arrange
        var xml = @"<key>
            <fifths>2</fifths>
            <mode>major</mode>
        </key>";

        // Act
        var key = MusicXmlReader.ParseKey(xml);

        // Assert
        key.Tonic.Name.ShouldBe(NoteName.D);
        key.Mode.ShouldBe(KeyMode.Major);
    }

    [Fact]
    public void MusicXml_DeserializeKey_BbMajor_ShouldParseTwoFlats()
    {
        // Arrange
        var xml = @"<key>
            <fifths>-2</fifths>
            <mode>major</mode>
        </key>";

        // Act
        var key = MusicXmlReader.ParseKey(xml);

        // Assert
        key.Tonic.Name.ShouldBe(NoteName.B);
        key.Tonic.Alteration.ShouldBe(Alteration.Flat);
        key.Mode.ShouldBe(KeyMode.Major);
    }

    #endregion

    #region Chord Serialization

    [Fact]
    public void MusicXml_SerializeChord_CMajor_ShouldCreateHarmonyElement()
    {
        // Arrange
        var chord = new Chord(new Note(NoteName.C), ChordQuality.Major);

        // Act
        var xml = MusicXmlWriter.ChordToXml(chord);

        // Assert
        xml.ShouldContain("<harmony>");
        xml.ShouldContain("<root-step>C</root-step>");
        xml.ShouldContain("<kind>major</kind>");
    }

    [Fact]
    public void MusicXml_SerializeChord_G7_ShouldIncludeDominant()
    {
        // Arrange
        var chord = new Chord(new Note(NoteName.G), ChordType.Dominant7);

        // Act
        var xml = MusicXmlWriter.ChordToXml(chord);

        // Assert
        xml.ShouldContain("<root-step>G</root-step>");
        xml.ShouldContain("<kind>dominant</kind>");
    }

    [Fact]
    public void MusicXml_SerializeChord_FSharpMinor_ShouldIncludeAlter()
    {
        // Arrange
        var chord = new Chord(new Note(NoteName.F, Alteration.Sharp), ChordQuality.Minor);

        // Act
        var xml = MusicXmlWriter.ChordToXml(chord);

        // Assert
        xml.ShouldContain("<root-step>F</root-step>");
        xml.ShouldContain("<root-alter>1</root-alter>");
        xml.ShouldContain("<kind>minor</kind>");
    }

    #endregion

    #region Round-Trip Tests

    [Fact]
    public void MusicXml_RoundTrip_Note_ShouldPreserveData()
    {
        // Arrange
        var original = new Note(NoteName.E, Alteration.Flat, 4);

        // Act
        var xml = MusicXmlWriter.NoteToXml(original);
        var roundTripped = MusicXmlReader.ParsePitch(xml);

        // Assert
        roundTripped.Name.ShouldBe(original.Name);
        roundTripped.Alteration.ShouldBe(original.Alteration);
        roundTripped.Octave.ShouldBe(original.Octave);
    }

    [Fact]
    public void MusicXml_RoundTrip_KeySignature_ShouldPreserveData()
    {
        // Arrange
        var original = new KeySignature(new Note(NoteName.A), KeyMode.Major);

        // Act
        var xml = MusicXmlWriter.KeyToXml(original);
        var roundTripped = MusicXmlReader.ParseKey(xml);

        // Assert
        roundTripped.Tonic.Name.ShouldBe(original.Tonic.Name);
        roundTripped.Mode.ShouldBe(original.Mode);
    }

    #endregion
}
