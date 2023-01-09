using System;

namespace SocketFlow
{
    public class Subscription : IDisposable
    {
        public bool Subscribed => unsubscribe != null;

        private Action unsubscribe;

        public Subscription(Action unsubscribe)
        {
            this.unsubscribe = unsubscribe;
        }
        
        public void Dispose()
        {
            var action = unsubscribe;
            unsubscribe = null;
            action?.Invoke();
        }
    }
}
