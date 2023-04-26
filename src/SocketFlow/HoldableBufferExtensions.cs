using System;

namespace SocketFlow;

public static class HoldableBufferExtensions
{
    public static HoldableBuffer CreateHoldable(this byte[] data)
    {
        var holdable = new HoldableBuffer(data.Length);
        data.AsSpan().CopyTo(holdable.AsSpan());
        return holdable;
    }

    public static HoldableBuffer CreateHoldable(this ReadOnlySpan<byte> data)
    {
        var holdable = new HoldableBuffer(data.Length);
        data.CopyTo(holdable.AsSpan());
        return holdable;
    }

    public static HoldableBuffer CreateHoldable(this ReadOnlyMemory<byte> data)
    {
        var holdable = new HoldableBuffer(data.Length);
        data.CopyTo(holdable.AsMemory());
        return holdable;
    }
}
