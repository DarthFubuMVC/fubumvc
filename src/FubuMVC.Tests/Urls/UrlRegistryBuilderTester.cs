using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Urls;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Urls
{
    [TestFixture]
    public class when_registering_a_route_for_an_action_with_no_input : InteractionContext<UrlRegistryBuilder>
    {
        private RouteDefinition route;
        private ActionCall action;
        private BehaviorChain chain;

        protected override void beforeEach()
        {
            route = new RouteDefinition("some/pattern");
            action = ActionCall.For<TargetController>(x => x.Go());
            chain = new BehaviorChain();
            chain.Append(action);

            ClassUnderTest.VisitRoute(route, chain);
        }

        [Test]
        public void should_add_an_ActionUrl_to_the_registration()
        {
            var actionUrl = new ActionUrl(route, action);
            MockFor<IUrlRegistration>().AssertWasCalled(x => x.AddAction(actionUrl));
        }

        [Test]
        public void should_not_try_to_add_any_model_url()
        {
            MockFor<IUrlRegistration>().AssertWasNotCalled(x => x.AddModel(null), x => x.IgnoreArguments());
        }
    }

    [TestFixture]
    public class when_registering_a_route_for_an_action_with_an_input : InteractionContext<UrlRegistryBuilder>
    {
        private RouteDefinition<RouteInput> route;
        private ActionCall action;
        private BehaviorChain chain;

        protected override void beforeEach()
        {
            route = new RouteDefinition<RouteInput>("some/pattern");
            action = ActionCall.For<TargetController>(x => x.GoWithInput(null));
            chain = new BehaviorChain();
            chain.Append(action);

            ClassUnderTest.VisitRoute(route, chain);
        }

        [Test]
        public void should_have_registered_the_route_as_a_model_url()
        {
            MockFor<IUrlRegistration>().AssertWasCalled(x => x.AddModel(route));
        }
    }

    public class TargetController
    {
        public void Go()
        {
        }

        public void GoWithInput(RouteInput input)
        {
        }
    }
}