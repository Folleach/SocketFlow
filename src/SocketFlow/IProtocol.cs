using System;

namespace SocketFlow
{
    public interface IProtocol : IDisposable
    {
        void StartListening();
        void Send(int type, byte[] data);
        event Action OnClose;
        event Action<Exception> OnError;
        event Action<int, byte[]> OnData;
    }
}
