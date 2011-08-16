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
    public class CombinationCandidateCacheTester
    {
        private IEnumerable<AssetFile> files(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var file = new AssetFile(){
                    Name = "script" + i + ".js"
                };

                yield return file;
            }
        }

        [Test]
        public void add_candidates_and_retrieve_by_mime_type()
        {
            var theCache = new CombinationCandidateCache();
            theCache.AddFiles(MimeType.Javascript, "js1", files(5));
            theCache.AddFiles(MimeType.Javascript, "js2", files(6));
            theCache.AddFiles(MimeType.Javascript, "js3", files(3));
            
            theCache.AddFiles(MimeType.Css, "css1", files(3));
            theCache.AddFiles(MimeType.Css, "css2", files(5));
            theCache.AddFiles(MimeType.Css, "css3", files(2));

            theCache.OrderedCombinationCandidatesFor(MimeType.Javascript)
                .Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("js2", "js1", "js3");

            theCache.OrderedCombinationCandidatesFor(MimeType.Css)
                .Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("css2", "css1", "css3");


        }
    }
}