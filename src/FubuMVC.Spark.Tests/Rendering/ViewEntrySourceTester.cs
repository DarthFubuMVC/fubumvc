using FubuMVC.Core.View.Model;
using FubuMVC.Spark.Registration;
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
        private ISparkViewEntry _entry;
        private ISparkViewEntry _partialEntry;

        protected override void beforeEach()
        {
            var template = MockRepository.GenerateMock<ITemplate>();
            var master = MockRepository.GenerateMock<ITemplate>();

            template.Stub(x => x.ViewPath).Return("/Views/Home/index.spark");
            master.Stub(x => x.ViewPath).Return("/Views/Shared/appplication.spark");

            var descriptor = new SparkDescriptor(template, new SparkViewEngine()) { Master = master };
            var definition = descriptor.ToViewDefinition();


            _entry = MockRepository.GenerateMock<ISparkViewEntry>();
            _partialEntry = MockRepository.GenerateMock<ISparkViewEntry>();

//            var provider = MockFor<IViewEntryProviderCache>();
//            provider
//                .Stub(x => x.GetViewEntry(definition.ViewDescriptor))
//                .Return(_entry);
//            provider
//                .Stub(x => x.GetViewEntry(definition.PartialDescriptor))
//                .Return(_partialEntry);

            
            Services.Inject(descriptor);
        }

        [Test]
        public void get_view_entry_uses_the_provider_and_view_descriptor_from_the_resolver()
        {
            ClassUnderTest.GetViewEntry().ShouldEqual(_entry);
        }

        [Test]
        public void get_partial_view_entry_uses_the_provider_and_partial_view_descriptor_from_the_resolver()
        {
            ClassUnderTest.GetPartialViewEntry().ShouldEqual(_partialEntry);
        }
    }
}