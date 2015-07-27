using System;
using System.Collections.Generic;
using FubuCore.Binding;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.ServiceBus.Configuration;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Runtime.Invocation
{
    public class StubServiceFactory : IServiceFactory
    {
        private readonly HandlerChain _chain;
        private readonly IActionBehavior _behavior;
        private readonly object[] _cascadingMessages;
        public FubuMVC.Core.ServiceBus.Runtime.Invocation.InvocationContext Arguments;

        public StubServiceFactory(HandlerChain chain, IActionBehavior behavior, params object[] cascadingMessages)
        {
            _chain = chain;
            _behavior = behavior;
            _cascadingMessages = cascadingMessages;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IActionBehavior BuildBehavior(ServiceArguments arguments, Guid behaviorId)
        {
            Arguments = arguments.ShouldBeOfType<FubuMVC.Core.ServiceBus.Runtime.Invocation.InvocationContext>();
            _cascadingMessages.Each(x => Arguments.EnqueueCascading(x));

            _chain.UniqueId.ShouldBe(behaviorId);

            return _behavior;
        }

        public T Build<T>(ServiceArguments arguments)
        {
            throw new NotImplementedException();
        }

        public T Get<T>()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetAll<T>()
        {
            throw new NotImplementedException();
        }
    }
}