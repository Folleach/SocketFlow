using System;
using System.Net;
using SocketFlow;
using SocketFlow.Client;

namespace Examples
{
    public class ClientExample
    {
        private static FlowClient client;

        private static readonly FlowOptions options = new FlowOptions()
        {
            DefaultNonPrimitivesObjectUsingAsJson = true
        };

        public static void Start(int port)
        {
            Console.WriteLine("Press any key to connect");
            Console.ReadKey();
            client = new FlowClient(IPAddress.Parse("127.0.0.1"), port, options);
            client.Bind<UserMessage>((int)ScEventId.SendUserMessage, ReceiveMessage);
            client.Connect();
            SendMyName();
            ReadAndSend();
        }

        private static void SendMyName()
        {
            var name = Console.ReadLine();
            client.Send((int)CsEventId.SendName, new UserInput(name));
        }

        private static void ReceiveMessage(UserMessage value)
        {
            var t = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{value.UserName}");
            Console.ForegroundColor = t;
            Console.WriteLine($": {value.Message}");
        }

        private static void ReadAndSend()
        {
            while (true)
            {
                var message = Console.ReadLine();
                client.Send((int)CsEventId.SendMessage, new UserInput(message));
            }
        }
    }
}
