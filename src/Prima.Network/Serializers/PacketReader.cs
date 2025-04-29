using System.Text;

namespace Prima.Network.Serializers;

/// <summary>
///     A modern, optimized packet reader for processing network packets.
///     Provides methods to read various data types from a byte buffer with proper boundary checking.
/// </summary>
public class PacketReader : IDisposable
{
    private readonly byte[] _buffer;
    private bool _disposed;

    /// <summary>
    ///     Initializes a new instance of the <see cref="PacketReader" /> class.
    /// </summary>
    /// <param name="data">The byte array containing packet data.</param>
    /// <param name="size">The size of the packet data.</param>
    /// <param name="fixedSize">Whether the packet has a fixed size header.</param>
    public PacketReader(byte[] data, int size, bool fixedSize)
    {
        _buffer = data ?? throw new ArgumentNullException(nameof(data));
        Size = Math.Min(size, data.Length);
        Position = fixedSize ? 1 : 3;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="PacketReader" /> class.
    /// </summary>
    /// <param name="data">The byte array containing packet data.</param>
    public PacketReader(byte[] data) : this(data, data.Length, false)
    {
    }

    /// <summary>
    ///     Gets the underlying buffer.
    /// </summary>
    public ReadOnlySpan<byte> Buffer => _buffer;

    /// <summary>
    ///     Gets the size of the packet.
    /// </summary>
    public int Size { get; }

    /// <summary>
    ///     Gets the current position in the buffer.
    /// </summary>
    public int Position { get; private set; }

    /// <summary>
    ///     Gets the number of bytes available to read.
    /// </summary>
    public int Available => Size - Position;

    /// <summary>
    ///     Disposes the resources used by the packet reader.
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    ///     Sets the read position to the specified position.
    /// </summary>
    /// <param name="offset">The position offset.</param>
    /// <param name="origin">The reference point for the offset.</param>
    /// <returns>The new position.</returns>
    public int Seek(int offset, SeekOrigin origin)
    {
        switch (origin)
        {
            case SeekOrigin.Begin:
                Position = offset;
                break;
            case SeekOrigin.Current:
                Position += offset;
                break;
            case SeekOrigin.End:
                Position = Size - offset;
                break;
        }

        // Ensure position stays within bounds
        Position = Math.Clamp(Position, 0, Size);
        return Position;
    }

    /// <summary>
    ///     Reads a 32-bit signed integer in network byte order (big endian).
    /// </summary>
    /// <returns>The 32-bit signed integer value.</returns>
    public int ReadInt32()
    {
        if (Position + 4 > Size)
        {
            return 0;
        }

        var value = (_buffer[Position] << 24) |
                    (_buffer[Position + 1] << 16) |
                    (_buffer[Position + 2] << 8) |
                    _buffer[Position + 3];

        Position += 4;
        return value;
    }

    /// <summary>
    ///     Reads a 16-bit signed integer in network byte order (big endian).
    /// </summary>
    /// <returns>The 16-bit signed integer value.</returns>
    public short ReadInt16()
    {
        if (Position + 2 > Size)
        {
            return 0;
        }

        var value = (short)((_buffer[Position] << 8) | _buffer[Position + 1]);
        Position += 2;
        return value;
    }

    /// <summary>
    ///     Reads an 8-bit unsigned integer.
    /// </summary>
    /// <returns>The 8-bit unsigned integer value.</returns>
    public byte ReadByte()
    {
        if (Position + 1 > Size)
        {
            return 0;
        }

        return _buffer[Position++];
    }

    /// <summary>
    ///     Reads a 32-bit unsigned integer in network byte order (big endian).
    /// </summary>
    /// <returns>The 32-bit unsigned integer value.</returns>
    public uint ReadUInt32()
    {
        if (Position + 4 > Size)
        {
            return 0;
        }

        var value = (uint)((_buffer[Position] << 24) |
                           (_buffer[Position + 1] << 16) |
                           (_buffer[Position + 2] << 8) |
                           _buffer[Position + 3]);

        Position += 4;
        return value;
    }

    /// <summary>
    ///     Reads a 16-bit unsigned integer in network byte order (big endian).
    /// </summary>
    /// <returns>The 16-bit unsigned integer value.</returns>
    public ushort ReadUInt16()
    {
        if (Position + 2 > Size)
        {
            return 0;
        }

        var value = (ushort)((_buffer[Position] << 8) | _buffer[Position + 1]);
        Position += 2;
        return value;
    }

    /// <summary>
    ///     Reads a 16-bit unsigned integer in big-endian format.
    /// </summary>
    /// <returns>The 16-bit unsigned integer value.</returns>
    public ushort ReadUInt16BE()
    {
        return ReadUInt16();
    }

    /// <summary>
    ///     Reads a 32-bit unsigned integer in big-endian format.
    /// </summary>
    /// <returns>The 32-bit unsigned integer value.</returns>
    public uint ReadUInt32BE()
    {
        return ReadUInt32();
    }

    /// <summary>
    ///     Reads an 8-bit signed integer.
    /// </summary>
    /// <returns>The 8-bit signed integer value.</returns>
    public sbyte ReadSByte()
    {
        if (Position + 1 > Size)
        {
            return 0;
        }

        return (sbyte)_buffer[Position++];
    }

    /// <summary>
    ///     Reads a Boolean value.
    /// </summary>
    /// <returns>The Boolean value.</returns>
    public bool ReadBoolean()
    {
        if (Position + 1 > Size)
        {
            return false;
        }

        return _buffer[Position++] != 0;
    }

    /// <summary>
    ///     Reads a specified number of bytes from the buffer.
    /// </summary>
    /// <param name="length">The number of bytes to read.</param>
    /// <returns>The byte array containing the read bytes.</returns>
    public byte[] ReadBytes(int length)
    {
        if (length <= 0)
        {
            return Array.Empty<byte>();
        }

        var available = Math.Min(length, Size - Position);

        if (available <= 0)
        {
            return Array.Empty<byte>();
        }

        var result = new byte[available];
        _buffer.AsSpan(Position, available).CopyTo(result);
        Position += available;

        return result;
    }

    /// <summary>
    ///     Reads a string in ASCII format of fixed length.
    /// </summary>
    /// <param name="length">The fixed length of the string.</param>
    /// <returns>The string read from the buffer.</returns>
    public string ReadFixedString(int length)
    {
        if (length <= 0 || Position >= Size)
        {
            return string.Empty;
        }

        var available = Math.Min(length, Size - Position);
        if (available <= 0)
        {
            return string.Empty;
        }

        // Find the actual string length (up to null terminator)
        var actualLength = 0;
        for (var i = 0; i < available; i++)
        {
            if (_buffer[Position + i] == 0)
            {
                break;
            }

            actualLength++;
        }

        // Read the string characters
        var result = Encoding.ASCII.GetString(_buffer, Position, actualLength);
        Position += length; // Always advance by the full length

        return result;
    }

    /// <summary>
    ///     Reads a null-terminated string from the current position.
    /// </summary>
    /// <returns>The string read from the buffer.</returns>
    public string ReadString()
    {
        if (Position >= Size)
        {
            return string.Empty;
        }

        // Find the null terminator
        var end = Position;
        while (end < Size && _buffer[end] != 0)
        {
            end++;
        }

        var length = end - Position;
        if (length <= 0)
        {
            // Skip the null terminator
            if (end < Size)
            {
                Position = end + 1;
            }

            return string.Empty;
        }

        var result = Encoding.ASCII.GetString(_buffer, Position, length);
        Position = end + 1; // Skip the null terminator

        return result;
    }

    /// <summary>
    ///     Reads an enumeration value.
    /// </summary>
    /// <typeparam name="T">The enumeration type.</typeparam>
    /// <returns>The enumeration value.</returns>
    public T ReadEnum<T>() where T : Enum
    {
        var value = ReadByte();
        return (T)Enum.ToObject(typeof(T), value);
    }

    /// <summary>
    ///     Reads a null-terminated Unicode string in little-endian format.
    /// </summary>
    /// <returns>The Unicode string.</returns>
    public string ReadUnicodeStringLE()
    {
        if (Position + 1 >= Size)
        {
            return string.Empty;
        }

        StringBuilder sb = new();

        while (Position + 1 < Size)
        {
            var c = _buffer[Position++] | (_buffer[Position++] << 8);
            if (c == 0)
            {
                break;
            }

            sb.Append((char)c);
        }

        return sb.ToString();
    }

    /// <summary>
    ///     Reads a null-terminated Unicode string.
    /// </summary>
    /// <returns>The Unicode string.</returns>
    public string ReadUnicodeString()
    {
        if (Position + 1 >= Size)
        {
            return string.Empty;
        }

        StringBuilder sb = new();

        while (Position + 1 < Size)
        {
            var c = (_buffer[Position++] << 8) | _buffer[Position++];
            if (c == 0)
            {
                break;
            }

            sb.Append((char)c);
        }

        return sb.ToString();
    }
}
