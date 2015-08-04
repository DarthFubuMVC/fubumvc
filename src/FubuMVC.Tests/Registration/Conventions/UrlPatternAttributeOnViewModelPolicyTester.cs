using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class UrlPatternAttributeOnViewModelPolicyTester
    {
        [Test]
        public void should_build_a_route_for_view_model_marked_with_UrlPattern()
        {
            var graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Configure(g =>
                {
                    var c = new RoutedChain(new RouteDefinition("foo/{Name}"), typeof (Foo), typeof (Foo));
                    c.AddToEnd(new OutputNode(typeof (Foo)));

                    g.AddChain(c);
                });
            });

            var chain = graph.Chains.First(x => x.ResourceType() == typeof (Foo))
                .As<RoutedChain>();
            chain.Route.Pattern.ShouldBe("foo/{Name}");

            chain.Route.Input.ShouldBeOfType<RouteInput<Foo>>()
                .RouteParameters.Single().Name.ShouldBe("Name");
        }
    }

    [UrlPattern("foo/{Name}")]
    public class Foo
    {
        public string Name { get; set; }
    }

    public class FooWriter : IMediaWriter<Foo>
    {
        public void Write(string mimeType, IFubuRequestContext context, Foo resource)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> Mimetypes { get; private set; }
    }
}