using System;
using System.Collections.Generic;
using SocketFlow.DataWrappers;
using SocketFlow.Server.Modules;

namespace SocketFlow.Server
{
    public class FlowServer
    {
        private readonly FlowBinder flowBinder;
        private readonly FlowOptions options;
        private readonly LinkedList<IModule> modules = new LinkedList<IModule>();
        private readonly FlowGroup serverClients;
        
        public FlowServer(FlowOptions options = null)
        {
            this.options = options ?? new FlowOptions();
            flowBinder = new FlowBinder(this.options);
            serverClients = new FlowGroup(this);
        }

        public int ClientsCount => serverClients.Count;

        public event Action<DestinationClient> ClientConnected;
        public event Action<DestinationClient> ClientDisconnected;

        public void Bind<T>(int clientServerId, Action<DestinationClient, T> handler)
        {
            if (clientServerId < 0)
                throw new Exception("Negative ids are reserved for SocketFlow");
            flowBinder.Bind<T>(clientServerId, handler);
        }

        public FlowServer UsingModule(IModule module)
        {
            module.Initialize(this);
            modules.AddLast(module);
            return this;
        }

        public FlowServer UsingWrapper<T>(IDataWrapper<T> wrapper)
        {
            flowBinder.Using(wrapper);
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

        public void Broadcast<T>(int serverClientId, T value)
        {
            serverClients.Send(serverClientId, value);
        }

        public bool ContainsClient(DestinationClient client)
        {
            return serverClients.Contains(client);
        }

        internal void DisconnectMe(DestinationClient client)
        {
            serverClients.Remove(client);
            ClientDisconnected?.Invoke(client);
        }

        internal void ConnectMe(DestinationClient client)
        {
            serverClients.UnsafeAdd(client);
            ClientConnected?.Invoke(client);
        }

        internal byte[] GetData<T>(T value)
        {
            var wrapper = flowBinder.GetWrapper<T>();
            return wrapper.DataWrapper.FormatObject(value);
        }

        internal void ReceivedData(DestinationClient client, int clientServerId, byte[] data)
        {
            var wrapper = flowBinder.GetWrapper(clientServerId);
            var handler = flowBinder.GetHandler(clientServerId);
            handler.Method.Invoke(handler.Target, new[]
            {
                client,
                wrapper.DataWrapper.FormatRaw(data)
            });
        }
    }
}
