using System.Net;
using System.Net.WebSockets;

namespace SocketFlow.AspNetCore
{
    public class WebSocketInfo
    {
        public WebSocket Socket { get; }
        public IPAddress Address { get; }

        public WebSocketInfo(WebSocket socket, IPAddress address)
        {
            Socket = socket;
            Address = address;
        }
    }
}
