using Prima.Core.Server.Data.Uo;
using Prima.Core.Server.Types.Uo;

namespace Prima.UOData.Context;

public static class UOContext
{
    public static ClientVersion ClientVersion { get; set; }

    public static Expansion Expansion { get; set; }
}
