using System;
using System.Threading.Tasks;

namespace SocketFlow;

public class AdHocClientHandler<T> : IClientHandler<DestinationClientBase<T>>
{
    private readonly Func<DestinationClientBase<T>, Task> handle;
    public AdHocClientHandler(Func<DestinationClientBase<T>, Task> handle) => this.handle = handle;
    public Task Handle(DestinationClientBase<T> client) => handle(client);
}
