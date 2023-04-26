using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SocketFlow.Protocols;

namespace SocketFlow.Tests.Protocols;

[TestFixture]
public class JsonStreamProtocolTests
{
    [Test]
    public void EmptyData()
    {
        var (protocol, context) = Create<string>();
        var input = "{\"method\":\"hello\",\"value\":{}}";

        var packets = protocol.Receive(context, input.AsHoldable()).ToArray();

        packets.Should().HaveCount(1);
        packets.First().Key.Should().Be("hello");
    }

    [Test]
    public void NotClosedPacket()
    {
        var (protocol, context) = Create<string>();
        var input = "{\"method\":\"hello\",\"value\":{}";

        var packets = protocol.Receive(context, input.AsHoldable()).ToArray();

        packets.Should().HaveCount(0);
    }

    [Test]
    public void SplitPacket()
    {
        var (protocol, context) = Create<string>();
        var input1 = "{\"method\":\"hello\",";
        var input2 = "\"value\":{}";

        var packets1 = protocol.Receive(context, input1.AsHoldable()).ToArray();
        var packets2 = protocol.Receive(context, input2.AsHoldable()).ToArray();

        packets1.Should().HaveCount(0);
        packets2.Should().HaveCount(1);
        packets2.First().Key.Should().Be("hello");
    }

    [Test]
    public void MultiplePackets()
    {
        var (protocol, context) = Create<string>();
        var input = "{\"method\":\"packet1\",\"params\":\"one\"}{\"method\":\"packet2\",\"params\":\"two\"}";

        var packets = protocol.Receive(context, input.AsHoldable()).ToArray();

        packets.Should().HaveCount(2);
        packets.Select(x => x.Key).Should().BeEquivalentTo(new List<string>()
        {
            "packet1",
            "packet2"
        });
        packets.Select(x => x.PacketData.AsString()).Should().BeEquivalentTo(new List<string>()
        {
            "one",
            "two"
        });
    }

    [Test]
    public void MultiplePacketsWithNewLine()
    {
        var (protocol, context) = Create<string>();
        var input = "{\"method\":\"packet1\",\"params\":\"one\"}\r\n{\"method\":\"packet2\",\"params\":\"two\"}";

        var packets = protocol.Receive(context, input.AsHoldable()).ToArray();

        packets.Should().HaveCount(2);
        packets.Select(x => x.Key).Should().BeEquivalentTo(new List<string>()
        {
            "packet1",
            "packet2"
        });
        packets.Select(x => x.PacketData.AsString()).Should().BeEquivalentTo(new List<string>()
        {
            "one",
            "two"
        });
    }

    public (JsonStreamProtocol<TKey> protocol, JsonProtocolContext) Create<TKey>()
    {
        var protocol = new JsonStreamProtocol<TKey>();
        return (protocol, protocol.CreateContext());
    }
}
