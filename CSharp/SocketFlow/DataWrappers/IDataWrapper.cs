namespace SocketFlow.DataWrappers
{
    public interface IDataWrapper<T>
    {
        T FormatRaw(byte[] data);
        byte[] FormatObject(T value);
    }
}
