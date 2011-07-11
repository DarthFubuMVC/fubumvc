using FubuMVC.Spark.Registration;
using FubuMVC.Spark.Registration.Nodes;
using FubuMVC.Spark.Rendering;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Spark;

namespace FubuMVC.Spark.Tests.Rendering
{
    [TestFixture]
    public class ViewEntrySourceTester : InteractionContext<ViewEntrySource>
    {
        private ViewDescriptor _descriptor;
        private ViewDefinition _definition;
        private ISparkViewEntry _entry;
        private ISparkViewEntry _partialEntry;

        protected override void beforeEach()
        {
            var template = MockRepository.GenerateMock<ITemplate>();
            var master = MockRepository.GenerateMock<ITemplate>();

            template.Stub(x => x.ViewPath).Return("/Views/Home/index.spark");
            master.Stub(x => x.ViewPath).Return("/Views/Shared/appplication.spark");

            _descriptor = new ViewDescriptor(template) { Master = master };
            _definition = _descriptor.ToViewDefinition();

            _entry = MockRepository.GenerateMock<ISparkViewEntry>();
            _partialEntry = MockRepository.GenerateMock<ISparkViewEntry>();

            var provider = MockFor<IViewEntryProviderCache>();
            provider
                .Stub(x => x.GetViewEntry(_definition.ViewDescriptor))
                .Return(_entry);
            provider
                .Stub(x => x.GetViewEntry(_definition.PartialDescriptor))
                .Return(_partialEntry);

            Services.Inject(_descriptor);
        }

        [Test]
        public void get_view_entry_uses_the_provider_and_view_descriptor()
        {
            ClassUnderTest.GetViewEntry().ShouldEqual(_entry);
        }

        [Test]
        public void get_partial_view_entry_uses_the_provider_and_partial_view_descriptor()
        {
            ClassUnderTest.GetPartialViewEntry().ShouldEqual(_partialEntry);
        }

        [Test]
        public void when_a_policy_matches_uses_the_generated_definition_to_return_the_view_entry()
        {
            var policy = getMockedPolicy();
            ClassUnderTest.GetViewEntry().ShouldEqual(_entry);
            policy.VerifyAllExpectations();
        }

        [Test]
        public void when_a_policy_matches_uses_the_generated_definition_to_return_the_partial_view_entry()
        {
            var policy = getMockedPolicy();
            ClassUnderTest.GetPartialViewEntry().ShouldEqual(_partialEntry);
            policy.VerifyAllExpectations();
        }

        private IViewDefinitionPolicy getMockedPolicy()
        {
            var policies = Services.CreateMockArrayFor<IViewDefinitionPolicy>(5);
            policies[3].Expect(x => x.Matches(_descriptor)).Return(true);
            policies[3].Expect(x => x.Create(_descriptor)).Return(_definition);
            return policies[3];
        }

    }
}