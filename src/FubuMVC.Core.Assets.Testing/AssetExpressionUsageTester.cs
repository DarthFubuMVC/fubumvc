using System;
using System.Collections.Generic;
using Bottles.Diagnostics;
using FubuMVC.Core;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Assets.Tags;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.DSL;
using HtmlTags;
using NUnit.Framework;
using FubuTestingSupport;
using System.Linq;
using Rhino.Mocks;
using FubuMVC.StructureMap;
using StructureMap;

namespace FubuMVC.Tests.Assets
{
    [TestFixture]
    public class using_the_dsl_to_register_assets
    {
        private IAssetRegistration theRegistration;
        private FubuRegistry theRegistry;
        private AssetsExpression theExpression;

        

        [SetUp]
        public void SetUp()
        {
            theRegistration = MockRepository.GenerateMock<IAssetRegistration>();
            theRegistry = new FubuRegistry();

            theExpression = new AssetsExpression(theRegistry, theRegistration);
        }

        [Test]
        public void create_an_alias()
        {
            theExpression.Alias("jquery").Is("jquery.1.4.2.js")
                .ShouldBeTheSameAs(theExpression);

            theRegistration.AssertWasCalled(x => x.Alias("jquery.1.4.2.js", "jquery"));
        }

        [Test]
        public void register_dependency_singular()
        {
            theExpression.Asset("jquery.forms.js").Requires("jquery.js")
                .ShouldBeTheSameAs(theExpression);

            theRegistration.AssertWasCalled(x => x.Dependency("jquery.forms.js", "jquery.js"));
        }

        [Test]
        public void register_dependency_multiples()
        {
            theExpression.Asset("crud.js").Requires("jquery.js, jquery.form.js");

            theRegistration.AssertWasCalled(x => x.Dependency("crud.js", "jquery.js"));
            theRegistration.AssertWasCalled(x => x.Dependency("crud.js", "jquery.form.js"));
        }

        [Test]
        public void register_extension()
        {
            theExpression.Asset("crud-extension.js").Extends("crud.js")
                .ShouldBeTheSameAs(theExpression);

            theRegistration.AssertWasCalled(x => x.Extension("crud-extension.js", "crud.js"));

        }

        [Test]
        public void register_a_set()
        {
            theExpression.AssetSet("A").Includes("a.js, b.js, c.js")
                .ShouldBeTheSameAs(theExpression);

            theRegistration.AssertWasCalled(x => x.AddToSet("A", "a.js"));
            theRegistration.AssertWasCalled(x => x.AddToSet("A", "b.js"));
            theRegistration.AssertWasCalled(x => x.AddToSet("A", "c.js"));
        }

        [Test]
        public void register_a_set_2()
        {
            theExpression.AssetSet("A").Includes("a.js,b.js,c.js")
                .ShouldBeTheSameAs(theExpression);

            theRegistration.AssertWasCalled(x => x.AddToSet("A", "a.js"));
            theRegistration.AssertWasCalled(x => x.AddToSet("A", "b.js"));
            theRegistration.AssertWasCalled(x => x.AddToSet("A", "c.js"));
        }

        [Test]
        public void register_an_ordered_set()
        {
            theExpression.OrderedSet("crud").Is("a.js, b.js, c.js, d.js")
                .ShouldBeTheSameAs(theExpression);

            theRegistration.AssertWasCalled(x => x.AddToSet("crud", "a.js"));
            theRegistration.AssertWasCalled(x => x.AddToSet("crud", "b.js"));
            theRegistration.AssertWasCalled(x => x.AddToSet("crud", "c.js"));
            theRegistration.AssertWasCalled(x => x.AddToSet("crud", "d.js"));

            theRegistration.AssertWasCalled(x => x.Dependency("b.js", "a.js"));
            theRegistration.AssertWasCalled(x => x.Dependency("c.js", "b.js"));
            theRegistration.AssertWasCalled(x => x.Dependency("d.js", "c.js"));
        }

        [Test]
        public void register_a_preceeding_relationship()
        {
            theExpression.Asset("a.js").Preceeds("b.js")
                .ShouldBeTheSameAs(theExpression);

            theRegistration.AssertWasCalled(x => x.Preceeding("a.js", "b.js"));
        }

        [Test]
        public void register_a_combination()
        {
            theExpression.Combination("something.js").Includes("a.js, b.js, c.js")
                .ShouldBeTheSameAs(theExpression);

            theRegistration.AssertWasCalled(x => x.AddToCombination("something.js", "a.js, b.js, c.js"));
        }

