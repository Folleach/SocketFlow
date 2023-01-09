using System;
using System.Net;
using System.Net.Sockets;

namespace SocketFlow.Server.Modules
{
    public class TcpModule : IModule
    {
        private const int Backlog = 20;
        private readonly TcpListener listener;
        private readonly FlowServer owner;
        private bool working = false;

        public TcpModule(FlowServer server, IPAddress address, int port)
        {
            owner = server ?? throw new ArgumentException("Can not be null", nameof(server));
            listener = new TcpListener(address, port);
        }

        public void Start()
        {
            listener.Start(Backlog);
            working = true;
            AcceptHandler();
        }

        public void Stop()
        {
            working = false;
            listener.Stop();
        }

        private async void AcceptHandler()
        {
            try
            {
                while (working)
                {
                    var client = await listener.AcceptTcpClientAsync();
                    var destinationClient = new DestinationClient(new TcpProtocol(client), owner, client.Client.RemoteEndPoint);
                    owner.Clients.Connect(destinationClient);
                    destinationClient.Run();
                }
            }
            catch (ObjectDisposedException exception)
            {
                if (working)
                    throw;
            }
        }
    }
}
