using System;
using System.Net;
using FubuLocalization;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.UI.Navigation;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.UI.Navigation
{
    [TestFixture]
    public class when_building_a_MenuItemToken_for_a_single_node : InteractionContext<NavigationService>
    {
        private MenuNode theNode;
        private BehaviorChain theChain;
        private MenuItemToken theToken;

        protected override void beforeEach()
        {
            theChain = new BehaviorChain();
            theChain.Route = new RouteDefinition("something"){
                Input = new RouteInput<FakeInput>("somepattern")
            };
            

            theNode = new MenuNode(FakeKeys.Key1, r => theChain);
            theNode.Resolve(null);
            theNode.BehaviorChain.ShouldBeTheSameAs(theChain);
            theNode.UrlInput = new object();

            theNode.Children.AddToEnd(new MenuNode(FakeKeys.Key2));
            theNode.Children.AddToEnd(new MenuNode(FakeKeys.Key3));
            theNode.Children.AddToEnd(new MenuNode(FakeKeys.Key4));

            MockFor<IMenuStateService>().Stub(x => x.DetermineStateFor(theNode))
                .Return(MenuItemState.Available);

            MockFor<ICurrentHttpRequest>().Stub(x => x.ToFullUrl(theNode.CreateUrl()))
                .Return("the full url");

            theToken = ClassUnderTest.BuildToken(theNode);
        }

        [Test]
        public void has_state_from_the_state_service()
        {
            theToken.MenuItemState.ShouldEqual(MenuItemState.Available);
        }

        [Test]
        public void has_the_key()
        {
            theToken.Key.ShouldEqual(theNode.Key.Key);
        }

        [Test]
        public void has_the_text_from_the_StringTOken_on_the_node()
        {
            theToken.Text.ShouldEqual(theNode.Key.ToString());
        }

        [Test]
        public void has_the_url()
        {
            theToken.Url.ShouldEqual("the full url");
        }

        [Test]
        public void has_children()
        {
            theToken.Children.ShouldHaveCount(3);
        }
    }


}