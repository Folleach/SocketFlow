using System;
using System.Threading.Tasks;

namespace SocketFlow.Server;

public static class FlowServerBuilderExtensions
{
    public static FlowServerBuilder<TKey, TClient> Map<TClient, TKey, TValue>(
        this FlowServerBuilder<TKey, TClient> configuration,
        TKey key,
        Func<TClient, TKey, TValue, Task> handle)
        where TClient : DestinationClientBase<TKey>
        => configuration.Map(key, new AdHocPacketHandler<TClient, TKey, TValue>(handle));

    public static FlowServerBuilder<TKey, TClient> AddOnConnectedHandler<TKey, TClient>(
        this FlowServerBuilder<TKey, TClient> configuration,
        Func<DestinationClientBase<TKey>, Task> handle)
        where TClient : DestinationClientBase<TKey>
        => configuration.AddOnConnectedHandler(new AdHocClientHandler<TKey>(handle));
    
    public static FlowServerBuilder<TKey, TClient> AddOnDisconnectHandler<TKey, TClient>(
        this FlowServerBuilder<TKey, TClient> configuration,
        Func<DestinationClientBase<TKey>, Task> handle)
        where TClient : DestinationClientBase<TKey>
        => configuration.AddOnDisconnectHandler(new AdHocClientHandler<TKey>(handle));
}
