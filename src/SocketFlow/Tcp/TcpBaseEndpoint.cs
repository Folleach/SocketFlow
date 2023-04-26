using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SocketFlow.Annotations;

namespace SocketFlow.Tcp;

public class TcpBaseEndpoint : IBaseEndpoint, IStreamLike
{
    private readonly IPAddress address;
    private readonly int port;
    private readonly ConcurrentDictionary<Guid, IObserver<IEndpoint>> observers = new();

    public TcpBaseEndpoint(IPAddress address, int port)
    {
        this.address = address;
        this.port = port;
    }

    public async Task Start(CancellationToken token)
    {
        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(new IPEndPoint(address, port));
        socket.Listen(20);
        // todo: do not snapshot observers, because FlowServer can call Subscribe
        var currentObservers = observers.Values.ToArray();
        while (!token.IsCancellationRequested)
        {
            try
            {
                await socket.AcceptAsync().ContinueWith(client =>
                {
                    if (!client.IsCompleted)
                        return;
                    var endpoint = new TcpEndpoint(client.Result);
                    foreach (var observer in currentObservers)
                        observer.OnNext(endpoint);
                }, token);
            }
            catch (OperationCanceledException)
            {
                // ignore
            }
        }

        foreach (var observer in currentObservers)
            observer.OnCompleted();

        // todo: gracefully close bounded socket
    }

    public IDisposable Subscribe(IObserver<IEndpoint> observer)
    {
        Guid id;
        do
        {
            id = Guid.NewGuid();
        } while (!observers.TryAdd(id, observer));

        return new IdSubscription(key => observers.TryRemove(key, out _), id);
    }
}
