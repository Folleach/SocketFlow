using System.Net;

namespace SocketFlow.Tests
{
    public class Config
    {
        public static readonly IPAddress LocalAddress = IPAddress.Parse("127.0.0.1");
        public static readonly int Port1 = 30000;
        public static readonly int Port2 = 30001;
        public static readonly int MillisecondsToWaitForTransfer = 10;
        public static readonly FlowOptions LazyOptions = new FlowOptions()
        {
            DefaultNonPrimitivesObjectUsingAsJson = true
        };
    }
}
