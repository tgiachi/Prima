using Orion.Foundations.Spans;
using Prima.UOData.Id;

namespace Prima.UOData.Extensions;

public static class SpanExtensions
{
    public static void Write(this SpanWriter writer, Serial serial)
    {
        writer.Write((uint)serial);
    }
}
