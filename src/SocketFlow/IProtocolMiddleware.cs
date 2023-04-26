using System;
using System.Threading.Tasks;

namespace SocketFlow
{
    public interface IProtocolMiddleware
    {
        Task<ReadOnlyMemory<byte>> Transform(ReadOnlyMemory<byte> incoming);
    }
}
