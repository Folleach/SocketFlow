using System;
using System.Net.WebSockets;
using System.Threading;

namespace SocketFlow.Server.Protocols
{
    public class WebSocketProtocol : IProtocol
    {
        private const int ProtocolTypePosition = 4;
        private const int ProtocolLengthPosition = 0;
        private readonly WebSocket socket;
        private readonly PacketQueue pinnedForSending;

        public WebSocketProtocol(WebSocket socket)
        {
            this.socket = socket;
            pinnedForSending = new PacketQueue(async (data, end) => await socket.SendAsync(new ArraySegment<byte>(data),
                WebSocketMessageType.Binary,
                end,
                CancellationToken.None));
        }

        public async void Reader()
        {
            var cancelToken = CancellationToken.None;
            var headBuffer = new byte[8];
            var headSegment = new ArraySegment<byte>(headBuffer);

            do
            {
                var received = await socket.ReceiveAsync(headSegment, cancelToken);
                if (received.CloseStatus.HasValue)
                {
                    OnClose?.Invoke();
                    return;
                }
                var type = BitConverter.ToInt32(headBuffer, ProtocolTypePosition);
                var length = BitConverter.ToInt32(headBuffer, ProtocolLengthPosition);

                var bodyBuffer = new byte[length];
                var bodySegment = new ArraySegment<byte>(bodyBuffer);

                received = await socket.ReceiveAsync(bodySegment, cancelToken);
                if (received.CloseStatus.HasValue)
                {
                    OnClose?.Invoke();
                    return;
                }
                OnData?.Invoke(type, bodyBuffer);
            } while (!socket.CloseStatus.HasValue);

            OnClose?.Invoke();
            await socket.CloseAsync(socket.CloseStatus.Value, socket.CloseStatusDescription, cancelToken);
        }

        public void Send(int type, byte[] data)
        {
            pinnedForSending.Add(new FlowPacket(type, data));
        }

        public event Action OnClose;
        public event Action<int, byte[]> OnData;
    }
}
