using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SocketFlow.Client;
using SocketFlow.Protocols;
using SocketFlow.Server;
using static SocketFlow.Tests.Config;

namespace SocketFlow.Tests
{
    [TestFixture]
    public class ConnectionTests
    {
        private Task serverRun;
        private CancellationTokenSource cts;

        [SetUp]
        public void SetUp()
        {
            cts = new CancellationTokenSource();
            server = new FlowServerBuilder<string, DestinationClientBase<string>>()
                .ConfigureProtocolStack(stack => stack
                    .UseTcpBaseEndpoint(IPAddress.Loopback, Port1)
                    .UseJsonFlowProtocol()
                )
                .Build();
            serverRun = server.RunAsync(cts.Token);
        }

        [TearDown]
        public void TearDown()
        {
            cts.Cancel();
            serverRun.Wait();
        }

        private FlowServer<string, DestinationClientBase<string>> server;
        private FlowClient<string> client;

        [Test]
        public async Task Server_ShouldBeEventOnClientConnected()
        {
            client = new FlowClient<string>(
                new MiddlewareProtocol<string,JsonProtocolContext>(new JsonStreamProtocol<string>()),
                new AltFlowBinder<FlowClient<string>, string>(),
                IPAddress.Loopback,
                Port1);

            server.ClientsCount.Should().Be(0);
            await client.ConnectAsync(CancellationToken.None);

            await Task.Delay(100);
            server.ClientsCount.Should().Be(1);
            await client.DisconnectAsync();

            await Task.Delay(100);
            server.ClientsCount.Should().Be(0);
        }

        // [Test]
        // public void Client_ShouldBeEventOnClientConnected()
        // {
        //     client = new FlowClient(LocalAddress, Port1);
        //
        //     var wasCalled = false;
        //     client.Connected += destinationClient => wasCalled = true;
        //
        //     client.Connect();
        //
        //     Assert.IsTrue(wasCalled);
        // }
    }
}
