using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SocketFlow.Tests
{
    public class ConcurrentRunner
    {
        private List<Exception> exceptions = new List<Exception>();
        private List<Thread> threads = new List<Thread>();
        
        public void Case(Action action)
        {
            threads.Add(new Thread(() =>
            {
                try
                {
                    action();
                }
                catch (Exception exception)
                {
                    Fail(exception);
                }
            }));
        }

        public void Run()
        {
            threads.ForEach(x => x.Start());
            threads.ForEach(x => x.Join());
        }

        private void Fail(Exception exception)
        {
            lock (exceptions)
                exceptions.Add(exception);
        }

        public IEnumerable<Exception> GetExceptions() => exceptions ?? Enumerable.Empty<Exception>();
    }
}