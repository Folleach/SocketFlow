using System.Threading;
using System.Threading.Tasks;

namespace SocketFlow.Server
{
    public class FlowServer<TKey, TClient> : ITransferUnit<TKey> where TClient : DestinationClientBase<TKey>
    {
        private readonly IBaseEndpoint[] baseEndpoints;
        private readonly FlowProtocolProcessor<TKey, TClient> processor;

        public int ClientsCount => processor.Count;

        public FlowServer(IBaseEndpoint[] baseEndpoints, FlowProtocolProcessor<TKey, TClient> processor)
        {
            this.baseEndpoints = baseEndpoints;
            this.processor = processor;
        }

        public async Task RunAsync(CancellationToken token)
        {
            var tasks = new Task[baseEndpoints.Length];
            for (var i = 0; i < tasks.Length; i++)
                tasks[i] = baseEndpoints[i].Start(token);

            await Task.WhenAll(tasks);
        }

        public FlowGroup<TKey, TClient> CreateGroup()
        {
            return new FlowGroup<TKey, TClient>();
        }

        public Task Send<TValue>(TKey key, TValue value)
        {
            return Task.CompletedTask;
        }
    }

    /*
        private readonly FlowBinder flowBinder;
        private readonly FlowOptions options;
        private readonly List<IModule> modules = new List<IModule>();
        private readonly FlowGroup clients;
        private readonly EventPipe<DestinationClient> ConnectedPipe = new();
        private readonly EventPipe<DestinationClient> DisconnectedPipe = new();

        public IInOut Clients { get; }

        public FlowServer(FlowOptions options = null)
        {
            this.options = options ?? new FlowOptions();
            flowBinder = new FlowBinder(this.options);
            clients = new FlowGroup(this);
            Clients = new InOut(clients, ConnectedPipe, DisconnectedPipe);
        }

        public void Bind<T>(int clientServerId, Action<DestinationClient, T> handler)
        {
            if (clientServerId < 0)
                throw new Exception("Negative ids are reserved for SocketFlow");
            flowBinder.Bind<T>(clientServerId, handler);
        }

        public FlowServer UseModule(IModule module)
        {
            modules.Add(module);
            return this;
        }

        public FlowServer UsingWrapper<T>(IDataWrapper<T> wrapper)
        {
            flowBinder.Using(wrapper);
            return this;
        }

        public FlowServer Start()
        {
            foreach (var module in modules)
                module.Start();
            return this;
        }

        public FlowServer Stop()
        {
            foreach (var module in modules)
                module.Stop();
            return this;
        }

        public void Broadcast<T>(int serverClientId, T value)
        {
            clients.Send(serverClientId, value);
        }

        public bool ContainsClient(DestinationClient client)
        {
            return clients.Contains(client);
        }

        public IDisposable OnConnected(IObserver<DestinationClient> observer) => ConnectedPipe.Subscribe(observer);
        public IDisposable OnDisconnected(IObserver<DestinationClient> observer) => DisconnectedPipe.Subscribe(observer);

        internal byte[] GetData<T>(T value)
        {
            var wrapper = flowBinder.GetWrapper<T>();
            return wrapper.DataWrapper.FormatObject(value);
        }

        internal void ReceivedData(DestinationClient client, int clientServerId, byte[] data)
        {
            var wrapper = flowBinder.GetWrapper(clientServerId);
            var handler = flowBinder.GetHandler(clientServerId);
            handler.Method.Invoke(handler.Target, new[]
            {
                client,
                wrapper.DataWrapper.FormatRaw(data)
            });
        }
     */
}
