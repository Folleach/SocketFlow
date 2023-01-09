using System;
using System.Net.WebSockets;
using SocketFlow.Server;
using SocketFlow.Server.Modules;

namespace SocketFlow.AspNetCore
{
    public class WebSocketModule : IModule
    {
        private readonly FlowServer server;
        private readonly EventPipe<WebSocketInfo> pipe;
        private IDisposable subscription;

        public WebSocketModule(FlowServer server, EventPipe<WebSocketInfo> pipe)
        {
            this.server = server;
            this.pipe = pipe;
        }

        public void Start()
        {
            subscription = pipe.Subscribe(info =>
            {
                var client = info.Socket.ToFlowDestinationClient(server, info.Address);
                server.Clients.Connect(client);
            });
        }

        public void Stop()
        {
            subscription?.Dispose();
        }
    }
}
