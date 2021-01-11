using System;
using System.Collections.Generic;
using SocketFlow.DataWrappers;
using SocketFlow.Server.Modules;

namespace SocketFlow.Server
{
    public class FlowServer
    {
        internal readonly FlowBinder FlowBinder;
        internal readonly FlowOptions Options;
        private readonly LinkedList<IModule> modules = new LinkedList<IModule>();

        public FlowServer(FlowOptions options = null)
        {
            Options = options ?? new FlowOptions();
            FlowBinder = new FlowBinder(Options);
        }

        public event Action<DestinationClient> ClientConnected;
        public event Action<DestinationClient> ClientDisconnected;

        public void Bind<T>(int clientServerId, Action<DestinationClient, T> handler)
        {
            if (clientServerId < 0)
                throw new Exception("Negative ids are reserved for SocketFlow");
            FlowBinder.Bind<T>(clientServerId, handler);
        }

        public FlowServer UsingModule(IModule module)
        {
            module.Initialize(this);
            modules.AddLast(module);
            return this;
        }

        public FlowServer UsingWrapper<T>(IDataWrapper<T> wrapper)
        {
            FlowBinder.Using(wrapper);
            return this;
        }

        public FlowServer Start()
        {
            foreach (var module in modules)
                module.Start();
            return this;
        }

        public FlowServer Stop()
        {
            foreach (var module in modules)
                module.Stop();
            return this;
        }

        internal void DisconnectMe(DestinationClient client)
        {
            ClientDisconnected?.Invoke(client);
        }

        internal void ConnectMe(DestinationClient client)
        {
            ClientConnected?.Invoke(client);
        }

        internal void ReceivedData(DestinationClient client, int clientServerId, byte[] data)
        {
            var wrapper = FlowBinder.GetWrapper(clientServerId);
            var handler = FlowBinder.GetHandler(clientServerId);
            handler.Method.Invoke(handler.Target, new[]
            {
                client,
                wrapper.DataWrapper.FormatRaw(data)
            });
        }
    }
}
