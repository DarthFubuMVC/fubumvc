using System;
using System.Linq;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics.Endpoints;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Diagnostics.Routes
{
    [TestFixture]
    public class EndpointReportTester
    {
        private Lazy<EndpointReport> _report;
        private BehaviorChain theChain;
        private StubUrlRegistry theUrls;
        private RoutedChain theRoutedChain;
        private Lazy<EndpointReport> _routedReport;

        [SetUp]
        public void SetUp()
        {
            theChain = new BehaviorChain();
            theRoutedChain = new RoutedChain("something");
            theUrls = new StubUrlRegistry();
            _report = new Lazy<EndpointReport>(() => EndpointReport.ForChain(theChain));
            _routedReport = new Lazy<EndpointReport>(() => EndpointReport.ForChain(theRoutedChain));
        }

        private EndpointReport theReport
        {
            get
            {
                return _report.Value;
            }
        }

        private EndpointReport theRoutedReport
        {
            get
            {
                return _routedReport.Value;
            }
        }

        [Test]
        public void resource_type()
        {
            theChain.AddToEnd(ActionCall.For<FakeEndpoint>(x => x.get_something(null)));
            theReport.ResourceType.ShouldBe(typeof (Output));
        }

        [Test]
        public void input_model()
        {
            theChain.AddToEnd(ActionCall.For<FakeEndpoint>(x => x.get_something(null)));
            theReport.InputModel.ShouldBe(typeof (Input));
        }

        [Test]
        public void action_with_no_actions()
        {
            theReport.Action.Any().ShouldBeFalse();
        }

        [Test]
        public void one_action()
        {
            theChain.AddToEnd(ActionCall.For<FakeEndpoint>(x => x.get_something(null)));
            theReport.Action.Single().ShouldBe("FakeEndpoint.get_something(Input input) : Output");
        }

        [Test]
        public void multiple_actions()
        {
            theChain.AddToEnd(ActionCall.For<FakeEndpoint>(x => x.get_something(null)));
            theChain.AddToEnd(ActionCall.For<FakeEndpoint>(x => x.get_another(null)));
            theChain.AddToEnd(ActionCall.For<FakeEndpoint>(x => x.get_third(null)));

            theReport.Action.ShouldHaveTheSameElementsAs(
                "FakeEndpoint.get_something(Input input) : Output", 
                "FakeEndpoint.get_another(Input input) : Output", 
                "FakeEndpoint.get_third(Input input) : Output");
        }

        [Test]
        public void constraints_with_no_route()
        {
            theReport.Constraints.ShouldBe("N/A");
        }

        [Test]
        public void constraints_with_a_route_but_no_constraints()
        {
            theRoutedChain.Route.AllowedHttpMethods.Any().ShouldBeFalse();

            theRoutedReport.Constraints.ShouldBe("Any");
        }

        [Test]
        public void one_constraint()
        {
            theRoutedChain.Route.AddHttpMethodConstraint("GET");

            theRoutedReport.Constraints.ShouldBe("GET");
        }

        [Test]
        public void multiple_constraints()
        {
            theRoutedChain.Route.AddHttpMethodConstraint("POST");
            theRoutedChain.Route.AddHttpMethodConstraint("GET");

            theRoutedReport.Constraints.ShouldBe("GET, POST");
        }

        [Test]
        public void normal_route()
        {

            theRoutedReport.Title.ShouldBe("something");
        }

        [Test]
        public void url_category()
        {
            theRoutedChain.UrlCategory.Category = "weird";

            theRoutedReport.UrlCategory.ShouldBe("weird");
        }

        [Test]
        public void wrappers_with_no_wrappers()
        {
            theReport.Wrappers.Any().ShouldBeFalse();
        }

        [Test]
        public void multiple_wrappers()
        {
            theChain.AddToEnd(ActionCall.For<FakeEndpoint>(x => x.get_something(null)));
            theChain.FirstCall().WrapWith(typeof (SimpleWrapper));
            theChain.FirstCall().WrapWith(typeof (AnotherWrapper));

            theReport.Wrappers.ShouldHaveTheSameElementsAs("SimpleWrapper", "AnotherWrapper");
        }

        [Test]
        public void input_with_no_InputNode_is_assumed_to_be_http_form()
        {
            theChain.Any(x => x is InputNode).ShouldBeFalse();

            theReport.Accepts.Single().ShouldBe(MimeType.HttpFormMimetype);
        }

        public class SimpleWrapper : IActionBehavior
        {
            public void Invoke()
            {
                throw new NotImplementedException();
            }

            public void InvokePartial()
            {
                throw new NotImplementedException();
            }
        }

        public class AnotherWrapper : IActionBehavior
        {
            public void Invoke()
            {
                throw new NotImplementedException();
            }

            public void InvokePartial()
            {
                throw new NotImplementedException();
            }
        }

        public class Input{}
        public class Output{}

        public class FakeEndpoint
        {
            public Output get_something(Input input)
            {
                return null;
            }

            public Output get_another(Input input)
            {
                return null;
            }

            public Output get_third(Input input)
            {
                return null;
            }
        }
    }
}