using System.Globalization;
using Orion.Foundations.Extensions;

namespace Prima.Core.Server.Extensions;

public static class SpanExtensions
{
    public static bool ToInt32(this ReadOnlySpan<char> value, out int i) =>
        value.InsensitiveStartsWith("0x")
            ? int.TryParse(value[2..], NumberStyles.HexNumber, null, out i)
            : int.TryParse(value, out i);

    public static bool ToUInt32(this ReadOnlySpan<char> value, out uint i) =>
        value.InsensitiveStartsWith("0x")
            ? uint.TryParse(value[2..], NumberStyles.HexNumber, null, out i)
            : uint.TryParse(value, out i);


    public static uint ToUInt32(this ReadOnlySpan<char> value)
    {
        uint i;

#pragma warning disable CA1806 // Do not ignore method results
        if (value.InsensitiveStartsWith("0x"))
        {
            uint.TryParse(value[2..], NumberStyles.HexNumber, null, out i);
        }
        else
        {
            uint.TryParse(value, out i);
        }
#pragma warning restore CA1806 // Do not ignore method results

        return i;
    }
}
