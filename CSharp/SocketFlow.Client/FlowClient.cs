using System;
using System.Net;
using System.Net.Sockets;
using SocketFlow.DataWrappers;

namespace SocketFlow.Client
{
    public class FlowClient
    {
        private readonly IPAddress address;
        private readonly TcpClient clientSocket;
        private readonly int port;
        private readonly FlowOptions options;
        private readonly FlowBinder flowBinder;
        private TcpProtocol protocol;

        public event Action<FlowClient> Disconnected;
        public event Action<FlowClient> Connected;

        public FlowClient(IPAddress address, int port, FlowOptions options = null)
        {
            this.address = address;
            this.port = port;
            this.options = options ?? new FlowOptions();
            flowBinder = new FlowBinder(this.options);
            clientSocket = new TcpClient();
        }

        public void Connect()
        {
            clientSocket.Connect(address, port);

            protocol = new TcpProtocol(clientSocket);

            protocol.OnClose += Protocol_OnClose;
            protocol.OnData += Protocol_OnData;

            protocol.Reader();
            Connected?.Invoke(this);
        }

        public void Disconnect()
        {
            if (!clientSocket.Connected)
                return;
            clientSocket.GetStream().Dispose();
            clientSocket.Close();
            clientSocket.Dispose();
            Disconnected?.Invoke(this);
        }

        public FlowClient UsingWrapper<T>(IDataWrapper<T> wrapper)
        {
            flowBinder.Using(wrapper);
            return this;
        }

        public void Bind<T>(int serverClientId, Action<T> handler)
        {
            if (serverClientId < 0)
                throw new Exception("Negative ids are reserved for SocketFlow");
            flowBinder.Bind<T>(serverClientId, handler);
        }

        public void Send<T>(int csId, T value)
        {
            if (csId < 0)
                throw new Exception("Negative ids are reserved for SocketFlow");
            var wrapper = flowBinder.GetWrapper<T>();
            protocol.Send(csId, wrapper.DataWrapper.FormatObject(value));
        }

        private void Protocol_OnData(int serverClientId, byte[] data)
        {
            var handler = flowBinder.GetHandler(serverClientId);
            handler.Method.Invoke(handler.Target, new[]
            {
                flowBinder.GetWrapper(serverClientId).DataWrapper.FormatRaw(data)
            });
        }

        private void Protocol_OnClose()
        {
            Disconnect();
        }
    }
}
