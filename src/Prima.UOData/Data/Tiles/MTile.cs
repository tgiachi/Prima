using Prima.UOData.Mul;
using Prima.UOData.Types;
using Server;

namespace Prima.UOData.Data.Tiles;

public struct MTile : IComparable
{
    internal ushort m_ID;
    internal sbyte m_Z;
    internal TileFlag m_Flag;
    internal int m_Solver;

    public ushort ID => m_ID;

    public int Z
    {
        get => m_Z;
        set => m_Z = (sbyte)value;
    }

    public TileFlag Flag
    {
        get => m_Flag;
        set => m_Flag = value;
    }

    public int Solver
    {
        get => m_Solver;
        set => m_Solver = value;
    }

    public MTile(ushort id, sbyte z)
    {
        m_ID = Art.GetLegalItemID(id);
        m_Z = z;
        m_Flag = TileFlag.Background;
        m_Solver = 0;
    }

    public MTile(ushort id, sbyte z, TileFlag flag)
    {
        m_ID = Art.GetLegalItemID(id);
        m_Z = z;
        m_Flag = flag;
        m_Solver = 0;
    }

    public void Set(ushort id, sbyte z)
    {
        m_ID = Art.GetLegalItemID(id);
        m_Z = z;
    }

    public void Set(ushort id, sbyte z, TileFlag flag)
    {
        m_ID = Art.GetLegalItemID(id);
        m_Z = z;
        m_Flag = flag;
    }

    public int CompareTo(object x)
    {
        if (x == null)
        {
            return 1;
        }

        if (!(x is MTile))
        {
            throw new ArgumentNullException();
        }

        var a = (MTile)x;

        ItemData ourData = TileData.ItemTable[m_ID];
        ItemData theirData = TileData.ItemTable[a.ID];

        int ourTreshold = 0;
        if (ourData.Height > 0)
        {
            ++ourTreshold;
        }

        if (!ourData.Background)
        {
            ++ourTreshold;
        }

        int ourZ = Z;
        int theirTreshold = 0;
        if (theirData.Height > 0)
        {
            ++theirTreshold;
        }

        if (!theirData.Background)
        {
            ++theirTreshold;
        }

        int theirZ = a.Z;

        ourZ += ourTreshold;
        theirZ += theirTreshold;
        int res = ourZ - theirZ;
        if (res == 0)
        {
            res = ourTreshold - theirTreshold;
        }

        if (res == 0)
        {
            res = m_Solver - a.Solver;
        }

        return res;
    }
}
