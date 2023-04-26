using System;
using System.Threading;
using System.Threading.Tasks;

namespace SocketFlow
{
    public interface IBaseEndpoint : IObservable<IEndpoint>
    {
        Task Start(CancellationToken token);
    }
}
