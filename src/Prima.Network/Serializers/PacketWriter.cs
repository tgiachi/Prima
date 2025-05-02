using System.Net;

namespace Prima.Network.Serializers;

using System;
using System.IO;
using System.Text;

/// <summary>
/// Provides functionality for writing primitive binary data for network packets.
/// </summary>
public sealed class PacketWriter : IDisposable
{
    private readonly byte[] _buffer = new byte[4];
    private readonly int _capacity;
    private bool _disposed;


    /// <summary>
    /// Instantiates a new PacketWriter instance with the default capacity of 32 bytes.
    /// </summary>
    public PacketWriter() : this(32)
    {
    }

    /// <summary>
    /// Instantiates a new PacketWriter instance with a given capacity.
    /// </summary>
    /// <param name="capacity">Initial capacity for the internal stream.</param>
    public PacketWriter(int capacity)
    {
        UnderlyingStream = new MemoryStream(capacity);
        _capacity = capacity;
        UnderlyingStream.SetLength(0);
    }

    /// <summary>
    /// Gets the total stream length.
    /// </summary>
    public long Length => UnderlyingStream.Length;

    /// <summary>
    /// Gets or sets the current stream position.
    /// </summary>
    public long Position
    {
        get => UnderlyingStream.Position;
        set => UnderlyingStream.Position = value;
    }

    /// <summary>
    /// The internal stream used by this PacketWriter instance.
    /// </summary>
    public MemoryStream UnderlyingStream { get; }

    /// <summary>
    /// Writes a 1-byte boolean value to the underlying stream. False is represented by 0, true by 1.
    /// </summary>
    /// <param name="value">The boolean value to write.</param>
    public void Write(bool value)
    {
        UnderlyingStream.WriteByte((byte)(value ? 255 : 0));
    }

    /// <summary>
    /// Writes a 1-byte unsigned integer value to the underlying stream.
    /// </summary>
    /// <param name="value">The byte value to write.</param>
    public void Write(byte value)
    {
        UnderlyingStream.WriteByte(value);
    }

    /// <summary>
    /// Writes a 1-byte unsigned integer value to the underlying stream.
    /// </summary>
    /// <param name="value">The byte value to write.</param>
    public void WriteByte(byte value)
    {
        Write(value);
    }

    /// <summary>
    /// Writes a 1-byte signed integer value to the underlying stream.
    /// </summary>
    /// <param name="value">The signed byte value to write.</param>
    public void Write(sbyte value)
    {
        UnderlyingStream.WriteByte((byte)value);
    }

    /// <summary>
    /// Writes a 2-byte signed integer value to the underlying stream in big-endian format.
    /// </summary>
    /// <param name="value">The signed short value to write.</param>
    public void Write(short value)
    {
        _buffer[0] = (byte)(value >> 8);
        _buffer[1] = (byte)value;

        UnderlyingStream.Write(_buffer, 0, 2);
    }

    /// <summary>
    /// Writes a 2-byte unsigned integer value to the underlying stream in big-endian format.
    /// </summary>
    /// <param name="value">The unsigned short value to write.</param>
    public void Write(ushort value)
    {
        _buffer[0] = (byte)(value >> 8);
        _buffer[1] = (byte)value;

        UnderlyingStream.Write(_buffer, 0, 2);
    }

    /// <summary>
    /// Writes a 2-byte unsigned integer value to the underlying stream in big-endian format.
    /// </summary>
    /// <param name="value">The unsigned short value to write.</param>
    public void WriteUInt16BE(ushort value)
    {
        Write(value);
    }

    /// <summary>
    /// Writes a 4-byte signed integer value to the underlying stream in big-endian format.
    /// </summary>
    /// <param name="value">The signed integer value to write.</param>
    public void Write(int value)
    {
        _buffer[0] = (byte)(value >> 24);
        _buffer[1] = (byte)(value >> 16);
        _buffer[2] = (byte)(value >> 8);
        _buffer[3] = (byte)value;

        UnderlyingStream.Write(_buffer, 0, 4);
    }

    /// <summary>
    /// Writes a 4-byte unsigned integer value to the underlying stream in big-endian format.
    /// </summary>
    /// <param name="value">The unsigned integer value to write.</param>
    public void Write(uint value)
    {
        _buffer[0] = (byte)(value >> 24);
        _buffer[1] = (byte)(value >> 16);
        _buffer[2] = (byte)(value >> 8);
        _buffer[3] = (byte)value;

        UnderlyingStream.Write(_buffer, 0, 4);
    }

