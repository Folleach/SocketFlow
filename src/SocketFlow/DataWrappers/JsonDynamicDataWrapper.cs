using System;
using System.Text.Json;

namespace SocketFlow.DataWrappers;

public class JsonDynamicDataWrapper : IDataWrapper<JsonDocument>
{
    public JsonDocument FormatRaw(ReadOnlyMemory<byte> data)
    {
        return JsonDocument.Parse(data.ToArray());
    }

    public ReadOnlyMemory<byte> FormatObject(object value)
    {
        return JsonSerializer.SerializeToUtf8Bytes(value);
    }
}
