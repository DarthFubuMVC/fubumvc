using FubuMVC.Core.View.Model;
using FubuMVC.Spark.Rendering;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests.Rendering
{
    [TestFixture]
    public class ViewDefinitionResolverTester : InteractionContext<ViewDefinitionResolver>
    {
        private IViewDefinitionPolicy[] _policies;
        private ViewDefinition _defaultDefinition;
        private ViewDefinition _policyDefinition;
        private SparkDescriptor _descriptor;

        protected override void beforeEach()
        {
            _policies = Services.CreateMockArrayFor<IViewDefinitionPolicy>(5);
            _descriptor = new SparkDescriptor(MockRepository.GenerateMock<ITemplate>());
            _policyDefinition = new ViewDefinition(null, null);
            _defaultDefinition = new ViewDefinition(null, null);

            MockFor<DefaultViewDefinitionPolicy>().Stub(x => x.Create(_descriptor)).Return(_defaultDefinition);
        }

        [Test]
        public void if_no_policy_matches_returns_the_view_definition_from_the_default_policy()
        {
            ClassUnderTest.Resolve(_descriptor).ShouldEqual(_defaultDefinition);
        }

        [Test]
        public void returns_the_view_definition_from_the_matching_policy()
        {
            _policies[3].Expect(x => x.Matches(_descriptor)).Return(true);
            _policies[3].Expect(x => x.Create(_descriptor)).Return(_policyDefinition);
            ClassUnderTest.Resolve(_descriptor).ShouldEqual(_policyDefinition);
            _policies[3].VerifyAllExpectations();
        }
    }
}