namespace SocketFlow.Server.Modules
{
    public interface IModule
    {
        void Initialize(FlowServer server);
        void Start();
        void Stop();
    }
}
