using System.Linq;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Assets.Combination
{
    [TestFixture]
    public class CombineAllStyleSheetsTester
    {
        [Test]
        public void mime_type_is_javascript_because_it_matters()
        {
            new CombineAllStylesheets().MimeType.ShouldEqual(MimeType.Css);
        }

        [Test]
        public void apply_to_an_asset_tag_plan_simple_condition_of_all_files()
        {
            var files = new AssetFile[]{
                new AssetFile("a.css"), 
                new AssetFile("b.css"), 
                new AssetFile("c.css"), 
                new AssetFile("d.css") 
            };

            var plan = new AssetTagPlan(MimeType.Css, files);

            var policy = new CombineAllStylesheets();

            var combo = policy.DetermineCombinations(plan).Single();

            combo.Files.ShouldHaveTheSameElementsAs(files);
        }

        [Test]
        public void skip_over_non_files_in_the_plan()
        {
            var files = new IAssetTagSubject[]{
                new AssetFile("a.css"),
                new AssetFile("b.css"),
                new MissingAssetTagSubject("something.wrong"),
                new AssetFile("c.css"),
                new AssetFile("d.css"),
                new MissingAssetTagSubject("else.wrong"),
                new AssetFile("e.css")
            };

            var plan = new AssetTagPlan(MimeType.Css);
            plan.AddSubjects(files);

            var combos = new CombineAllStylesheets().DetermineCombinations(plan);

            combos.Count().ShouldEqual(2);

            combos.First().Files.Select(x => x.Name).ShouldHaveTheSameElementsAs("a.css", "b.css");
            combos.Last().Files.Select(x => x.Name).ShouldHaveTheSameElementsAs("c.css", "d.css");

        }


        [Test]
        public void only_group_by_the_same_folder()
        {
            var files = new AssetFile[]{
                new AssetFile("a.css"), 
                new AssetFile("b.css"), 
                new AssetFile("f1/c.css"), 
                new AssetFile("f1/d.css"), 
                new AssetFile("e.css"), 
                new AssetFile("f2/f.css"), 
                new AssetFile("f2/g.css") 
            };

            var plan = new AssetTagPlan(MimeType.Css, files);

            var policy = new CombineAllStylesheets();
            var combos = policy.DetermineCombinations(plan);

            combos.Count().ShouldEqual(3);

            combos.ElementAt(0).Files.Select(x => x.Name).ShouldHaveTheSameElementsAs("a.css", "b.css");

            combos.ElementAt(1).Files.Select(x => x.Name).ShouldHaveTheSameElementsAs("f1/c.css", "f1/d.css");
            combos.ElementAt(1).ShouldBeOfType<StyleFileCombination>().Name.ShouldStartWith("f1/");

            combos.ElementAt(2).Files.Select(x => x.Name).ShouldHaveTheSameElementsAs("f2/f.css", "f2/g.css");
            combos.ElementAt(2).ShouldBeOfType<StyleFileCombination>().Name.ShouldStartWith("f2/");
        }

    }
}