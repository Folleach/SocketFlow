using System;
using System.Net.WebSockets;
using System.Threading;

namespace SocketFlow.Server.Protocols
{
    public class WebSocketProtocol : IProtocol
    {
        private const int ProtocolTypePosition = 0;
        private const int ProtocolLengthPosition = 4;
        private readonly WebSocket socket;

        public WebSocketProtocol(WebSocket socket)
        {
            this.socket = socket;
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

        public async void Send(int type, byte[] data)
        {
            var typeBytes = BitConverter.GetBytes(type);
            var lengthBytes = BitConverter.GetBytes(data.Length);

            await socket.SendAsync(new ArraySegment<byte>(typeBytes), WebSocketMessageType.Binary, false, CancellationToken.None);
            await socket.SendAsync(new ArraySegment<byte>(lengthBytes), WebSocketMessageType.Binary, false, CancellationToken.None);
            await socket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Binary, true, CancellationToken.None);
        }

        public event Action OnClose;
        public event Action<int, byte[]> OnData;
    }
}
