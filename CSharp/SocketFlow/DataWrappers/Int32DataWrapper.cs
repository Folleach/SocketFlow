using System;

namespace SocketFlow.DataWrappers
{
    public class Int32DataWrapper : IDataWrapper<int>
    {
        public int FormatRaw(byte[] data)
        {
            return BitConverter.ToInt32(data, 0);
        }

        public byte[] FormatObject(object value)
        {
            return BitConverter.GetBytes((int)value);
        }
    }
}
