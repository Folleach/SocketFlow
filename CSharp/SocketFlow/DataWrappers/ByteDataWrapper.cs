namespace SocketFlow.DataWrappers
{
    public class ByteDataWrapper : IDataWrapper<byte[]>
    {
        public byte[] FormatRaw(byte[] data)
        {
            return data;
        }

        public byte[] FormatObject(byte[] value)
        {
            return value;
        }
    }
}
