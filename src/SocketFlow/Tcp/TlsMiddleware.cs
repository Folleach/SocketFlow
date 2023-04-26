using System;
using System.Threading.Tasks;

namespace SocketFlow.Tcp
{
    public class TlsMiddleware : IProtocolMiddleware
    {
        public TlsMiddleware(IEndpoint endpoint)
        {
            
        }

        public Task<ReadOnlyMemory<byte>> Transform(ReadOnlyMemory<byte> incoming)
        {
            throw new NotImplementedException();
        }
    }
}
