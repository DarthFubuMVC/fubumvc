using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Tags;
using FubuMVC.Core.Http;
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

    public class StubCurrentHttpRequest : ICurrentHttpRequest
    {
        public string TheRawUrl;
        public string TheRelativeUrl;
        public string TheApplicationRoot = "http://server";
        public string TheHttpMethod = "GET";
        public string StubFullUrl = "http://server/";

        public string RawUrl()
        {
            return TheRawUrl;
        }

        public string RelativeUrl()
        {
            return TheRelativeUrl;
        }

        public string FullUrl()
        {
            return StubFullUrl;
        }

        public string ToFullUrl(string url)
        {
            return url.ToAbsoluteUrl(TheApplicationRoot);
        }

        public string HttpMethod()
        {
            return TheHttpMethod;
        }
    }
}