using FubuCore.Util;
using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuMVC.Tests.Assets.Combination
{
    [TestFixture]
    public class CombinationCandidateTester
    {
        private readonly Cache<string, AssetFile> _files = new Cache<string, AssetFile>(name => new AssetFile(name));
        private MimeType theCurrentMimeType = MimeType.Javascript;

        [SetUp]
        public void SetUp()
        {
            _files.ClearAll();
            theCurrentMimeType = MimeType.Javascript;
            
        }

        private AssetTagPlan buildPlan(params string[] names)
        {
            return new AssetTagPlan(theCurrentMimeType, names.Select(x => _files[x]));
        }

        private CombinationCandidate buildCandidate(params string[] names)
        {
            return new CombinationCandidate(theCurrentMimeType, "theCombo", names.Select(x => _files[x]));
        }

        [Test]
        public void negative_match_because_the_mimetype_is_all_wrong()
        {
            var assetFiles = new []{_files["a"], _files["b"]};
            var thePlan = new AssetTagPlan(MimeType.Css, assetFiles);
            var theCandidate = new CombinationCandidate(MimeType.Javascript, "c1", assetFiles);

            theCandidate.DetermineCombinations(thePlan).Any().ShouldBeFalse();
        }

        [Test]
        public void negative_match_with_the_files_not_being_consecutive()
        {
            var thePlan = buildPlan("a", "b", "c", "d", "e");
            var theCandidate = buildCandidate("b", "d", "e");

            theCandidate.DetermineCombinations(thePlan).Any().ShouldBeFalse();
        }

        [Test]
        public void positive_match_with_the_exact_order()
        {
            var thePlan = buildPlan("a", "b", "c", "d", "e");
            var theCandidate = buildCandidate("b", "c", "d");

            var theCombo = theCandidate.DetermineCombinations(thePlan).Single();
            theCombo.Name.ShouldEqual(theCandidate.Name);
            theCombo.Files.ShouldHaveTheSameElementsAs(_files["b"], _files["c"], _files["d"]);
        
        }

        [Test]
        public void positive_match_with_a_slightly_different_order_and_the_order_of_the_asset_plan_wins()
        {
            var thePlan = buildPlan("a", "b", "c", "d", "e");
            var theCandidate = buildCandidate("d", "c", "b");

            var theCombo = theCandidate.DetermineCombinations(thePlan).Single();
            theCombo.Name.ShouldEqual(theCandidate.Name);
            theCombo.Files.ShouldHaveTheSameElementsAs(_files["b"], _files["c"], _files["d"]);   
        }

        [Test]
        public void positive_match_with_a_different_order_2()
        {
            var thePlan = buildPlan("a", "b", "c", "d", "e");
            var theCandidate = buildCandidate("c", "d", "b");

            var theCombo = theCandidate.DetermineCombinations(thePlan).Single();
            theCombo.Name.ShouldEqual(theCandidate.Name);
            theCombo.Files.ShouldHaveTheSameElementsAs(_files["b"], _files["c"], _files["d"]);
        }

        [Test]
        public void builds_a_script_combination_when_the_mime_type_is_script()
        {
            theCurrentMimeType = MimeType.Javascript;
            var thePlan = buildPlan("a", "b", "c", "d", "e");
            var theCandidate = buildCandidate("c", "d", "b");

            var theCombo = theCandidate.DetermineCombinations(thePlan).Single();

            theCombo.MimeType.ShouldEqual(MimeType.Javascript);
            theCombo.ShouldBeOfType<ScriptFileCombination>();
        }

        [Test]
        public void builds_a_css_combination_when_the_mime_type_is_css()
        {
            theCurrentMimeType = MimeType.Css;
            var thePlan = buildPlan("a", "b", "c", "d", "e");
            var theCandidate = buildCandidate("c", "d", "b");

            var theCombo = theCandidate.DetermineCombinations(thePlan).Single();

            theCombo.MimeType.ShouldEqual(MimeType.Css);
            theCombo.ShouldBeOfType<StyleFileCombination>();
        }

        [Test]
        public void use_the_name_and_folder_for_the_css_combination()
        {
            theCurrentMimeType = MimeType.Css;
            var thePlan = buildPlan("a", "b", "c", "d", "e");
            var theCandidate = buildCandidate("c", "d", "b");
            theCandidate.Folder = "folder1";

            var theCombo = theCandidate.DetermineCombinations(thePlan).Single();

            theCombo.Name.ShouldEqual("folder1" + "/" + theCandidate.Name);
        }
    }
}