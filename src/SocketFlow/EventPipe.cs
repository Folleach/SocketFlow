using System;
using System.Collections.Generic;
using System.Threading;

namespace SocketFlow
{
    public class EventPipe<T> : IObservable<T>, IObserver<T>
    {
        private readonly List<IObserver<T>> observers = new List<IObserver<T>>();
        private readonly ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        public IDisposable Subscribe(IObserver<T> observer)
        {
            rwLock.EnterWriteLock();
            observers.Add(observer);
            rwLock.ExitWriteLock();
            return new Subscription(() =>
            {
                rwLock.EnterWriteLock();
                observers.Remove(observer);
                rwLock.ExitWriteLock();
            });
        }

        public void OnCompleted()
        {
            rwLock.EnterReadLock();
            foreach (var observer in observers)
                observer.OnCompleted();
            rwLock.ExitReadLock();
        }

        public void OnError(Exception error)
        {
            rwLock.EnterReadLock();
            foreach (var observer in observers)
                observer.OnError(error);
            rwLock.ExitReadLock();
        }

        public void OnNext(T value)
        {
            rwLock.EnterReadLock();
            foreach (var observer in observers)
                observer.OnNext(value);
            rwLock.ExitReadLock();
        }
    }
}
