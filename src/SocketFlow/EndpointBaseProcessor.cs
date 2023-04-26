using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocketFlow;

/// <summary>
/// Handle IEndpoint's<br/>
/// such as connect/disconnect
///
/// May be multiple in one server. Handle one IBaseEndpoint
/// </summary>
public class EndpointBaseProcessor<TKey, TClient> : IObserver<IEndpoint> where TClient : DestinationClientBase<TKey>
{
    public int Count => processors.Count;

    private readonly Func<IEndpoint, IProtocolMiddleware>[] middlewareFactories;
    private readonly IFlowBinder<TClient, TKey> binder;
    private readonly IFlowProtocol<TKey, IProtocolContext> flowProtocol;
    private readonly IClientHandler<TClient>[] onConnectedHandlers;
    private readonly IClientHandler<TClient>[] onDisconnectedHandlers;
    private readonly Func<IEndpoint, TClient> clientFactory;
    private readonly HashSet<EndpointProcessor<TKey, TClient>> processors = new();
    private bool shutdown;

    public EndpointBaseProcessor(
        Func<IEndpoint, IProtocolMiddleware>[] middlewareFactories,
        IFlowBinder<TClient, TKey> binder,
        IFlowProtocol<TKey, IProtocolContext> flowProtocol,
        IClientHandler<TClient>[] onConnectedHandlers,
        IClientHandler<TClient>[] onDisconnectedHandlers)
    {
        this.middlewareFactories = middlewareFactories;
        this.binder = binder;
        this.flowProtocol = flowProtocol;
        this.onConnectedHandlers = onConnectedHandlers;
        this.onDisconnectedHandlers = onDisconnectedHandlers;
        clientFactory = CreateFactory; // todo: to cached reflection
    }

    private TClient CreateFactory(IEndpoint endpoint)
    {
        var instance = (TClient)Activator.CreateInstance(typeof(TClient));
        instance.GetType().GetProperty(nameof(instance.Protocol))?.SetValue(instance, flowProtocol);
        instance.GetType().GetProperty(nameof(instance.Binder))?.SetValue(instance, binder);
        instance.GetType().GetProperty(nameof(instance.Endpoint))?.SetValue(instance, endpoint);
        return instance;
    }

    private async Task CompleteEndpoint(EndpointProcessor<TKey, TClient> processor)
    {
        foreach (var handler in onDisconnectedHandlers)
            await handler.Handle(processor.Client);
        processor.CancellationToken.Cancel();
        await processor.WhenObserver();
        processors.Remove(processor);
    }

    private Task SelfCompleteEndpoint(EndpointProcessor<TKey, TClient> processor)
    {
        if (shutdown)
            return Task.CompletedTask;
        return CompleteEndpoint(processor);
    }

    #region from base endpoint events

    public void OnCompleted()
    {
        shutdown = true;
        foreach (var processor in processors)
            CompleteEndpoint(processor).GetAwaiter().GetResult();
    }

    public void OnError(Exception error)
    {
        throw new NotImplementedException();
    }

    public void OnNext(IEndpoint endpoint)
    {
        var client = clientFactory(endpoint);
        var processor = new EndpointProcessor<TKey, TClient>(client, binder, flowProtocol, SelfCompleteEndpoint);
        processors.Add(processor);
        foreach (var handler in onConnectedHandlers)
            handler.Handle(client);
    }

    #endregion
}
