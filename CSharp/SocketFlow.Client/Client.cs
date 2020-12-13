using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using SocketFlow.DataWrappers;

namespace SocketFlow.Client
{
    public class Client<T>
    {
        private readonly IPAddress address;
        private readonly TcpClient clientSocket;
        private readonly IDataWrapper<T> dataWrapper;
        private readonly Dictionary<int, Action<T>> handlers;
        private readonly int port;
        private TcpProtocol protocol;
        private Thread thread;

        public event Action<Client<T>> Disconnected;
        public event Action<Client<T>> Connected;

        public Client(IPAddress address, int port, IDataWrapper<T> dataWrapper)
        {
            this.address = address;
            this.port = port;
            this.dataWrapper = dataWrapper;
            handlers = new Dictionary<int, Action<T>>();
            clientSocket = new TcpClient();
        }

        private void Protocol_OnData(int type, byte[] data)
        {
            handlers[type].Invoke(dataWrapper.FormatRaw(data));
        }

        private void Protocol_OnClose()
        {
            Disconnect();
        }

        public void Connect()
        {
            clientSocket.Connect(address, port);

            protocol = new TcpProtocol(clientSocket);

            protocol.OnClose += Protocol_OnClose;
            protocol.OnData += Protocol_OnData;

            thread = new Thread(protocol.Reader) { IsBackground = true };
            thread.Start();
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

        public void Bind(int type, Action<T> handler)
        {
            handlers.Add(type, handler);
        }

        public void Send(int type, T value)
        {
            protocol.Send(type, dataWrapper.FormatObject(value));
        }
    }
}
