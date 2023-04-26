using System.Threading.Tasks;
using SocketFlow;

namespace Tester;

public class LogPacketHandler : IPacketHandler<AlternativeDstClient, string, string>
{
    public Task Handle(AlternativeDstClient client, string key, string value)
    {
        return Task.CompletedTask;
    }
}
