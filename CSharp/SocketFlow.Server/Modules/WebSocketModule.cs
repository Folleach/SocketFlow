using System.Net;
using SocketFlow.Server.Protocols;

namespace SocketFlow.Server.Modules
{
    public class WebSocketModule<T> : IModule<T>
    {
        private readonly string host;
        private HttpListener listener;
        private SocketFlowServer<T> currentServer;
        private bool working = false;

        public WebSocketModule(string host)
        {
            this.host = $"http://{host}/";
        }

        public void Initialize(SocketFlowServer<T> server)
        {
            currentServer = server;
            listener = new HttpListener();
            listener.Prefixes.Add(host);
            listener.Start();
            working = true;
            AcceptHandler();
        }

        public void Finalize(SocketFlowServer<T> server)
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
                currentServer.ConnectMe(new DestinationClient<T>(new WebSocketProtocol(socket.WebSocket), currentServer, context.Request.RemoteEndPoint));
            }
        }
    }
}
