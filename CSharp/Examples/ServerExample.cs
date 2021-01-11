using System;
using System.Collections.Generic;
using System.Net;
using SocketFlow;
using SocketFlow.Server;
using SocketFlow.Server.Modules;

namespace Examples
{
    public class ServerExample
    {
        private static readonly Dictionary<DestinationClient, string> clients = new Dictionary<DestinationClient, string>();

        private static readonly FlowOptions options = new FlowOptions()
        {
            DefaultNonPrimitivesObjectUsingAsJson = true
        };

        public static void Start(int port)
        {
            var server = new FlowServer(options)
                .Using(new TcpModule(IPAddress.Any, port))
                .Using(new WebSocketModule("127.0.0.1:3333"));
            server.ClientConnected += Server_ClientConnected;
            server.ClientDisconnected += Server_ClientDisconnected;
            server.Bind<UserInput>((int)CsEventId.SendName, NameReceive);
            server.Bind<UserInput>((int)CsEventId.SendMessage, MessageReceive);
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
            Console.WriteLine($"{(clients.ContainsKey(client) ? clients[client] : client.RemoteEndPoint.ToString())} disconnected");
            clients.Remove(client);
        }

        private static void NameReceive(DestinationClient client, UserInput value)
        {
            var name = value.Input;
            clients[client] = name;
            Console.WriteLine($"{client.RemoteEndPoint} now is {name}");
        }

        private static void MessageReceive(DestinationClient client, UserInput value)
        {
            var senderName = clients[client];
            var message = value.Input;
            Console.WriteLine($"{senderName} say: {message}");
            var userMassage = new UserMessage(senderName, message);
            foreach (var otherClient in clients)
                otherClient.Key.Send((int)ScEventId.SendUserMessage, userMassage);
        }
    }
}
