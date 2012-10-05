using System;
using System.Collections.Generic;
using Bottles;
using Bottles.Diagnostics;
using FubuCore.Util;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Bootstrapping;
using FubuMVC.Core.Runtime;
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
    public class when_there_are_combination_candidates_registered : InteractionContext<AssetCombinationBuildingActivator>
    {
        private AssetGraph theGraph;
        private AssetCombinationCache theCache;

        protected override void beforeEach()
        {
            theGraph = new AssetGraph();
            Services.Inject(theGraph);

            theCache = new AssetCombinationCache();
            Services.Inject<IAssetCombinationCache>(theCache);

            Services.Inject<IAssetFileGraph>(new StubAssetFileGraph());

            theGraph.AddToCombination("combo1", "a.js");
            theGraph.AddToCombination("combo1", "b.js");
            theGraph.AddToCombination("combo1", "c.js");

            theGraph.AddToCombination("combo2", "a.js");
            theGraph.AddToCombination("combo2", "b.js");

            ClassUnderTest.Activate(new IPackageInfo[0], MockFor<IPackageLog>());
        }

        [Test]
        public void should_build_out_combination_candidates()
        {
            theCache.OrderedCombinationCandidatesFor(MimeType.Javascript)
                .Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("combo1", "combo2");
        }

        [Test]
        public void should_put_the_right_files_into_combo_candidates()
        {
            theCache.OrderedCombinationCandidatesFor(MimeType.Javascript)
                .First()
                .Files.Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("a.js", "b.js", "c.js");
        }
    }



    public class StubAssetFileGraph : IAssetFileGraph
    {
        private readonly Cache<string, AssetFile> _files = new Cache<string, AssetFile>(name => new AssetFile(name));

        public AssetFile Find(string path)
        {
            return _files[path];
        }

        public AssetPath AssetPathOf(AssetFile file)
        {
            throw new NotImplementedException();
        }

        public AssetFile FindByPath(string path)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AssetFile> AllFiles()
        {
            throw new NotImplementedException();
        }

        public AssetFile Find(AssetPath path)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PackageAssets> AllPackages
        {
            get { throw new NotImplementedException(); }
        }
    }


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
        public void Apply(IPackageLog log, IAssetFileGraph fileGraph, AssetGraph graph)
        {
            throw new NotImplementedException();
        }
    }

    public class FakeAssetPolicy2 : IAssetPolicy
    {
        public void Apply(IPackageLog log, IAssetFileGraph fileGraph, AssetGraph graph)
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