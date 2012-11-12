using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Urls;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Navigation.Testing
{
    [TestFixture]
    public class when_building_a_MenuItemToken_for_a_single_node : InteractionContext<NavigationService>
    {
        private MenuNode theNode;
        private BehaviorChain theChain;
        private MenuItemToken theToken;
        private StubUrlRegistry theUrls;

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

            theNode.Icon("some image");

            theNode.Children.AddToEnd(new MenuNode(FakeKeys.Key2));
            theNode.Children.AddToEnd(new MenuNode(FakeKeys.Key3));
            theNode.Children.AddToEnd(new MenuNode(FakeKeys.Key4));

            theUrls = new StubUrlRegistry();
            Services.Inject<IUrlRegistry>(theUrls);

            MockFor<IMenuStateService>().Stub(x => x.DetermineStateFor(theNode))
                .Return(MenuItemState.Available);

            MockFor<ICurrentHttpRequest>().Stub(x => x.ToFullUrl(theNode.CreateUrl()))
                .Return("the full url");

            theToken = ClassUnderTest.BuildToken(theNode);
        }

        [Test, Ignore("Until navigation is moved out")]
        public void will_resolve_the_asset_url_for_the_icon_if_it_exists()
        {
            //theToken.IconUrl.ShouldEqual(theUrls.UrlForAsset(AssetFolder.images, theNode.Icon()));
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

    [TestFixture]
    public class when_building_a_MenuItemToken_for_a_single_node_without_an_icon : InteractionContext<NavigationService>
    {
        private MenuNode theNode;
        private BehaviorChain theChain;
        private MenuItemToken theToken;
        private StubUrlRegistry theUrls;

        protected override void beforeEach()
        {
            theChain = new BehaviorChain();
            theChain.Route = new RouteDefinition("something")
            {
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
        public void icon_url_is_null()
        {
            theToken.IconUrl.ShouldBeNull();
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