using System;
using System.Linq;
using FubuMVC.Core.Assets.JavascriptRouting;
using FubuMVC.Core.Registration.Nodes;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Assets
{
    [TestFixture]
    public class JavascriptRouterTester
    {
        [Test]
        public void add_route_happy_path()
        {
            var chain = new RoutedChain("foo");
            chain.RouteName = "bar";
            chain.Route.AddHttpMethodConstraint("PUT");

            var router = new MyJavascriptRouter();
            router.Add(chain);

            var route = router.Routes().Single();

            route.Name.ShouldEqual("bar");
            route.Method.ShouldEqual("PUT");
            route.Finder(null).ShouldBeTheSameAs(chain);
        }

        [Test]
        public void got_to_have_a_route_name()
        {
            var chain = new RoutedChain("foo");
            //chain.RouteName = "bar";
            chain.Route.AddHttpMethodConstraint("PUT");

            var router = new MyJavascriptRouter();
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() => {
                router.Add(chain);
            });
            
        }
        
        [Test]
        public void got_to_have_at_least_one_http_method()
        {
            var chain = new RoutedChain("foo");
            chain.RouteName = "bar";
            //chain.Route.AddHttpMethodConstraint("PUT");

            var router = new MyJavascriptRouter();
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() =>
            {
                router.Add(chain);
            });
        }
    }

    public class MyJavascriptRouter : JavascriptRouter
    {
        
    }
}