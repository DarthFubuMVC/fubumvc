using System.Collections.Generic;
using FubuMVC.Core.Assets.Content;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.Assets.Content
{
    [TestFixture]
    public class TransformerPolicyLibraryTester
    {
        private JavascriptTransformerPolicy<StubTransformer> globalJS;
        private JavascriptTransformerPolicy<StubTransformer> js1;
        private JavascriptTransformerPolicy<StubTransformer> js2;
        private JavascriptTransformerPolicy<StubTransformer> js3;
        private CssTransformerPolicy<StubTransformer> globalCSS;
        private CssTransformerPolicy<StubTransformer> css1;
        private CssTransformerPolicy<StubTransformer> less;
        private CssTransformerPolicy<StubTransformer> sass;
        private TransformerPolicyLibrary theLibrary;

        [SetUp]
        public void SetUp()
        {
            globalJS = new JavascriptTransformerPolicy<StubTransformer>(ActionType.Global);
            js1 = JavascriptTransformerPolicy<StubTransformer>.For(ActionType.Transformation, ".coffee");
            js2 = JavascriptTransformerPolicy<StubTransformer>.For(ActionType.Transformation, ".tr1", "tr2");
            js3 = JavascriptTransformerPolicy<StubTransformer>.For(ActionType.Generate, ".a");


            globalCSS = new CssTransformerPolicy<StubTransformer>(ActionType.Global);
            css1 = new CssTransformerPolicy<StubTransformer>(ActionType.Transformation, ".css");
            less = new CssTransformerPolicy<StubTransformer>(ActionType.Transformation, ".less");
            sass = new CssTransformerPolicy<StubTransformer>(ActionType.Transformation, ".sass");
            
            var policies = new List<ITransformerPolicy>(){
                globalJS,
                js1,
                js2,
                js3,
                globalCSS,
                css1,
                less,
                sass,
            };

            theLibrary = new TransformerPolicyLibrary(policies);
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

            theLibrary.FindPoliciesFor(assetFile).ShouldHaveTheSameElementsAs(js2, js1);
        }

        [Test]
        public void find_policies_for_an_asset_file_should_sort_matching_transforms()
        {
            // js3 is a generate action, which must come before the transformation of js2
            theLibrary.FindPoliciesFor(new AssetFile("s.tr1.a.js"))
                .ShouldHaveTheSameElementsAs(js3, js2);
        }

        [Test]
        public void find_policies_for_an_asset_file_should_return_all_matching_transforms_for_that_mimetype_but_excluding_the_globals_2()
        {
            var assetFile = new AssetFile("main.less.css");
            assetFile.MimeType.ShouldEqual(MimeType.Css);

            theLibrary.FindPoliciesFor(assetFile).ShouldHaveTheSameElementsAs(less, css1);
        }

        [Test]
        public void find_global_policies_for_a_mimetype()
        {
            theLibrary.FindGlobalPoliciesFor(MimeType.Javascript).ShouldHaveTheSameElementsAs(globalJS);
            theLibrary.FindGlobalPoliciesFor(MimeType.Css).ShouldHaveTheSameElementsAs(globalCSS);
        }
    }
}