using System;
using System.Net;
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
                .Using(new UserMessageDataWrapper())
                .Using(new Utf8DataWrapper());
            server.Bind<UserMessage>((int)ScEventId.SendUserMessage, ReceiveMessage);
            server.Connect();
            SendMyName();
            ReadAndSend();
        }

        private static void SendMyName()
        {
            var name = Console.ReadLine();
            server.Send((int)CsEventId.SendName, name);
        }

        private static void ReceiveMessage(UserMessage message)
        {
            var t = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{message.UserName}");
            Console.ForegroundColor = t;
            Console.WriteLine($": {message.Message}");
        }

        private static void ReadAndSend()
        {
            while (true)
            {
                var message = Console.ReadLine();
                server.Send((int)CsEventId.SendMessage, message);
            }
        }
    }
}
