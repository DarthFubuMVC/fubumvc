using System;
using Bottles;
using Bottles.Diagnostics;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Bootstrapping;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;
using StructureMap;
using System.Linq;
using Rhino.Mocks;
using FubuCore;

namespace FubuMVC.Tests.Assets
{
    [TestFixture]
    public class when_there_are_combination_policy_types_in_the_asset_graph : AssetCombinationBuildingActivatorContext
    {
        protected override void theContextIs()
        {
            theGraph.ApplyPolicy(typeof(CombineAllScriptFiles).AssemblyQualifiedName);
            theGraph.ApplyPolicy(typeof(CombineAllStylesheets).AssemblyQualifiedName);
        }

        [Test]
        public void should_register_both_of_the_combination_policies()
        {
            theContainer.GetAllInstances<ICombinationPolicy>().Select(x => x.GetType())
                .ShouldHaveTheSameElementsAs(typeof(CombineAllScriptFiles), typeof(CombineAllStylesheets));
        }
    }

    [TestFixture]
    public class when_there_are_asset_policy_types_in_the_asset_graph : AssetCombinationBuildingActivatorContext
    {
        protected override void theContextIs()
        {
            theGraph.ApplyPolicy(typeof(FakeAssetPolicy1).AssemblyQualifiedName);
            theGraph.ApplyPolicy(typeof(FakeAssetPolicy2).AssemblyQualifiedName);
        }

        [Test]
        public void should_register_both_of_the_asset_policy_classes()
        {
            theContainer.GetAllInstances<IAssetPolicy>().Select(x => x.GetType())
                .ShouldHaveTheSameElementsAs(typeof(FakeAssetPolicy1), typeof(FakeAssetPolicy2));
        }
    }

    [TestFixture]
    public class when_there_are_mixed_combination_and_asset_policty_types_registered_in_the_asset_graph : AssetCombinationBuildingActivatorContext
    {
        protected override void theContextIs()
        {
            theGraph.ApplyPolicy(typeof(FakeAssetPolicy1).AssemblyQualifiedName);
            theGraph.ApplyPolicy(typeof(FakeAssetPolicy2).AssemblyQualifiedName);

            theGraph.ApplyPolicy(typeof(CombineAllScriptFiles).AssemblyQualifiedName);
            theGraph.ApplyPolicy(typeof(CombineAllStylesheets).AssemblyQualifiedName);
        }

        [Test]
        public void should_register_both_of_the_asset_policy_classes()
        {
            theContainer.GetAllInstances<IAssetPolicy>().Select(x => x.GetType())
                .ShouldHaveTheSameElementsAs(typeof(FakeAssetPolicy1), typeof(FakeAssetPolicy2));
        }

        [Test]
        public void should_register_both_of_the_combination_policies()
        {
            theContainer.GetAllInstances<ICombinationPolicy>().Select(x => x.GetType())
                .ShouldHaveTheSameElementsAs(typeof(CombineAllScriptFiles), typeof(CombineAllStylesheets));
        }

        [Test]
        public void should_trace_the_discovery_of_an_asset_policy()
        {
            var traceMessage = "Registering {0} as an IAssetPolicy";
            MockFor<IPackageLog>().AssertWasCalled(x => x.Trace(traceMessage, typeof(FakeAssetPolicy1).FullName));
        }

        [Test]
        public void should_trace_the_discovery_of_a_combination_policy()
        {
            MockFor<IPackageLog>().AssertWasCalled(x => x.Trace("Registering {0} as an ICombinationPolicy",typeof (CombineAllScriptFiles).FullName ));
        }
    }

    public class FakeAssetPolicy1 : IAssetPolicy
    {
        public void Apply(IPackageLog log, IAssetPipeline pipeline, AssetGraph graph)
        {
            throw new NotImplementedException();
        }
    }

    public class FakeAssetPolicy2 : IAssetPolicy
    {
        public void Apply(IPackageLog log, IAssetPipeline pipeline, AssetGraph graph)
        {
            throw new NotImplementedException();
        }
    }

    
    public abstract class AssetCombinationBuildingActivatorContext : InteractionContext<AssetCombinationBuildingActivator>
    {
        protected AssetGraph theGraph;
        protected IContainer theContainer;

        protected override void beforeEach()
        {
            theContainer = new Container();
            Services.Inject<IContainerFacility>(new StructureMapContainerFacility(theContainer));

            theGraph = new AssetGraph();
            Services.Inject(theGraph);

            theContextIs();

            ClassUnderTest.Activate(new IPackageInfo[0], MockFor<IPackageLog>());
        }

        protected abstract void theContextIs();
    }
}