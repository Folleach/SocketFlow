using System;
using System.Net;
using System.Net.Sockets;

namespace SocketFlow.Server.Modules
{
    public class TcpModule : IModule
    {
        private const int Backlog = 20;
        private readonly IPAddress address;
        private readonly int port;
        private readonly TcpListener listener;
        private SocketFlowServer owner;
        private bool working = false;

        public TcpModule(IPAddress address, int port)
        {
            this.address = address;
            this.port = port;
            listener = new TcpListener(address, port);
        }

        public void Initialize(SocketFlowServer server)
        {
            if (owner != null)
                throw new Exception("Module already initialized. Maybe you called the 'Initialize(SocketFlowServer)' method yourself?");
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
            while (working)
            {
                var client = await listener.AcceptTcpClientAsync();
                var destinationClient = new DestinationClient(new TcpProtocol(client), owner, client.Client.RemoteEndPoint);
                owner.ConnectMe(destinationClient);;
            }
        }
    }
}
