using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics.Runtime;
using FubuMVC.Core.Diagnostics.Runtime.Tracing;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Urls;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Diagnostics.Tests.Runtime
{
    [TestFixture]
    public class DiagnosticsRegistryTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry();
            registry.Import<DiagnosticsRegistration>();

            graph = BehaviorGraph.BuildFrom(registry);
            urls = MockRepository.GenerateMock<IUrlRegistry>();

            graph.Behaviors.Any().ShouldBeTrue();
        }

        #endregion

        private BehaviorGraph graph;
        private IUrlRegistry urls;


        [Test]
        public void request_history_cache_is_registered()
        {
            graph.Services.DefaultServiceFor<IRequestHistoryCache>()
                .Type.ShouldEqual(typeof (RequestHistoryCache));

            ServiceRegistry.ShouldBeSingleton(typeof (RequestHistoryCache))
                .ShouldBeTrue();
        }

        [Test]
        public void RequestTrace_is_registered()
        {
            graph.Services.DefaultServiceFor<IRequestTrace>().Type.ShouldEqual(typeof (RequestTrace));
        }

        [Test]
        public void RequestLogBuilder_is_registered()
        {
            graph.Services.DefaultServiceFor<IRequestLogBuilder>().Type.ShouldEqual(typeof(RequestLogBuilder));
        }
    }

    public class WrappingBehavior : BasicBehavior
    {
        public WrappingBehavior(PartialBehavior partialBehavior)
            : base(partialBehavior)
        {
        }

        protected override DoNext performInvoke()
        {
            return DoNext.Continue;
        }
    }

    public class WrappingBehavior2 : WrappingBehavior
    {
        public WrappingBehavior2(PartialBehavior partialBehavior) : base(partialBehavior)
        {
        }
    }
}