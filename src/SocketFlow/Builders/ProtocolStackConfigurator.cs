using System;
using System.Collections.Generic;

namespace SocketFlow;

public class ProtocolStackConfigurator<TKey, T>
{
    public T Configurator { get; }

    public List<Func<IEndpoint, IProtocolMiddleware>> Middlewares = new();
    public IFlowProtocol<TKey, IProtocolContext> Protocol;
    public IBaseEndpoint BaseEndpoint;

    public ProtocolStackConfigurator(T configurator)
    {
        Configurator = configurator;
    }

    public ProtocolStackConfigurator<TKey, T> BindToBase(IBaseEndpoint baseEndpoint)
    {
        BaseEndpoint = baseEndpoint;
        return this;
    }

    public T UseHighLevelProtocol<TProtoContext>(IFlowProtocol<TKey, TProtoContext> protocol) where TProtoContext : IProtocolContext
    {
        Protocol = new MiddlewareProtocol<TKey, TProtoContext>(protocol);
        return Configurator;
    }
}
