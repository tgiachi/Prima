using Prima.Core.Server.Data.Uo;
using Prima.Core.Server.Types.Uo;
using Prima.UOData.Data;

namespace Prima.UOData.Context;

public static class UOContext
{
    public static ClientVersion ClientVersion { get; set; }

    public static Expansion Expansion { get; set; }

    public static ExpansionInfo ExpansionInfo { get; set; }
}
