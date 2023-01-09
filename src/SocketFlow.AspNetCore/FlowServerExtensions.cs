using System.Net.WebSockets;
using SocketFlow.Server;

namespace SocketFlow.AspNetCore
{
    public static class FlowServerExtensions
    {
        public static FlowServer UseWebSocketModule(this FlowServer server, EventPipe<WebSocketInfo> pipe)
        {
            return server.UseModule(new WebSocketModule(server, pipe));
        }
    }
}