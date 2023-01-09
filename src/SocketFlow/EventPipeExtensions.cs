using System;

namespace SocketFlow
{
    public static class EventPipeExtensions
    {
        public static IDisposable Subscribe<T>(this EventPipe<T> pipe, Action<T> onNext)
        {
            return pipe.Subscribe(new AnonymousObserver<T>(onNext));
        }
    }
}
