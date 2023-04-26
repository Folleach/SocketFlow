using System.Threading.Tasks;

namespace SocketFlow
{
    public interface IProtocolOneTimeMiddleware
    {
        Task Handle(IEndpoint endpoint);
    }
}
