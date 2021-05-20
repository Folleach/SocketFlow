using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocketFlow
{
    public class PacketQueue
    {
        private readonly Func<byte[], bool, Task> send;
        private readonly Queue<FlowPacket> packets = new Queue<FlowPacket>();
        private bool flushing;

        public PacketQueue(Func<byte[], bool, Task> send)
        {
            this.send = send;
        }

        public void Add(FlowPacket packet)
        {
            lock (packets)
            {
                packets.Enqueue(packet);
                if (flushing)
                    return;
                flushing = true;
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

                await send(BitConverter.GetBytes(packet.Length), false);
                await send(BitConverter.GetBytes(packet.Type), false);
                await send(packet.PacketData, true);

                lock (packets)
                {
                    if (packets.Count != 0)
                        continue;
                    flushing = false;
                    return;
                }
            }
        }
    }
}