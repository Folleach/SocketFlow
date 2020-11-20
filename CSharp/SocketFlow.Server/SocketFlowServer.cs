using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using SocketFlow.DataWrappers;

namespace SocketFlow.Server
{
    public class SocketFlowServer<T>
    {
        public readonly IDataWrapper<T> DataWrapper;
        private readonly TcpListener listener;
        private readonly Dictionary<int, Action<DestinationClient<T>, T>> handlers;
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

        public void Start(int backlog)
        {
            listener.Start(backlog);
            working = true;
            AcceptHandler();
        }

        public void Stop()
        {
            working = false;
            listener.Stop();
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
