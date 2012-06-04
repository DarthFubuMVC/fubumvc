using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Urls;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Diagnostics
{
    [TestFixture]
    public class DiagnosticsRegistryTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            graph = new DiagnosticsSubSystem().BuildGraph();
            urls = MockRepository.GenerateMock<IUrlRegistry>();

            graph.Behaviors.Any().ShouldBeTrue();
            graph.Actions().Each(x => Debug.WriteLine(x.Description));
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