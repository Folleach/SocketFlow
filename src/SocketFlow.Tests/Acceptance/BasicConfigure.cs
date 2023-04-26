using System.Net;
using System.Threading;
using NUnit.Framework;
using SocketFlow.Client;
using SocketFlow.Protocols;
using SocketFlow.Server;
using SocketFlow.Tcp;

namespace SocketFlow.Tests.Acceptance
{
    // [TestFixture]
    // public class BasicConfigure
    // {
    //     [Test]
    //     public void Server()
    //     {
    //         var server = new FlowServerBuilder<string, DestinationClientBase<string>>()
    //             .ConfigureProtocolStack()
    //             .AddTls()
    //             .BindToBase(new TcpBaseEndpoint(IPAddress.Any, 65000))
    //             .ConfigureProtocolStack()
    //             .BindToBase(new TcpBaseEndpoint(IPAddress.Any, 65001))
    //             .ConfigureFlowProtocol()
    //             .UseJsonStream()
    //             .AddOnConnectedHandler(new LogClientHandler<string>(c => $"client connected {c}"))
    //             .AddOnDisconnectHandler(new LogClientHandler<string>(c => $"client disconnected {c}"))
    //             .Build();
    //
    //         _ = server.RunAsync(new CancellationToken());
    //
    //         server.Send("/init", "hello!");
    //         var group = server.CreateGroup();
    //         group.Send("/time-to-start", 12);
    //     }
    //
    //     [Test]
    //     public void Client()
    //     {
    //         var client = new FlowClient(
    //             new JsonStreamProtocol(),
    //             new TcpBaseEndpoint());
    //     }
    // }
}
