using System;
using System.Net;

namespace SocketFlow.Server.Modules
{
    public class HttpListenerWebSocketModule : IModule
    {
        private readonly string host;
        private readonly FlowServer owner;
        private HttpListener listener;
        private bool working = false;

        public HttpListenerWebSocketModule(FlowServer server, string host)
        {
            owner = server ?? throw new ArgumentException("Can not be null", nameof(server));
            this.host = $"http://{host}/";
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
                owner.Clients.Connect(socket.WebSocket.ToFlowDestinationClient(owner, context.Request.RemoteEndPoint));
            }
        }
    }
}
