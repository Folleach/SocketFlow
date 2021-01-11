using System.Threading;
using NUnit.Framework;
using SocketFlow.Client;
using SocketFlow.DataWrappers;
using SocketFlow.Server;
using SocketFlow.Server.Modules;
using static SocketFlow.Tests.Config;

namespace SocketFlow.Tests
{
    [TestFixture]
    public class EventTransferTests
    {
        [SetUp]
        public void SetUp()
        {
            server = new FlowServer()
                .UsingModule(new TcpModule(LocalAddress, Port1))
                .UsingWrapper(new Utf8DataWrapper())
                .Start();
            server.ClientConnected += c => destinationClient = c;
            client = new FlowClient(LocalAddress, Port1)
                .UsingWrapper(new Utf8DataWrapper());
            client.Connect();
        }

        [TearDown]
        public void TearDown()
        {
            client.Disconnect();
            server.Stop();
        }

        private FlowServer server;
        private DestinationClient destinationClient;
        private FlowClient client;

        [Test]
        public void Server_ShouldBeAcceptEventFromClient()
        {
            const string message = "Message which should be sent";

            string receivedMessage = null;
            server.Bind<string>(1, (c, value) =>
            {
                receivedMessage = value;
            });

            client.Send(1, message);

            Thread.Sleep(MillisecondsToWaitForTransfer);

            Assert.AreEqual(message, receivedMessage);
        }

        [Test]
        public void Client_ShouldBeAcceptEventFromServer()
        {
            const string message = "Message which should be sent";

            string receivedMessage = null;
            client.Bind<string>(1, value =>
            {
                receivedMessage = value;
            });

            destinationClient.Send(1, message);

            Thread.Sleep(MillisecondsToWaitForTransfer);

            Assert.AreEqual(message, receivedMessage);
        }
    }
}
