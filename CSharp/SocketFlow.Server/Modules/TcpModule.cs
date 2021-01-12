using System;
using System.Net;
using System.Net.Sockets;

namespace SocketFlow.Server.Modules
{
    public class TcpModule : IModule
    {
        private const int Backlog = 20;
        private readonly TcpListener listener;
        private FlowServer owner;
        private bool working = false;

        public TcpModule(IPAddress address, int port)
        {
            listener = new TcpListener(address, port);
        }

        public void Initialize(FlowServer server)
        {
            if (owner != null)
                throw new Exception("Module already initialized. Maybe you called the 'Initialize(FlowServer)' method yourself?");
            owner = server;
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
                    owner.ConnectMe(destinationClient);
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
