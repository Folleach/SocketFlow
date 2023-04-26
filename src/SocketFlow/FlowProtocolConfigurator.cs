using System.Threading.Tasks;

namespace SocketFlow;

public class FlowProtocolConfigurator<TKey, T>
{
    public T Configurator { get; }
    public IFlowProtocol<TKey, IProtocolContext> Protocol { get; private set; }

    public FlowProtocolConfigurator(T configurator)
    {
        Configurator = configurator;
    }
}
