using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;
using NUnit.Framework;
using Rhino.Mocks;
using Spark.Web.FubuMVC.Registration;

namespace Spark.Web.FubuMVC.Tests.Registration
{
    [TestFixture]
    public class when_resolving_policies : InteractionContext<SparkPolicyResolver>
    {
        private List<ISparkPolicy> _policies;
        private ActionCall _call;

        protected override void beforeEach()
        {
            _call = ActionCall.For<SampleEndpoint>(e => e.Get(new SampleInput()));
            _policies = new List<ISparkPolicy> { MockFor<ISparkPolicy>() };
            Container.Configure(x => x.For<List<ISparkPolicy>>().Use(() => _policies));
        }

        [Test]
        public void should_have_match_when_policy_matches()
        {
            MockFor<ISparkPolicy>()
                .Expect(policy => policy.Matches(_call))
                .Return(true);

            ClassUnderTest
                .HasMatchFor(_call)
                .ShouldBeTrue();
        }

        [Test]
        public void should_not_have_match_when_no_policies_match()
        {
            MockFor<ISparkPolicy>()
                .Expect(policy => policy.Matches(_call))
                .Return(false);

            ClassUnderTest
                .HasMatchFor(_call)
                .ShouldBeFalse();
        }

        [Test]
        public void should_resolve_view_locator_with_first_matching_policy()
        {
            MockFor<ISparkPolicy>()
                .Expect(policy => policy.Matches(_call))
                .Return(true);

            MockFor<ISparkPolicy>()
                .Expect(policy => policy.BuildViewLocator(_call))
                .Return("TestFolder");

            // test policy blows up if used
            _policies.Add(new TestSparkPolicy());

            ClassUnderTest
                .ResolveViewLocator(_call)
                .ShouldEqual("TestFolder");
        }

        [Test]
        public void should_resolve_view_name_with_first_matching_policy()
        {
            MockFor<ISparkPolicy>()
                .Expect(policy => policy.Matches(_call))
                .Return(true);

            MockFor<ISparkPolicy>()
                .Expect(policy => policy.BuildViewName(_call))
                .Return("Sample");

            // test policy blows up if used
            _policies.Add(new TestSparkPolicy());

            ClassUnderTest
                .ResolveViewName(_call)
                .ShouldEqual("Sample");
        }
    }
}