using System.Linq;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.View
{
    [TestFixture]
    public class ViewAttacherTester
    {
        private ViewAttacher _viewAttacher;
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
            var types = new TypePool(null);
            _observer = new RecordingConfigurationObserver();
            _action = ActionCall.For<ViewsForActionFilterTesterController>(x => x.AAction());
            _fromFindsOne = new FakeViewToken();
            _fromSecondFindsOne = new FakeViewToken();
            _views = new ViewBag(new IViewToken[] { _fromFindsOne, _fromSecondFindsOne });
            _filterThatFindsNone = createFilterThatReturns(new IViewToken[0]);
            _firstFilterThatFindsExactlyOne = createFilterThatReturns(_fromFindsOne);
            _secondFilterThatFindsExactlyOne = createFilterThatReturns(_fromSecondFindsOne);
            _filterThatFindsMultiple = createFilterThatReturns(_fromFindsOne, _fromSecondFindsOne);
            _viewAttacher = new ViewAttacher(types);
        }

        [Test]
        public void does_not_attach_a_view_if_no_filters_find_a_match()
        {
            _viewAttacher.AddViewsForActionFilter(_filterThatFindsNone);
            
            _viewAttacher.AttemptToAttachViewToAction(_views, _action, _observer);
            _action.OfType<FakeViewToken>().ShouldHaveCount(0);
        }

        [Test]
        public void should_use_first_filter_that_returns_exactly_one_view()
        {
            _viewAttacher.AddViewsForActionFilter(_filterThatFindsNone);
            _viewAttacher.AddViewsForActionFilter(_firstFilterThatFindsExactlyOne);
            _viewAttacher.AddViewsForActionFilter(_secondFilterThatFindsExactlyOne);

            _viewAttacher.AttemptToAttachViewToAction(_views, _action, _observer);
            var attachedView = _action.OfType<FakeViewToken>().ShouldHaveCount(1).FirstOrDefault();
            attachedView.ShouldBeTheSameAs(_fromFindsOne);
        }

        [Test]
        public void should_not_attach_a_view_from_a_filter_that_returns_more_than_one_token()
        {
            _viewAttacher.AddViewsForActionFilter(_filterThatFindsMultiple);

            _viewAttacher.AttemptToAttachViewToAction(_views, _action, _observer);
            _action.OfType<FakeViewToken>().ShouldHaveCount(0);
        }

        [Test]
        public void should_log_when_a_view_is_attached()
        {
            _fromFindsOne.Name = "TestToken";

            _viewAttacher.AddViewsForActionFilter(_firstFilterThatFindsExactlyOne);
            _viewAttacher.AttemptToAttachViewToAction(_views, _action, _observer);

            _observer.LastLogEntry.ShouldContain(_fromFindsOne.Name);
        }
        
        private static IViewsForActionFilter createFilterThatReturns(params IViewToken[] viewTokens)
        {
            var filter = MockRepository.GenerateMock<IViewsForActionFilter>();
            filter.Stub(x => x.Apply(null, null)).IgnoreArguments().Return(viewTokens);
            return filter;
        }
    }
}