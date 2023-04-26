using SocketFlow.Tcp;

namespace SocketFlow
{
    public static class TlsConfigurator
    {
        public static ProtocolStackConfigurator<TKey, T> AddTls<TKey, T>(this ProtocolStackConfigurator<TKey, T> configurator)
        {
            configurator.Middlewares.Add(e => new TlsMiddleware(e));
            configurator.Middlewares.Add(e => new LogMiddleware(e));
            return configurator;
        }
    }
}
