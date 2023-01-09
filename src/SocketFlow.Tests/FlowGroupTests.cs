using System;
using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using NUnit.Framework;
using SocketFlow.Client;
using SocketFlow.DataWrappers;
using SocketFlow.Server;
using SocketFlow.Server.Modules;
using static SocketFlow.Tests.Config;

namespace SocketFlow.Tests
{
    [TestFixture]
    public class FlowGroupTests
    {
        private static readonly int countOfClient = 3;
        private readonly DestinationClient[] destinationClients = new DestinationClient[countOfClient];
        private readonly FlowClient[] clients = new FlowClient[countOfClient];
        private FlowServer server;

        [SetUp]
        public void SetUp()
        {
            // At some point, I stopped considering this setup easy
            server = new FlowServer(LazyOptions)
                .UseTcpModule(LocalAddress, Port1)
                .UsingWrapper(new Utf8DataWrapper())
                .Start();
            
            var manualReset = new ManualResetEvent(false);
            for (var i = 0; i < destinationClients.Length; i++)
            {
                var index = i;
                void add(DestinationClient client)
                {
                    destinationClients[index] = client;
                    manualReset.Set();
                }
                server.OnConnected(add, out var subscription);
                clients[index] = new FlowClient(LocalAddress, Port1)
                    .UsingWrapper(new Utf8DataWrapper());
                clients[index].Connect();
                manualReset.WaitOne();
                manualReset.Reset();
                subscription.Dispose();
            }
        }
        
        [TearDown]
        public void TearDown()
        {
            foreach (var client in clients)
                client.Disconnect();
            server.Stop();
        }

        [Test]
        public void Group_Send_AllClientsShouldBeReceiveEvent()
        {
            var group = new FlowGroup(server);
            group.Add(destinationClients[0]);
            group.Add(destinationClients[2]);
            var transferString = "Hello client under index 0 & 2";
            var expected = new HashSet<int> {0, 2};

            var receivedOn = new HashSet<int>();

            for (var i = 0; i < clients.Length; i++)
            {
                var index = i;
                clients[i].Bind<string>(1, value =>
                {
                    receivedOn.Add(index);
                });
            }
            
            group.Send(1, transferString);
            
            Thread.Sleep(MillisecondsToWaitForTransfer);
            
            receivedOn.Should().BeEquivalentTo(expected);
        }
        
        [Test]
        public void Group_Remove_ShouldWorkCorrectly()
        {
            var group = new FlowGroup(server);
            group.Add(destinationClients[1]);
            group.Add(destinationClients[2]);
            group.Remove(destinationClients[1]);
            
            Assert.AreEqual(false, group.Contains(destinationClients[0]));
            Assert.AreEqual(false, group.Contains(destinationClients[1]));
            Assert.AreEqual(true, group.Contains(destinationClients[2]));
        }

        [Test]
        public void Group_Contains_ShouldWorkCorrectly()
        {
            var group = new FlowGroup(server);
            group.Add(destinationClients[1]);
            
            Assert.AreEqual(false, group.Contains(destinationClients[0]));
            Assert.AreEqual(true, group.Contains(destinationClients[1]));
            Assert.AreEqual(false, group.Contains(destinationClients[2]));
        }
        
        [Test]
        public void Group_Add_ShouldNotAddClientFromOtherServer()
        {
            var additionalServer = new FlowServer()
                .UseTcpModule(LocalAddress, Port2)
                .Start();

            DestinationClient additionalClient = null;
            additionalServer.OnConnected(c => additionalClient = c, out _);

            new FlowClient(LocalAddress, Port2).Connect();

            var group = new FlowGroup(server);
            Assert.Throws(typeof(Exception), () => group.Add(additionalClient));
        }
    }
}