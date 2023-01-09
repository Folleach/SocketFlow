using System;
using System.Net;

namespace SocketFlow.Server
{
    public class DestinationClient
    {
        private readonly IProtocol protocol;
        private readonly FlowServer server;

        public event Action<Exception> OnError;

        public DestinationClient(IProtocol protocol, FlowServer server, EndPoint endPoint)
        {
            this.protocol = protocol;
            this.server = server;
            RemoteEndPoint = endPoint;
        }

        public EndPoint RemoteEndPoint { get; }

        public void Disconnect()
        {
            server.Clients.Disconnect(this);
            protocol.Dispose();
        }

        public void Send<T>(int serverClientId, T value)
        {
            if (serverClientId < 0)
                throw new Exception("Negative ids are reserved for SocketFlow");
            Send(serverClientId, server.GetData(value));
        }

        internal void Run()
        {
            protocol.OnClose += Protocol_OnClose;
            protocol.OnError += Protocol_OnError;
            protocol.OnData += Protocol_OnData;
            protocol.StartListening();
        }

        internal void Send(int serverClientId, byte[] data)
        {
            protocol.Send(serverClientId, data);
        }

        private void Protocol_OnData(int type, byte[] data)
        {
            server.ReceivedData(this, type, data);
        }

        private void Protocol_OnError(Exception exception)
        {
            OnError?.Invoke(exception);
        }

        private void Protocol_OnClose()
        {
            Disconnect();
        }
    }
}
