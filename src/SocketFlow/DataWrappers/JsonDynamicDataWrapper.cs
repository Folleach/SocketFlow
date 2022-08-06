using System.Text.Json;

namespace SocketFlow.DataWrappers
{
    public class JsonDynamicDataWrapper : IDataWrapper<JsonDocument>
    {
        public JsonDocument FormatRaw(byte[] data)
        {
            return JsonDocument.Parse(data);
        }

        public byte[] FormatObject(object value)
        {
            return JsonSerializer.SerializeToUtf8Bytes(value);
        }
    }
}
