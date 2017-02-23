using FubuMVC.Core.Assets;
using FubuMVC.Core.Navigation;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Urls;
using FubuMVC.Tests.TestSupport;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.Navigation
{
    
    public class when_building_a_MenuItemToken_for_a_single_node : InteractionContext<NavigationService>
    {
        private MenuNode theNode;
        private RoutedChain theChain;
        private StubUrlRegistry theUrls;

	    private string theIconUrl;

        protected override void beforeEach()
        {
            theChain = new RoutedChain(new RouteDefinition("something"){
                Input = new RouteInput<FakeInput>("somepattern")
            });

            Services.Inject(new BehaviorGraph());
            

            theNode = new MenuNode(FakeKeys.Key1, r => theChain);
            theNode.Resolve(null);
            theNode.BehaviorChain.ShouldBeTheSameAs(theChain);
            theNode.UrlInput = new object();

            theNode.Icon("some image");

            theNode.Children.AddToEnd(new MenuNode(FakeKeys.Key2));
            theNode.Children.AddToEnd(new MenuNode(FakeKeys.Key3));
            theNode.Children.AddToEnd(new MenuNode(FakeKeys.Key4));

	        theIconUrl = "test.png";
	        MockFor<IAssetTagBuilder>().Stub(x => x.FindImageUrl(theNode.Icon())).Return(theIconUrl);

            theUrls = new StubUrlRegistry();
            Services.Inject<IUrlRegistry>(theUrls);

            MockFor<IMenuStateService>().Stub(x => x.DetermineStateFor(theNode))
                .Return(MenuItemState.Available);

			MockFor<IChainUrlResolver>().Stub(x => x.UrlFor(theNode.UrlInput, theNode.BehaviorChain))
                .Return("the full url");
        }

		private MenuItemToken theToken { get { return ClassUnderTest.BuildToken(theNode); } }

        [Fact]
        public void will_resolve_the_asset_url_for_the_icon_if_it_exists()
        {
            theToken.IconUrl.ShouldBe(theIconUrl);
        }

        [Fact]
        public void has_state_from_the_state_service()
        {
            theToken.MenuItemState.ShouldBe(MenuItemState.Available);
        }

        [Fact]
        public void has_the_key()
        {
            theToken.Key.ShouldBe(theNode.Key.Key);
        }

        [Fact]
        public void has_the_text_from_the_StringTOken_on_the_node()
        {
            theToken.Text.ShouldBe(theNode.Key.ToString());
        }

        [Fact]
        public void has_the_url()
        {
            theToken.Url.ShouldBe("the full url");
        }

        [Fact]
        public void has_children()
        {
            theToken.Children.ShouldHaveCount(3);
        }

		[Fact]
		public void no_category()
		{
			theToken.Category.ShouldBeNull();
		}

		[Fact]
		public void sets_the_category()
		{
			theNode.Category = "test";
			theToken.Category.ShouldEndWith(theNode.Category);
		}

		[Fact]
		public void sets_the_data()
		{
			theNode["k1"] = "value1";
			theNode["k2"] = "value2";

			theToken.Get<string>("k1").ShouldBe("value1");
			theToken.Get<string>("k2").ShouldBe("value2");

			var value = "empty";
			theToken.Value<string>("k2", x => value = x);

			value.ShouldBe("value2");
		}
    }

    
    public class when_building_a_MenuItemToken_for_a_single_node_without_an_icon : InteractionContext<NavigationService>
    {
        private MenuNode theNode;
        private BehaviorChain theChain;
        private MenuItemToken theToken;
        private StubUrlRegistry theUrls;

        protected override void beforeEach()
        {
            theChain = new RoutedChain(new RouteDefinition("something")
            {
                Input = new RouteInput<FakeInput>("somepattern")
            });

            Services.Inject(new BehaviorGraph());

            theNode = new MenuNode(FakeKeys.Key1, r => theChain);
            theNode.Resolve(null);
            theNode.BehaviorChain.ShouldBeTheSameAs(theChain);
            theNode.UrlInput = new object();

            theNode.Children.AddToEnd(new MenuNode(FakeKeys.Key2));
            theNode.Children.AddToEnd(new MenuNode(FakeKeys.Key3));
            theNode.Children.AddToEnd(new MenuNode(FakeKeys.Key4));


            MockFor<IMenuStateService>().Stub(x => x.DetermineStateFor(theNode))
                .Return(MenuItemState.Available);

            MockFor<IChainUrlResolver>().Stub(x => x.UrlFor(theNode.UrlInput, theNode.BehaviorChain))
                .Return("the full url");

            theToken = ClassUnderTest.BuildToken(theNode);
        }

        [Fact]
        public void icon_url_is_null()
        {
            theToken.IconUrl.ShouldBeNull();
        }

        [Fact]
        public void has_state_from_the_state_service()
        {
            theToken.MenuItemState.ShouldBe(MenuItemState.Available);
        }

        [Fact]
        public void has_the_key()
        {
            theToken.Key.ShouldBe(theNode.Key.Key);
        }

        [Fact]
        public void has_the_text_from_the_StringTOken_on_the_node()
        {
            theToken.Text.ShouldBe(theNode.Key.ToString());
        }

        [Fact]
        public void has_the_url()
        {
            theToken.Url.ShouldBe("the full url");
        }

        [Fact]
        public void has_children()
        {
            theToken.Children.ShouldHaveCount(3);
        }
    }
}