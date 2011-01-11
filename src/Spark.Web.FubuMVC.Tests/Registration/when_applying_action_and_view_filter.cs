using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View;
using NUnit.Framework;
using Rhino.Mocks;
using Spark.Web.FubuMVC.Registration;
using Spark.Web.FubuMVC.ViewCreation;

namespace Spark.Web.FubuMVC.Tests.Registration
{
    [TestFixture]
    public class when_applying_action_and_view_filter : InteractionContext<ActionAndViewMatchedBySparkViewDescriptors>
    {
        private ActionCall _call;
        private SparkViewDescriptor _descriptor;
        private ViewBag _views;
    	private SparkViewToken _token;

        protected override void beforeEach()
        {
            _call = ActionCall.For<SampleEndpoint>(e => e.Get(new SampleInput()));
        	_descriptor = newDescriptor();
            
			_token = new SparkViewToken(_call, _descriptor, "Sample", "Sample");
            _token.Descriptors.Add(_descriptor);

            _views = new ViewBag(new List<IViewToken> { _token });
        }

		protected SparkViewDescriptor newDescriptor()
		{
			return new SparkViewDescriptor
			       	{
			       		Accessors = new List<SparkViewDescriptor.Accessor>(),
			       		Templates = new List<string>()
			       	};
		}

        [Test]
        public void should_return_empty_collection_when_action_call_returns_a_fubu_continuation()
        {
            var call = ActionCall.For<SampleEndpoint>(e => e.Post());

            ClassUnderTest
                .Apply(call, _views)
                .ShouldHaveCount(0);
        }

        [Test]
        public void should_return_empty_collection_when_resolver_has_no_matching_policies()
        {
            MockFor<ISparkPolicyResolver>()
                .Expect(resolver => resolver.HasMatchFor(_call))
                .Return(false);

            ClassUnderTest
                .Apply(_call, _views)
                .ShouldHaveCount(0);
        }

        [Test]
        public void should_return_empty_collection_when_no_desciptors_are_matched()
        {
            MockFor<ISparkPolicyResolver>()
                .Expect(resolver => resolver.HasMatchFor(_call))
                .Return(true);

            MockFor<ISparkPolicyResolver>()
                .Expect(resolver => resolver.ResolveViewLocator(_call))
                .Return("");

            MockFor<ISparkPolicyResolver>()
                .Expect(resolver => resolver.ResolveViewName(_call))
                .Return("");

            ClassUnderTest
                .Apply(_call, _views)
                .ShouldHaveCount(0);
        }

        [Test]
        public void should_return_single_view_token_when_desciptor_is_matched()
        {
            _descriptor.Templates.Add("TestFolder\\Sample.spark");

            MockFor<ISparkPolicyResolver>()
                .Expect(resolver => resolver.HasMatchFor(_call))
                .Return(true);

            MockFor<ISparkPolicyResolver>()
                .Expect(resolver => resolver.ResolveViewLocator(_call))
                .Return("TestFolder");

            MockFor<ISparkPolicyResolver>()
                .Expect(resolver => resolver.ResolveViewName(_call))
                .Return("Sample");

            var token = ClassUnderTest
                        .Apply(_call, _views)
                        .FirstOrDefault();

            token.ShouldNotBeNull();
            token.Name.ShouldEqual("Sample");
        }

		[Test]
		public void should_return_single_view_token_matched_on_policy_criteria()
		{
			_descriptor.Templates.Add("Projects\\Dashboard.spark");
			_descriptor.Templates.Add("Shared\\Application.spark");

			var descriptor2 = newDescriptor();
			descriptor2.Templates.Add("Projects\\StoryMap.spark");
			descriptor2.Templates.Add("Shared\\Application.spark");
			_token.Descriptors.Add(descriptor2);

			var descriptor3 = newDescriptor();
			descriptor3.Templates.Add("Projects.spark");
			descriptor3.Templates.Add("Shared\\Application.spark");
			_token.Descriptors.Add(descriptor3);


			MockFor<ISparkPolicyResolver>()
				.Expect(resolver => resolver.HasMatchFor(_call))
				.Return(true);

			MockFor<ISparkPolicyResolver>()
				.Expect(resolver => resolver.ResolveViewLocator(_call))
				.Return("");

			MockFor<ISparkPolicyResolver>()
				.Expect(resolver => resolver.ResolveViewName(_call))
				.Return("Projects");

			ClassUnderTest
				.Apply(_call, _views)
				.FirstOrDefault()
				.ShouldNotBeNull();
		}
    }
}