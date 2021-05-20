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
        private readonly Queue<FlowPacket> packets = new Queue<FlowPacket>();
        private bool Flushing = false;

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

        public void Send(int type, byte[] data)
        {
            var packet = new FlowPacket(type, data);
            lock (packets)
            {
                packets.Enqueue(packet);
                if (Flushing)
                    return;
                Flushing = true;
                Flush();
            }
        }

        private async void Flush()
        {
            while (true)
            {
                FlowPacket packet;
                lock (packets) 
                    packet = packets.Dequeue();

                await stream.WriteAllAsync(BitConverter.GetBytes(packet.Length));
                await stream.WriteAllAsync(BitConverter.GetBytes(packet.Type));
                await stream.WriteAllAsync(packet.PacketData);

                lock (packets)
                {
                    if (packets.Count != 0)
                        continue;
                    Flushing = false;
                    return;
                }
            }
        }

        public event Action OnClose;
        public event Action<int, byte[]> OnData;
    }
}
