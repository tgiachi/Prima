using Orion.Foundations.Spans;

namespace Prima.UOData.Packets.SubCommands;

/// <summary>
/// Represents a Screen Size sub-packet of the General Information packet.
/// This packet is used to inform the server about the client's screen dimensions.
/// </summary>
/// <remarks>
/// Subcommand: 0x05
/// </remarks>
public class ScreenSizePacket : GeneralInfoSubPacket
{
    /// <summary>
    /// Gets the subcommand value for Screen Size information.
    /// </summary>
    public override ushort Subcommand => 0x05;

    /// <summary>
    /// Gets or sets the client screen width in pixels.
    /// </summary>
    public ushort Width { get; set; }

    /// <summary>
    /// Gets or sets the client screen height in pixels.
    /// </summary>
    public ushort Height { get; set; }

    /// <summary>
    /// Initializes a new instance of the ScreenSizePacket class.
    /// </summary>
    public ScreenSizePacket()
    {
    }

    /// <summary>
    /// Initializes a new instance of the ScreenSizePacket class with the specified dimensions.
    /// </summary>
    /// <param name="width">The client screen width in pixels.</param>
    /// <param name="height">The client screen height in pixels.</param>
    public ScreenSizePacket(ushort width, ushort height)
    {
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Serializes the screen size data to a byte array.
    /// </summary>
    /// <returns>A byte array containing the serialized screen size data.</returns>
    public override byte[] Serialize()
    {
        using var writer = new SpanWriter(stackalloc byte[4], true);
        writer.Write(Width);
        writer.Write(Height);
        return writer.ToSpan().Span.ToArray();
    }

    /// <summary>
    /// Deserializes the screen size data from a packet reader.
    /// </summary>
    /// <param name="reader">The packet reader containing the screen size data.</param>
    public override void Deserialize(SpanReader reader)
    {
        Width = reader.ReadUInt16();
        Height = reader.ReadUInt16();
    }
}
