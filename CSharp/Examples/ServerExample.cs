using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
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
            var server = new FlowServer()
                .Using(new TcpModule(IPAddress.Any, port))
                .Using(new WebSocketModule("127.0.0.1:3333"))
                .Using(new UserMessageDataWrapper())
                .Using(new Utf8DataWrapper())
                .Using(new JsonDynamicDataWrapper());
            server.ClientConnected += Server_ClientConnected;
            server.ClientDisconnected += Server_ClientDisconnected;
            server.Bind<JsonDocument>((int)CsEventId.SendName, NameReceive);
            server.Bind<JsonDocument>((int)CsEventId.SendMessage, MessageReceive);
            server.Start();
            Console.WriteLine("The server is started");
        }

        private static void Server_ClientConnected(DestinationClient client)
        {
            Console.WriteLine($"Someone connected on {client.RemoteEndPoint}");
            client.Send((int)ScEventId.SendUserMessage, JsonSerializer.Serialize(new UserMessage("Server", "What is your name?")));
        }

        private static void Server_ClientDisconnected(DestinationClient client)
        {
            Console.WriteLine($"{(clients.ContainsKey(client) ? clients[client] : client.RemoteEndPoint.ToString())} disconnected");
            clients.Remove(client);
        }

        private static void NameReceive(DestinationClient client, JsonDocument value)
        {
            var name = value.RootElement.GetProperty("name").GetString();
            clients[client] = name;
            Console.WriteLine($"{client.RemoteEndPoint} now is {name}");
        }

        private static void MessageReceive(DestinationClient client, JsonDocument value)
        {
            var senderName = clients[client];
            var message = value.RootElement.GetProperty("message").GetString();
            Console.WriteLine($"{senderName} say: {message}");
            var userMassage = JsonSerializer.Serialize(new UserMessage(senderName, message));
            foreach (var otherClient in clients)
                otherClient.Key.Send((int)ScEventId.SendUserMessage, userMassage);
        }
    }
}
