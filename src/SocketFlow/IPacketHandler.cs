using System.Threading.Tasks;

namespace SocketFlow;

public interface IPacketHandler<in TClient, in TKey, in TValue>
{
    Task Handle(TClient client, TKey key, TValue value);
}
