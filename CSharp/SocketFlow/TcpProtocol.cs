using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace SocketFlow
{
    public class TcpProtocol : IProtocol
    {
        private const int ProtocolTypePosition = 4;
        private const int ProtocolLengthPosition = 0;
        private readonly TcpClient socket;
        private readonly NetworkStream stream;
        private readonly PacketQueue pinnedForSending;

        public TcpProtocol(TcpClient socket)
        {
            this.socket = socket;
            stream = socket.GetStream();
            pinnedForSending = new PacketQueue(async (data, end) => await stream.WriteAllAsync(data));
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

        public void Send(int type, byte[] data)
        {
            pinnedForSending.Add(new FlowPacket(type, data));
        }

        public event Action OnClose;
        public event Action<int, byte[]> OnData;
    }
}
