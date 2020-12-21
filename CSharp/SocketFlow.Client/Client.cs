using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using SocketFlow.DataWrappers;

namespace SocketFlow.Client
{
    public class Client
    {
        private readonly IPAddress address;
        private readonly TcpClient clientSocket;
        private readonly Dictionary<int, WrapperInfo> dataWrappers;
        private readonly Dictionary<Type, WrapperInfo> wrapperTypes;
        private readonly Dictionary<int, MethodInfo> handlers;
        private readonly int port;
        private TcpProtocol protocol;
        private Thread thread;

        public event Action<Client> Disconnected;
        public event Action<Client> Connected;

        public Client(IPAddress address, int port)
        {
            this.address = address;
            this.port = port;
            dataWrappers = new Dictionary<int, WrapperInfo>();
            wrapperTypes = new Dictionary<Type, WrapperInfo>();
            handlers = new Dictionary<int, MethodInfo>();
            clientSocket = new TcpClient();
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

        public Client Using<T>(IDataWrapper<T> wrapper)
        {
            if (wrapperTypes.ContainsKey(typeof(T)))
                throw new Exception("Already registered");
            var type = typeof(T);
            var wrapperInfo = new WrapperInfo(type, (IDataWrapper<object>)wrapper);
            wrapperTypes.Add(type, wrapperInfo);
            return this;
        }

        public void Bind<T>(int scId, Action<T> handler)
        {
            if (!wrapperTypes.ContainsKey(typeof(T)))
                throw new Exception($"WrapperInfo for '{typeof(T)}' doesn't registered. Use 'Using<T>(IDataWrapper) for register");
            dataWrappers.Add(scId, wrapperTypes[typeof(T)]);
            handlers.Add(scId, handler.GetMethodInfo());
        }

        public void Send<T>(int type, T value)
        {
            protocol.Send(type, wrapperTypes[typeof(T)].DataWrapper.FormatObject(value));
        }

        private void Protocol_OnData(int scId, byte[] data)
        {
            if (!handlers.TryGetValue(scId, out var handler))
                throw new Exception($"The server send event with {scId} id, but client can't handle it");
            handler.Invoke(this, new[]
            {
                dataWrappers[scId].DataWrapper.FormatRaw(data)
            });
        }

        private void Protocol_OnClose()
        {
            Disconnect();
        }
    }
}
