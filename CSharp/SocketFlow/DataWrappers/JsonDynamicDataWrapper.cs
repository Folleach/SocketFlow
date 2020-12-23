using System.Text;
using System.Text.Json;

namespace SocketFlow.DataWrappers
{
    public class JsonDynamicDataWrapper : IDataWrapper<JsonDocument>
    {
        private static readonly Encoding encoding = Encoding.UTF8;

        public JsonDocument FormatRaw(byte[] data)
        {
            return JsonDocument.Parse(encoding.GetString(data));
        }

        public byte[] FormatObject(object value)
        {
            return encoding.GetBytes(JsonSerializer.Serialize(value));
        }
    }
}
