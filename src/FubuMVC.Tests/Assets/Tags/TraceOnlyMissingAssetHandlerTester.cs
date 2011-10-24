using System.Linq;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Tags;
using FubuMVC.Tests.Urls;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Assets.Tags
{
    [TestFixture]
    public class TraceOnlyMissingAssetHandlerTester
    {
        [Test]
        public void return_a_tag_that_will_just_fail_with_404()
        {
            var subjects = new[]{
                new MissingAssetTagSubject("script1.js"),
                new MissingAssetTagSubject("script2.js"),
                new MissingAssetTagSubject("script3.js")
            };

            var tags = new TraceOnlyMissingAssetHandler(new StubCurrentHttpRequest{TheApplicationRoot = "http://myserver"}).BuildTagsAndRecord(subjects);
            tags.Count().ShouldEqual(3);

            tags.First().ToString().ShouldEqual("<script type=\"application/javascript\" src=\"http://myserver/missing/assets/script1.js\"></script>");
        }

        [Test]
        public void return_no_tags_for_no_missing_assets()
        {
            new TraceOnlyMissingAssetHandler(new StubCurrentHttpRequest { TheApplicationRoot = "http://myserver" }).BuildTagsAndRecord(new MissingAssetTagSubject[0])
                .Any().ShouldBeFalse();
        }
    }
}