using System;
using System.Collections.Generic;
using System.Net;
using SocketFlow.DataWrappers;
using SocketFlow.Server;
using SocketFlow.Server.Modules;

namespace Examples
{
    public class ServerExample
    {
        private static readonly Dictionary<DestinationClient, string> clients = new Dictionary<DestinationClient, string>();

        public static void Start(int port)
        {
            var server = new SocketFlowServer()
                .Using(new TcpModule(IPAddress.Any, port))
                .Using(new WebSocketModule("127.0.0.1:3333"))
                .Using(new UserMessageDataWrapper())
                .Using(new Utf8DataWrapper());
            server.ClientConnected += Server_ClientConnected;
            server.ClientDisconnected += Server_ClientDisconnected;
            server.Bind<string>((int)CsEventId.SendName, NameReceive);
            server.Bind<string>((int)CsEventId.SendMessage, MessageReceive);
            server.Start();
            Console.WriteLine("The server is started");
        }

        private static void Server_ClientConnected(DestinationClient client)
        {
            Console.WriteLine($"Someone connected on {client.RemoteEndPoint}");
            client.Send((int)ScEventId.SendUserMessage, new UserMessage("Server", "What is your name?"));
        }

        private static void Server_ClientDisconnected(DestinationClient client)
        {
            Console.WriteLine($"{clients[client]} disconnected");
            clients.Remove(client);
        }

        private static void NameReceive(DestinationClient client, string name)
        {
            clients.Add(client, name);
            Console.WriteLine($"{client.RemoteEndPoint} is {name}");
        }

        private static void MessageReceive(DestinationClient client, string message)
        {
            var senderName = clients[client];
            Console.WriteLine($"{senderName} say: {message}");
            var userMassage = new UserMessage(senderName, message);
            foreach (var otherClient in clients)
                otherClient.Key.Send((int)ScEventId.SendUserMessage, userMassage);
        }
    }
}
