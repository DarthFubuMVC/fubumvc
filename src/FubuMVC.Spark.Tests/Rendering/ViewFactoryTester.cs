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
        private FubuSparkView _sparkView;
        private ISparkViewModification _modification1;
        private ISparkViewModification _modification2;
        private ISparkViewModification _modification3;

        private IFubuSparkView _generatedView;

        protected override void beforeEach()
        {
            var source = MockFor<IViewEntrySource>();
            var entry = MockFor<ISparkViewEntry>();
            _sparkView = MockFor<FubuSparkView>();
            source.Stub(x => x.GetViewEntry()).Return(entry);
            entry.Stub(x => x.CreateInstance()).Return(_sparkView);
            var modifications = Services.CreateMockArrayFor<ISparkViewModification>(3);
            _modification1 = modifications[0];
            _modification2 = modifications[1];
            _modification3 = modifications[2];
            
            _modification1.Expect(x => x.Applies(_sparkView)).Return(true);
            _modification2.Expect(x => x.Applies(_sparkView)).Return(false);
            _modification3.Expect(x => x.Applies(_sparkView)).Return(true);

            _modification1.Expect(x => x.Modify(_sparkView)).Return(_sparkView);
            _modification2.Expect(x => x.Modify(_sparkView)).Repeat.Never();
            _modification3.Expect(x => x.Modify(_sparkView)).Return(_sparkView);

            _generatedView = ClassUnderTest.GetView();
        }

        [Test]
        public void creates_the_instance_from_the_entry_returned_by_the_injected_entry_source()
        {
            _generatedView.ShouldEqual(_sparkView);
        }

        [Test]
        public void only_the_applicable_modifications_are_used_against_the_view_instance()
        {
            _modification2.AssertWasNotCalled(x => x.Modify(_sparkView));
            _modification1.VerifyAllExpectations();
            _modification2.VerifyAllExpectations();
            _modification3.VerifyAllExpectations();
        }
    }
}