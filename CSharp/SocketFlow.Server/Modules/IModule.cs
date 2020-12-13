namespace SocketFlow.Server.Modules
{
    public interface IModule<T>
    {
        void Initialize(SocketFlowServer<T> server);
        void Finalize(SocketFlowServer<T> server);
    }
}
