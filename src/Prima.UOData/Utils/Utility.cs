using System.Buffers.Binary;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using Orion.Foundations.Buffers;
using Orion.Foundations.Extensions;
using Prima.UOData.Data.Geometry;
using Server;

namespace Prima.UOData.Utils;

public static partial class Utility
{
    private static Dictionary<IPAddress, IPAddress> _ipAddressTable;

    public static void Separate(StringBuilder sb, string value, string separator)
    {
        if (sb.Length > 0)
        {
            sb.Append(separator);
        }

        sb.Append(value);
    }


    public static bool ToBoolean(string value) =>
        bool.TryParse(value, out var b)
            ? b
            : value.InsensitiveEquals("enabled") || value.InsensitiveEquals("on");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Intern(this string str) => str?.Length > 0 ? string.Intern(str) : str;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Intern(ref string str)
    {
        str = Intern(str);
    }

    public static IPAddress Intern(IPAddress ipAddress)
    {
        if (ipAddress == null)
        {
            return null;
        }

        if (ipAddress.IsIPv4MappedToIPv6)
        {
            ipAddress = ipAddress.MapToIPv4();
        }

        _ipAddressTable ??= new Dictionary<IPAddress, IPAddress>();

        if (!_ipAddressTable.TryGetValue(ipAddress, out var interned))
        {
            interned = ipAddress;
            _ipAddressTable[ipAddress] = interned;
        }

        return interned;
    }

