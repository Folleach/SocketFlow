using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using SocketFlow.DataWrappers;

namespace SocketFlow;

public class AltFlowBinder<TClient, TKey> : IFlowBinder<TClient, TKey>
{
    private readonly Dictionary<TKey, WrapperInfo> dataWrappers = new();
    private readonly Dictionary<TKey, HandlerInfo> handlers = new();
    private readonly ConcurrentDictionary<Type, WrapperInfo> wrapperTypes = new();

    public IFlowBinder<TClient, TKey> Bind<T>(TKey key, IPacketHandler<TClient, TKey, T> handler)
    {
        var type = typeof(T);
        if (!wrapperTypes.ContainsKey(type))
        {
            if (!type.IsPrimitive)
                UseWrapper(new JsonDataWrapper<T>());
            else
                throw new Exception($"WrapperInfo for {type} doesn't registered. Use 'UsingType<T>(IDataWrapper) for register");
        }
        dataWrappers[key] = wrapperTypes[type];
        var method = handler.GetType().GetMethod(nameof(handler.Handle), BindingFlags.Public | BindingFlags.Instance);
        handlers[key] = new HandlerInfo(method, handler);
        return this;
    }

    public IFlowBinder<TClient, TKey> UseWrapper<T>(IDataWrapper<T> wrapper)
    {
        if (wrapperTypes.ContainsKey(typeof(T)))
            return this;
        var type = typeof(T);
        if (type.IsValueType)
            throw new Exception("Value types are unsupported. Use classes and wrap base types. https://github.com/Folleach/SocketFlow/issues/4");
        var wrapperInfo = new WrapperInfo(type, (IDataWrapper<object>)wrapper);
        wrapperTypes.TryAdd(type, wrapperInfo);
        return this;
    }

    public IDataWrapper<T> GetDataWrapper<T>()
    {
        if (!wrapperTypes.TryGetValue(typeof(T), out var info))
        {
            var wrapper = new JsonDataWrapper<T>();
            UseWrapper(wrapper);
            return wrapper;
            throw new InvalidOperationException($"There is no wrapper for type '{typeof(T).FullName}'");
        }
        return (IDataWrapper<T>)info.DataWrapper;
    }

    public bool TryGetHandlerInfo(TKey key, out HandlerInfo info) => handlers.TryGetValue(key, out info);

    public bool TryGetWrapperInfo(TKey key, out WrapperInfo info) => dataWrappers.TryGetValue(key, out info);
}
