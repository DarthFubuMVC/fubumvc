using System;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Registration
{
    [TestFixture]
    public class RouteVisitorTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            visitor = new RouteVisitor();
            route = MockRepository.GenerateMock<IRouteDefinition>();
            chain = new BehaviorChain();

            processor = MockRepository.GenerateMock<RouteProcessor>();

            visitor.Actions += (x, y) => processor.Got(x, y);
        }

        #endregion

        private RouteVisitor visitor;
        private IRouteDefinition route;
        private BehaviorChain chain;
        private RouteProcessor processor;

        [Test]
        public void should_call_the_inner_processor_with_a_matching_behavior_filter()
        {
            visitor.BehaviorFilters += x => true;
            visitor.BehaviorFilters.MatchesAll(chain).ShouldBeTrue();
            visitor.RouteFilters.MatchesAll(route).ShouldBeTrue();

            visitor.VisitRoute(route, chain);
            processor.AssertWasCalled(x => x.Got(route, chain));
        }

        [Test]
        public void should_call_the_inner_processor_with_a_matching_route_filter()
        {
            visitor.RouteFilters += x => true;
            visitor.BehaviorFilters.MatchesAll(chain).ShouldBeTrue();
            visitor.RouteFilters.MatchesAll(route).ShouldBeTrue();

            visitor.VisitRoute(route, chain);
            processor.AssertWasCalled(x => x.Got(route, chain));
        }

        [Test]
        public void should_call_the_inner_processor_with_no_filters()
        {
            visitor.BehaviorFilters.MatchesAll(chain).ShouldBeTrue();
            visitor.RouteFilters.MatchesAll(route).ShouldBeTrue();

            visitor.VisitRoute(route, chain);
            processor.AssertWasCalled(x => x.Got(route, chain));
        }

        [Test]
        public void should_not_call_the_inner_processor_with_a_behavior_filter_that_does_not_match()
        {
            visitor.BehaviorFilters += x => false;
            visitor.BehaviorFilters.MatchesAll(chain).ShouldBeFalse();
            visitor.RouteFilters.MatchesAll(route).ShouldBeTrue();

            visitor.VisitRoute(route, chain);
            processor.AssertWasNotCalled(x => x.Got(route, chain));
        }

        [Test]
        public void should_not_call_the_inner_processor_with_a_route_filter_that_does_not_match()
        {
            visitor.RouteFilters += x => false;
            visitor.BehaviorFilters.MatchesAll(chain).ShouldBeTrue();
            visitor.RouteFilters.MatchesAll(route).ShouldBeFalse();

            visitor.VisitRoute(route, chain);
            processor.AssertWasNotCalled(x => x.Got(route, chain));
        }

        [Test]
        public void configure_not_implemented()
        {
            IConfigurationAction configActionVisitor = new RouteVisitor();
            typeof (NotImplementedException).ShouldBeThrownBy(
                () => configActionVisitor.Configure(new FubuRegistry().BuildGraph()));
        }
    }

    public interface RouteProcessor
    {
        void Got(IRouteDefinition route, BehaviorChain chain);
    }
}