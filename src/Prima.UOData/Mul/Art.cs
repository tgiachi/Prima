namespace Prima.UOData.Mul;

public class Art
{
    private static FileIndex m_FileIndex = new FileIndex(
        "Artidx.mul",
        "Art.mul",
        "artLegacyMUL.uop",
        0x10000 /*0x13FDC*/,
        4,
        ".tga",
        0x13FDC,
        false
    );


    public static bool IsUOAHS()
    {
        return (GetIdxLength() >= 0x13FDC);
    }

    public static int GetMaxItemID()
    {
        if (GetIdxLength() >= 0x13FDC)
        {
            return 0xFFFF;
        }

        if (GetIdxLength() == 0xC000)
        {
            return 0x7FFF;
        }

        return 0x3FFF;
    }

    public static ushort GetLegalItemID(int itemID, bool checkmaxid = true)
    {
        if (itemID < 0)
        {
            return 0;
        }

        if (checkmaxid)
        {
            int max = GetMaxItemID();
            if (itemID > max)
            {
                return 0;
            }
        }

        return (ushort)itemID;
    }

    public static int GetIdxLength()
    {
        return (int)(m_FileIndex.IdxLength / 12);
    }
}
