using System;
using System.Collections.Generic;
using System.Net;
using SocketFlow.DataWrappers;
using SocketFlow.Server.Modules;

namespace SocketFlow.Server
{
    public class SocketFlowServer<T>
    {
        public readonly IDataWrapper<T> DataWrapper;
        private readonly Dictionary<int, Action<DestinationClient<T>, T>> handlers;
        private readonly LinkedList<IModule<T>> modules = new LinkedList<IModule<T>>();

        public SocketFlowServer(IDataWrapper<T> dataWrapper)
        {
            DataWrapper = dataWrapper;
            handlers = new Dictionary<int, Action<DestinationClient<T>, T>>();
        }

        public event Action<DestinationClient<T>> ClientConnected;
        public event Action<DestinationClient<T>> ClientDisconnected;

        public void Bind(int type, Action<DestinationClient<T>, T> handler)
        {
            handlers.Add(type, handler);
        }

        public SocketFlowServer<T> Using(IModule<T> module)
        {
            modules.AddLast(module);
            return this;
        }

        public SocketFlowServer<T> Start()
        {
            foreach (var module in modules)
                module.Initialize(this);
            return this;
        }

        public SocketFlowServer<T> Stop()
        {
            foreach (var module in modules)
                module.Finalize(this);
            return this;
        }

        internal void DisconnectMe(DestinationClient<T> client)
        {
            ClientDisconnected?.Invoke(client);
        }

        internal void ConnectMe(DestinationClient<T> client)
        {
            ClientConnected?.Invoke(client);
        }

        internal void ReceivedData(DestinationClient<T> client, int type, byte[] data)
        {
            var obj = DataWrapper.FormatRaw(data);
            handlers[type](client, obj);
        }
    }
}
