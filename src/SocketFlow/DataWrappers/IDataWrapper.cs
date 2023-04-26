using System;

namespace SocketFlow.DataWrappers;

public interface IDataWrapper<out T>
{
    T FormatRaw(ReadOnlyMemory<byte> data);
    ReadOnlyMemory<byte> FormatObject(object value);
}
