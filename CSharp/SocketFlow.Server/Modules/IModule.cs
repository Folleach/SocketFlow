namespace SocketFlow.Server.Modules
{
    public interface IModule
    {
        void Initialize(SocketFlowServer server);
        void Start();
        void Stop();
    }
}
