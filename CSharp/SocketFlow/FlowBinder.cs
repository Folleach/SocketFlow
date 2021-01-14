using System;
using System.Collections.Generic;
using SocketFlow.DataWrappers;

namespace SocketFlow
{
    public class FlowBinder
    {
        private readonly FlowOptions options;
        private readonly Dictionary<int, WrapperInfo> dataWrappers;
        private readonly Dictionary<Type, WrapperInfo> wrapperTypes;
        private readonly Dictionary<int, HandlerInfo> handlers;

        public FlowBinder(FlowOptions options)
        {
            this.options = options;
            dataWrappers = new Dictionary<int, WrapperInfo>();
            wrapperTypes = new Dictionary<Type, WrapperInfo>();
            handlers = new Dictionary<int, HandlerInfo>();
        }

        public void Using<T>(IDataWrapper<T> wrapper)
        {
            if (wrapperTypes.ContainsKey(typeof(T)))
                throw new Exception("Already registered");
            var type = typeof(T);
            var wrapperInfo = new WrapperInfo(type, (IDataWrapper<object>)wrapper);
            wrapperTypes.Add(type, wrapperInfo);
        }

        public void Bind<T>(int id, Delegate handler)
        {
            var type = typeof(T);
            if (!wrapperTypes.ContainsKey(type))
            {
                if (options.DefaultNonPrimitivesObjectUsingAsJson && !type.IsPrimitive)
                    Using(new JsonDataWrapper<T>());
                else
                    throw new Exception($"WrapperInfo for {type} doesn't registered. Use 'UsingType<T>(IDataWrapper) for register");
            }
            dataWrappers[id] = wrapperTypes[type];
            handlers[id] = new HandlerInfo(handler.Method, handler.Target);
        }

        public WrapperInfo GetWrapper<T>()
        {
            var type = typeof(T);
            if (wrapperTypes.TryGetValue(type, out var wrapper))
                return wrapper;
            if (!options.DefaultNonPrimitivesObjectUsingAsJson || type.IsPrimitive)
                throw new Exception($"WrapperInfo for {type} doesn't registered. Use 'UsingType<T>(IDataWrapper) for register");
            Using(new JsonDataWrapper<T>());
            return wrapperTypes[type];
        }

        public WrapperInfo GetWrapper(int id)
        {
            if (dataWrappers.TryGetValue(id, out var wrapper))
                return wrapper;
            throw new Exception($"There is no handler for {id} id");
        }

        public HandlerInfo GetHandler(int id)
        {
            if (handlers.TryGetValue(id, out var handler))
                return handler;
            throw new Exception($"There is no handler for {id} id");
        }
    }
}
