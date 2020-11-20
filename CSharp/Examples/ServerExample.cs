using System;
using System.Net;
using SocketFlow.DataWrappers;
using SocketFlow.Server;

namespace Examples
{
    public class ServerExample
    {
        public static void Start(int port)
        {
            var server = new SocketFlowServer<string>(IPAddress.Any, port, new Utf8DataWrapper());
            server.ClientConnected += Server_ClientConnected;
            server.ClientDisconnected += Server_ClientDisconnected;
            server.Bind(1, MessageReceive);
            server.Start(20);
            Console.WriteLine("The server is started");
        }

        private static void Server_ClientConnected(DestinationClient<string> client)
        {
            Console.WriteLine($"Someone connected on {client.RemoteEndPoint}");
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
