namespace SocketFlow.DataWrappers
{
    public interface IDataWrapper<out T>
    {
        T FormatRaw(byte[] data);
        byte[] FormatObject(object value);
    }
}
