using System;
using System.Net;
using System.Threading;

namespace SocketFlow.Server
{
    public class DestinationClient
    {
        private readonly Thread readerThread;
        private readonly IProtocol protocol;
        private readonly FlowServer server;

        public DestinationClient(IProtocol protocol, FlowServer server, EndPoint endPoint)
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

        public void Send<T>(int serverClientId, T value)
        {
            if (serverClientId < 0)
                throw new Exception("Negative ids are reserved for SocketFlow");
            Send(serverClientId, server.GetData(value));
        }

        internal void Send(int serverClientId, byte[] data)
        {
            protocol.Send(serverClientId, data);
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
