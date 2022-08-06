using System;
using FluentAssertions;
using NUnit.Framework;
using SocketFlow.DataWrappers;
using SocketFlow.Tests.TestingObjects;

namespace SocketFlow.Tests
{
    [TestFixture]
    public class FlowBinderTests
    {
        [SetUp]
        public void SetUp()
        {
            binder = new FlowBinder(new FlowOptions());
        }

        private FlowBinder binder;

        private void TestActionForThisContext()
        {
        }

        [Test]
        public void Bind_NoWrapper_BindShouldBeBindActionTwiceAndMore()
        {
            Action<Planet> action = Console.WriteLine;
            var wrapper = new JsonDataWrapper<Planet>();
            binder.Using(wrapper);

            Assert.DoesNotThrow(() => binder.Bind<Planet>(1, action));
            Assert.DoesNotThrow(() => binder.Bind<Planet>(1, (Action)TestActionForThisContext));
            Assert.DoesNotThrow(() => binder.Bind<Planet>(1, action));
            Assert.DoesNotThrow(() => binder.Bind<Planet>(1, (Action)TestActionForThisContext));
        }

        [Test]
        public void GetWrapper_OneWrapper_ShouldBeReturnCorrectWrapper()
        {
            Action<Planet> action = Console.WriteLine;
            var wrapper = new JsonDataWrapper<Planet>();
            binder.Using(wrapper);
            binder.Bind<Planet>(1, action);

            var actualWrapperInfo = binder.GetWrapper(1);

            Assert.AreEqual(typeof(Planet), actualWrapperInfo.Type);
            Assert.AreEqual(wrapper, actualWrapperInfo.DataWrapper);
        }

        [Test]
        public void GetHandler_OneWrapper_ShouldBeReturnCorrectHandlerMethod()
        {
            Action<Planet> action = Console.WriteLine;
            var wrapper = new JsonDataWrapper<Planet>();
            binder.Using(wrapper);
            binder.Bind<Planet>(1, action);

            var actualHandlerInfo = binder.GetHandler(1);

            Assert.AreEqual(action.Method, actualHandlerInfo.Method);
        }

        [Test]
        public void GetHandler_OneWrapper_ShouldBeReturnCorrectHandlerTarget()
        {
            Action action = TestActionForThisContext;
            var wrapper = new JsonDataWrapper<Planet>();
            binder.Using(wrapper);
            binder.Bind<Planet>(1, action);

            var actualHandlerInfo = binder.GetHandler(1);

            Assert.AreEqual(this, actualHandlerInfo.Target);
        }

        [Test]
        public void Bind_OneWrapper_PossibleToMemoryLeakOnReBind()
        {
            var wrapper = new JsonDataWrapper<Planet>();
            binder.Using(wrapper);

            var memoryBefore = GC.GetTotalMemory(true);
            for (var i = 0; i < 10000000; ++i)
                new SomeBindWorker(binder);
            var memoryAfter = GC.GetTotalMemory(true);

            var diff = memoryAfter / (double)memoryBefore;

            diff.Should().BeInRange(0, 1.5);
        }

        [Test]
        public void F_NoWrapper_ShouldBeThrow()
        {
            Assert.Throws<Exception>(() => binder.GetWrapper(1));
            Assert.Throws<Exception>(() => binder.GetWrapper<Planet>());
            Assert.Throws<Exception>(() => binder.GetHandler(1));
            Assert.Throws<Exception>(() => binder.Bind<Planet>(1, (Action)Console.WriteLine));
        }

        [Test]
        public void Using_ConcurrentUsing()
        {
            var runner = new ConcurrentRunner();
            runner.Case(() => binder.Using(new JsonDataWrapper<Dummy1>()));
            runner.Case(() => binder.Using(new JsonDataWrapper<Dummy2>()));
            runner.Case(() => binder.Using(new JsonDataWrapper<Dummy3>()));
            runner.Case(() => binder.Using(new JsonDataWrapper<Dummy4>()));
            runner.Case(() => binder.Using(new JsonDataWrapper<Dummy5>()));
            runner.Case(() => binder.Using(new JsonDataWrapper<Dummy6>()));
            runner.Case(() => binder.Using(new JsonDataWrapper<Dummy7>()));
            runner.Case(() => binder.Using(new JsonDataWrapper<Dummy8>()));
            runner.Case(() => binder.Using(new JsonDataWrapper<Dummy1>()));
            runner.Case(() => binder.Using(new JsonDataWrapper<Dummy2>()));
            runner.Case(() => binder.Using(new JsonDataWrapper<Dummy3>()));
            runner.Case(() => binder.Using(new JsonDataWrapper<Dummy4>()));
            runner.Case(() => binder.Using(new JsonDataWrapper<Dummy5>()));
            runner.Case(() => binder.Using(new JsonDataWrapper<Dummy6>()));
            runner.Case(() => binder.Using(new JsonDataWrapper<Dummy7>()));
            runner.Case(() => binder.Using(new JsonDataWrapper<Dummy8>()));
            runner.Run();
            
            foreach (var exception in runner.GetExceptions())
                Assert.Fail(exception.Message);
        }
    }
}
