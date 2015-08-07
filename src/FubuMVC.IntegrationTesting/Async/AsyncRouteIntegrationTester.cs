using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Http.Hosting;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Async
{
    [TestFixture]
    public class AsyncRouteIntegrationTester
    {
        private FubuRuntime _server;

        [SetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry(x =>
            {
                x.Actions.IncludeType<AsyncAction>();
                x.Policies.Local.Add<EarlyReturnConvention>();
                x.HostWith<Katana>();
            });

            _server = registry.ToRuntime();
        }

        [TearDown]
        public void TearDown()
        {
            _server.Dispose();
        }

        [Test]
        public void when_invoke_chain_with_earlyReturnBehavior_then_httpResponse_should_complete()
        {
            var responseTask = Task.Factory.StartNew(() => _server.Scenario(x =>
            {
                x.Get.Action<AsyncAction>(_ => _.AsyncCall());
            }));

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