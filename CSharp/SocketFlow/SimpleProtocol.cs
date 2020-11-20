using System;
using System.Net.Sockets;

namespace SocketFlow
{
    public class SimpleProtocol : IProtocol
    {
        private const int ProtocolTypePosition = 0;
        private const int ProtocolLengthPosition = 4;
        private readonly TcpClient socket;
        private readonly NetworkStream stream;

        public SimpleProtocol(TcpClient socket)
        {
            this.socket = socket;
            stream = socket.GetStream();
        }

        public void Reader()
        {
            var headBuffer = new byte[8];
            while (socket.Connected)
            {
                if (!stream.ReadAll(headBuffer, headBuffer.Length))
                {
                    OnClose?.Invoke();
                    return;
                }

                var type = BitConverter.ToInt32(headBuffer, ProtocolTypePosition);
                var length = BitConverter.ToInt32(headBuffer, ProtocolLengthPosition);

                var bodyBuffer = new byte[length];

                if (!stream.ReadAll(bodyBuffer, length))
                {
                    OnClose?.Invoke();
                    return;
                }

                OnData?.Invoke(type, bodyBuffer);
            }
        }

        public async void Send(int type, byte[] data)
        {
            var typeBytes = BitConverter.GetBytes(type);
            var lengthBytes = BitConverter.GetBytes(data.Length);

            await stream.WriteAllAsync(typeBytes);
            await stream.WriteAllAsync(lengthBytes);
            await stream.WriteAllAsync(data);
        }

        public event Action OnClose;
        public event Action<int, byte[]> OnData;
    }
}
