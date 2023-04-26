using System;
using System.Threading;
using System.Threading.Tasks;

namespace SocketFlow;

/// <summary>
/// A ip based protocols implementation
/// </summary>
public interface IEndpoint : IObservable<HoldableBuffer>
{
    Task Send(ReadOnlyMemory<byte> data);
    Task StartAsync(CancellationToken token);
    Task ObserveAsync(CancellationToken token);
}
