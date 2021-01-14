using System;

namespace SocketFlow.Tests.TestingObjects
{
    public class SomeBindWorker
    {
        public SomeBindWorker(FlowBinder binder)
        {
            binder.Bind<Planet>(1, (Action<Planet>)Console.WriteLine);
        }
    }
}
