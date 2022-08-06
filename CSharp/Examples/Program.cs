using System;
using System.Linq;
using System.Threading;
using Folleach.ConsoleUtils;

namespace Examples
{
    public static class Program
    {
        private const int Port = 8731;

        private static void Main(string[] rawArgs)
        {
            var args = new Args(rawArgs);
            while (!args.Any())
            {
                Console.WriteLine("Type the args if you miss them");
                Console.WriteLine(" -c, --client\tRun client");
                Console.WriteLine(" -s, --server\tRun server");

                args = new Args(Console.ReadLine()?.Split(' ') ?? throw new InvalidOperationException("Can't get the args"));
            }

            if (args.Contains("client") || args.Contains("c"))
                ClientExample.Start(Port);
            else if (args.Contains("server") || args.Contains("s"))
                ServerExample.Start(Port);
            else
                return;

            CancellationToken.None.WaitHandle.WaitOne();
        }
    }
}
