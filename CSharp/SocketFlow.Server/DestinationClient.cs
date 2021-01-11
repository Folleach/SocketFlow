using System;
using System.Net;
using System.Threading;
using SocketFlow.DataWrappers;

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

        public void Send<T>(int scId, T value)
        {
            if (scId < 0)
                throw new Exception("Negative ids are reserved for SocketFlow");
            var type = typeof(T);
            if (!server.WrapperTypes.TryGetValue(type, out var wrapper))
            {
                if (server.Options.DefaultNonPrimitivesObjectUsingAsJson && !type.IsPrimitive)
                {
                    server.Using(new JsonDataWrapper<T>());
                    wrapper = server.WrapperTypes[type];
                }
                else
                    throw new Exception($"WrapperInfo for {type} doesn't registered. Use 'Using<T>(IDataWrapper) for register");
            }
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
