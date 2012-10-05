using FubuMVC.Core.Assets;
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
        public void skip_over_non_files_in_the_plan()
        {
            var files = new IAssetTagSubject[]{
                new AssetFile("a.js"), 
                new AssetFile("b.js"), 
                new MissingAssetTagSubject("something.wrong"),
                new AssetFile("c.js"), 
                new AssetFile("d.js") ,
                new MissingAssetTagSubject("else.wrong"),
                new AssetFile("e.js")
            };

            var plan = new AssetTagPlan(MimeType.Javascript);
            plan.AddSubjects(files);

            var combos = new CombineAllScriptFiles().DetermineCombinations(plan);

            combos.Count().ShouldEqual(2);

            combos.First().Files.Select(x => x.Name).ShouldHaveTheSameElementsAs("a.js", "b.js");
            combos.Last().Files.Select(x => x.Name).ShouldHaveTheSameElementsAs("c.js", "d.js");



        }
    }
}