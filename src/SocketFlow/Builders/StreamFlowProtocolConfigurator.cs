using SocketFlow.Protocols;

namespace SocketFlow.Builders;

public class StreamFlowProtocolConfigurator<TKey, T> : IFlowProtocolConfigurator
{
    private readonly ProtocolStackConfigurator<TKey, T> configurator;

    public StreamFlowProtocolConfigurator(ProtocolStackConfigurator<TKey, T> configurator)
    {
        this.configurator = configurator;
    }

    public StreamFlowProtocolConfigurator<TKey, T> UseJsonFlowProtocol()
    {
        configurator.UseHighLevelProtocol(new JsonStreamProtocol<TKey>());
        return this;
    }
}
