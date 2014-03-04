using FubuMVC.Core.View.Rendering;
using FubuMVC.Spark.Rendering;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Spark;

namespace FubuMVC.Spark.Tests.Rendering
{
    [TestFixture]
    public class ViewFactoryTester : InteractionContext<ViewFactory>
    {
        private IViewModifierService<IFubuSparkView> _service;
        private IViewEntrySource _entrySource;

        private ISparkViewEntry _sourceEntry;
        private FubuSparkView _entryView;
        private IFubuSparkView _serviceView;

        protected override void beforeEach()
        {
            _service = MockFor<IViewModifierService<IFubuSparkView>>();
            _entrySource = MockFor<IViewEntrySource>();

            _sourceEntry = MockRepository.GenerateMock<ISparkViewEntry>();
            _entryView = MockRepository.GenerateMock<FubuSparkView>();
            _serviceView = MockRepository.GenerateMock<IFubuSparkView>();

            _sourceEntry.Expect(x => x.CreateInstance()).Return(_entryView);
            _service.Expect(x => x.Modify(_entryView)).Return(_serviceView);
        }

        [Test]
        public void getview_returns_uses_the_entry_from_the_entrysource_and_applies_the_service_modifications()
        {
            _entrySource.Expect(x => x.GetViewEntry()).Return(_sourceEntry);

            ClassUnderTest.GetView().ShouldEqual(_serviceView);
            _entrySource.VerifyAllExpectations();
            _sourceEntry.VerifyAllExpectations();
            _service.VerifyAllExpectations();
        }

        [Test]
        public void getpartialview_returns_uses_the_entry_from_the_entrysource_and_applies_the_service_modifications()
        {
            _entrySource.Expect(x => x.GetPartialViewEntry()).Return(_sourceEntry);

            ClassUnderTest.GetPartialView().ShouldEqual(_serviceView);
            _entrySource.VerifyAllExpectations();
            _sourceEntry.VerifyAllExpectations();
            _service.VerifyAllExpectations();
        }
    }
}