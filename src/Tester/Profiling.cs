using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using SocketFlow;
using SocketFlow.Client;
using SocketFlow.DataWrappers;
using SocketFlow.Server;
using SocketFlow.Server.Modules;

namespace Tester
{
    public class Profiling : ITest
    {
        public string Name => nameof(Profiling);

        public int ClientsCount { get; set; } = 20;
        public IPAddress IP { get; set; } = IPAddress.Parse("127.0.0.1");
        public int Port { get; set; } = 33333;

        private FlowServer server;
        private FlowClient[] clients;
        private List<DestinationClient> destinationClients = new();

        public void Run()
        {
            Initialize();
            Broadcast();
            //ClientSend();
        }

        private void Broadcast()
        {
            for (var i = 0; i < ClientsCount * 100; i++)
            {
                server.Broadcast(1, "Helloworld asop as");
            }
        }

        private void ClientSend()
        {
            foreach (var client in clients.AsParallel())
            {
                for (var i = 0; i < ClientsCount; i++)
                    client.Send(1, "hello too");
            }
        }

        private void Initialize()
        {
            server = new FlowServer(FlowOptions.Lazy)
                .UseModule(new TcpModule(IP, Port))
                .UsingWrapper(new Utf8DataWrapper())
                .Start();

            server.ClientConnected += c => destinationClients.Add(c);
            server.Bind<string>(1, (c, v) => { });

            clients = new FlowClient[ClientsCount];
            for (var i = 0; i < ClientsCount; i++)
            {
                clients[i] = new FlowClient(IP, Port)
                    .UsingWrapper(new Utf8DataWrapper());
                clients[i].Bind<string>(1, v => { });
                clients[i].Connect();
            }
            Thread.Sleep(100);
        }
    }
}