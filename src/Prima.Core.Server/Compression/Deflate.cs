using System.IO.Compression;

namespace Prima.Core.Server.Compression;

public static class Deflate
{
    [ThreadStatic]
    private static LibDeflateBinding _standard;

    public static LibDeflateBinding Standard => _standard ??= new LibDeflateBinding();
}
