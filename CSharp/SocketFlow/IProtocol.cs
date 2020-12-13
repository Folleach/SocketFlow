using System;

namespace SocketFlow
{
    public interface IProtocol
    {
        void Reader();
        void Send(int type, byte[] data);
        event Action OnClose;
        event Action<int, byte[]> OnData;
    }
}
