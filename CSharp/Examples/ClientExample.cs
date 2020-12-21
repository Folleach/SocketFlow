using System;
using System.Net;
using System.Threading;
using SocketFlow.Client;
using SocketFlow.DataWrappers;

namespace Examples
{
    public class ClientExample
    {
        private static Client client;

        public static void Start(int port)
        {
            Console.WriteLine("Press any key to connect");
            Console.ReadKey();
            client = new Client(IPAddress.Parse("127.0.0.1"), port)
                .Using(new Utf8DataWrapper());
            client.Bind<string>(1, Response);
            client.Connect();
            ReadAndSend();
            while (true)
                Thread.Sleep(60 * 1000);
        }

        private static void Response(string value)
        {
            Console.WriteLine(value);
            ReadAndSend();
        }

        private static void ReadAndSend()
        {
            Console.Write("Input: ");
            client.Send(1, Console.ReadLine());
        }
    }
}
