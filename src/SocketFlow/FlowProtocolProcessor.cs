using System.Linq;

namespace SocketFlow;

public class FlowProtocolProcessor<TKey, TClient> where TClient : DestinationClientBase<TKey>
{
    public int Count => baseProcessors.Sum(x => x.Count);

    private readonly EndpointBaseProcessor<TKey, TClient>[] baseProcessors;

    public FlowProtocolProcessor(EndpointBaseProcessor<TKey, TClient>[] baseProcessors)
    {
        this.baseProcessors = baseProcessors;
    }
}
