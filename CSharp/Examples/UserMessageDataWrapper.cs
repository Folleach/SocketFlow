using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using SocketFlow.DataWrappers;

namespace Examples
{
    public class UserMessageDataWrapper : IDataWrapper<UserMessage>
    {
        public UserMessage FormatRaw(byte[] data)
        {
            return (UserMessage)new BinaryFormatter().Deserialize(new MemoryStream(data));
        }

        public byte[] FormatObject(object value)
        {
            var memoryStream = new MemoryStream();
            new BinaryFormatter().Serialize(memoryStream, value);
            return memoryStream.ToArray();
        }
    }
}
