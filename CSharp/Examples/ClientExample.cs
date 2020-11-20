using System;
using System.Net;
using System.Threading;
using SocketFlow.Client;
using SocketFlow.DataWrappers;

namespace Examples
{
    public class ClientExample
    {
        private static Client<string> client;

        public static void Start(int port)
        {
            client = new Client<string>(IPAddress.Parse("127.0.0.1"), port, new Utf8DataWrapper());
            client.Bind(1, Response);
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
