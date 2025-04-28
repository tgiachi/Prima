using Prima.Network.Interfaces.Packets;
using Prima.Network.Serializers;

namespace Prima.Network.Packets.Base;

public abstract class BaseUoNetworkPacket : IUoNetworkPacket
{
    public byte OpCode { get; }

    protected BaseUoNetworkPacket(byte opCode)
    {
        OpCode = opCode;
    }

    public virtual void Read(PacketReader reader)
    {
        throw new NotImplementedException();
    }

    public virtual void Write(PacketWriter writer)
    {
        throw new NotImplementedException();
    }


    public override string ToString()
    {
        return GetType().Name + " { OpCode: " + OpCode.ToString("X2") + " }";
    }
}
