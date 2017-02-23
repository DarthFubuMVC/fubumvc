using FubuMVC.Core.Navigation;
using FubuMVC.Tests.TestSupport;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.Navigation
{
    
    public class MenuRegistrationTester : InteractionContext<MenuRegistration>
    {
        private MenuNode theNode;

        protected override void beforeEach()
        {
            theNode = MenuNode.Node("something");
            Services.Inject(theNode);
        }

        [Fact]
        public void depends_on_positive()
        {
            var token = new NavigationKey("something");

            MockFor<IStringTokenMatcher>().Stub(x => x.Matches(token)).Return(true);

            ClassUnderTest.DependsOn(token).ShouldBeTrue();
        }

        [Fact]
        public void depends_on_negative()
        {
            var token = new NavigationKey("something");

            MockFor<IStringTokenMatcher>().Stub(x => x.Matches(token)).Return(false);

            ClassUnderTest.DependsOn(token).ShouldBeFalse();
        }

        [Fact]
        public void get_the_description()
        {
            var theStringTokenDescription = "the string token description";
            MockFor<IStringTokenMatcher>().Stub(x => x.Description).Return(theStringTokenDescription);
            MockFor<IMenuPlacementStrategy>().Stub(x => x.FormatDescription(theStringTokenDescription, theNode.Key))
                .Return("some description");

            ClassUnderTest.Description.ShouldBe("some description");
        }

        [Fact]
        public void configure_if_the_matcher_can_find_something()
        {
            var graph = new NavigationGraph();
            graph.MenuFor("one");
            graph.MenuFor("two");
            var theChain = graph.MenuFor("three");

            MockFor<IStringTokenMatcher>().Stub(x => x.Matches(theChain.Key)).Return(true);

            ClassUnderTest.Configure(graph);

            MockFor<IMenuPlacementStrategy>().AssertWasCalled(x => x.Apply(theChain, theNode));
        }

        [Fact]
        public void configure_if_the_matcher_cannot_find_something_will_add_a_menu_chain_for_the_parent()
        {
            var graph = new NavigationGraph();
            graph.MenuFor("one");
            graph.MenuFor("two");

            MockFor<IStringTokenMatcher>().Stub(x => x.DefaultKey()).Return(new NavigationKey("three"));
            
            ClassUnderTest.Configure(graph);


            var theChain = graph.MenuFor("three");
            MockFor<IMenuPlacementStrategy>().AssertWasCalled(x => x.Apply(theChain, theNode)); 
        }
    }
}