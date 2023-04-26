using System;
using System.Collections.Generic;
using System.Linq;
using SocketFlow.DataWrappers;

namespace SocketFlow.Server;

public class FlowServerBuilder<TKey, TClient> where TClient : DestinationClientBase<TKey>
{
    private List<ProtocolStackConfigurator<TKey, FlowServerBuilder<TKey, TClient>>> protocolStacks = new();

    private readonly List<IClientHandler<TClient>> onConnectedHandlers = new();
    private readonly List<IClientHandler<TClient>> onDisconnectedHandlers = new();
    private readonly IFlowBinder<TClient, TKey> binder = new AltFlowBinder<TClient, TKey>();

    public FlowServerBuilder<TKey, TClient> ConfigureProtocolStack(
        Action<ProtocolStackConfigurator<TKey, FlowServerBuilder<TKey, TClient>>> configure)
    {
        var stackConfigurator = new ProtocolStackConfigurator<TKey, FlowServerBuilder<TKey, TClient>>(this);
        configure(stackConfigurator);
        protocolStacks.Add(stackConfigurator);
        return this;
    }

    public FlowServerBuilder<TKey, TClient> AddOnConnectedHandler(IClientHandler<TClient> handler)
    {
        onConnectedHandlers.Add(handler);
        return this;
    }

    public FlowServerBuilder<TKey, TClient> AddOnDisconnectHandler(IClientHandler<TClient> handler)
    {
        onDisconnectedHandlers.Add(handler);
        return this;
    }

    public FlowServerBuilder<TKey, TClient> Map<TValue>(TKey key, IPacketHandler<TClient, TKey, TValue> handler)
    {
        binder.Bind(key, handler);
        return this;
    }

    public FlowServerBuilder<TKey, TClient> UseWrapper<T>(IDataWrapper<T> wrapper)
    {
        binder.UseWrapper(wrapper);
        return this;
    }

    public FlowServer<TKey, TClient> Build()
    {
        var processors = new List<EndpointBaseProcessor<TKey, TClient>>();
        foreach (var stack in protocolStacks)
        {
            var processor = new EndpointBaseProcessor<TKey, TClient>(
                stack.Middlewares.ToArray(),
                binder,
                stack.Protocol,
                onConnectedHandlers.ToArray(),
                onDisconnectedHandlers.ToArray());
            stack.BaseEndpoint.Subscribe(processor);
            processors.Add(processor);
        }
        var flowProcessor = new FlowProtocolProcessor<TKey, TClient>(processors.ToArray());
        return new FlowServer<TKey, TClient>(protocolStacks.Select(x => x.BaseEndpoint).ToArray(), flowProcessor);
    }
}
