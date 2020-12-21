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
            var server = new SocketFlowServer()
                .Using(new TcpModule(IPAddress.Any, port))
                .Using(new WebSocketModule("127.0.0.1:3333"))
                .Using(new Utf8DataWrapper());
            server.ClientConnected += Server_ClientConnected;
            server.ClientDisconnected += Server_ClientDisconnected;
            server.Bind<string>(1, MessageReceive);
            server.Start();
            Console.WriteLine("The server is started");
        }

        private static void Server_ClientConnected(DestinationClient client)
        {
            Console.WriteLine($"Someone connected on {client.RemoteEndPoint}");
            client.Send(1, "Hello!");
        }

        private static void Server_ClientDisconnected(DestinationClient client)
        {
            Console.WriteLine($"{client.RemoteEndPoint} disconnected");
        }

        private static void MessageReceive(DestinationClient client, string value)
        {
            Console.WriteLine($"{client}: {value}");
            client.Send(1, "Sent");
        }
    }
}
