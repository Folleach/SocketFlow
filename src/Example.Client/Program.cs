using System.Net;
using SocketFlow;
using SocketFlow.Client;
using SocketFlow.DataWrappers;
using SocketFlow.Protocols;

namespace Example.Client;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        var client = new FlowClient<string>(
            new MiddlewareProtocol<string,JsonProtocolContext>(new JsonStreamProtocol<string>()),
            new AltFlowBinder<FlowClient<string>, string>()
                .UseWrapper(new Utf8DataWrapper())
                .Bind("/welcome", new AdHocPacketHandler<DestinationClientBase<string>, string, string>(async (c, key, value) => Console.WriteLine($"{key} raised"))),
            IPAddress.Loopback,
            65000);

        Console.WriteLine("connect client");
        await client.ConnectAsync(CancellationToken.None);

        await Task.Delay(2000);
        
        Console.WriteLine("send packet");
        await client.Send("/who-you-are", "a");

        await Task.Delay(1000);
        
        Console.WriteLine("disconnect client");
        await client.DisconnectAsync();
    }
}
