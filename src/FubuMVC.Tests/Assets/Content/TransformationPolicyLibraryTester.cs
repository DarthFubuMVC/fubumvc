using System.Collections.Generic;
using FubuMVC.Core.Assets.Content;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.Assets.Content
{
    [TestFixture]
    public class TransformationPolicyLibraryTester
    {
        private JavascriptTransformationPolicy<StubTransformer> globalJS;
        private JavascriptTransformationPolicy<StubTransformer> js1;
        private JavascriptTransformationPolicy<StubTransformer> js2;
        private JavascriptTransformationPolicy<StubTransformer> js3;
        private CssTransformationPolicy<StubTransformer> globalCSS;
        private CssTransformationPolicy<StubTransformer> css1;
        private CssTransformationPolicy<StubTransformer> less;
        private CssTransformationPolicy<StubTransformer> sass;
        private TransformationPolicyLibrary theLibrary;

        [SetUp]
        public void SetUp()
        {
            globalJS = new JavascriptTransformationPolicy<StubTransformer>(ActionType.Global);
            js1 = JavascriptTransformationPolicy<StubTransformer>.For(ActionType.Transformation, ".coffee");
            js2 = JavascriptTransformationPolicy<StubTransformer>.For(ActionType.Transformation, ".tr1", "tr2");
            js3 = JavascriptTransformationPolicy<StubTransformer>.For(ActionType.Transformation, ".a");


            globalCSS = CssTransformationPolicy<StubTransformer>.For(ActionType.Global);
            css1 = CssTransformationPolicy<StubTransformer>.For(ActionType.Transformation, ".css");
            less = CssTransformationPolicy<StubTransformer>.For(ActionType.Transformation, ".less");
            sass = CssTransformationPolicy<StubTransformer>.For(ActionType.Transformation, ".sass");
            
            var policies = new List<ITransformationPolicy>(){
                globalJS,
                js1,
                js2,
                js3,
                globalCSS,
                css1,
                less,
                sass,
            };

            theLibrary = new TransformationPolicyLibrary(policies);
        }

        [Test]
        public void find_policies_for_an_asset_file_does_NOT_include_any_globals()
        {
            var assetFile = new AssetFile("script.coffee.js");
            assetFile.MimeType.ShouldEqual(MimeType.Javascript);

            theLibrary.FindPoliciesFor(assetFile).ShouldNotContain(globalJS);
        }

        [Test]
        public void find_policies_for_an_asset_file_should_return_all_matching_transforms_for_that_mimetype_but_excluding_the_globals()
        {
            var assetFile = new AssetFile("script.tr1.coffee.js");
            assetFile.MimeType.ShouldEqual(MimeType.Javascript);

            theLibrary.FindPoliciesFor(assetFile).ShouldHaveTheSameElementsAs(js1, js2);
        }

        [Test]
        public void find_policies_for_an_asset_file_should_return_all_matching_transforms_for_that_mimetype_but_excluding_the_globals_2()
        {
            var assetFile = new AssetFile("main.less.css");
            assetFile.MimeType.ShouldEqual(MimeType.Css);

            theLibrary.FindPoliciesFor(assetFile).ShouldHaveTheSameElementsAs(css1, less);
        }

        [Test]
        public void find_global_policies_for_a_mimetype()
        {
            theLibrary.FindGlobalPoliciesFor(MimeType.Javascript).ShouldHaveTheSameElementsAs(globalJS);
            theLibrary.FindGlobalPoliciesFor(MimeType.Css).ShouldHaveTheSameElementsAs(globalCSS);
        }
    }
}