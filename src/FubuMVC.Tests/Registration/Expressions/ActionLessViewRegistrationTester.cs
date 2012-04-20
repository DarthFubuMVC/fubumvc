using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.View;
using FubuMVC.Tests.Urls;
using FubuMVC.WebForms;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Expressions
{
    [TestFixture]
    public class ActionLessViewRegistrationTester
    {
        private BehaviorGraph theBehaviorGraph;

        [SetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry();
            registry.Import<WebFormsEngine>();

            registry.Views.RegisterActionLessViews(token => token.ViewType.Name.StartsWith("FakeView"), chain => chain.IsPartialOnly = true);

            theBehaviorGraph = registry.BuildGraph();
        }

        [Test]
        public void should_have_a_chain_for_each_matching_view()
        {
            theBehaviorGraph.BehaviorFor(typeof (InputModel)).ShouldNotBeNull();
            theBehaviorGraph.BehaviorFor(typeof (TestInputModel)).ShouldNotBeNull();
            theBehaviorGraph.BehaviorFor(typeof (Model1)).ShouldNotBeNull();
            theBehaviorGraph.BehaviorFor(typeof (Model3)).ShouldNotBeNull();
        }

        [Test]
        public void all_behavior_chains_for_the_action_less_views_should_be_marked_as_partial_only()
        {
            theBehaviorGraph.BehaviorFor(typeof(InputModel)).IsPartialOnly.ShouldBeTrue();
            theBehaviorGraph.BehaviorFor(typeof(TestInputModel)).IsPartialOnly.ShouldBeTrue();
            theBehaviorGraph.BehaviorFor(typeof(Model1)).IsPartialOnly.ShouldBeTrue();
            theBehaviorGraph.BehaviorFor(typeof(Model3)).IsPartialOnly.ShouldBeTrue();
        }
    }

	[TestFixture]
    public class ActionLessViewRegistrationWithIViewToken
	{
        private BehaviorGraph theBehaviorGraph;

        [SetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry();
            registry.Import<WebFormsEngine>();

            registry.Views.RegisterActionLessViews(token => token.ViewType.Name.StartsWith("FakeView"), (chain, token) => chain.Route = new RouteDefinition(token.Name()));

            theBehaviorGraph = registry.BuildGraph();
        }

		[Test]
		public void chain_is_configured_depending_on_token()
		{
			var behavior = theBehaviorGraph.BehaviorFor(typeof (InputModel));
			behavior.Route.Pattern.ShouldEqual(typeof (FakeView1).Name);
		}
		
	}

    public class FakeView1 : FubuPage<InputModel>{}
    public class FakeView2 : FubuPage<TestInputModel> { }
    public class FakeView3 : FubuPage<Model1> { }
    public class FakeView4 : FubuPage<Model3> { }

}