    public static void Intern(ref IPAddress ipAddress)
    {
        ipAddress = Intern(ipAddress);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ApplyCidrMask(ref ulong high, ref ulong low, int prefixLength, bool isMax)
    {
        // This should never happen, a 0 CIDR is not valid
        if (prefixLength == 0)
        {
            high = low = ~0UL;
            return;
        }

        if (prefixLength == 128)
        {
            return;
        }

        if (prefixLength == 64)
        {
            low = 0;
            return;
        }

        if (prefixLength < 64)
        {
            int bitsToFlip = 64 - prefixLength;
            ulong highMask = isMax ? ~0UL >> bitsToFlip : ~0UL << (bitsToFlip + 1);

            high = isMax ? high | highMask : high & highMask;
            low = isMax ? ~0UL : 0UL;
        }
        else
        {
            int bitsToFlip = 128 - prefixLength;
            ulong lowMask = isMax ? ~0UL >> (64 - bitsToFlip) : ~0UL << bitsToFlip;

            low = isMax ? low | lowMask : low & lowMask;
        }
    }

    // Converts an IPAddress to a UInt128 in IPv6 format
    public static UInt128 ToUInt128(this IPAddress ip)
    {
        if (ip.AddressFamily == AddressFamily.InterNetwork && !ip.IsIPv4MappedToIPv6)
        {
            Span<byte> integer = stackalloc byte[4];
            return !ip.TryWriteBytes(integer, out _)
                ? (UInt128)0
                : new UInt128(0, 0xFFFF00000000UL | BinaryPrimitives.ReadUInt32BigEndian(integer));
        }

        Span<byte> bytes = stackalloc byte[16];
        if (!ip.TryWriteBytes(bytes, out _))
        {
            return 0;
        }

        ulong high = BinaryPrimitives.ReadUInt64BigEndian(bytes[..8]);
        ulong low = BinaryPrimitives.ReadUInt64BigEndian(bytes.Slice(8, 8));

        return new UInt128(high, low);
    }

    public static int ToInt32(ReadOnlySpan<char> value)
    {
        int i;

#pragma warning disable CA1806 // Do not ignore method results
        if (value.StartsWithOrdinal("0x"))
        {
            int.TryParse(value[2..], NumberStyles.HexNumber, null, out i);
        }
        else
        {
            int.TryParse(value, out i);
        }
#pragma warning restore CA1806 // Do not ignore method results

        return i;
    }

    public static uint ToUInt32(ReadOnlySpan<char> value)
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

    public static bool ToInt32(ReadOnlySpan<char> value, out int i) =>
        value.InsensitiveStartsWith("0x")
            ? int.TryParse(value[2..], NumberStyles.HexNumber, null, out i)
            : int.TryParse(value, out i);

    public static bool ToUInt32(ReadOnlySpan<char> value, out uint i) =>
        value.InsensitiveStartsWith("0x")
            ? uint.TryParse(value[2..], NumberStyles.HexNumber, null, out i)
            : uint.TryParse(value, out i);


    // Converts a UInt128 in IPv6 format to an IPAddress
    public static IPAddress ToIpAddress(this UInt128 value, bool mapToIpv6 = false)
    {
        // IPv4 mapped IPv6 address
        if (!mapToIpv6 && value >= 0xFFFF00000000UL && value <= 0xFFFFFFFFFFFFUL)
        {
            var newAddress = IPAddress.HostToNetworkOrder((int)value);
            return new IPAddress(unchecked((uint)newAddress));
        }

        Span<byte> bytes = stackalloc byte[16]; // 128 bits for IPv6 address
        ((IBinaryInteger<UInt128>)value).WriteBigEndian(bytes);

        return new IPAddress(bytes);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt128 CreateCidrAddress(ReadOnlySpan<byte> bytes, int prefixLength, bool isMax)
    {
        ulong high = BinaryPrimitives.ReadUInt64BigEndian(bytes[..8]);
        ulong low = BinaryPrimitives.ReadUInt64BigEndian(bytes.Slice(8, 8));

        if (prefixLength < 128)
        {
            ApplyCidrMask(ref high, ref low, prefixLength, isMax);
        }

        return new UInt128(high, low);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteMappedIPv6To(this IPAddress ipAddress, Span<byte> destination)
    {
        if (ipAddress.AddressFamily == AddressFamily.InterNetworkV6)
        {
            ipAddress.TryWriteBytes(destination, out _);
            return;
        }

        destination[..8].Clear(); // Local init is off
        BinaryPrimitives.WriteUInt32BigEndian(destination.Slice(8, 4), 0xFFFF);
        ipAddress.TryWriteBytes(destination.Slice(12, 4), out _);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool MatchClassC(this IPAddress ip1, IPAddress ip2) => ip1.MatchCidr(24, ip2);

    public static bool MatchCidr(this IPAddress cidrAddress, int prefixLength, IPAddress address)
    {
        Span<byte> cidrBytes = stackalloc byte[16];
        cidrAddress.WriteMappedIPv6To(cidrBytes);

        if (cidrAddress.AddressFamily != AddressFamily.InterNetworkV6)
        {
            prefixLength += 96; // 32 -> 128
        }

        var min = CreateCidrAddress(cidrBytes, prefixLength, false);
        var max = CreateCidrAddress(cidrBytes, prefixLength, true);
        var ip = address.ToUInt128();

        return ip >= min && ip <= max;
    }


    public static void FixHtml(this Span<char> chars)
    {
        if (chars.Length == 0)
        {
            return;
        }

        ReadOnlySpan<char> invalid = ['<', '>', '#'];
        ReadOnlySpan<char> replacement = ['(', ')', '-'];

        chars.ReplaceAny(invalid, replacement);
    }


    public static object GetArrayCap(Array array, int index, object emptyValue = null) =>
        array.Length > 0 ? array.GetValue(Math.Clamp(index, 0, array.Length - 1)) : emptyValue;

    public static void FixPoints(ref Point3D top, ref Point3D bottom)
    {
        if (bottom.m_X < top.m_X)
        {
            (top.m_X, bottom.m_X) = (bottom.m_X, top.m_X);
        }

        if (bottom.m_Y < top.m_Y)
        {
            (top.m_Y, bottom.m_Y) = (bottom.m_Y, top.m_Y);
        }

        if (bottom.m_Z < top.m_Z)
        {
            (top.m_Z, bottom.m_Z) = (bottom.m_Z, top.m_Z);
        }
    }

    public static T RandomList<T>(params T[] array)
    {
        return array.ToList().RandomElement();
    }
}
