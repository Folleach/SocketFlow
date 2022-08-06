using System;
using System.Collections;
using System.Collections.Generic;

namespace SocketFlow.Server
{
    public class FlowGroup : IEnumerable<DestinationClient>
    {
        private readonly FlowServer server;
        private readonly HashSet<DestinationClient> clients = new HashSet<DestinationClient>();

        public int Count => clients.Count;

        public FlowGroup(FlowServer server)
        {
            this.server = server;
        }

        public FlowGroup(FlowServer server, params DestinationClient[] groupClients) : this(server)
        {
            foreach (var client in groupClients)
                UnsafeAdd(client);
        }


        public void Add(DestinationClient client)
        {
            if (!server.ContainsClient(client))
                throw new Exception("Client is located on another server");
            if (Contains(client))
                return;
            clients.Add(client);
        }
        
        internal void UnsafeAdd(DestinationClient client)
        {
            clients.Add(client);
        }

        public void Remove(DestinationClient client)
        {
            clients.Remove(client);
        }

        public bool Contains(DestinationClient client)
        {
            return clients.Contains(client);
        }

        public void Send<T>(int serverClientId, T value)
        {
            if (serverClientId < 0)
                throw new Exception("Negative ids are reserved for SocketFlow");
            var data = server.GetData(value);
            foreach (var client in clients)
                client.Send(serverClientId, data);
        }

        public IEnumerator<DestinationClient> GetEnumerator()
        {
            return clients.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return clients.GetEnumerator();
        }
    }
}