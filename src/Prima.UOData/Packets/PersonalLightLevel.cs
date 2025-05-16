using Orion.Foundations.Spans;
using Prima.Network.Packets.Base;
using Prima.UOData.Id;
using Prima.UOData.Interfaces.Entities;

namespace Prima.UOData.Packets;

public class PersonalLightLevel : BaseUoNetworkPacket
{
    public Serial Serial { get; set; }

    public int LightLevel { get; set; }



    public PersonalLightLevel() : base(0x4E, 6)
    {
    }

    public PersonalLightLevel(IHaveSerial serial, int lightLevel) : this()
    {
        Serial = serial.Id;
        LightLevel = lightLevel;
    }

    public PersonalLightLevel(Serial serial, int lightLevel) : this()
    {
        Serial = serial;
        LightLevel = lightLevel;
    }

    public override Span<byte> Write()
    {
        using var packetWriter = new SpanWriter(stackalloc byte[Length -1]);
        packetWriter.Write((uint)Serial);
        packetWriter.Write((sbyte)LightLevel);
        return packetWriter.Span.ToArray();
    }
}
