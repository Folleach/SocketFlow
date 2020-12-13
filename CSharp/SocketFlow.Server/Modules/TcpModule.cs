using System;
using System.Net;
using System.Net.Sockets;

namespace SocketFlow.Server.Modules
{
    public class TcpModule<T> : IModule<T>
    {
        private const int Backlog = 20;
        private readonly IPAddress address;
        private readonly int port;
        private readonly TcpListener listener;
        private bool working = false;
        private SocketFlowServer<T> currentServer;

        public TcpModule(IPAddress address, int port)
        {
            this.address = address;
            this.port = port;
            listener = new TcpListener(address, port);
        }

        public void Initialize(SocketFlowServer<T> server)
        {
            if (working)
                throw new Exception("This module already initialized");
            currentServer = server;
            listener.Start(Backlog);
            working = true;
            AcceptHandler();
        }

        public void Finalize(SocketFlowServer<T> server)
        {
            working = false;
            listener.Stop();
        }

        private async void AcceptHandler()
        {
            while (working)
            {
                var client = await listener.AcceptTcpClientAsync();
                var destinationClient = new DestinationClient<T>(new TcpProtocol(client), currentServer, client.Client.RemoteEndPoint);
                currentServer.ConnectMe(destinationClient);;
            }
        }
    }
}
