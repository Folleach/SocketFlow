using System;

namespace SocketFlow
{
    public class IdSubscription : IDisposable
    {
        private readonly Action<Guid> action;
        private readonly Guid id;

        public IdSubscription(Action<Guid> action, Guid id)
        {
            this.action = action;
            this.id = id;
        }

        public void Dispose()
        {
            action(id);
        }
    }
}
