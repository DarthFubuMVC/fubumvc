using FubuCore;
using FubuMVC.Core.Resources.Hypermedia;
using FubuMVC.Core.Urls;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Resources.Hypermedia
{
    
    public class LinkExpressionTester
    {
        [Fact]
        public void create_a_link()
        {
            var expression = new LinkExpression(x => x.UrlFor(new SomeLinkInput()))
                .Rel("some rel");
            var urls = new StubUrlRegistry();

            var link = expression.As<ILinkCreator>().CreateLink(urls);

            link.Url.ShouldBe(urls.UrlFor(new SomeLinkInput()));

            link.Rel.ShouldBe("some rel");
        }
    }

    public class SomeLinkInput
    {
        
    }
}