using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.UI.Bootstrap.Collapsibles;
using FubuMVC.Diagnostics.Chains;
using FubuMVC.Diagnostics.Endpoints;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Diagnostics.Tests.Chains
{
    [TestFixture]
    public class ChainVisualizationTester
    {
        [Test]
        public void Title_if_there_is_a_route()
        {
            var chain = new RoutedChain("some/pattern");

            var visualization = new ChainVisualization()
            {
                Report = new RouteReport(chain, null, null),
                Chain = chain
            };

            visualization.Title.ShouldEqual("some/pattern");
        }

        [Test]
        public void Title_if_there_is_no_route_partial_with_controller_action()
        {
            var chain = new BehaviorChain();
            chain.IsPartialOnly = true;
            chain.AddToEnd(ActionCall.For<SomeEndpoint>(x => x.Go()));

            new ChainVisualization{
                Report = new RouteReport(chain, null, null),
                Chain = chain
            }.Title.ShouldEqual("SomeEndpoint.Go() : String");
        }

        [Test]
        public void Title_if_there_is_no_route_no_action_but_a_writer()
        {
            var chain = BehaviorChain.ForResource(typeof(Something));
            chain.Output.Add(new FakeWriter());

            new ChainVisualization
            {
                Report = new RouteReport(chain, null, null),
                Chain = chain
            }.Title.ShouldContain("Fake Writer");
        }

        [Test]
        public void show_nothing_for_the_route_if_there_is_nothing()
        {
            var chain = new BehaviorChain();

            var visualization = new ChainVisualization{
                Chain = chain
            };

            visualization.RouteTag.Render().ShouldBeFalse();
        }

        [Test]
        public void show_route_description_in_collapsed_body_for_a_route()
        {
            var chain = new RoutedChain("something");

            var visualization = new ChainVisualization
            {
                Chain = chain
            };

            var tag = visualization.RouteTag.As<CollapsibleTag>();

            tag.Render().ShouldBeTrue();
            tag.ToString().ShouldContain(new DescriptionBodyTag(Description.For(chain.Route)).ToString());

        }
    }

    [Title("Fake Writer")]
    public class FakeWriter : IMediaWriter<Something>
    {
        public void Write(string mimeType, IFubuRequestContext context, Something resource)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> Mimetypes
        {
            get
            {
                yield return MimeType.Html.Value;
            }
        }
    }

    public class Something{}

    public class SomeEndpoint
    {
        public string Go()
        {
            return "Hello.";
        }

        public string GoElse()
        {
            return "Hello.";
        }
    }
}