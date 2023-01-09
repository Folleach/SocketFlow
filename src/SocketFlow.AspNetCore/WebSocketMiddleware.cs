using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SocketFlow.Server;

namespace SocketFlow.AspNetCore
{
    public class WebSocketMiddleware
    {
        private readonly RequestDelegate next;
        private readonly SocketFlowMiddlewareOptions options;
        private readonly FlowServer server;
        private readonly EventPipe<WebSocketInfo> pipe = new();

        public WebSocketMiddleware(RequestDelegate next, SocketFlowMiddlewareOptions options, ILogger<WebSocketMiddleware> logger)
        {
            this.next = next;
            this.options = options;
            logger.LogInformation("Configuring SocketFlow");
            server = new FlowServer(FlowOptions.Lazy)
                .UseWebSocketModule(pipe);
            options.ConfigureServer(server);
            logger.LogInformation("SocketFlow configured");
            server.Start();
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await next(context);
                return;
            }

            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            pipe.OnNext(new WebSocketInfo(webSocket, context.Connection.RemoteIpAddress));
            context.RequestAborted.WaitHandle.WaitOne();
        }
    }
}
