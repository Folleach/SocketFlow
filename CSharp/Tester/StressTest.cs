using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using SocketFlow;
using SocketFlow.Client;
using SocketFlow.DataWrappers;
using SocketFlow.Server;
using SocketFlow.Server.Modules;

namespace Tester
{
    public class StressTest : ITest
    {
        public string Name => nameof(StressTest);

        public int ClientsCount { get; set; } = 20;
        public IPAddress IP { get; set; } = IPAddress.Parse("127.0.0.1");
        public int Port { get; set; } = 33333;

        private FlowServer server;
        private FlowClient[] clients;
        private List<DestinationClient> destinationClients = new();
        private bool work = true;
        private Dictionary<FlowClient, long> dataTransdered = new();

        private string data = "Lorem ipsum? Vseja se a sjhej ajksheqwu  asqw  a";
        private int errors = 0;

        public void Run()
        {
            Initialize();
            Start();
        }

        private void Start()
        {
            new Thread(UI).Start();
            new Thread(Sender).Start();
            new Thread(Sender).Start();
            Console.ReadKey();
        }

        private void Sender()
        {
            while (work)
            {
                server.Broadcast(1, data);
            }
        }

        private void UI()
        {
            while (work)
            {
                Console.Clear();
                lock (server)
                {
                    Console.WriteLine($"Errors: {errors}");
                    var total = 0L;
                    var i = 0;
                    foreach (var (key, value) in dataTransdered)
                    {
                        total += value;
                        Console.WriteLine($"Client{i++}: {value} bytes");
                        //dataTransdered[key] = 0;
                    }

                    Console.WriteLine($"Total: {total}");
                }
                

                Thread.Sleep(1000);
            }
        }

        private void Initialize()
        {
            server = new FlowServer(FlowOptions.Lazy)
                .UsingModule(new TcpModule(IP, Port))
                .UsingWrapper(new Utf8DataWrapper())
                .Start();

            server.ClientConnected += c => destinationClients.Add(c);

            clients = new FlowClient[ClientsCount];
            for (var i = 0; i < ClientsCount; i++)
            {
                clients[i] = new FlowClient(IP, Port)
                    .UsingWrapper(new Utf8DataWrapper());
                var index = i;
                clients[i].Bind<string>(1, v => FlowClientReceiveOne(clients[index], v));
                clients[i].Connect();
                dataTransdered.Add(clients[i], 0);
            }
        }

        private void FlowClientReceiveOne(FlowClient client, string value)
        {
            if (value != data)
            {
                errors++;
                return;
            }
            
            lock (server)
            {
                dataTransdered[client] += data.Length * 8;
            }
        }
    }
}