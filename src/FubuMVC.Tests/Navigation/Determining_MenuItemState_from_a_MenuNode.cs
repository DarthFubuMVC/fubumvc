using FubuMVC.Core.Http;
using FubuMVC.Core.Navigation;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime.Conditionals;
using FubuMVC.Core.Security.Authorization;
using FubuMVC.Tests.TestSupport;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.Navigation
{
    
    public class Determining_MenuItemState_from_a_MenuNode : InteractionContext<MenuStateService>
    {
        private MenuNode theNode;
        private BehaviorChain theChain;

        protected override void beforeEach()
        {
            theChain = new BehaviorChain();

            theNode = new MenuNode(FakeKeys.Key1, r => theChain);
            theNode.Resolve(null);
            theNode.BehaviorChain.ShouldBeTheSameAs(theChain);
            theNode.UrlInput = new object();
        }

        private void theRightsAre(AuthorizationRight right)
        {
            MockFor<IChainAuthorizor>().Stub(x => x.Authorize(theNode.BehaviorChain, theNode.UrlInput))
                .Return(right);
        }

        [Fact]
        public void when_the_chain_is_not_authorized_with_the_default_unauthorized_state()
        {
            theRightsAre(AuthorizationRight.None);

            ClassUnderTest.DetermineStateFor(theNode)
                .ShouldBe(MenuItemState.Hidden);
        }

        [Fact]
        public void when_the_chain_is_not_authorized_with_the_default_unauthorized_state_2()
        {
            theRightsAre(AuthorizationRight.Deny);

            ClassUnderTest.DetermineStateFor(theNode)
                .ShouldBe(MenuItemState.Hidden);
        }


        [Fact]
        public void when_the_chain_is_not_authorized_and_the_unauthorized_state_is_disabled()
        {
            theRightsAre(AuthorizationRight.None);
            theNode.UnauthorizedState = MenuItemState.Disabled;

            ClassUnderTest.DetermineStateFor(theNode)
                .ShouldBe(MenuItemState.Disabled);
        }

        [Fact]
        public void when_the_chain_is_not_authorized_and_the_unauthorized_state_is_disabled_2()
        {
            theRightsAre(AuthorizationRight.Deny);
            theNode.UnauthorizedState = MenuItemState.Disabled;

            ClassUnderTest.DetermineStateFor(theNode)
                .ShouldBe(MenuItemState.Disabled);
        }

        [Fact]
        public void assuming_that_authorization_is_fine_state_should_be_active_if_this_is_the_currently_displayed_chain()
        {
            theRightsAre(AuthorizationRight.Allow);
            MockFor<ICurrentChain>().Stub(x => x.OriginatingChain).Return(theNode.BehaviorChain);

            ClassUnderTest.DetermineStateFor(theNode)
                .ShouldBe(MenuItemState.Active);
        }

        [Fact]
        public void authenticated_but_not_the_current_chain_and_enabled_condition_returns_true()
        {
            theRightsAre(AuthorizationRight.Allow);
            theNode.IsEnabledBy(typeof (FakeConditional));

            MockFor<IConditionalService>().Stub(x => x.IsTrue(typeof (FakeConditional)))
                .Return(true);

            ClassUnderTest.DetermineStateFor(theNode)
                .ShouldBe(MenuItemState.Available);
        }

        [Fact]
        public void authenticated_but_not_the_current_chain_and_hide_if_condition_returns_true()
        {
            theRightsAre(AuthorizationRight.Allow);
            theNode.HideIf<FakeConditional>();

            MockFor<IConditionalService>().Stub(x => x.IsTrue(typeof(FakeConditional)))
                .Return(true);

            ClassUnderTest.DetermineStateFor(theNode)
                .ShouldBe(MenuItemState.Hidden);
        }

        [Fact]
        public void authenticated_but_not_the_current_chain_and_enabled_condition_returns_false()
        {
            theRightsAre(AuthorizationRight.Allow);
            theNode.IsEnabledBy(typeof(FakeConditional));

            MockFor<IConditionalService>().Stub(x => x.IsTrue(typeof(FakeConditional)))
                .Return(false);

            ClassUnderTest.DetermineStateFor(theNode)
                .ShouldBe(MenuItemState.Disabled);
        }

        [Fact]
        public void when_the_menu_node_is_only_a_node_with_no_behavior_chain()
        {
            theNode = MenuNode.Node("Something");

            ClassUnderTest.DetermineStateFor(theNode)
                .ShouldBe(MenuItemState.Available);
        }
    }
}