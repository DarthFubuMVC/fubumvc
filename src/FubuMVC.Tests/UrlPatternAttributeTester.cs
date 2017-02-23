using FubuMVC.Core;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests
{
    
    public class UrlPatternAttributeTester
    {
        [Fact]
        public void build_route_with_a_single_method_restrictions()
        {
            var attribute = new UrlPatternAttribute("GET::foo");
            var route = attribute.BuildRoute(null);
            route.Pattern.ShouldBe("foo");
            route.AllowedHttpMethods.ShouldHaveTheSameElementsAs("GET");
        }

        [Fact]
        public void build_route_with_multiple_method_restrictions()
        {
            var attribute = new UrlPatternAttribute("GET,post,PUT::foo");
            var route = attribute.BuildRoute(null);
            route.Pattern.ShouldBe("foo");
            route.AllowedHttpMethods.ShouldHaveTheSameElementsAs("GET", "POST", "PUT");
        }

        [Fact]
        public void GetAttribute()
        {
            var attribute = new GetAttribute("foo");
            var route = attribute.BuildRoute(null);
            route.Pattern.ShouldBe("foo");
            route.AllowedHttpMethods.ShouldHaveTheSameElementsAs("GET");
        }

        [Fact]
        public void PostAttribute()
        {
            var attribute = new PostAttribute("foo");
            var route = attribute.BuildRoute(null);
            route.Pattern.ShouldBe("foo");
            route.AllowedHttpMethods.ShouldHaveTheSameElementsAs("POST");
        }


        [Fact]
        public void PutAttribute()
        {
            var attribute = new PutAttribute("foo");
            var route = attribute.BuildRoute(null);
            route.Pattern.ShouldBe("foo");
            route.AllowedHttpMethods.ShouldHaveTheSameElementsAs("PUT");
        }



        [Fact]
        public void DeleteAttribute()
        {
            var attribute = new DeleteAttribute("foo");
            var route = attribute.BuildRoute(null);
            route.Pattern.ShouldBe("foo");
            route.AllowedHttpMethods.ShouldHaveTheSameElementsAs("DELETE");
        }
    }
}