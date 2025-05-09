using Prima.Network.Packets.Base;
using Prima.UOData.Data.Map;
using Prima.UOData.Packets.Entries;

namespace Prima.UOData.Packets;

public class CharactersStartingLocations : BaseUoNetworkPacket
{
    public List<CityInfo> Cities { get; } = new();

    public List<CharacterEntry> Characters { get; } = new();

    public CharactersStartingLocations() : base(0xA9, -1)
    {
    }

    public void FillCharacters(List<CharacterEntry>? characters = null, int size = 5)
    {
        Characters.Clear();

        if (characters != null)
        {
            Characters.AddRange(characters);
        }
        else
        {
            for (var i = 0; i < size; i++)
            {
                Characters.Add(new CharacterEntry());
            }
        }
    }
}
