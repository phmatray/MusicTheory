using System.Buffers.Binary;

namespace MusicTheory.Midi;

/// <summary>Chunk and variable-length-quantity encoding for Standard MIDI Files.</summary>
internal static class SmfEncoder
{
    /// <summary>The largest value a variable-length quantity of four bytes holds.</summary>
    private const long MaxVlq = 0x0FFFFFFF;

    /// <summary>Writes the MThd chunk of a format 1 file.</summary>
    internal static void WriteHeader(Stream output, int trackCount, int division)
    {
        Span<byte> header = stackalloc byte[14];
        "MThd"u8.CopyTo(header);
        BinaryPrimitives.WriteInt32BigEndian(header[4..], 6);
        BinaryPrimitives.WriteInt16BigEndian(header[8..], 1);
        BinaryPrimitives.WriteInt16BigEndian(header[10..], (short)trackCount);
        BinaryPrimitives.WriteInt16BigEndian(header[12..], (short)division);
        output.Write(header);
    }

    /// <summary>Writes an MTrk chunk from events sorted by tick, closed by an end-of-track at <paramref name="endTick"/>.</summary>
    internal static void WriteTrack(Stream output, IEnumerable<(long Tick, byte[] Data)> events, long endTick)
    {
        var body = new List<byte>();
        long last = 0;
        foreach (var (tick, data) in events.Append((endTick, [0xFF, 0x2F, 0x00])))
        {
            WriteVlq(body, tick - last);
            body.AddRange(data);
            last = tick;
        }

        Span<byte> head = stackalloc byte[8];
        "MTrk"u8.CopyTo(head);
        BinaryPrimitives.WriteInt32BigEndian(head[4..], body.Count);
        output.Write(head);
        output.Write(body.ToArray());
    }

    private static void WriteVlq(List<byte> into, long value)
    {
        if (value > MaxVlq)
            throw new ArgumentException($"A gap of {value} ticks is too long for a MIDI delta time (at most {MaxVlq}).");
        Span<byte> buffer = stackalloc byte[4];
        int n = 0;
        buffer[n++] = (byte)(value & 0x7F);
        while ((value >>= 7) > 0)
            buffer[n++] = (byte)((value & 0x7F) | 0x80);
        for (int i = n - 1; i >= 0; i--)
            into.Add(buffer[i]);
    }
}
