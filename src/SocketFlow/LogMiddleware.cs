using System;
using System.Threading.Tasks;

namespace SocketFlow
{
    public class LogMiddleware : IProtocolMiddleware
    {
        public LogMiddleware(IEndpoint endpoint)
        {
            
        }

        public Task<ReadOnlyMemory<byte>> Transform(ReadOnlyMemory<byte> incoming)
        {
            throw new NotImplementedException();
        }
    }
}
