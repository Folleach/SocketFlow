using System;
using System.Net;
using System.Threading;

namespace SocketFlow.Server
{
    public class DestinationClient
    {
        private readonly Thread readerThread;
        private readonly IProtocol protocol;
        private readonly SocketFlowServer server;

        public DestinationClient(IProtocol protocol, SocketFlowServer server, EndPoint endPoint)
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

        public void Send<T>(int scId, T value)
        {
            var wrapper = server.DataWrappers[scId];
            if (wrapper.Type != typeof(T))
                throw new Exception($"The handler for ${scId} server-client event id is ${wrapper.Type} but you tried to use ${typeof(T)}");
            protocol.Send(scId, wrapper.DataWrapper.FormatObject(value));
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
