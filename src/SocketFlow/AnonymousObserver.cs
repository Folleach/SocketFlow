using System;

namespace SocketFlow
{
    public class AnonymousObserver<T> : IObserver<T>
    {
        private readonly Action<T> onNext;
        private readonly Action<Exception> onError;
        private readonly Action onCompleted;

        public AnonymousObserver(Action<T> onNext, Action<Exception> onError = null, Action onCompleted = null)
        {
            this.onNext = onNext ?? throw new ArgumentException("Can not be null", nameof(onNext));
            this.onError = onError;
            this.onCompleted = onCompleted;
        }
        
        public void OnCompleted() => onCompleted?.Invoke();

        public void OnError(Exception error) => onError?.Invoke(error);

        public void OnNext(T value) => onNext?.Invoke(value);
    }
}
