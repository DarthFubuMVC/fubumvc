using System;
using System.Web;
using NUnit.Framework;
using System.Web.Routing;
using FubuTestingSupport;

namespace FubuMVC.OwinHost.Testing
{
    [TestFixture]
    public class Playing
    {
        private Route route(string url)
        {
            var r = new Route(url, new StubRouteHandler());
            RouteTable.Routes.Add(r);

            return r;
        }

        private RouteBase selectRouteFor(string url)
        {
            var context = new FakeHttpContext(url);
            var routeData = RouteTable.Routes.GetRouteData(context);

            return routeData.Route;
        }

        [SetUp]
        public void SetUp()
        {
            RouteTable.Routes.Clear();
        }

        [Test]
        public void try_to_get_a_route()
        {
            var r1 = route("f1/1");
            var r2 = route("f1/2");
            var r3 = route("f1/3");
            var r4 = route("f1/4");

            selectRouteFor("f1/1").ShouldBeTheSameAs(r1);
            selectRouteFor("f1/2").ShouldBeTheSameAs(r2);
            selectRouteFor("f1/3").ShouldBeTheSameAs(r3);
            selectRouteFor("f1/4").ShouldBeTheSameAs(r4);
        }
    }

    public class FakeHttpContext : HttpContextBase
    {
        private readonly string _url;

        public FakeHttpContext(string url)
        {
            _url = url;
        }

        public override HttpRequestBase Request
        {
            get { return new FakeHttpRequest(_url); }
        }
    }

    public class FakeHttpRequest : HttpRequestBase
    {
        private readonly string _url;

        public FakeHttpRequest(string url)
        {
            _url = url;
        }

        public override string AppRelativeCurrentExecutionFilePath
        {
            get { return "~/"; }
        }

        public override string PathInfo
        {
            get { return _url; }
        }
    }

    public class StubRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            throw new NotImplementedException();
        }
    }
}