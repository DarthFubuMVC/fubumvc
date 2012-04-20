using System.Linq;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Attachment;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.View
{
    [TestFixture]
    public class ViewAttacherConventionTester
    {
        private ViewAttacherConvention _viewAttacherConvention;
        private ActionCall _action;
        private IViewsForActionFilter _firstFilterThatFindsExactlyOne;
        private ViewBag _views;
        private FakeViewToken _fromFindsOne;
        private IViewsForActionFilter _secondFilterThatFindsExactlyOne;
        private FakeViewToken _fromSecondFindsOne;
        private IViewsForActionFilter _filterThatFindsNone;
        private IViewsForActionFilter _filterThatFindsMultiple;
        private RecordingConfigurationObserver _observer;

        [SetUp]
        public void Setup()
        {
            _observer = new RecordingConfigurationObserver();
            _action = ActionCall.For<ViewsForActionFilterTesterController>(x => x.AAction());
            _fromFindsOne = new FakeViewToken();
            _fromSecondFindsOne = new FakeViewToken();
            _views = new ViewBag(new IViewToken[] { _fromFindsOne, _fromSecondFindsOne });
            _filterThatFindsNone = createFilterThatReturns(new IViewToken[0]);
            _firstFilterThatFindsExactlyOne = createFilterThatReturns(_fromFindsOne);
            _secondFilterThatFindsExactlyOne = createFilterThatReturns(_fromSecondFindsOne);
            _filterThatFindsMultiple = createFilterThatReturns(_fromFindsOne, _fromSecondFindsOne);
            _viewAttacherConvention = new ViewAttacherConvention();
        }

        [Test]
        public void does_not_attach_a_view_if_no_filters_find_a_match()
        {
            _viewAttacherConvention.AddViewsForActionFilter(_filterThatFindsNone);
            
            _viewAttacherConvention.AttemptToAttachViewToAction(_views, _action, _observer);
            _action.OfType<FakeViewToken>().ShouldHaveCount(0);
        }

        [Test]
        public void should_use_first_filter_that_returns_exactly_one_view()
        {
            _viewAttacherConvention.AddViewsForActionFilter(_filterThatFindsNone);
            _viewAttacherConvention.AddViewsForActionFilter(_firstFilterThatFindsExactlyOne);
            _viewAttacherConvention.AddViewsForActionFilter(_secondFilterThatFindsExactlyOne);

            _viewAttacherConvention.AttemptToAttachViewToAction(_views, _action, _observer);
            var attachedView = _action.OfType<FakeViewToken>().ShouldHaveCount(1).FirstOrDefault();
            attachedView.ShouldBeTheSameAs(_fromFindsOne);
        }

        [Test]
        public void should_not_attach_a_view_from_a_filter_that_returns_more_than_one_token()
        {
            _viewAttacherConvention.AddViewsForActionFilter(_filterThatFindsMultiple);

            _viewAttacherConvention.AttemptToAttachViewToAction(_views, _action, _observer);
            _action.OfType<FakeViewToken>().ShouldHaveCount(0);
        }

        [Test]
        public void should_log_when_a_view_is_attached()
        {
            _fromFindsOne.ViewName = "TestToken";

            _viewAttacherConvention.AddViewsForActionFilter(_firstFilterThatFindsExactlyOne);
            _viewAttacherConvention.AttemptToAttachViewToAction(_views, _action, _observer);

            _observer.LastLogEntry.ShouldContain(_fromFindsOne.Name());
        }

        private static IViewsForActionFilter createFilterThatReturns(params IViewToken[] viewTokens)
        {
            var filter = MockRepository.GenerateMock<IViewsForActionFilter>();
            filter.Stub(x => x.Apply(null, null)).IgnoreArguments().Return(viewTokens);
            return filter;
        }
    }
}