    /// <summary>
    /// Writes a 4-byte unsigned integer value to the underlying stream in big-endian format.
    /// </summary>
    /// <param name="value">The unsigned integer value to write.</param>
    public void WriteUInt32BE(uint value)
    {
        Write(value);
    }

    /// <summary>
    /// Writes a sequence of bytes to the underlying stream.
    /// </summary>
    /// <param name="buffer">The byte array containing data to write.</param>
    /// <param name="offset">The starting position in the buffer.</param>
    /// <param name="size">The number of bytes to write.</param>
    public void Write(byte[] buffer, int offset, int size)
    {
        UnderlyingStream.Write(buffer, offset, size);
    }

    /// <summary>
    /// Writes a byte array to the underlying stream.
    /// </summary>
    /// <param name="buffer">The byte array to write.</param>
    public void Write(byte[] buffer)
    {
        if (buffer == null || buffer.Length == 0)
            return;

        UnderlyingStream.Write(buffer, 0, buffer.Length);
    }

    public void WriteIpAddress(IPAddress ipAddress)
    {
        if  (ipAddress == null)
            return;

        byte[] ipBytes = ipAddress.GetAddressBytes();
        //Array.Reverse(ipBytes);
        UnderlyingStream.Write(ipBytes, 0, ipBytes.Length);
    }

    /// <summary>
    /// Writes a fixed-length ASCII-encoded string value to the underlying stream.
    /// To fit (size), the string content is either truncated or padded with null characters.
    /// </summary>
    /// <param name="value">The string value to write.</param>
    /// <param name="size">The fixed size to write.</param>
    public void WriteFixedString(string value, int size)
    {
        if (value == null)
        {
            value = string.Empty;
        }

        int length = value.Length;

        UnderlyingStream.SetLength(UnderlyingStream.Length + size);

        if (length >= size)
            UnderlyingStream.Position += Encoding.ASCII.GetBytes(
                value,
                0,
                size,
                UnderlyingStream.GetBuffer(),
                (int)UnderlyingStream.Position
            );
        else
        {
            Encoding.ASCII.GetBytes(value, 0, length, UnderlyingStream.GetBuffer(), (int)UnderlyingStream.Position);
            UnderlyingStream.Position += size;
        }
    }



    /// <summary>
    /// Writes a dynamic-length ASCII-encoded string value to the underlying stream, followed by a 1-byte null character.
    /// </summary>
    /// <param name="value">The string value to write.</param>
    public void WriteAsciiNull(string value)
    {
        if (value == null)
        {
            value = string.Empty;
        }

        int length = value.Length;

        UnderlyingStream.SetLength(UnderlyingStream.Length + length + 1);

        Encoding.ASCII.GetBytes(value, 0, length, UnderlyingStream.GetBuffer(), (int)UnderlyingStream.Position);
        UnderlyingStream.Position += length + 1;
    }

    /// <summary>
    /// Writes a dynamic-length little-endian unicode string value to the underlying stream, followed by a 2-byte null character.
    /// </summary>
    /// <param name="value">The string value to write.</param>
    public void WriteLittleUniNull(string value)
    {
        if (value == null)
        {
            value = string.Empty;
        }

        int length = value.Length;

        UnderlyingStream.SetLength(UnderlyingStream.Length + ((length + 1) * 2));

        UnderlyingStream.Position += Encoding.Unicode.GetBytes(
            value,
            0,
            length,
            UnderlyingStream.GetBuffer(),
            (int)UnderlyingStream.Position
        );
        UnderlyingStream.Position += 2;
    }

    /// <summary>
    /// Writes a fixed-length little-endian unicode string value to the underlying stream.
    /// To fit (size), the string content is either truncated or padded with null characters.
    /// </summary>
    /// <param name="value">The string value to write.</param>
    /// <param name="size">The fixed size (in characters) to write.</param>
    public void WriteLittleUniFixed(string value, int size)
    {
        if (value == null)
        {
            value = string.Empty;
        }

        size *= 2;

        int length = value.Length;

        UnderlyingStream.SetLength(UnderlyingStream.Length + size);

        if ((length * 2) >= size)
            UnderlyingStream.Position += Encoding.Unicode.GetBytes(
                value,
                0,
                length,
                UnderlyingStream.GetBuffer(),
                (int)UnderlyingStream.Position
            );
        else
        {
            Encoding.Unicode.GetBytes(value, 0, length, UnderlyingStream.GetBuffer(), (int)UnderlyingStream.Position);
            UnderlyingStream.Position += size;
        }
    }

