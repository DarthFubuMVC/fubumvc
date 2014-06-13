using FubuMVC.Core;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class UrlPatternAttributeTester
    {
        [Test]
        public void build_route_with_a_single_method_restrictions()
        {
            var attribute = new UrlPatternAttribute("GET::foo");
            var route = attribute.BuildRoute(null);
            route.Pattern.ShouldEqual("foo");
            route.AllowedHttpMethods.ShouldHaveTheSameElementsAs("GET");
        }

        [Test]
        public void build_route_with_multiple_method_restrictions()
        {
            var attribute = new UrlPatternAttribute("GET,post,PUT::foo");
            var route = attribute.BuildRoute(null);
            route.Pattern.ShouldEqual("foo");
            route.AllowedHttpMethods.ShouldHaveTheSameElementsAs("GET", "POST", "PUT");
        }

        [Test]
        public void GetAttribute()
        {
            var attribute = new GetAttribute("foo");
            var route = attribute.BuildRoute(null);
            route.Pattern.ShouldEqual("foo");
            route.AllowedHttpMethods.ShouldHaveTheSameElementsAs("GET");
        }

        [Test]
        public void PostAttribute()
        {
            var attribute = new PostAttribute("foo");
            var route = attribute.BuildRoute(null);
            route.Pattern.ShouldEqual("foo");
            route.AllowedHttpMethods.ShouldHaveTheSameElementsAs("POST");
        }


        [Test]
        public void PutAttribute()
        {
            var attribute = new PutAttribute("foo");
            var route = attribute.BuildRoute(null);
            route.Pattern.ShouldEqual("foo");
            route.AllowedHttpMethods.ShouldHaveTheSameElementsAs("PUT");
        }



        [Test]
        public void DeleteAttribute()
        {
            var attribute = new DeleteAttribute("foo");
            var route = attribute.BuildRoute(null);
            route.Pattern.ShouldEqual("foo");
            route.AllowedHttpMethods.ShouldHaveTheSameElementsAs("DELETE");
        }
    }
}