namespace SocketFlow.Server
{
    public interface IInOut
    {
        void Connect(DestinationClient client);
        void Disconnect(DestinationClient client);
    }
}
