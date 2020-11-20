using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SocketFlow.Server
{
    public class DestinationClient<T>
    {
        private readonly SimpleProtocol protocol;
        private readonly Thread readerThread;
        private readonly SocketFlowServer<T> server;
        private readonly TcpClient socket;

        public DestinationClient(SocketFlowServer<T> server, TcpClient socket)
        {
            this.server = server;
            this.socket = socket;

            protocol = new SimpleProtocol(socket);
            protocol.OnClose += Protocol_OnClose;
            protocol.OnData += Protocol_OnData;

            readerThread = new Thread(protocol.Reader) {IsBackground = true};
            readerThread.Start();
        }

        public EndPoint RemoteEndPoint => socket.Client.RemoteEndPoint;

        public void Disconnect()
        {
            server.DisconnectMe(this);
        }

        public void Send(int type, T value)
        {
            protocol.Send(type, server.DataWrapper.FormatObject(value));
        }

        public override string ToString()
        {
            return socket.Client.RemoteEndPoint.ToString();
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
