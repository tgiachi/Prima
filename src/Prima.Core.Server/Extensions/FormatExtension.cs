namespace Prima.Core.Server.Extensions;

public static class FormatExtension
{
    public static void FormatBuffer(this TextWriter op, ReadOnlySpan<byte> data)
    {
        op.WriteLine("        0  1  2  3  4  5  6  7  8  9  A  B  C  D  E  F");
        op.WriteLine("       -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- --");

        var totalLength = data.Length;
        if (totalLength <= 0)
        {
            op.WriteLine("0000   ");
            return;
        }

        Span<byte> lineBytes = stackalloc byte[16];
        Span<char> lineChars = stackalloc char[47];
        for (var i = 0; i < totalLength; i += 16)
        {
            var length = Math.Min(data.Length - i, 16);
            data.Slice(i, length).CopyTo(lineBytes);

            var charsWritten = ((ReadOnlySpan<byte>)lineBytes[..length]).ToSpacedHexString(lineChars);

            op.Write("{0:X4}   ", i);
            op.Write(lineChars[..charsWritten]);
            op.WriteLine();
        }
    }

}
