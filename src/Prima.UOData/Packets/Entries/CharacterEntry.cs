using Prima.Network.Serializers;

namespace Prima.UOData.Packets.Entries;

public class CharacterEntry
{
    public string Name { get; set; }

    public string Password { get; set; }


    public CharacterEntry(string name = "", string password = "")
    {
        Name = name;
        Password = password;
    }



    public byte[] ToArray()
    {
        using var pr = new PacketWriter();

        pr.WriteAsciiFixed(Name, 30);
        pr.WriteAsciiFixed(Password, 30);

        var arr= pr.ToArray();

        return arr;
    }

    public static int Lenght => 60;

}
