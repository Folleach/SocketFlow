using System.Threading.Tasks;

namespace SocketFlow;

public class DestinationClientBase<TKey> : ITransferUnit<TKey>
{
    public IFlowProtocol<TKey, IProtocolContext> Protocol { get; init; }
    public IFlowBinder<DestinationClientBase<TKey>, TKey> Binder { get; init; }
    public IEndpoint Endpoint { get; protected set; }

    public async Task Send<TValue>(TKey key, TValue value)
    {
        var wrapper = Binder.GetDataWrapper<TValue>();
        var data = wrapper.FormatObject(value);
        var packed = Protocol.Pack<TValue>(key, data.CreateHoldable());
        await Endpoint.Send(packed.AsReadOnlyMemory());
    }

    public override string ToString()
    {
        return $"[DstClientBase: {Endpoint}]";
    }
}
