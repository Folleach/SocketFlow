using System.Threading;
using FluentAssertions;
using NUnit.Framework;
using SocketFlow.Client;
using SocketFlow.DataWrappers;
using SocketFlow.Server;
using SocketFlow.Server.Modules;
using SocketFlow.Tests.TestingObjects;
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
                .UseTcpModule(LocalAddress, Port1)
                .UsingWrapper(new Utf8DataWrapper())
                .Start();
            server.OnConnected(c => destinationClient = c, out _);
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
        public void Server_BroadcastShouldBeSendValueToAllClients()
        {
            var testString = "Hear me?";
            var additionalClient = new FlowClient(LocalAddress, Port1, LazyOptions)
                .UsingWrapper(new Utf8DataWrapper());
            additionalClient.Connect();
            string client1response = null, client2response = null;
            client.Bind<string>(1, x => client1response = x);
            additionalClient.Bind<string>(1, x => client2response = x);

            server.Broadcast(1, testString);

            Thread.Sleep(MillisecondsToWaitForTransfer);

            Assert.AreEqual(testString, client1response, "First client should be receive message");
            Assert.AreEqual(testString, client2response, "Second client should be receive message");
        }

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

        [Test]
        public void Server_ShouldBeAcceptJsonDataFromClient()
        {
            var wrapper = new JsonDataWrapper<Planet>();
            server.UsingWrapper(wrapper);
            client.UsingWrapper(wrapper);
            
            var obj = new Planet() { Name = "Earth", Radius = 6378.1, Creatures = 349653671 };
            Planet receivedObject = null;

            server.Bind<Planet>(1, (c, value) =>
            {
                receivedObject = value;
            });

            client.Send(1, obj);

            Thread.Sleep(MillisecondsToWaitForTransfer * 10);
            receivedObject.Should().BeEquivalentTo(obj);
        }
    }
}
