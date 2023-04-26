using System.Net;
using System.Threading;
using System.Threading.Tasks;
using SocketFlow.Tcp;

namespace SocketFlow.Client;

public class FlowClient<TKey> : DestinationClientBase<TKey>
{
    private readonly IPAddress address;
    private readonly int port;
    private EndpointProcessor<TKey, DestinationClientBase<TKey>> processor;
    private bool shutdown;

    public FlowClient(IFlowProtocol<TKey, IProtocolContext> protocol, IFlowBinder<DestinationClientBase<TKey>, TKey> binder, IPAddress address, int port)
    {
        Protocol = protocol;
        Binder = binder;
        this.address = address;
        this.port = port;
    }

    public async Task ConnectAsync(CancellationToken token)
    {
        Endpoint = new TcpEndpoint(new IPEndPoint(address, port));
        await Endpoint.StartAsync(token);
        processor = new EndpointProcessor<TKey, DestinationClientBase<TKey>>(this, Binder, Protocol, SelfOnComplete);
    }

    public Task DisconnectAsync()
    {
        shutdown = true;
        return OnComplete(processor);
    }

    private static Task OnComplete(EndpointProcessor<TKey, DestinationClientBase<TKey>> processor)
    {
        processor.CancellationToken.Cancel();
        return processor.WhenObserver();
    }

    private Task SelfOnComplete(EndpointProcessor<TKey, DestinationClientBase<TKey>> c)
    {
        if (shutdown)
            return Task.CompletedTask;
        return OnComplete(c);
    }
}
