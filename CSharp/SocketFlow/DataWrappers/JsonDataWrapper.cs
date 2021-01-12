using System.Text.Json;

namespace SocketFlow.DataWrappers
{
    public class JsonDataWrapper<T> : IDataWrapper<T>
    {
        private readonly JsonSerializerOptions options;

        public JsonDataWrapper(JsonSerializerOptions options = null)
        {
            this.options = options ?? new JsonSerializerOptions();
        }

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
