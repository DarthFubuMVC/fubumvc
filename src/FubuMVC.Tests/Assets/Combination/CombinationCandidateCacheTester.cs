using System.Collections.Generic;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Assets.Files;
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
            theCache.AddFiles(MimeTypeProvider.JAVASCRIPT, "js1", files(5));
            theCache.AddFiles(MimeTypeProvider.JAVASCRIPT, "js2", files(6));
            theCache.AddFiles(MimeTypeProvider.JAVASCRIPT, "js3", files(3));
            
            theCache.AddFiles(MimeTypeProvider.CSS, "css1", files(3));
            theCache.AddFiles(MimeTypeProvider.CSS, "css2", files(5));
            theCache.AddFiles(MimeTypeProvider.CSS, "css3", files(2));

            theCache.OrderedCombinationCandidatesFor(MimeTypeProvider.JAVASCRIPT)
                .Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("js2", "js1", "js3");

            theCache.OrderedCombinationCandidatesFor(MimeTypeProvider.CSS)
                .Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("css2", "css1", "css3");


        }
    }
}