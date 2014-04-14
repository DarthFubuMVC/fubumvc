using FubuCore;
using FubuMVC.Core.Resources.Hypermedia;
using FubuMVC.Core.Urls;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Resources.Hypermedia
{
    [TestFixture]
    public class LinkExpressionTester
    {
        [Test]
        public void create_a_link()
        {
            var expression = new LinkExpression(x => x.UrlFor(new SomeLinkInput()))
                .Rel("some rel");
            var urls = new StubUrlRegistry();

            var link = expression.As<ILinkCreator>().CreateLink(urls);

            link.Url.ShouldEqual(urls.UrlFor(new SomeLinkInput()));

            link.Rel.ShouldEqual("some rel");
        }
    }

    public class SomeLinkInput
    {
        
    }
}