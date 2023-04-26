using SocketFlow.DataWrappers;

namespace SocketFlow;

public interface IFlowBinder<out TClient, TKey>
{
    IFlowBinder<TClient, TKey> Bind<T>(TKey key, IPacketHandler<TClient, TKey, T> handler);
    IFlowBinder<TClient, TKey> UseWrapper<T>(IDataWrapper<T> wrapper);
    IDataWrapper<T> GetDataWrapper<T>();
    bool TryGetHandlerInfo(TKey key, out HandlerInfo info);
    bool TryGetWrapperInfo(TKey key, out WrapperInfo info);
}
