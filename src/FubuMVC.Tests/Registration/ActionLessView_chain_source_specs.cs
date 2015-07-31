using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Urls;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Registration
{
    [TestFixture]
    public class ActionLessView_chain_source_specs
    {
        [Test]
        public void classes_marked_as_ViewSubject_are_partials()
        {
            using (var runtime = FubuRuntime.Basic())
            {
                runtime.Behaviors.ChainFor(typeof(FooSubject)).IsPartialOnly.ShouldBeTrue();
                runtime.Behaviors.ChainFor(typeof(BarSubject)).IsPartialOnly.ShouldBeTrue();
                runtime.Behaviors.ChainFor(typeof(BazSubject)).ShouldBeNull();

                runtime.Behaviors.ChainFor(typeof(FooSubject)).OfType<OutputNode>().Any().ShouldBeTrue();
                runtime.Behaviors.ChainFor(typeof(RouteSubject)).OfType<OutputNode>().Any().ShouldBeTrue();


                runtime.Behaviors.ChainFor(typeof(BarSubject)).Tags.ShouldContain("ActionlessView");
                runtime.Behaviors.ChainFor(typeof(BarSubject)).UrlCategory.Category.ShouldBe(Categories.VIEW);

                runtime.Behaviors.ChainFor(typeof (RouteSubject))
                    .ShouldBeOfType<RoutedChain>()
                    .GetRoutePattern().ShouldBe("route1/{Name}");
            }
        }
    }

    [ViewSubject]
    public class FooSubject { }

    [ViewSubject]
    public class BarSubject { }
    public class BazSubject { }

    [UrlPattern("route1/{Name}")]
    public class RouteSubject
    {
        public string Name { get; set; }
    }
}