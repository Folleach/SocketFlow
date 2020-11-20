using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Threading;
using SocketFlow.DataWrappers;
using SocketFlow.Server;

namespace Examples
{
    class Program
    {
        private const int Port = 8731;

        static void Main(string[] args)
        {
            if (args.Length == 0)
                ServerExample.Start(Port);
            else
                ClientExample.Start(Port);
            
            while (true)
            {
                Console.ReadLine();
            }
        }
    }
}
