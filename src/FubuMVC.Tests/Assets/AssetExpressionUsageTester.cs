using System;
using System.Collections.Generic;
using FubuMVC.Core;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Assets.Tags;
using FubuMVC.Core.Registration;
using HtmlTags;
using NUnit.Framework;
using FubuTestingSupport;
using System.Linq;

namespace FubuMVC.Tests.Assets
{
    [TestFixture]
    public class AssetExpressionUsageTester
    {
        [Test]
        public void should_use_the_trace_only_missing_handler_option_if_nothing_else_is_configured()
        {
            new FubuRegistry().BuildGraph().Services.DefaultServiceFor<IMissingAssetHandler>()
                .Type.ShouldEqual(typeof (TraceOnlyMissingAssetHandler));
        }

        [Test]
        public void YSOD_false()
        {
            var registry = new FubuRegistry();
            registry.Assets.YSOD_on_missing_assets(false);

            registry.BuildGraph().Services.DefaultServiceFor<IMissingAssetHandler>()
                .Type.ShouldEqual(typeof (TraceOnlyMissingAssetHandler));
        }

        [Test]
        public void YSOD_true()
        {
            var registry = new FubuRegistry();
            registry.Assets.YSOD_on_missing_assets(true);

            registry.BuildGraph().Services.DefaultServiceFor<IMissingAssetHandler>()
                .Type.ShouldEqual(typeof(YellowScreenMissingAssetHandler));
        }

        [Test]
        public void register_a_custom_missing_asset_handler()
        {
            var registry = new FubuRegistry();
            registry.Assets.HandleMissingAssetsWith<MyDifferentMissingAssetHandler>();

            registry.BuildGraph().Services.DefaultServiceFor<IMissingAssetHandler>()
                .Type.ShouldEqual(typeof(MyDifferentMissingAssetHandler));
        }

        [Test]
        public void apply_the_simplistic_asset_combination_approach()
        {
            var registry = new FubuRegistry();
            registry.Assets.CombineAllUniqueAssetRequests();

            registry.BuildGraph().Services.DefaultServiceFor<ICombinationDeterminationService>()
                .Type.ShouldEqual(typeof(CombineAllUniqueSetsCombinationDeterminationService)); 


        }

        [Test]
        public void register_a_combination_policy_with_CombineWith()
        {
            var registry = new FubuRegistry();
            registry.Assets
                .CombineWith<CombineAllScriptFiles>()
                .CombineWith<CombineAllStylesheets>();

            registry.BuildGraph().Services.ServicesFor(typeof(ICombinationPolicy))
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