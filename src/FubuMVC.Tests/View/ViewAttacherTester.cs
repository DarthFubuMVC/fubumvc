using System.Linq;
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
        private IViewAttachmentStrategy _firstStrategyThatFindsExactlyOne;
        private ViewBag _views;
        private FakeViewToken _fromFindsOne;
        private IViewAttachmentStrategy _secondStrategyThatFindsExactlyOne;
        private FakeViewToken _fromSecondFindsOne;
        private IViewAttachmentStrategy _strategyThatFindsNone;
        private IViewAttachmentStrategy _strategyThatFindsMultiple;

        [SetUp]
        public void Setup()
        {
            var types = new TypePool();
            _action = ActionCall.For<ViewAttachmentStrategiesTesterController>(x => x.AAction());
            _fromFindsOne = new FakeViewToken();
            _fromSecondFindsOne = new FakeViewToken();
            _views = new ViewBag(new IViewToken[] { _fromFindsOne, _fromSecondFindsOne });
            _strategyThatFindsNone = createStrategyThatReturns(new IViewToken[0]);
            _firstStrategyThatFindsExactlyOne = createStrategyThatReturns(_fromFindsOne);
            _secondStrategyThatFindsExactlyOne = createStrategyThatReturns(_fromSecondFindsOne);
            _strategyThatFindsMultiple = createStrategyThatReturns(_fromFindsOne, _fromSecondFindsOne);
            _viewAttacher = new ViewAttacher(types);
        }

        [Test]
        public void does_not_attach_a_view_if_no_strategies_find_a_match()
        {
            _viewAttacher.AddAttachmentStrategy(_strategyThatFindsNone);
            
            _viewAttacher.AttemptToAttachViewToAction(_views, _action);
            _action.OfType<FakeViewToken>().ShouldHaveCount(0);
        }

        [Test]
        public void should_use_first_strategy_that_succeeds()
        {
            _viewAttacher.AddAttachmentStrategy(_strategyThatFindsNone);
            _viewAttacher.AddAttachmentStrategy(_firstStrategyThatFindsExactlyOne);
            _viewAttacher.AddAttachmentStrategy(_secondStrategyThatFindsExactlyOne);

            _viewAttacher.AttemptToAttachViewToAction(_views, _action);
            var attachedView = _action.OfType<FakeViewToken>().ShouldHaveCount(1).FirstOrDefault();
            attachedView.ShouldBeTheSameAs(_fromFindsOne);
        }

        [Test]
        public void should_not_attach_a_view_from_a_strategy_that_returns_more_than_one_token()
        {
            _viewAttacher.AddAttachmentStrategy(_strategyThatFindsMultiple);

            _viewAttacher.AttemptToAttachViewToAction(_views, _action);
            _action.OfType<FakeViewToken>().ShouldHaveCount(0);
        }

        
        private static IViewAttachmentStrategy createStrategyThatReturns(params IViewToken[] viewTokens)
        {
            var strategy = MockRepository.GenerateMock<IViewAttachmentStrategy>();
            strategy.Stub(x => x.Find(null, null)).IgnoreArguments().Return(viewTokens);
            return strategy;
        }
    }
}