using System;
using System.Net;
using SocketFlow.Builders;
using SocketFlow.Tcp;

namespace SocketFlow;

public static class ProtocolStackExtensions
{
    public static StreamFlowProtocolConfigurator<TKey, T> UseTcpBaseEndpoint<TKey, T>(this ProtocolStackConfigurator<TKey, T> configurator, IPAddress address, int port)
    {
        configurator.BindToBase(new TcpBaseEndpoint(address, port));
        return new StreamFlowProtocolConfigurator<TKey, T>(configurator);
    }

    public static DatagramFlowProtocolConfigurator UseUdpBaseEndpoint<TKey, T>(this ProtocolStackConfigurator<TKey, T> configurator)
    {
        // todo: add udp
        throw new NotImplementedException();
    }
}
