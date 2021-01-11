using System;
using System.Collections.Generic;
using SocketFlow.DataWrappers;
using SocketFlow.Server.Modules;

namespace SocketFlow.Server
{
    public class FlowServer
    {
        internal readonly Dictionary<int, WrapperInfo> DataWrappers;
        internal readonly Dictionary<Type, WrapperInfo> WrapperTypes;
        internal readonly FlowOptions Options;
        private readonly Dictionary<int, HandlerInfo> handlers;
        private readonly LinkedList<IModule> modules = new LinkedList<IModule>();

        public FlowServer(FlowOptions options = null)
        {
            Options = options ?? new FlowOptions();
            DataWrappers = new Dictionary<int, WrapperInfo>();
            WrapperTypes = new Dictionary<Type, WrapperInfo>();
            handlers = new Dictionary<int, HandlerInfo>();
        }

        public event Action<DestinationClient> ClientConnected;
        public event Action<DestinationClient> ClientDisconnected;

        public void Bind<T>(int csId, Action<DestinationClient, T> handler)
        {
            if (csId < 0)
                throw new Exception("Negative ids are reserved for SocketFlow");
            var type = typeof(T);
            if (!WrapperTypes.ContainsKey(type))
            {
                if (Options.DefaultNonPrimitivesObjectUsingAsJson && !type.IsPrimitive)
                    Using(new JsonDataWrapper<T>());
                else
                    throw new Exception($"WrapperInfo for {type} doesn't registered. Use 'Using<T>(IDataWrapper) for register");
            }
            DataWrappers.Add(csId, WrapperTypes[type]);
            handlers.Add(csId, new HandlerInfo(handler.Method, handler.Target));
        }

        public FlowServer Using(IModule module)
        {
            module.Initialize(this);
            modules.AddLast(module);
            return this;
        }

        public FlowServer Using<T>(IDataWrapper<T> wrapper)
        {
            if (WrapperTypes.ContainsKey(typeof(T)))
                throw new Exception("Already registered");
            var type = typeof(T);
            var wrapperInfo = new WrapperInfo(type, (IDataWrapper<object>)wrapper);
            WrapperTypes.Add(type, wrapperInfo);
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

        internal void ReceivedData(DestinationClient client, int csId, byte[] data)
        {
            var wrapper = DataWrappers[csId];
            var handler = handlers[csId];
            handler.Method.Invoke(handler.Target, new[]
            {
                client,
                wrapper.DataWrapper.FormatRaw(data)
            });
        }
    }
}
