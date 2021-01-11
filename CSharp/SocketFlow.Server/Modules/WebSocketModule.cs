using System;
using System.Net;
using SocketFlow.Server.Protocols;

namespace SocketFlow.Server.Modules
{
    public class WebSocketModule : IModule
    {
        private readonly string host;
        private FlowServer owner;
        private HttpListener listener;
        private bool working = false;

        public WebSocketModule(string host)
        {
            this.host = $"http://{host}/";
        }

        public void Initialize(FlowServer server)
        {
            if (owner != null)
                throw new Exception("Module already initialized. Maybe you called the 'Initialize(FlowServer)' method yourself?");
            owner = server;
        }

        public void Start()
        {
            listener = new HttpListener();
            listener.Prefixes.Add(host);
            listener.Start();
            working = true;
            AcceptHandler();
        }

        public void Stop()
        {
            working = false;
            listener.Close();
        }

        private async void AcceptHandler()
        {
            while (working)
            {
                var context = await listener.GetContextAsync();
                if (!context.Request.IsWebSocketRequest)
                {
                    context.Response.Close();
                    continue;
                }
                var socket = await context.AcceptWebSocketAsync(null);
                owner.ConnectMe(new DestinationClient(new WebSocketProtocol(socket.WebSocket), owner, context.Request.RemoteEndPoint));
            }
        }
    }
}
