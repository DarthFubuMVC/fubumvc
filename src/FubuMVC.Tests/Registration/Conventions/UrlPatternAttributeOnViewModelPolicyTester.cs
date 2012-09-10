using System;
using System.Collections.Generic;
using FubuCore.Descriptions;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Resources.Conneg;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

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
                    g.AddChain(BehaviorChain.ForWriter(new FooWriter()));
                });
            });

            var chain = graph.Behaviors.First(x => x.ResourceType() == typeof (Foo));
            chain.Route.Pattern.ShouldEqual("foo/{Name}");

            chain.Route.Input.ShouldBeOfType<RouteInput<Foo>>()
                .RouteParameters.Single().Name.ShouldEqual("Name");
        }
    }

    [UrlPattern("foo/{Name}")]
    public class Foo
    {
        public string Name { get; set; }
    }

    public class FooWriter : WriterNode
    {
        public override Type ResourceType
        {
            get { return typeof(Foo); }
        }

        protected override ObjectDef toWriterDef()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<string> Mimetypes
        {
            get { throw new NotImplementedException(); }
        }

        protected override void createDescription(Description description)
        {
            throw new NotImplementedException();
        }
    }


}