using System;
using System.Net.Sockets;
using System.Threading.Tasks;

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
            pinnedForSending = new PacketQueue(ProtocolSend);
        }

        public async void StartListening()
        {
            var headBuffer = new byte[8];
            try
            {
                while (socket.Connected)
                {
                    if (!await stream.ReadAllAsync(headBuffer, headBuffer.Length))
                    {
                        OnClose?.Invoke();
                        return;
                    }

                    var type = BitConverter.ToInt32(headBuffer, ProtocolTypePosition);
                    var length = BitConverter.ToInt32(headBuffer, ProtocolLengthPosition);

                    var bodyBuffer = new byte[length];

                    if (!await stream.ReadAllAsync(bodyBuffer, length))
                    {
                        OnClose?.Invoke();
                        return;
                    }

                    OnData?.Invoke(type, bodyBuffer);
                }
            }
            catch (Exception exception)
            {
                OnError?.Invoke(exception);
            }
        }

        public void Send(int type, byte[] data)
        {
            pinnedForSending.Add(new FlowPacket(type, data));
        }

        private async Task ProtocolSend(byte[] bytes, bool isEnd)
        {
            try
            {
                await stream.WriteAllAsync(bytes);
            }
            catch (Exception exception)
            {
                OnError?.Invoke(exception);
            }
        }

        public event Action OnClose;
        public event Action<Exception> OnError;
        public event Action<int, byte[]> OnData;

        public void Dispose()
        {
            socket.Close();
            socket?.Dispose();
            stream?.Dispose();
        }
    }
}
