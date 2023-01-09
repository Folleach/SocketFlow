using System;
using SocketFlow.Server;

namespace SocketFlow.AspNetCore
{
    public class SocketFlowMiddlewareOptions
    {
        public Action<FlowServer> ConfigureServer { get; }

        public SocketFlowMiddlewareOptions(Action<FlowServer> configureServer)
        {
            ConfigureServer = configureServer;
        }
    }
}
