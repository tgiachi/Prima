/*************************************************************************
 * ModernUO                                                              *
 * Copyright 2019-2023 - ModernUO Development Team                       *
 * Email: hi@modernuo.com                                                *
 * File: Point3D.cs                                                      *
 *                                                                       *
 * This program is free software: you can redistribute it and/or modify  *
 * it under the terms of the GNU General Public License as published by  *
 * the Free Software Foundation, either version 3 of the License, or     *
 * (at your option) any later version.                                   *
 *                                                                       *
 * You should have received a copy of the GNU General Public License     *
 * along with this program.  If not, see <http://www.gnu.org/licenses/>. *
 *************************************************************************/

using System.ComponentModel;
using System.Runtime.CompilerServices;
using Orion.Foundations.Extensions;
using Prima.UOData.Interfaces.Geometry;
using Prima.UOData.Utils;


namespace Prima.UOData.Data.Geometry;

public struct Point3D
    : IPoint3D, IComparable<Point3D>, IComparable<IPoint3D>, IEquatable<object>, IEquatable<Point3D>,
        IEquatable<IPoint3D>, ISpanFormattable, ISpanParsable<Point3D>, INotifyPropertyChanged
{
#pragma warning disable 67
    public event PropertyChangedEventHandler? PropertyChanged;

#pragma warning restore 67

    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }

    public static readonly Point3D Zero = new(0, 0, 0);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Point3D(IPoint3D p) : this(p.X, p.Y, p.Z)
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Point3D(Point3D p) : this(p.X, p.Y, p.Z)
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Point3D(Point2D p, int z) : this(p.X, p.Y, z)
    {
    }

    public Point3D(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public bool Equals(Point3D other) => X == other.X && Y == other.Y && Z == other.Z;

    public bool Equals(IPoint3D other) =>
        X == other?.X && Y == other.Y && Z == other.Z;

    public override bool Equals(object obj) => obj is Point3D other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(X, Y, Z);

    public static bool operator ==(Point3D l, Point3D r) => l.X == r.X && l.Y == r.Y && l.Z == r.Z;

    public static bool operator ==(Point3D l, IPoint3D r) =>
        !ReferenceEquals(r, null) && l.X == r.X && l.Y == r.Y && l.Z == r.Z;

    public static bool operator !=(Point3D l, Point3D r) => l.X != r.X || l.Y != r.Y || l.Z != r.Z;

    public static bool operator !=(Point3D l, IPoint3D r) =>
        !ReferenceEquals(r, null) && (l.X != r.X || l.Y != r.Y || l.Z != r.Z);

    public static bool operator >(Point3D l, Point3D r) => l.X > r.X && l.Y > r.Y && l.Z > r.Z;

    public static bool operator >(Point3D l, IPoint3D r) =>
        !ReferenceEquals(r, null) && l.X > r.X && l.Y > r.Y && l.Z > r.Z;

    public static bool operator <(Point3D l, Point3D r) => l.X < r.X && l.Y < r.Y && l.Z > r.Z;

    public static bool operator <(Point3D l, IPoint3D r) =>
        !ReferenceEquals(r, null) && l.X < r.X && l.Y < r.Y && l.Z > r.Z;

    public static bool operator >=(Point3D l, Point3D r) => l.X >= r.X && l.Y >= r.Y && l.Z > r.Z;

    public static bool operator >=(Point3D l, IPoint3D r) =>
        !ReferenceEquals(r, null) && l.X >= r.X && l.Y >= r.Y && l.Z > r.Z;

    public static bool operator <=(Point3D l, Point3D r) => l.X <= r.X && l.Y <= r.Y && l.Z > r.Z;

    public static bool operator <=(Point3D l, IPoint3D r) =>
        !ReferenceEquals(r, null) && l.X <= r.X && l.Y <= r.Y && l.Z > r.Z;

    public int CompareTo(Point3D other)
    {
        var xComparison = X.CompareTo(other.X);
        if (xComparison != 0)
        {
            return xComparison;
        }

        var yComparison = Y.CompareTo(other.Y);
        if (yComparison != 0)
        {
            return yComparison;
        }

        return Z.CompareTo(other.Z);
    }

    public int CompareTo(IPoint3D other)
    {
        var xComparison = X.CompareTo(other.X);
        if (xComparison != 0)
        {
            return xComparison;
        }

        var yComparison = Y.CompareTo(other.Y);
        if (yComparison != 0)
        {
            return yComparison;
        }

        return Z.CompareTo(other.Z);
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider provider)
        => destination.TryWrite(provider, $"({X}, {Y}, {Z})", out charsWritten);

    public override string ToString()
    {
        // Maximum number of characters that are needed to represent this:
        // 6 characters for (, , )
        // Up to 11 characters to represent each integer
        const int maxLength = 6 + 11 * 3;
        Span<char> span = stackalloc char[maxLength];
        TryFormat(span, out var charsWritten, null, null);
        return span[..charsWritten].ToString();
    }

    public string ToString(string format, IFormatProvider formatProvider)
    {
        // format and formatProvider are not doing anything right now, so use the
        // default ToString implementation.
        return ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Point3D Parse(string s) => Parse(s, null);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Point3D Parse(string s, IFormatProvider provider) => Parse(s.AsSpan(), provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse(string s, IFormatProvider provider, out Point3D result) =>
        TryParse(s.AsSpan(), provider, out result);

    public static Point3D Parse(ReadOnlySpan<char> s, IFormatProvider provider)
    {
        s = s.Trim();

        if (!s.StartsWithOrdinal('(') || !s.EndsWithOrdinal(')'))
        {
            throw new FormatException($"The input string '{s}' was not in a correct format.");
        }

        var firstComma = s.IndexOfOrdinal(',');
        if (firstComma == -1)
        {
            throw new FormatException($"The input string '{s}' was not in a correct format.");
        }

        var first = s.Slice(1, firstComma - 1).Trim();

        if (!Utility.ToInt32(first, out var x))
        {
            throw new FormatException($"The input string '{s}' was not in a correct format.");
        }

        var offset = firstComma + 1;

        var secondComma = s[offset..].IndexOfOrdinal(',');
        if (secondComma == -1)
        {
            throw new FormatException($"The input string '{s}' was not in a correct format.");
        }

        var second = s.Slice(firstComma + 1, secondComma).Trim();

        if (!Utility.ToInt32(second, out var y))
        {
            throw new FormatException($"The input string '{s}' was not in a correct format.");
        }

        offset += secondComma + 1;

        var third = s.Slice(offset, s.Length - offset - 1).Trim();
        if (!Utility.ToInt32(third, out var z))
        {
            throw new FormatException($"The input string '{s}' was not in a correct format.");
        }

        return new Point3D(x, y, z);
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider provider, out Point3D result)
    {
        s = s.Trim();

        if (!s.StartsWithOrdinal('(') || !s.EndsWithOrdinal(')'))
        {
            result = default;
            return false;
        }

        var firstComma = s.IndexOfOrdinal(',');
        if (firstComma == -1)
        {
            result = default;
            return false;
        }

        var first = s.Slice(1, firstComma - 1).Trim();
        if (!Utility.ToInt32(first, out var x))
        {
            result = default;
            return false;
        }

        var offset = firstComma + 1;

        var secondComma = s[offset..].IndexOfOrdinal(',');
        if (secondComma == -1)
        {
            result = default;
            return false;
        }

        var second = s.Slice(firstComma + 1, secondComma).Trim();
        if (!Utility.ToInt32(second, out var y))
        {
            result = default;
            return false;
        }

        offset += secondComma + 1;

        var third = s.Slice(offset, s.Length - offset - 1).Trim();
        if (!Utility.ToInt32(third, out var z))
        {
            result = default;
            return false;
        }

        result = new Point3D(x, y, z);
        return true;
    }
}
