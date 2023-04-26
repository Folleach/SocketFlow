using System;
using System.Threading.Tasks;

namespace SocketFlow.Server;

public class LogClientHandler<T> : IClientHandler<DestinationClientBase<T>>
{
    private readonly Func<DestinationClientBase<T>, string> getMessage;

    public LogClientHandler(Func<DestinationClientBase<T>, string> getMessage)
    {
        this.getMessage = getMessage;
    }

    public Task Handle(DestinationClientBase<T> client)
    {
        Console.WriteLine(getMessage(client));
        return Task.CompletedTask;
    }
}
