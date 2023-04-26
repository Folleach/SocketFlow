using System.Collections.Generic;

namespace SocketFlow;

public class MiddlewareProtocol<TKey, TContext> : IFlowProtocol<TKey, IProtocolContext>
    where TContext : IProtocolContext
{
    private readonly IFlowProtocol<TKey, TContext> baseProtocol;

    public MiddlewareProtocol(IFlowProtocol<TKey, TContext> baseProtocol)
    {
        this.baseProtocol = baseProtocol;
    }

    public IProtocolContext CreateContext()
    {
        return baseProtocol.CreateContext();
    }

    public IEnumerable<FlowPacket<TKey>> Receive(IProtocolContext ctx, HoldableBuffer buffer)
    {
        return baseProtocol.Receive((TContext)ctx, buffer);
    }

    public HoldableBuffer Pack<TValue>(TKey key, HoldableBuffer data)
    {
        return baseProtocol.Pack<TValue>(key, data);
    }
}
