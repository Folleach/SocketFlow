using System;
using System.Collections.Generic;
using System.Reflection;
using SocketFlow.DataWrappers;
using SocketFlow.Server.Modules;

namespace SocketFlow.Server
{
    public class SocketFlowServer
    {
        internal readonly Dictionary<int, WrapperInfo> DataWrappers;
        internal readonly Dictionary<Type, WrapperInfo> WrapperTypes;
        private readonly Dictionary<int, MethodInfo> handlers;
        private readonly LinkedList<IModule> modules = new LinkedList<IModule>();

        public SocketFlowServer()
        {
            DataWrappers = new Dictionary<int, WrapperInfo>();
            WrapperTypes = new Dictionary<Type, WrapperInfo>();
            handlers = new Dictionary<int, MethodInfo>();
        }

        public event Action<DestinationClient> ClientConnected;
        public event Action<DestinationClient> ClientDisconnected;

        public void Bind<T>(int csId, Action<DestinationClient, T> handler)
        {
            if (csId < 0)
                throw new Exception("Negative ids are reserved for SocketFlow");
            if (!WrapperTypes.ContainsKey(typeof(T)))
                throw new Exception($"WrapperInfo for {typeof(T)} doesn't registered. Use 'Using<T>(IDataWrapper) for register");
            DataWrappers.Add(csId, WrapperTypes[typeof(T)]);
            handlers.Add(csId, handler.GetMethodInfo());
        }

        public SocketFlowServer Using(IModule module)
        {
            module.Initialize(this);
            modules.AddLast(module);
            return this;
        }

        public SocketFlowServer Using<T>(IDataWrapper<T> wrapper)
        {
            if (WrapperTypes.ContainsKey(typeof(T)))
                throw new Exception("Already registered");
            var type = typeof(T);
            var wrapperInfo = new WrapperInfo(type, (IDataWrapper<object>)wrapper);
            WrapperTypes.Add(type, wrapperInfo);
            return this;
        }

        public SocketFlowServer Start()
        {
            foreach (var module in modules)
                module.Start();
            return this;
        }

        public SocketFlowServer Stop()
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

        internal void ReceivedData(DestinationClient client, int csId, byte[] data)
        {
            var wrapper = DataWrappers[csId];
            var handler = handlers[csId];
            handler.Invoke(this, new[]
            {
                client,
                wrapper.DataWrapper.FormatRaw(data)
            });
        }
    }
}
