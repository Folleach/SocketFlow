using System.Net;
using System.Threading;

namespace SocketFlow.Server
{
    public class DestinationClient<T>
    {
        private readonly Thread readerThread;
        private readonly IProtocol protocol;
        private readonly SocketFlowServer<T> server;

        public DestinationClient(IProtocol protocol, SocketFlowServer<T> server, EndPoint endPoint)
        {
            this.protocol = protocol;
            this.server = server;
            RemoteEndPoint = endPoint;

            protocol.OnClose += Protocol_OnClose;
            protocol.OnData += Protocol_OnData;

            readerThread = new Thread(protocol.Reader) {IsBackground = true};
            readerThread.Start();
        }

        public EndPoint RemoteEndPoint { get; }

        public void Disconnect()
        {
            server.DisconnectMe(this);
        }

        public void Send(int type, T value)
        {
            protocol.Send(type, server.DataWrapper.FormatObject(value));
        }

        private void Protocol_OnData(int type, byte[] data)
        {
            server.ReceivedData(this, type, data);
        }

        private void Protocol_OnClose()
        {
            Disconnect();
        }
    }
}
