namespace SocketFlow
{
    public interface IProtocol
    {
        void Reader();
        void Send(int type, byte[] data);
    }
}
