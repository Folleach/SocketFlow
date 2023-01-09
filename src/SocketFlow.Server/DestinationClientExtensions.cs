using System.Net;
using System.Net.WebSockets;

namespace SocketFlow.Server
{
    public static class DestinationClientExtensions
    {
        public static DestinationClient ToFlowDestinationClient(this WebSocket socket, FlowServer server, IPAddress address)
        {
            return socket.ToFlowDestinationClient(server, new IPEndPoint(address, 0));
        }
        
        public static DestinationClient ToFlowDestinationClient(this WebSocket socket, FlowServer server, EndPoint endPoint)
        {
            return new DestinationClient(new Protocols.WebSocketProtocol(socket), server, endPoint);
        }
    }
}
