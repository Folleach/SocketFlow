using System.Threading.Tasks;

namespace SocketFlow
{
    public interface IClientHandler<in TClient>
    {
        Task Handle(TClient client);
    }
}
