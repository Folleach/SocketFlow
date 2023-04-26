using System;
using System.Buffers;

namespace SocketFlow;

public unsafe class HoldableBuffer : IDisposable
{
    public int Length { get; private set; }
    private byte[] buffer;

    public HoldableBuffer(int length)
    {
        Length = length;
        buffer = ArrayPool<byte>.Shared.Rent(length);
#if DEBUG && FALSE
        var reference = __makeref(buffer);
        var pointer = **(IntPtr**)(&reference);
        Console.WriteLine($"[{nameof(HoldableBuffer)}] rent at 0x{(ulong)pointer:x}");
#endif
    }

    public ReadOnlyMemory<byte> AsReadOnlyMemory() => new(buffer, 0, Length);
    public ReadOnlySpan<byte> AsReadOnlySpan() => new(buffer, 0, Length);
    public ReadOnlySpan<byte> AsReadOnlySpan(int start, int length) => new(buffer, start, length);

    public Memory<byte> AsMemory() => new(buffer, 0, Length);
    public Span<byte> AsSpan() => new(buffer, 0, Length);
    public ArraySegment<byte> AsArraySegment() => new(buffer, 0, Length);

    public void Trim(int length)
    {
        Length = length;
    }

    void IDisposable.Dispose()
    {
        var t = buffer;
        buffer = null;
#if DEBUG && FALSE
        var reference = __makeref(t);
        var pointer = **(IntPtr**)(&reference);
        Console.WriteLine($"[{nameof(HoldableBuffer)}] return at 0x{(ulong)pointer:x}");
#endif
        ArrayPool<byte>.Shared.Return(t);
    }
}
