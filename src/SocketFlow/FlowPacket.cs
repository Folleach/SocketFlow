namespace SocketFlow
{
    public class FlowPacket
    {
        public readonly int Type;
        public int Length => PacketData.Length;
        public readonly byte[] PacketData;
        
        public FlowPacket(int type, byte[] data)
        {
            Type = type;
            PacketData = data;
        }
    }
}