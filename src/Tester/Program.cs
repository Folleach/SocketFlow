using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using SocketFlow;
using SocketFlow.DataWrappers;
using SocketFlow.Server;

namespace Tester;

internal class Program
{
    public static async Task<int> Main(string[] args)
    {
        SetupExceptionCatcher();

        Console.WriteLine("build server");
        var builder = new FlowServerBuilder<string, AlternativeDstClient>()
            .ConfigureProtocolStack(stack => stack
                .UseTcpBaseEndpoint(IPAddress.Any, 65000)
                .UseJsonFlowProtocol()
            )
            // .ConfigureProtocolStack(stack => stack
            //     .UseTcpBaseEndpoint(IPAddress.Any, 65001)
            //     .UseJsonFlowProtocol()
            // )
            .AddOnConnectedHandler(new LogClientHandler<string>(c => $"client connected {c}"))
            // .AddOnConnectedHandler(c => c.Send("hello", "welcome to the server!"))
            // .AddOnDisconnectHandler(c => c.Send("/bye", "i am closing"))
            .AddOnDisconnectHandler(new LogClientHandler<string>(c => $"client disconnected {c}"))
            .UseWrapper(new Utf8DataWrapper())
            .Map("/who-you-are", (AlternativeDstClient c, string key, string value) =>
            {
                Console.WriteLine($"{c} wondering who I am");
                return c.Send("/welcome", new IAm()
                {
                    Name = "SocketFlow",
                    Age = 1
                });
            })
            .Map("/welcome", (AlternativeDstClient c, string key, IAm value) =>
            {
                Console.WriteLine($"{c} says he's {value.Name}. His age is {value.Age}");
                c.Name = value.Name;
                return Task.CompletedTask;
            });

        builder.Map("/who-am-i", (AlternativeDstClient c, string key, string value) =>
        {
            Console.WriteLine($"{c} is {c.Name}");
            return Task.CompletedTask;
        });

        var server = builder.Build();

        Console.WriteLine("run server");
        await server.RunAsync(CreateCancellationToken());

        Console.WriteLine("server has been closed");
        return 0;
    }

    private static CancellationToken CreateCancellationToken()
    {
        var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (s, e) =>
        {
            Console.WriteLine("close server");
            cts.Cancel();
        };
        return cts.Token;
    }

    private static void SetupExceptionCatcher()
    {
        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            var old = SetColor(ConsoleColor.DarkRed);
            Console.WriteLine(e.ExceptionObject);
            SetColor(old);
        };
    }

    private static ConsoleColor SetColor(ConsoleColor color)
    {
        var old = Console.ForegroundColor;
        Console.ForegroundColor = color;
        return old;
    }
}
