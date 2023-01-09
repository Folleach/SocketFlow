using System;
using System.Net;
using SocketFlow.Server.Modules;

namespace SocketFlow.Server
{
    public static class FlowServerExtensions
    {
        public static FlowServer UseTcpModule(this FlowServer server, IPAddress address, int port)
        {
            return server.UseModule(new TcpModule(server, address, port));
        }

        public static FlowServer OnConnected(this FlowServer server, Action<DestinationClient> on, out IDisposable subscription)
        {
            subscription = server.OnConnected(new AnonymousObserver<DestinationClient>(on));
            return server;
        }

        public static FlowServer OnDisconnected(this FlowServer server, Action<DestinationClient> on, out IDisposable subscription)
        {
            subscription = server.OnDisconnected(new AnonymousObserver<DestinationClient>(on));
            return server;
        }

        public static FlowServer OnConnected(this FlowServer server, Action<DestinationClient> on) => server.OnConnected(on, out _);

        public static FlowServer OnDisconnected(this FlowServer server, Action<DestinationClient> on) => server.OnDisconnected(on, out _);
    }
}
