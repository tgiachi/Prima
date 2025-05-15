using System.Runtime.InteropServices;
using Prima.UOData.Mul;

namespace Prima.UOData.Data.Tiles;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Tile : IComparable
{
    internal ushort m_ID;
    internal sbyte m_Z;

    public ushort ID
    {
        get { return m_ID; }
    }

    public int Z
    {
        get { return m_Z; }
        set { m_Z = (sbyte)value; }
    }

    public Tile(ushort id, sbyte z)
    {
        m_ID = id;
        m_Z = z;
    }

    public Tile(ushort id, sbyte z, sbyte flag)
    {
        m_ID = id;
        m_Z = z;
    }

    public void Set(ushort id, sbyte z)
    {
        m_ID = id;
        m_Z = z;
    }

    public void Set(ushort id, sbyte z, sbyte flag)
    {
        m_ID = id;
        m_Z = z;
    }

    public int CompareTo(object x)
    {
        if (x == null)
        {
            return 1;
        }

        if (!(x is Tile))
        {
            throw new ArgumentNullException();
        }

        var a = (Tile)x;

        if (m_Z > a.m_Z)
        {
            return 1;
        }
        else if (a.m_Z > m_Z)
        {
            return -1;
        }

        ItemData ourData = TileData.ItemTable[m_ID];
        ItemData theirData = TileData.ItemTable[a.m_ID];

        if (ourData.Height > theirData.Height)
        {
            return 1;
        }
        else if (theirData.Height > ourData.Height)
        {
            return -1;
        }

        if (ourData.Background && !theirData.Background)
        {
            return -1;
        }
        else if (theirData.Background && !ourData.Background)
        {
            return 1;
        }

        return 0;
    }
}
