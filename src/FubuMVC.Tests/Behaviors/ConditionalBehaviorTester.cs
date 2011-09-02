using System;
using System.Runtime.Remoting.Contexts;
using FubuCore.Binding;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Behaviors.Node
{
    public abstract class when_condition_is_true : InteractionContext<when_condition_is_true.TrueBehavior>
    {
        protected IActionBehavior InnerBehavior;

        protected override void beforeEach()
        {
            InnerBehavior = MockFor<IActionBehavior>();
            ClassUnderTest.InsideBehavior = InnerBehavior;
        }

        public class TrueBehavior : ConditionalBehavior
        {
            public TrueBehavior() : base(() => true)
            {
            }
        }
    }

    public abstract class when_condition_is_false : InteractionContext<when_condition_is_false.FalseBehavior>
    {
        protected IActionBehavior InnerBehavior;

        protected override void beforeEach()
        {
            InnerBehavior = MockFor<IActionBehavior>();
            ClassUnderTest.InsideBehavior = InnerBehavior;
        }

        public class FalseBehavior : ConditionalBehavior
        {
            public FalseBehavior(): base(() => false)
            {
            }
        }
    }

    [TestFixture]
    public class invoking_conditional_true_behavior : when_condition_is_true
    {
        protected override void beforeEach()
        {
            base.beforeEach();
            ClassUnderTest.Invoke();
        }

        [Test]
        public void it_should_call_the_inner_behavior()
        {
            InnerBehavior.AssertWasCalled(x => x.Invoke());
        }

        [Test]
        public void it_should_not_call_the_partial_invoke()
        {
            InnerBehavior.AssertWasNotCalled(x => x.InvokePartial());
        }
    }

    [TestFixture]
    public class partially_invoking_true_behavior : when_condition_is_true
    {
        protected override void beforeEach()
        {
            base.beforeEach();
            ClassUnderTest.InvokePartial();
        }

        [Test]
        public void it_should_call_the_inner_behavior()
        {
            InnerBehavior.AssertWasCalled(x => x.Invoke());
            InnerBehavior.AssertWasCalled(x => x.InvokePartial());
        }
    }

    [TestFixture]
    public class partially_invoking_false_behavior : when_condition_is_false
    {
        protected override void beforeEach()
        {
            base.beforeEach();
            ClassUnderTest.InvokePartial();
        }

        [Test]
        public void it_should_call_the_inner_behavior()
        {
            InnerBehavior.AssertWasNotCalled(x => x.Invoke());
            InnerBehavior.AssertWasNotCalled(x => x.InvokePartial());
        }
    }

    [TestFixture]
    public class invoking_false_behavior : when_condition_is_false
    {
        protected override void beforeEach()
        {
            base.beforeEach();
            ClassUnderTest.Invoke();
        }

        [Test]
        public void it_should_call_the_inner_behavior()
        {
            InnerBehavior.AssertWasNotCalled(x => x.Invoke());
            InnerBehavior.AssertWasNotCalled(x => x.InvokePartial());
        }
    }
}
//====================
// Conditional of T
// ===================
namespace FubuMVC.Tests.Behaviors.OfT
{
    public abstract class when_condition_is_true : InteractionContext<when_condition_is_true.TrueBehavior>
    {
        protected IActionBehavior InnerBehavior;

        protected override void beforeEach()
        {
            InnerBehavior = MockFor<IActionBehavior>();
            ClassUnderTest.InsideBehavior = InnerBehavior;
        }

        public class TrueBehavior : ConditionalBehavior<FakeContext>
        {
            public TrueBehavior() : base( new FakeContext(true), x => x.Condition)
            {
            }
        }
    }

    public class FakeContext
    {
        public FakeContext(bool b)
        {
            Condition = b;
        }

        public bool Condition { get; set; }
    }

    public abstract class when_condition_is_false : InteractionContext<when_condition_is_false.FalseBehavior>
    {
        protected IActionBehavior InnerBehavior;

        protected override void beforeEach()
        {
            InnerBehavior = MockFor<IActionBehavior>();
            ClassUnderTest.InsideBehavior = InnerBehavior;
        }

        public class FalseBehavior : ConditionalBehavior<FakeContext>
        {
            public FalseBehavior()
                : base(new FakeContext(false), x => x.Condition)
            {
            }
        }
    }

    [TestFixture]
    public class invoking_conditional_true_behavior : when_condition_is_true
    {
        protected override void beforeEach()
        {
            base.beforeEach();
            ClassUnderTest.Invoke();
        }

        [Test]
        public void it_should_call_the_inner_behavior()
        {
            InnerBehavior.AssertWasCalled(x => x.Invoke());
        }

        [Test]
        public void it_should_not_call_the_partial_invoke()
        {
            InnerBehavior.AssertWasNotCalled(x => x.InvokePartial());
        }
    }

