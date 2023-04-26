using System;
using System.Threading;
using System.Threading.Tasks;

namespace SocketFlow
{
    // public class MiddlewareEndpoint : IEndpoint
    // {
    //     private readonly IEndpoint endpoint;
    //     private readonly IProtocolMiddleware[] middlewares;
    //
    //     public MiddlewareEndpoint(IEndpoint endpoint, IProtocolMiddleware[] middlewares)
    //     {
    //         this.endpoint = endpoint;
    //         this.middlewares = middlewares;
    //     }
    //     
    //     public IDisposable Subscribe(IObserver<int> observer)
    //     {
    //     }
    //
    //     public Task<ReadOnlyMemory<byte>> GetBufferAsync(int length)
    //     {
    //         throw new NotImplementedException();
    //     }
    //
    //     public Task StartAsync(CancellationToken token)
    //     {
    //         throw new NotImplementedException();
    //     }
    // }
}
