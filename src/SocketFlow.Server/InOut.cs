using System;

namespace SocketFlow.Server
{
    public class InOut : IInOut
    {
        private readonly FlowGroup clients;
        private readonly IObserver<DestinationClient> connected;
        private readonly IObserver<DestinationClient> disconnected;

        public InOut(FlowGroup clients, IObserver<DestinationClient> connected, IObserver<DestinationClient> disconnected)
        {
            this.clients = clients;
            this.connected = connected;
            this.disconnected = disconnected;
        }

        public void Connect(DestinationClient client)
        {
            clients.UnsafeAdd(client);
            connected?.OnNext(client);
        }

        public void Disconnect(DestinationClient client)
        {
            clients.Remove(client);
            disconnected?.OnNext(client);
        }
    }
}
