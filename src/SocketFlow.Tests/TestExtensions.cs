using System.Text;

namespace SocketFlow.Tests;

public static class TestExtensions
{
    public static HoldableBuffer AsHoldable(this string value, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        var buffer = new HoldableBuffer(1024);
        var count = encoding.GetBytes(value, buffer.AsArraySegment());
        buffer.Trim(count);
        return buffer;
    }

    public static string AsString(this HoldableBuffer buffer)
    {
        return Encoding.UTF8.GetString(buffer.AsReadOnlySpan());
    }
}