        [Test]
        public void should_register_the_recording_registration_as_a_policy_in_behavior_graph()
        {
            var registry = new FubuRegistry();
            registry.Assets().Asset("a.js").Requires("b.js");

            var graph = BehaviorGraph.BuildFrom(registry);

            var registration = graph.Services.ServicesFor<IAssetPolicy>()
                .Single().Value.ShouldBeOfType<RecordingAssetRegistration>();

            var inner = MockRepository.GenerateMock<IAssetRegistration>();
            registration.Replay(inner);

            inner.AssertWasCalled(x => x.Dependency("a.js", "b.js"));
        }


        [Test]
        public void using_the_configure_method()
        {
            var graph = new AssetGraph();
            var expression = new AssetsExpression(theRegistry, graph);

            expression.Configure(@"
crud includes a.js, b.js, c.js
");

            graph.CompileDependencies(new PackageLog());

            graph.AssetSetFor("crud").AllFileDependencies()
                .Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("a.js", "b.js", "c.js");
        }

    }

    [TestFixture]
    public class AssetExpressionUsageTester
    {
        [Test]
        public void should_use_the_trace_only_missing_handler_option_if_nothing_else_is_configured()
        {
            BehaviorGraph.BuildFrom(x => x.Import<AssetBottleRegistration>()).Services.DefaultServiceFor<IMissingAssetHandler>()
                .Type.ShouldEqual(typeof (TraceOnlyMissingAssetHandler));
        }

        [Test]
        public void YSOD_false()
        {
            var registry = new FubuRegistry();
            registry.Assets().YSOD_on_missing_assets(false);

            BehaviorGraph.BuildFrom(x => x.Import<AssetBottleRegistration>()).Services.DefaultServiceFor<IMissingAssetHandler>()
                .Type.ShouldEqual(typeof (TraceOnlyMissingAssetHandler));
        }

        [Test]
        public void YSOD_true()
        {
            var registry = new FubuRegistry();
            registry.Assets().YSOD_on_missing_assets(true);

            BehaviorGraph.BuildFrom(registry).Services.DefaultServiceFor<IMissingAssetHandler>()
                .Type.ShouldEqual(typeof(YellowScreenMissingAssetHandler));
        }

        [Test]
        public void register_a_custom_missing_asset_handler()
        {
            var registry = new FubuRegistry();
            registry.Assets().HandleMissingAssetsWith<MyDifferentMissingAssetHandler>();

            BehaviorGraph.BuildFrom(registry).Services.DefaultServiceFor<IMissingAssetHandler>()
                .Type.ShouldEqual(typeof(MyDifferentMissingAssetHandler));
        }

        [Test]
        public void apply_the_simplistic_asset_combination_approach()
        {
            var registry = new FubuRegistry();
            registry.Assets().CombineAllUniqueAssetRequests();

            BehaviorGraph.BuildFrom(registry).Services.DefaultServiceFor<ICombinationDeterminationService>()
                .Type.ShouldEqual(typeof(CombineAllUniqueSetsCombinationDeterminationService)); 


        }

        [Test]
        public void adds_a_warm_up_policy_for_asset_combinations()
        {
            var registry = new FubuRegistry();
            registry.Assets().CombineAllUniqueAssetRequests();

            BehaviorGraph.BuildFrom(registry).Services.ServicesFor<IAssetPolicy>()
                .ShouldContain(x => x.Type == typeof(WarmUpSetsForCombinationPolicy));
        }

        [Test]
        public void register_a_combination_policy_with_CombineWith()
        {
            var registry = new FubuRegistry();
            registry.Assets()
                .CombineWith<CombineAllScriptFiles>()
                .CombineWith<CombineAllStylesheets>();

            BehaviorGraph.BuildFrom(registry).Services.ServicesFor(typeof(ICombinationPolicy))
                .Select(x => x.Type).ShouldHaveTheSameElementsAs(typeof(CombineAllScriptFiles), typeof(CombineAllStylesheets));
        }


    }

    public class MyDifferentMissingAssetHandler : IMissingAssetHandler
    {
        public IEnumerable<HtmlTag> BuildTagsAndRecord(IEnumerable<MissingAssetTagSubject> subjects)
        {
            throw new NotImplementedException();
        }
    }
}