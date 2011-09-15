using FubuMVC.Core.Projections;
using FubuMVC.Core.Urls;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.Projections
{
    [TestFixture]
    public class UrlProjectionTester
    {
        [Test]
        public void project_a_value()
        {
            var node = new InMemoryMediaNode();
            var urls = new StubUrlRegistry();
            var target = new SimpleProjectionTarget(new Address(), urls);

            var projection = new UrlProjection(r => r.Urls.UrlFor(new Address()), "url");

            projection.WriteValue(target, node);

            node.PropFor("url").ShouldEqual(urls.UrlFor(new Address()));
        }
    }
}