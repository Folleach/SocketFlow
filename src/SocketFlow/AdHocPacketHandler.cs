using System;
using System.Threading.Tasks;

namespace SocketFlow;

public class AdHocPacketHandler<TClient, TKey, TValue> : IPacketHandler<TClient, TKey, TValue>
{
    private readonly Func<TClient, TKey, TValue, Task> handle;

    public AdHocPacketHandler(Func<TClient, TKey, TValue, Task> handle)
    {
        this.handle = handle;
    }

    public Task Handle(TClient client, TKey key, TValue value)
    {
        return handle(client, key, value);
    }
}