    /// <summary>
    /// Writes a dynamic-length big-endian unicode string value to the underlying stream, followed by a 2-byte null character.
    /// </summary>
    /// <param name="value">The string value to write.</param>
    public void WriteBigUniNull(string value)
    {
        if (value == null)
        {
            value = string.Empty;
        }

        int length = value.Length;

        UnderlyingStream.SetLength(UnderlyingStream.Length + ((length + 1) * 2));

        UnderlyingStream.Position += Encoding.BigEndianUnicode.GetBytes(
            value,
            0,
            length,
            UnderlyingStream.GetBuffer(),
            (int)UnderlyingStream.Position
        );
        UnderlyingStream.Position += 2;
    }

    /// <summary>
    /// Writes a fixed-length big-endian unicode string value to the underlying stream.
    /// To fit (size), the string content is either truncated or padded with null characters.
    /// </summary>
    /// <param name="value">The string value to write.</param>
    /// <param name="size">The fixed size (in characters) to write.</param>
    public void WriteBigUniFixed(string value, int size)
    {
        if (value == null)
        {
            value = string.Empty;
        }

        size *= 2;

        int length = value.Length;

        UnderlyingStream.SetLength(UnderlyingStream.Length + size);

        if ((length * 2) >= size)
            UnderlyingStream.Position += Encoding.BigEndianUnicode.GetBytes(
                value,
                0,
                length,
                UnderlyingStream.GetBuffer(),
                (int)UnderlyingStream.Position
            );
        else
        {
            Encoding.BigEndianUnicode.GetBytes(
                value,
                0,
                length,
                UnderlyingStream.GetBuffer(),
                (int)UnderlyingStream.Position
            );
            UnderlyingStream.Position += size;
        }
    }

    /// <summary>
    /// Writes an enum value as a byte to the underlying stream.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <param name="value">The enum value to write.</param>
    public void WriteEnum<T>(T value) where T : Enum
    {
        Write(Convert.ToByte(value));
    }

    /// <summary>
    /// Fills the stream from the current position up to (capacity) with 0x00's
    /// </summary>
    public void Fill()
    {
        Fill((int)(_capacity - UnderlyingStream.Length));
    }

    /// <summary>
    /// Writes a number of 0x00 byte values to the underlying stream.
    /// </summary>
    /// <param name="length">The number of 0x00 bytes to write.</param>
    public void Fill(int length)
    {
        if (UnderlyingStream.Position == UnderlyingStream.Length)
        {
            UnderlyingStream.SetLength(UnderlyingStream.Length + length);
            UnderlyingStream.Seek(0, SeekOrigin.End);
        }
        else
        {
            UnderlyingStream.Write(new byte[length], 0, length);
        }
    }

    /// <summary>
    /// Offsets the current position from an origin.
    /// </summary>
    /// <param name="offset">A byte offset relative to origin.</param>
    /// <param name="origin">A value of type SeekOrigin indicating the reference point.</param>
    /// <returns>The new position within the stream.</returns>
    public long Seek(long offset, SeekOrigin origin)
    {
        return UnderlyingStream.Seek(offset, origin);
    }

    /// <summary>
    /// Gets the entire stream content as a byte array.
    /// </summary>
    /// <returns>A byte array containing the stream data.</returns>
    public byte[] ToArray()
    {
        return UnderlyingStream.ToArray();
    }

    /// <summary>
    /// Disposes of resources used by the PacketWriter.
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            UnderlyingStream?.Dispose();
            _disposed = true;
        }

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes of resources used by the PacketWriter.
    /// </summary>
    /// <param name="disposing">Whether the method is being called from Dispose() or the finalizer.</param>
    protected void Dispose(bool disposing)
    {
        if (disposing && !_disposed)
        {
            UnderlyingStream?.Dispose();
            _disposed = true;
        }
    }
}
