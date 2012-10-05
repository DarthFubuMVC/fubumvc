using System.Collections.Generic;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuMVC.Tests.Assets.Combination
{
    [TestFixture]
    public class AssetCombinationCacheTester
    {
        private IEnumerable<AssetFile> files(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var file = new AssetFile("script" + i + ".js");

                yield return file;
            }
        }

        [Test]
        public void add_candidates_and_retrieve_by_mime_type()
        {
            var theCache = new AssetCombinationCache();
            theCache.AddFilesToCandidate(MimeType.Javascript, "js1", files(5));
            theCache.AddFilesToCandidate(MimeType.Javascript, "js2", files(6));
            theCache.AddFilesToCandidate(MimeType.Javascript, "js3", files(3));

            theCache.AddFilesToCandidate(MimeType.Css, "css1", files(3));
            theCache.AddFilesToCandidate(MimeType.Css, "css2", files(5));
            theCache.AddFilesToCandidate(MimeType.Css, "css3", files(2));

            theCache.OrderedCombinationCandidatesFor(MimeType.Javascript)
                .Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("js2", "js1", "js3");

            theCache.OrderedCombinationCandidatesFor(MimeType.Css)
                .Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("css2", "css1", "css3");


        }

        [Test]
        public void find_combination_in_the_underlying_storage()
        {
            var theCache = new AssetCombinationCache();
            var scriptCombo1 = new ScriptFileCombination("script1", files(5));
            theCache.StoreCombination(MimeType.Javascript, scriptCombo1);
            theCache.StoreCombination(MimeType.Javascript, new ScriptFileCombination("script2", files(6)));
            theCache.StoreCombination(MimeType.Javascript, new ScriptFileCombination("script3", files(7)));
            theCache.StoreCombination(MimeType.Javascript, new ScriptFileCombination("script4", files(4)));
        
        
            theCache.StoreCombination(MimeType.Css, new StyleFileCombination("css1", null, files(10)));
            theCache.StoreCombination(MimeType.Css, new StyleFileCombination("css2", null, files(8)));
            var cssCombo3 = new StyleFileCombination("css3", null, files(12));
            theCache.StoreCombination(MimeType.Css, cssCombo3);
            theCache.StoreCombination(MimeType.Css, new StyleFileCombination("css4", null, files(15)));


            theCache.FindCombination("script1").ShouldBeTheSameAs(scriptCombo1);
            theCache.FindCombination("css3").ShouldBeTheSameAs(cssCombo3);


            theCache.FindCombination("something that does not exist").ShouldBeNull();

        }

        [Test]
        public void store_combinations_and_pull_out_in_order_by_size()
        {
            var theCache = new AssetCombinationCache();
            theCache.StoreCombination(MimeType.Javascript, new ScriptFileCombination("script1", files(5)));
            theCache.StoreCombination(MimeType.Javascript, new ScriptFileCombination("script2", files(6)));
            theCache.StoreCombination(MimeType.Javascript, new ScriptFileCombination("script3", files(7)));
            theCache.StoreCombination(MimeType.Javascript, new ScriptFileCombination("script4", files(4)));
        
        
            theCache.StoreCombination(MimeType.Css, new StyleFileCombination("css1", null, files(10)));
            theCache.StoreCombination(MimeType.Css, new StyleFileCombination("css2", null, files(8)));
            theCache.StoreCombination(MimeType.Css, new StyleFileCombination("css3", null, files(12)));
            theCache.StoreCombination(MimeType.Css, new StyleFileCombination("css4", null, files(15)));

            theCache.OrderedListOfCombinations(MimeType.Javascript).Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("script3", "script2", "script1", "script4");

            theCache.OrderedListOfCombinations(MimeType.Css).Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("css4", "css3", "css1", "css2");
        }
    }
}