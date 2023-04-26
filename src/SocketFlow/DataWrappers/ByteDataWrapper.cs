using System;

namespace SocketFlow.DataWrappers;

public class ByteDataWrapper : IDataWrapper<byte[]>
{
    public byte[] FormatRaw(ReadOnlyMemory<byte> data)
    {
        return data.ToArray();
    }

    public ReadOnlyMemory<byte> FormatObject(object value)
    {
        return value as byte[];
    }
}
