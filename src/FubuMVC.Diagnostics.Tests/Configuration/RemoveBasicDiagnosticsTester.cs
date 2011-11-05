using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Assets.Http;
using FubuMVC.Core.Registration;
using FubuMVC.Diagnostics.Core.Configuration;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Diagnostics.Tests.Configuration
{
    [TestFixture]
    public class RemoveBasicDiagnosticsTester : InteractionContext<RemoveBasicDiagnostics>
    {
        private BehaviorGraph theGraph;

        protected override void beforeEach()
        {
            var registry = new FubuRegistry();
            registry.IncludeDiagnostics(true);
            registry.Actions.IncludeType<Hello>();
            theGraph = registry.BuildGraph();

            ClassUnderTest.Configure(theGraph);
        }

        [Test]
        public void it_only_removes_relevant_internal_fubu_actions()
        {
            theGraph.Actions().Select(a => a.HandlerType).Distinct()
                .ShouldHaveCount(2)
                .ShouldHaveTheSameElementsAs(typeof(Hello), typeof(AssetWriter));
        }

        public class Hello
        {
            public string Hep()
            {
                return "Hep";
            }
        }
    }
}
