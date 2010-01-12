using FubuMVC.Core.Diagnostics.HtmlWriting;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.Tests.Diagnostics
{
    [TestFixture]
    public class RouteColumnTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        [Test]
        public void write_route_column_when_the_route_does_not_exist()
        {
            var chain = new BehaviorChain();

            var tag = new HtmlTag("td");

            new RouteColumn().WriteBody(chain, tag);

            tag.Text().ShouldEqual(" -");
        }

        [Test]
        public void write_route_column_when_the_route_exists()
        {
            var chain = new BehaviorChain();
            chain.Route = new RouteDefinition("some/thing/else");

            var tag = new HtmlTag("td");

            new RouteColumn().WriteBody(chain, tag);

            tag.Text().ShouldEqual(chain.Route.Pattern);
        }
    }
}