/*************************************************************************
 * ModernUO                                                              *
 * Copyright 2019-2023 - ModernUO Development Team                       *
 * Email: hi@modernuo.com                                                *
 * File: Serial.cs                                                       *
 *                                                                       *
 * This program is free software: you can redistribute it and/or modify  *
 * it under the terms of the GNU General Public License as published by  *
 * the Free Software Foundation, either version 3 of the License, or     *
 * (at your option) any later version.                                   *
 *                                                                       *
 * You should have received a copy of the GNU General Public License     *
 * along with this program.  If not, see <http://www.gnu.org/licenses/>. *
 *************************************************************************/

using System.Runtime.CompilerServices;
using Prima.Core.Server.Extensions;

namespace Prima.UOData.Id;

public readonly struct Serial
    : IComparable<Serial>, IComparable<uint>,
        IEquatable<Serial>, ISpanFormattable, ISpanParsable<Serial>
{
    public const uint ItemOffset = 0x40000000;
    public const uint MaxItemSerial = 0x7EEEEEEE;
    public const uint MaxMobileSerial = ItemOffset - 1;

    public static readonly Serial MinusOne = new(0xFFFFFFFF);
    public static readonly Serial Zero = new(0);

    public Serial(uint serial) => Value = serial;



    public uint Value { get; }

    public bool IsMobile
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Value is > 0 and < ItemOffset;
    }

    public bool IsItem
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Value is >= ItemOffset and < MaxItemSerial;
    }

    public bool IsValid
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Value > 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => Value.GetHashCode();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(Serial other) => Value.CompareTo(other.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(uint other) => Value.CompareTo(other);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object obj) =>
        obj switch
        {
            Serial serial => this == serial,
            uint u        => Value == u,
            _             => false
        };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Serial l, Serial r) => l.Value == r.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Serial l, uint r) => l.Value == r;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Serial l, Serial r) => l.Value != r.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Serial l, uint r) => l.Value != r;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(Serial l, Serial r) => l.Value > r.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(Serial l, uint r) => l.Value > r;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(Serial l, Serial r) => l.Value < r.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(Serial l, uint r) => l.Value < r;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(Serial l, Serial r) => l.Value >= r.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(Serial l, uint r) => l.Value >= r;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(Serial l, Serial r) => l.Value <= r.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(Serial l, uint r) => l.Value <= r;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Serial operator +(Serial l, Serial r) => (Serial)(l.Value + r.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Serial operator +(Serial l, uint r) => (Serial)(l.Value + r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Serial operator ++(Serial l) => (Serial)(l.Value + 1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Serial operator -(Serial l, Serial r) => (Serial)(l.Value - r.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Serial operator -(Serial l, uint r) => (Serial)(l.Value - r);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Serial operator --(Serial l) => (Serial)(l.Value - 1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString()
    {
        // Maximum number of characters that are needed to represent this:
        // 2 characters for 0x
        // Up to 8 characters to represent the value in hex
        Span<char> span = stackalloc char[10];
        TryFormat(span, out var charsWritten, null, null);
        return span[..charsWritten].ToString();
    }

    public string ToString(string format, IFormatProvider formatProvider)
    {
        // format and formatProvider are not doing anything right now, so use the
        // default ToString implementation.
        return ToString();
    }

    public bool TryFormat(
        Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider provider
    ) => format != ReadOnlySpan<char>.Empty
        ? Value.TryFormat(destination, out charsWritten, format, provider)
        : destination.TryWrite(provider, $"0x{Value:X8}", out charsWritten);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator uint(Serial a) => a.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator Serial(uint a) => new(a);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Serial other) => Value == other.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ToInt32() => (int)Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Serial Parse(string s) => Parse(s, null);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Serial Parse(string s, IFormatProvider provider) => Parse(s.AsSpan(), provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse(string s, IFormatProvider provider, out Serial result) =>
        TryParse(s.AsSpan(), provider, out result);

    public static Serial Parse(ReadOnlySpan<char> s, IFormatProvider provider) => new(s.ToUInt32());

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider provider, out Serial result)
    {
        if (s.ToUInt32(out var value))
        {
            result = new Serial(value);
            return true;
        }

        result = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Serial RandomSerial()
    {
        return new Serial((uint)Random.Shared.Next(1, int.MaxValue));
    }
}
