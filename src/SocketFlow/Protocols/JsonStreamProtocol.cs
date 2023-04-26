using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SocketFlow.Protocols;

[DataContract]
public class JsonPacket<TKey>
{
    [DataMember(Name = "method")] [JsonPropertyName("method")]
    public TKey Key { get; set; }

    [DataMember(Name = "params")] [JsonPropertyName("params")]
    public JObject Data { get; set; }
}

public class JsonProtocolContext : IProtocolContext
{
    public Queue<HoldableBuffer> Packets = new();
    public int TotalBytes = 0;
    public int OpenBracketOffset = 0;
    public int BracketsDepth = 0;
}

/// <summary>
/// Protocol partially similar to JSON-RPC 2.0<br />
/// https://www.jsonrpc.org/specification
/// </summary>
public class JsonStreamProtocol<TKey> : IFlowProtocol<TKey, JsonProtocolContext>
{
    public IEnumerable<FlowPacket<TKey>> Receive(JsonProtocolContext context, HoldableBuffer buffer)
    {
        var data = buffer.AsArraySegment();
        for (var i = 0; i < data.Count; i++)
        {
            if (data.Array[i] == '{')
            {
                if (context.BracketsDepth == 0)
                    context.OpenBracketOffset = i;
                context.BracketsDepth++;
            }

            if (data.Array[i] == '}')
            {
                if (context.BracketsDepth != 0)
                    context.BracketsDepth--;
                if (context.BracketsDepth == 0)
                {
                    var json = buffer.AsReadOnlySpan(context.OpenBracketOffset, i + 1 - context.OpenBracketOffset);
                    var obj = JObject.Parse(Encoding.UTF8.GetString(json.ToArray()));
                    var value = Encoding.UTF8.GetBytes(obj["params"]?.Value<JToken>()?.ToString() ?? string.Empty);
                    var b = new HoldableBuffer(value.Length);
                    var segment = b.AsArraySegment().AsSpan();
                    value.AsSpan().CopyTo(segment);
                    yield return new FlowPacket<TKey>(obj["method"]!.Value<TKey>() ?? default, b);
                }
            }
        }

        if (context.BracketsDepth > 0)
        {
            context.Packets.Enqueue(buffer);
            context.TotalBytes = buffer.Length;
        }
        else
            (buffer as IDisposable).Dispose();
    }

    public HoldableBuffer Pack<TValue>(TKey key, HoldableBuffer data)
    {
        var json = new JObject();
        json.Add("method", JToken.FromObject(key));
        json.Add("params",
            typeof(TValue) == typeof(string)
            ? JToken.Parse($"\"{Encoding.UTF8.GetString(data.AsReadOnlySpan().ToArray())}\"")
            : JToken.Parse($"{Encoding.UTF8.GetString(data.AsReadOnlySpan().ToArray())}"));
        return Encoding.UTF8.GetBytes(json.ToString(Formatting.None)).CreateHoldable();
    }

    public JsonProtocolContext CreateContext()
    {
        return new JsonProtocolContext();
    }
}
