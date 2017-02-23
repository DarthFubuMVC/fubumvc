using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Tests.ServiceBus.Async
{
    public class AsyncRegistry : FubuRegistry
    {
        public AsyncRegistry()
        {
            ServiceBus.Enable(true);

            ServiceBus.EnableInMemoryTransport();

            Policies.Local.Add<WrapWithFoo>();
        }
    }

    public class WrapWithFoo : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Handlers.Each(x => x.InsertFirst(Wrapper.For<FooWrapper>()));
        }
    }

    public static class AsyncWatcher
    {
        private static readonly object _locker = new object();

        private static readonly IList<string> _messages = new List<string>();

        public static IEnumerable<string> Messages
        {
            get { return _messages; }
        }

        public static void Write(string message)
        {
            lock (_locker)
            {
                _messages.Add(message);
            }
        }

        public static void Clear()
        {
            _messages.Clear();
        }
    }

    public class Foo
    {
        public string Name;
    }

    public class FooWrapper : WrappingBehavior
    {
        protected override void invoke(Action action)
        {
            AsyncWatcher.Write("wrapper:start");
            action();
            AsyncWatcher.Write("wrapper:finish");
        }
    }

    public class AsyncTestHandler
    {
        public Task Go(Foo foo)
        {
            return Task.Factory.StartNew(() =>
            {
                Thread.Sleep(100);
                AsyncWatcher.Write("go:" + foo.Name);
            });
        }

        public Task Other(Foo foo)
        {
            return Task.Factory.StartNew(() =>
            {
                Thread.Sleep(100);
                AsyncWatcher.Write("other:" + foo.Name);
            });
        }
    }
}