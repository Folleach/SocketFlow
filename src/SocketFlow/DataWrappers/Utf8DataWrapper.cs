using System;
using System.Text;

namespace SocketFlow.DataWrappers;

public class Utf8DataWrapper : IDataWrapper<string>
{
    public string FormatRaw(ReadOnlyMemory<byte> data)
    {
        return Encoding.UTF8.GetString(data.ToArray());
    }

    public ReadOnlyMemory<byte> FormatObject(object value)
    {
        return Encoding.UTF8.GetBytes(value as string ?? string.Empty);
    }
}
