using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using SocketFlow.DataWrappers;
using SocketFlow.Server.Modules;

namespace SocketFlow.Server
{
    public class SocketFlowServer<T>
    {
        public readonly IDataWrapper<T> DataWrapper;
        private readonly TcpListener listener;
        private readonly Dictionary<int, Action<DestinationClient<T>, T>> handlers;
        private readonly LinkedList<IModule> Modules = new LinkedList<IModule>();
        private bool working = false;

        public SocketFlowServer(IPAddress address, int port, IDataWrapper<T> dataWrapper)
        {
            listener = new TcpListener(address, port);
            DataWrapper = dataWrapper;
            handlers = new Dictionary<int, Action<DestinationClient<T>, T>>();
        }

        public event Action<DestinationClient<T>> ClientConnected;
        public event Action<DestinationClient<T>> ClientDisconnected;

        public void Bind(int type, Action<DestinationClient<T>, T> handler)
        {
            handlers.Add(type, handler);
        }

        public SocketFlowServer<T> Using(IModule module)
        {
            module.Initialize(this);
            Modules.AddLast(module);
            return this;
        }

        public SocketFlowServer<T> Start(int backlog)
        {
            listener.Start(backlog);
            working = true;
            AcceptHandler();
            return this;
        }

        public SocketFlowServer<T> Stop()
        {
            working = false;
            listener.Stop();
            return this;
        }

        internal void DisconnectMe(DestinationClient<T> client)
        {
            ClientDisconnected?.Invoke(client);
        }

        internal void ReceivedData(DestinationClient<T> client, int type, byte[] data)
        {
            var obj = DataWrapper.FormatRaw(data);
            handlers[type](client, obj);
        }

        private async void AcceptHandler()
        {
            while (working)
            {
                var client = await listener.AcceptTcpClientAsync();
                var destClient = new DestinationClient<T>(this, client);
                ClientConnected?.Invoke(destClient);
            }
        }
    }
}
