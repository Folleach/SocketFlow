using System;
using System.Net.Sockets;

namespace SocketFlow
{
    public class TcpProtocol : IProtocol
    {
        private const int ProtocolTypePosition = 4;
        private const int ProtocolLengthPosition = 0;
        private readonly TcpClient socket;
        private readonly NetworkStream stream;

        public TcpProtocol(TcpClient socket)
        {
            this.socket = socket;
            stream = socket.GetStream();
        }

        public async void Reader()
        {
            var headBuffer = new byte[8];
            while (socket.Connected)
            {
                if (!await stream.ReadAll(headBuffer, headBuffer.Length))
                {
                    OnClose?.Invoke();
                    return;
                }

                var type = BitConverter.ToInt32(headBuffer, ProtocolTypePosition);
                var length = BitConverter.ToInt32(headBuffer, ProtocolLengthPosition);

                var bodyBuffer = new byte[length];

                if (!await stream.ReadAll(bodyBuffer, length))
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

            await stream.WriteAllAsync(lengthBytes);
            await stream.WriteAllAsync(typeBytes);
            await stream.WriteAllAsync(data);
        }

        public event Action OnClose;
        public event Action<int, byte[]> OnData;
    }
}
