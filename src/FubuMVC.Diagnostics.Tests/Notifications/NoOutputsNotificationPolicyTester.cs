using System.Linq;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Attachment;
using FubuMVC.Diagnostics.Core.Grids.Columns.Routes;
using FubuMVC.Diagnostics.Core.Grids.Filters.Routes;
using FubuMVC.Diagnostics.Notifications;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Diagnostics.Tests.Notifications
{
    [TestFixture]
    public class NoOutputsNotificationPolicyTester
    {
        private NoOutputsNotificationPolicy _policy;

        [SetUp]
        public void setup()
        {
            var graph = new FubuRegistry(registry =>
                                             {
                                                 registry.Applies.ToThisAssembly();
                                                 registry.Actions.IncludeType<Test>();
                                                 registry.Actions.IncludeMethods(method => method.Name == "Index" || method.Name == "Continuation");
                                             })
                .BuildLightGraph(new ViewBag(new IViewToken[0]));
            
            graph.AddChain(new BehaviorChain());

            _policy = new NoOutputsNotificationPolicy(graph, new ViewFilter(new ViewColumn()));
        }

        [Test]
        public void should_apply_when_chains_without_output_exist()
        {
            _policy
                .Applies()
                .ShouldBeTrue();
        }

        [Test]
        public void should_set_count_of_chains_without_output()
        {
            _policy
                .Build()
                .As<NoOutputsNotification>()
                .BehaviorCount
                .ShouldEqual(1);
        }

        public class Test
        {
            public Test Index()
            {
                return this;
            }

            public FubuContinuation Continuation()
            {
                return FubuContinuation.NextBehavior();
            }
        }
    }
}