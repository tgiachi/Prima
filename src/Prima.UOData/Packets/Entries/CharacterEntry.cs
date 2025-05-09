using Prima.Network.Serializers;

namespace Prima.UOData.Packets.Entries;

public class CharacterEntry
{
    public string Name { get; set; }

    public string Password { get; set; }



    public byte[] ToArray()
    {
        using var pr = new PacketWriter();

        pr.WriteAsciiFixed(Name, 30);
        pr.WriteAsciiFixed(Password, 30);

        return pr.ToArray();
    }

    public int Lenght => 60;

}
