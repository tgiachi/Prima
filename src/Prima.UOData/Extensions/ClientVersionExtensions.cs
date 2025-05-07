using System.Runtime.CompilerServices;
using Prima.UOData.Types;

namespace Prima.UOData.Extensions;

public static class ClientVersionExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string TypeName(this ClientType type) =>
        type switch
        {
            ClientType.UOTD => "UO:TD",
            ClientType.KR   => "UO:KR",
            ClientType.SA   => "UO:SA",
            _               => "classic",
        };
}
