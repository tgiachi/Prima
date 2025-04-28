using Prima.Network.Serializers;

namespace Prima.Network.Interfaces.Packets;

public interface IUoNetworkPacket
{
    byte OpCode { get; }

    void Read(PacketReader reader);
    void Write(PacketWriter writer);
}
