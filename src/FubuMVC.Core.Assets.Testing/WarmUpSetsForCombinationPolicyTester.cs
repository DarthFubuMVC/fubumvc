using System.Collections.Generic;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Assets
{
    [TestFixture]
    public class WarmUpSetsForCombinationPolicyTester : InteractionContext<WarmUpSetsForCombinationPolicy>
    {
        private AssetGraph _graph;
        private IEnumerable<string> _assetsForSetA;
        private IEnumerable<string> _assetsForSetB;

        protected override void beforeEach()
        {
            _graph = new AssetGraph();

            _assetsForSetA = new[] { "a-1.js", "a-2.js" };
            _assetsForSetB = new[] { "b-1.css", "b-2.css" };

            _assetsForSetA.Each(x => _graph.AddToSet("setA", x));
            _assetsForSetB.Each(x => _graph.AddToSet("setB", x));

            _graph.CompileDependencies(null);
            MockFor<IAssetDependencyFinder>()
                .Stub(x => x.CompileDependenciesAndOrder(new[] { "setA" }))
                .Return(_assetsForSetA);
            MockFor<IAssetDependencyFinder>()
                .Stub(x => x.CompileDependenciesAndOrder(new[] { "setB" }))
                .Return(_assetsForSetB);

            ClassUnderTest.Apply(null, null, _graph);
        }

        [Test]
        public void plans_for_sets_are_generated()
        {
            MockFor<IAssetTagPlanCache>().AssertWasCalled(x => x.PlanFor(MimeType.Javascript, _assetsForSetA));
            MockFor<IAssetTagPlanCache>().AssertWasCalled(x => x.PlanFor(MimeType.Css, _assetsForSetB));
        }
    }
}