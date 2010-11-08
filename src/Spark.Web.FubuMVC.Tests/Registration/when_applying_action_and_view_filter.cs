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
        private SparkViewDescriptor _desciptor;
        private ViewBag _views;

        protected override void beforeEach()
        {
            _call = ActionCall.For<SampleEndpoint>(e => e.Get(new SampleInput()));
            _desciptor = new SparkViewDescriptor
                                {
                                    Accessors = new List<SparkViewDescriptor.Accessor>(),
                                    Templates = new List<string>()
                                };
            var token = new SparkViewToken(_call, _desciptor, "Sample", "Sample");
            token.Descriptors.Add(_desciptor);

            _views = new ViewBag(new List<IViewToken> { token });
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
            const string viewLocator = "TestFolder";
            const string viewName = "Sample";

            _desciptor.Templates.Add(viewLocator + "\\" + viewName);

            MockFor<ISparkPolicyResolver>()
                .Expect(resolver => resolver.HasMatchFor(_call))
                .Return(true);

            MockFor<ISparkPolicyResolver>()
                .Expect(resolver => resolver.ResolveViewLocator(_call))
                .Return(viewLocator);

            MockFor<ISparkPolicyResolver>()
                .Expect(resolver => resolver.ResolveViewName(_call))
                .Return(viewName);

            var token = ClassUnderTest
                        .Apply(_call, _views)
                        .FirstOrDefault();

            token.ShouldNotBeNull();
            token.Name.ShouldEqual(viewName);
        }   
    }
}