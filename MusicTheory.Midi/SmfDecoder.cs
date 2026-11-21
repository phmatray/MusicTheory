using System.Buffers.Binary;

namespace MusicTheory.Midi;

/// <summary>The events of a Standard MIDI File that <see cref="MidiReader"/> uses, all with absolute ticks.</summary>
internal sealed record SmfContent(
    int Division,
    List<(long Start, long End, int Pitch)> Notes,
    List<(long Tick, int Micros)> Tempos,
    List<(long Tick, int Numerator, int Denominator)> Meters,
    List<(long Tick, int Sharps, int Mode)> Keys);

/// <summary>Chunk walking, variable-length quantities and running status for Standard MIDI Files.</summary>
internal static class SmfDecoder
{
    internal static SmfContent Decode(Stream input)
    {
        using var copy = new MemoryStream();
        input.CopyTo(copy);
        var data = copy.ToArray();

        if (data.Length < 14 || !data.AsSpan(0, 4).SequenceEqual("MThd"u8) || BinaryPrimitives.ReadInt32BigEndian(data.AsSpan(4)) != 6)
            throw new FormatException("Not a Standard MIDI File: the MThd header chunk is missing.");
        int format = BinaryPrimitives.ReadInt16BigEndian(data.AsSpan(8));
        int tracks = BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan(10));
        int division = BinaryPrimitives.ReadInt16BigEndian(data.AsSpan(12));
        if (division < 0)
            throw new NotSupportedException("SMPTE time division is not supported; the file must count ticks per quarter note.");
        if (format > 1)
            throw new NotSupportedException($"MIDI format {format} is not supported; only formats 0 and 1 are.");
        if (division == 0)
            throw new FormatException("The division is 0 ticks per quarter note.");

        var content = new SmfContent(division, [], [], [], []);
        int pos = 14;
        for (int t = 0; t < tracks; t++)
        {
            if (pos + 8 > data.Length || !data.AsSpan(pos, 4).SequenceEqual("MTrk"u8))
                throw new FormatException($"Track {t} does not start with an MTrk chunk.");
            long end = pos + 8L + BinaryPrimitives.ReadUInt32BigEndian(data.AsSpan(pos + 4));
            if (end > data.Length)
                throw new FormatException($"Track {t} is longer than the file.");
            ReadTrack(data, pos + 8, (int)end, content);
            pos = (int)end;
        }

        return content;
    }

    private static void ReadTrack(byte[] data, int pos, int end, SmfContent content)
    {
        int Byte() => pos < end ? data[pos++] : throw new FormatException("The file ends in the middle of an event.");
        long Vlq()
        {
            long value = 0;
            int b;
            do
            {
                b = Byte();
                value = (value << 7) | (uint)(b & 0x7F);
            }
            while ((b & 0x80) != 0);
            return value;
        }

        long tick = 0;
        int status = 0;
        var open = new Dictionary<int, long>();
        while (pos < end)
        {
            tick += Vlq();
            int first = Byte();
            if (first >= 0x80)
            {
                status = first;
                first = -1;
            }
            else if (status == 0)
                throw new FormatException("An event has no status byte and no running status to reuse.");

            if (status == 0xFF)
            {
                int type = Byte();
                int length = (int)Vlq();
                int start = pos;
                for (int i = 0; i < length; i++)
                    Byte();
                if (type == 0x2F)
                    break;
                if (type == 0x51 && length == 3)
                    content.Tempos.Add((tick, (data[start] << 16) | (data[start + 1] << 8) | data[start + 2]));
                else if (type == 0x58 && length >= 2)
                    content.Meters.Add((tick, data[start], 1 << data[start + 1]));
                else if (type == 0x59 && length == 2)
                    content.Keys.Add((tick, (sbyte)data[start], data[start + 1]));
                continue;
            }

            if (status is 0xF0 or 0xF7)
            {
                for (long i = Vlq(); i > 0; i--)
                    Byte();
                status = 0;
                continue;
            }

            int d1 = first >= 0 ? first : Byte();
            int d2 = (status & 0xF0) is 0xC0 or 0xD0 ? 0 : Byte();
            int key = ((status & 0x0F) << 7) | d1;
            if ((status & 0xF0) == 0x90 && d2 > 0)
                open[key] = tick;
            else if ((status & 0xF0) is 0x80 or 0x90 && open.Remove(key, out long on))
                content.Notes.Add((on, tick, d1));
        }

        // A note never switched off ends with the track.
        foreach (var (key, on) in open)
            content.Notes.Add((on, tick, key & 0x7F));
    }
}
