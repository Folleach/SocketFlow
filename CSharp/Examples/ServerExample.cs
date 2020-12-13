using System;
using System.Net;
using SocketFlow.DataWrappers;
using SocketFlow.Server;
using SocketFlow.Server.Modules;

namespace Examples
{
    public class ServerExample
    {
        public static void Start(int port)
        {
            var server = new SocketFlowServer<string>(new Utf8DataWrapper())
                .Using(new TcpModule<string>(IPAddress.Any, port))
                .Using(new WebSocketModule<string>("127.0.0.1:3333"));
            server.ClientConnected += Server_ClientConnected;
            server.ClientDisconnected += Server_ClientDisconnected;
            server.Bind(1, MessageReceive);
            server.Start();
            Console.WriteLine("The server is started");
        }

        private static void Server_ClientConnected(DestinationClient<string> client)
        {
            Console.WriteLine($"Someone connected on {client.RemoteEndPoint}");
            client.Send(1, "Hello!");
        }

        private static void Server_ClientDisconnected(DestinationClient<string> client)
        {
            Console.WriteLine($"{client.RemoteEndPoint} disconnected");
        }

        private static void MessageReceive(DestinationClient<string> client, string value)
        {
            Console.WriteLine($"{client}: {value}");
            client.Send(1, "Sent");
        }
    }
}
