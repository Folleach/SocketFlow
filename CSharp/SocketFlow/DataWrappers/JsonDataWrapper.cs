using System.Text.Json;

namespace SocketFlow.DataWrappers
{
    public class JsonDataWrapper<T> : IDataWrapper<T>
    {
        private readonly JsonSerializerOptions options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        public T FormatRaw(byte[] data)
        {
            return JsonSerializer.Deserialize<T>(data, options);
        }

        public byte[] FormatObject(object value)
        {
            return JsonSerializer.SerializeToUtf8Bytes(value, options);
        }
    }
}
