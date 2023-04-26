using System.Collections.Generic;

namespace SocketFlow;

public interface IProtocolContext
{
}

public interface IFlowProtocol<TKey, TContext> where TContext : IProtocolContext
{
    TContext CreateContext();
    IEnumerable<FlowPacket<TKey>> Receive(TContext context, HoldableBuffer buffer);
    HoldableBuffer Pack<TValue>(TKey key, HoldableBuffer data);
}
