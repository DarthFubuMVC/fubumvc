using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.UI;
using FubuMVC.Core.View;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Behaviors
{
	[TestFixture]
    public class BasicBehaviorTester
    {
        public static IPartialInvokingHandler PartialInvokingHandler;
        private IFubuPage _page;
        private IPartialFactory _partialFactory;
        private BasicBehavior _behavior;
        private IFubuRequest _request;

        public class PartialHandlingBehavior : IActionBehavior
        {
            public void Invoke()
            {
                
            }

            public void InvokePartial()
            {
                PartialInvokingHandler.Invoke();
            }
        }
        public interface IPartialInvokingHandler{void Invoke();}
        public class FakeController
        {
            public void SomeAction() { }
        }
        public class WrappingBehavior : BasicBehavior
        {
            public WrappingBehavior()
                : base(PartialBehavior.Ignored)
            {
            }

            public DoNext PerformInvoke()
            {
                return base.performInvoke();
            }
        }
        public class InputModel { }

        [SetUp]
        public void SetUp()
        {
            _page = MockRepository.GenerateStub<IFubuPage>();
            _partialFactory = MockRepository.GenerateStub<IPartialFactory>();
            _behavior = new WrappingBehavior();
            PartialInvokingHandler = MockRepository.GenerateStub<IPartialInvokingHandler>();
            _behavior.InsideBehavior = new PartialHandlingBehavior();

            _partialFactory.Stub(f => f.BuildPartial(typeof(InputModel))).Return(_behavior);
            _page.Stub(p => p.Get<IPartialFactory>()).Return(_partialFactory);

            _request = MockRepository.GenerateStub<IFubuRequest>();
            _page.Stub(p => p.Get<IFubuRequest>()).Return(_request);
        }

        [Test, Ignore("TEMPORARILY ignored until the UI to FubuMVC.Core merge")]
        public void should_invoke_partial_inside_behavior_by_default_even_when_partial_behavior_does_not_execute()
        {
            _page.Partial<InputModel>();
            PartialInvokingHandler.AssertWasCalled(h=>h.Invoke());
        }

        [Test]
        public void perform_invoke_should_return_continue()
        {
            new WrappingBehavior().PerformInvoke().ShouldEqual(DoNext.Continue);
        }
    }
}