using System.Threading;
using NUnit.Framework;
using SocketFlow.Client;
using SocketFlow.Server;
using SocketFlow.Server.Modules;
using static SocketFlow.Tests.Config;

namespace SocketFlow.Tests
{
    [TestFixture]
    public class ConnectionTests
    {
        [SetUp]
        public void SetUp()
        {
            server = new FlowServer()
                .UseTcpModule(LocalAddress, Port1)
                .Start();
        }

        [TearDown]
        public void TearDown()
        {
            client.Disconnect();
            server.Stop();
        }

        private FlowServer server;
        private FlowClient client;

        [Test]
        public void Server_ShouldBeEventOnClientConnected()
        {
            client = new FlowClient(LocalAddress, Port1);

            var wasCalled = false;
            server.OnConnected(_ => wasCalled = true);

            client.Connect();

            // Wait while server accept client
            Thread.Sleep(MillisecondsToWaitForTransfer);

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void Client_ShouldBeEventOnClientConnected()
        {
            client = new FlowClient(LocalAddress, Port1);

            var wasCalled = false;
            client.Connected += destinationClient => wasCalled = true;

            client.Connect();

            Assert.IsTrue(wasCalled);
        }
    }
}
