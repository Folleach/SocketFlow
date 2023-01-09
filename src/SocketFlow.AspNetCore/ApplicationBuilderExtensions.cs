using System;
using Microsoft.AspNetCore.Builder;
using SocketFlow.Server;

namespace SocketFlow.AspNetCore
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSocketFlow(this IApplicationBuilder builder, string route, Action<FlowServer> configure)
        {
            return builder.UseMiddleware<WebSocketMiddleware>(new SocketFlowMiddlewareOptions(configure));
        }
    }
}