    [TestFixture]
    public class partially_invoking_true_behavior : when_condition_is_true
    {
        protected override void beforeEach()
        {
            base.beforeEach();
            ClassUnderTest.InvokePartial();
        }

        [Test]
        public void it_should_call_the_inner_behavior()
        {
            InnerBehavior.AssertWasCalled(x => x.Invoke());
            InnerBehavior.AssertWasCalled(x => x.InvokePartial());
        }
    }

    [TestFixture]
    public class partially_invoking_false_behavior : when_condition_is_false
    {
        protected override void beforeEach()
        {
            base.beforeEach();
            ClassUnderTest.InvokePartial();
        }

        [Test]
        public void it_should_call_the_inner_behavior()
        {
            InnerBehavior.AssertWasNotCalled(x => x.Invoke());
            InnerBehavior.AssertWasNotCalled(x => x.InvokePartial());
        }
    }

    [TestFixture]
    public class invoking_false_behavior : when_condition_is_false
    {
        protected override void beforeEach()
        {
            base.beforeEach();
            ClassUnderTest.Invoke();
        }

        [Test]
        public void it_should_call_the_inner_behavior()
        {
            InnerBehavior.AssertWasNotCalled(x => x.Invoke());
            InnerBehavior.AssertWasNotCalled(x => x.InvokePartial());
        }
    }
}

//====================
// Conditional of AjaxRequest
// ===================
namespace FubuMVC.Tests.Behaviors.AjaxRequest
{
    public abstract class when_condition_is_true : InteractionContext<when_condition_is_true.TrueBehavior>
    {
        protected IActionBehavior InnerBehavior;

        protected override void beforeEach()
        {
            InnerBehavior = MockFor<IActionBehavior>();
            Services.Inject<IRequestData>(new FakeData(true));
            ClassUnderTest.InsideBehavior = InnerBehavior;
        }

        public class TrueBehavior : IsAjaxRequest
        {
            public TrueBehavior(IRequestData context) : base(context)
            {
            }
        }

     
    }

    

    public abstract class when_condition_is_false : InteractionContext<when_condition_is_false.FalseBehavior>
    {
        protected IActionBehavior InnerBehavior;

        protected override void beforeEach()
        {
            InnerBehavior = MockFor<IActionBehavior>();
            Services.Inject<IRequestData>(new FakeData(false));
            ClassUnderTest.InsideBehavior = InnerBehavior;
        }

        public class FalseBehavior : IsAjaxRequest
        {
            public FalseBehavior(IRequestData context) : base(context)
            {
            }
        }
    }
    public class FakeData : IRequestData
    {
        private readonly bool _b;

        public FakeData(bool b)
        {
            _b = b;
        }

        public object Value(string key)
        {
            return "";
        }

        public bool Value(string key, Action<object> callback)
        {
            callback(_b ? AjaxExtensions.XmlHttpRequestValue: "");
            return _b;
        }

        public bool HasAnyValuePrefixedWith(string key)
        {
            return _b;
        }
    }
    [TestFixture]
    public class invoking_conditional_true_behavior : when_condition_is_true
    {
        protected override void beforeEach()
        {
            base.beforeEach();
            ClassUnderTest.Invoke();
        }

        [Test]
        public void it_should_call_the_inner_behavior()
        {
            InnerBehavior.AssertWasCalled(x => x.Invoke());
        }

        [Test]
        public void it_should_not_call_the_partial_invoke()
        {
            InnerBehavior.AssertWasNotCalled(x => x.InvokePartial());
        }
    }

    [TestFixture]
    public class partially_invoking_true_behavior : when_condition_is_true
    {
        protected override void beforeEach()
        {
            base.beforeEach();
            ClassUnderTest.InvokePartial();
        }

        [Test]
        public void it_should_call_the_inner_behavior()
        {
            InnerBehavior.AssertWasCalled(x => x.Invoke());
            InnerBehavior.AssertWasCalled(x => x.InvokePartial());
        }
    }

    [TestFixture]
    public class partially_invoking_false_behavior : when_condition_is_false
    {
        protected override void beforeEach()
        {
            base.beforeEach();
            ClassUnderTest.InvokePartial();
        }

        [Test]
        public void it_should_call_the_inner_behavior()
        {
            InnerBehavior.AssertWasNotCalled(x => x.Invoke());
            InnerBehavior.AssertWasNotCalled(x => x.InvokePartial());
        }
    }

    [TestFixture]
    public class invoking_false_behavior : when_condition_is_false
    {
        protected override void beforeEach()
        {
            base.beforeEach();
            ClassUnderTest.Invoke();
        }

        [Test]
        public void it_should_call_the_inner_behavior()
        {
            InnerBehavior.AssertWasNotCalled(x => x.Invoke());
            InnerBehavior.AssertWasNotCalled(x => x.InvokePartial());
        }
    }
}