using System;
using System.Net;
using System.Text.Json;
using SocketFlow.Client;
using SocketFlow.DataWrappers;

namespace Examples
{
    public class ClientExample
    {
        private static Client server;

        public static void Start(int port)
        {
            Console.WriteLine("Press any key to connect");
            Console.ReadKey();
            server = new Client(IPAddress.Parse("127.0.0.1"), port)
                .Using(new JsonDynamicDataWrapper())
                .Using(new Utf8DataWrapper());
            server.Bind<JsonDocument>((int)ScEventId.SendUserMessage, ReceiveMessage);
            server.Connect();
            SendMyName();
            ReadAndSend();
        }

        private static void SendMyName()
        {
            var name = Console.ReadLine();
            server.Send((int)CsEventId.SendName, $"{{\"name\":\"{name}\"}}");
        }

        private static void ReceiveMessage(JsonDocument value)
        {
            var json = value.RootElement;
            var t = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{json.GetProperty("name").GetString()}");
            Console.ForegroundColor = t;
            Console.WriteLine($": {json.GetProperty("message").GetString()}");
        }

        private static void ReadAndSend()
        {
            while (true)
            {
                var message = Console.ReadLine();
                server.Send((int)CsEventId.SendMessage, $"{{\"message\":\"{message}\"}}");
            }
        }
    }
}
