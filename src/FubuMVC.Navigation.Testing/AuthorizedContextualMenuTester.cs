using FubuMVC.Core;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Navigation.Testing
{
    [TestFixture]
    public class AuthorizedContextualMenuTester : InteractionContext<AuthorizedContextualMenu<ContextualObject>>
    {
        private IContextualAction<ContextualObject> definition;
    	private const string theCategory = "the category";
    	private const string theTextOfTheMenuItem = "menu text";
		private const string theDescriptionOfTheMenuItem = "menu item description";
    	private const string theUrl = "the url";
    	private const string theKey = "the key";
    	private ContextualObject theTarget;

        protected override void beforeEach()
        {
            _token = null;
            theTarget = new ContextualObject();

            definition = MockFor<IContextualAction<ContextualObject>>();

            definition.Stub(x => x.Category).Return(theCategory);
            definition.Stub(x => x.Text()).Return(theTextOfTheMenuItem);
			definition.Stub(x => x.Description()).Return(theDescriptionOfTheMenuItem);
			definition.Stub(x => x.Key).Return(theKey);
        }

        private bool WouldBeAuthorized
        {
            set
            {
                definition.Expect(x => x.FindEndpoint(MockFor<IEndpointService>(), theTarget))
                    .Return(new Endpoint(){
                        IsAuthorized = value,
                        Url = theUrl
                    });
            }
        }

        private MenuItemState AvailabilityAsDeterminedByTheStrategyIs
        {
            set
            {
                definition.Stub(x => x.IsAvailable(theTarget)).Return(value);
            }
        }

        private MenuItemState UnAuthorizedStateIs
        {
            set
            {
                definition.Stub(x => x.UnauthorizedState).Return(value);
            }
        }

        private MenuItemToken _token;
        private MenuItemToken theResultingMenuItemToken
        {
            get
            {
                if (_token == null)
                {
                    _token = ClassUnderTest.BuildMenuItem(theTarget, definition);
                }

                return _token;
            }
        }

        [Test]
        public void should_get_the_key_from_the_inner_definition()
        {
            WouldBeAuthorized = true;
            AvailabilityAsDeterminedByTheStrategyIs = MenuItemState.Available;

            theResultingMenuItemToken.Key.ShouldEqual(theKey);
        }

        [Test]
        public void should_get_the_text_from_the_inner_definition()
        {
            WouldBeAuthorized = true;
            AvailabilityAsDeterminedByTheStrategyIs = MenuItemState.Available;

            theResultingMenuItemToken.Text.ShouldEqual(theTextOfTheMenuItem);
        }

		[Test]
		public void should_get_the_description_from_the_inner_definition()
		{
			WouldBeAuthorized = true;
			AvailabilityAsDeterminedByTheStrategyIs = MenuItemState.Available;

			theResultingMenuItemToken.Description.ShouldEqual(theDescriptionOfTheMenuItem);
		}

		[Test]
		public void should_get_the_category_from_the_inner_definition()
		{
			WouldBeAuthorized = true;
			AvailabilityAsDeterminedByTheStrategyIs = MenuItemState.Available;

			theResultingMenuItemToken.Category.ShouldEqual(theCategory);
		}

        [Test]
        public void should_get_the_url_from_the_inner_definition()
        {
            WouldBeAuthorized = true;
            AvailabilityAsDeterminedByTheStrategyIs = MenuItemState.Available;

            theResultingMenuItemToken.Url.ShouldEqual(theUrl);
        }

        [Test]
        public void build_menu_state_when_the_authorization_succeeds_and_is_available()
        {
            WouldBeAuthorized = true;
            AvailabilityAsDeterminedByTheStrategyIs = MenuItemState.Available;

            theResultingMenuItemToken.MenuItemState.ShouldEqual(MenuItemState.Available);
        }

        [Test]
        public void build_menu_state_when_the_authorization_fails_and_the_inner_definition_says_unauthorized_endpoints_should_be_hidden()
        {
            WouldBeAuthorized = false;
            AvailabilityAsDeterminedByTheStrategyIs = MenuItemState.Available;
            UnAuthorizedStateIs = MenuItemState.Hidden;

            theResultingMenuItemToken.MenuItemState.ShouldEqual(MenuItemState.Hidden);
        }

        [Test]
        public void build_menu_state_when_the_authorization_fails_and_the_inner_definition_says_unauthorized_endpoints_should_be_disabled()
        {
            WouldBeAuthorized = false;
            AvailabilityAsDeterminedByTheStrategyIs = MenuItemState.Available;
            UnAuthorizedStateIs = MenuItemState.Disabled;

            theResultingMenuItemToken.MenuItemState.ShouldEqual(MenuItemState.Disabled);
        }

        [Test]
        public void build_menu_state_when_is_authorized_by_availibility_determinied_by_the_definition_is_disabled()
        {
            WouldBeAuthorized = true;
            AvailabilityAsDeterminedByTheStrategyIs = MenuItemState.Disabled;

            theResultingMenuItemToken.MenuItemState.ShouldEqual(MenuItemState.Disabled);
        }


        [Test]
        public void build_menu_state_when_is_authorized_by_availibility_determinied_by_the_definition_is_hidden()
        {
            WouldBeAuthorized = true;
            AvailabilityAsDeterminedByTheStrategyIs = MenuItemState.Hidden;

            theResultingMenuItemToken.MenuItemState.ShouldEqual(MenuItemState.Hidden);
        }

    }

    public class ContextualObject
    {
        public string Name { get; set; }
    }
}