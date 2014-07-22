using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Katana;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.IntegrationTesting.Async
{
    [TestFixture]
    public class AsyncRouteIntegrationTester
    {
        private EmbeddedFubuMvcServer _server;

        [SetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry(x =>
            {
                x.Actions.IncludeType<AsyncAction>();
                x.Policies.Local.Add<EarlyReturnConvention>();
            });

            _server = FubuApplication .For(registry)
                .StructureMap(new Container())
                .RunEmbedded(port: PortFinder.FindPort(5500));
        }

        [TearDown]
        public void TearDown()
        {
            _server.Dispose();
        }

        [Test]
        public void when_invoke_chain_with_earlyReturnBehavior_then_httpResponse_should_complete()
        {
            var responseTask = Task.Factory.StartNew(() => _server.Endpoints.Get<AsyncAction>(x => x.AsyncCall()));
            responseTask.Wait(TimeSpan.FromSeconds(10)).ShouldBeTrue();
        }
    }

    public class AsyncAction
    {
        public Task<object> AsyncCall()
        {
            return null;
        }
    }

    public class EarlyReturnConvention : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Actions().Each(x => x.AddBefore(Wrapper.For<EarlyReturnBehavior>()));
        }
    }

    public class EarlyReturnBehavior : IActionBehavior
    {
        public IActionBehavior Inner { get; set; }

        public void Invoke()
        {
            //Don't call inner action
        }

        public void InvokePartial()
        {
            //Don't call inner action
        }
    }
}