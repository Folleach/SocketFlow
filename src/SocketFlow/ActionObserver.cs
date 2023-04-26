using System;

namespace SocketFlow
{
    public class ActionObserver<T> : IObserver<T>
    {
        public ActionObserver()
        {
            
        }
        
        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(T value)
        {
            throw new NotImplementedException();
        }
    }
}
