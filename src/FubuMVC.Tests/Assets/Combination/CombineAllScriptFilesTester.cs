using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;
using NUnit.Framework;
using FubuTestingSupport;
using System.Linq;

namespace FubuMVC.Tests.Assets.Combination
{
    [TestFixture]
    public class CombineAllScriptFilesTester
    {
        [Test]
        public void mime_type_is_javascript_because_it_matters()
        {
            new CombineAllScriptFiles().MimeType.ShouldEqual(MimeType.Javascript);
        }

        [Test]
        public void apply_to_an_asset_tag_plan_simple_condition_of_all_files()
        {
            var files = new AssetFile[]{
                new AssetFile("a.js"), 
                new AssetFile("b.js"), 
                new AssetFile("c.js"), 
                new AssetFile("d.js") 
            };

            var plan = new AssetTagPlan(MimeType.Javascript, files);

            var policy = new CombineAllScriptFiles();

            var combo = policy.DetermineCombinations(plan).Single();

            combo.Files.ShouldHaveTheSameElementsAs(files);
        }

        [Test]
        public void do_more_scenarios_here()
        {
            Assert.Fail("do.");
        }
    }
}