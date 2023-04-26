namespace SocketFlow;

public class FlowPacket<TKey>
{
    public readonly TKey Key;
    public readonly HoldableBuffer PacketData;

    public FlowPacket(TKey key, HoldableBuffer data)
    {
        Key = key;
        PacketData = data;
    }
}
