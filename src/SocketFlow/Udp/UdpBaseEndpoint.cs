using System;
using System.Threading;
using System.Threading.Tasks;
using SocketFlow.Annotations;

namespace SocketFlow.Udp;

public class UdpBaseEndpoint : IBaseEndpoint, IDatagramLike
{
    public IDisposable Subscribe(IObserver<IEndpoint> observer)
    {
        throw new NotImplementedException();
    }

    public Task Start(CancellationToken token)
    {
        throw new NotImplementedException();
    }
}
