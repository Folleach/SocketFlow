using System.Text;

namespace SocketFlow.DataWrappers
{
    public class Utf8DataWrapper : IDataWrapper<string>
    {
        public string FormatRaw(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }

        public byte[] FormatObject(string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }
    }
}
