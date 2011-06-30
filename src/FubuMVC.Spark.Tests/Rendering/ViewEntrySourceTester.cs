using FubuMVC.Spark.Rendering;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Spark;

namespace FubuMVC.Spark.Tests.Rendering
{
    [TestFixture]
    public class ViewEntrySourceTester : InteractionContext<ViewEntrySource>
    {
        private ViewDefinition _viewDefinition;

        private ISparkViewEntry _viewEntry;
        private ISparkViewEntry _partialEntry;

        protected override void beforeEach()
        {
            var provider = MockFor<IViewEntryProviderCache>();

            _viewDefinition = new ViewDefinition(new SparkViewDescriptor(), new SparkViewDescriptor());
            _viewDefinition.ViewDescriptor.AddTemplate("Views/Home/home.spark");
            _viewDefinition.ViewDescriptor.AddTemplate("Shared/application.spark");

            _viewDefinition.PartialDescriptor.AddTemplate("Views/Home/home.spark");

            _viewEntry = MockRepository.GenerateMock<ISparkViewEntry>();
            _partialEntry = MockRepository.GenerateMock<ISparkViewEntry>();

            provider.Stub(x => x.GetViewEntry(_viewDefinition.ViewDescriptor)).Return(_viewEntry);
            provider.Stub(x => x.GetViewEntry(_viewDefinition.PartialDescriptor)).Return(_partialEntry);

            Services.Inject(_viewDefinition);
        }

        [Test]
        public void getentry_returns_the_entry_from_the_provider_using_the_definition_viewdescriptor()
        {
            ClassUnderTest.GetViewEntry().ShouldEqual(_viewEntry);
        }

        [Test]
        public void getpartialentry_returns_the_entry_from_the_provider_using_the_definition_partialdescriptor()
        {
            ClassUnderTest.GetPartialViewEntry().ShouldEqual(_partialEntry);
        }

    }
}