using System;
using System.Threading;
using System.Threading.Tasks;

namespace SocketFlow;

public class EndpointProcessor<TKey, TClient> : IObserver<HoldableBuffer> where TClient : DestinationClientBase<TKey>
{
    public TClient Client { get; }
    public readonly CancellationTokenSource CancellationToken;

    private readonly IFlowBinder<TClient, TKey> binder;
    private readonly IFlowProtocol<TKey, IProtocolContext> flowProtocol;
    private readonly Func<EndpointProcessor<TKey, TClient>, Task> onComplete;
    private readonly IProtocolContext context;
    private readonly Task observerTask;

    public EndpointProcessor(TClient client, IFlowBinder<TClient, TKey> binder, IFlowProtocol<TKey, IProtocolContext> flowProtocol, Func<EndpointProcessor<TKey, TClient>, Task> onComplete)
    {
        Client = client;
        this.binder = binder;
        this.flowProtocol = flowProtocol;
        this.onComplete = onComplete;
        Client.Endpoint.Subscribe(this);
        CancellationToken = new CancellationTokenSource();
        observerTask = Client.Endpoint.ObserveAsync(CancellationToken.Token);
        context = flowProtocol.CreateContext();
    }

    public async Task Execute(TKey key, TClient client, HoldableBuffer data)
    {
        if (!binder.TryGetHandlerInfo(key, out var handlerInfo))
            throw new InvalidOperationException($"There is no handler for {key} id");
        if (!binder.TryGetWrapperInfo(key, out var wrapperInfo))
            throw new InvalidOperationException($"There is no handler for {key} id");
        await (Task)handlerInfo.Method.Invoke(handlerInfo.Target, new[]
        {
            client,
            key,
            wrapperInfo.DataWrapper.FormatRaw(data.AsReadOnlyMemory())
        });
    }

    public async Task WhenObserver() => await Task.WhenAll(observerTask);

    #region from current endpoint events

    public void OnCompleted()
    {
        onComplete(this).GetAwaiter().GetResult();
    }

    public void OnError(Exception error)
    {
        Console.WriteLine(error);
    }

    public void OnNext(HoldableBuffer buffer)
    {
        var packets = flowProtocol.Receive(context, buffer);
        foreach (var packet in packets)
        {
            Execute(packet.Key, Client, packet.PacketData).ContinueWith(task =>
            {
                if (task.IsFaulted)
                    Console.WriteLine(task.Exception);
            });
        }
    }

    #endregion
}
