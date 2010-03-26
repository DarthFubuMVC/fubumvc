using FubuCore.Util;
using FubuMVC.Core.Registration.DSL;
using FubuMVC.Core.Registration.Nodes;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Expressions
{
    [TestFixture]
    public class ActionCallFilterExpressionTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            action1 = ActionCall.For<ControllerTarget>(x => x.OneInZeroOut(null));
            action2 = ActionCall.For<ActionCallFilterController>(x => x.DifferentMethod());
            filter = new CompositeFilter<ActionCall>();
            expression = new ActionCallFilterExpression(filter);
        }

        #endregion

        private ActionCall action1;
        private CompositeFilter<ActionCall> filter;
        private ActionCallFilterExpression expression;
        private ActionCall action2;

        [Test]
        public void specify_a_filter_by_action_call()
        {
            expression.WhenCallMatches(call => call == action1);
            filter.Matches(action1).ShouldBeTrue();
            filter.Matches(action2).ShouldBeFalse();
        }

        [Test]
        public void specify_a_filter_by_output_type()
        {
            expression.WhenTheOutputModelIs<ActionCallFilterModel>();
            filter.Matches(action1).ShouldBeFalse();
            filter.Matches(action2).ShouldBeTrue();
        }
    }

    public class ActionCallFilterModel
    {
    }

    public class ActionCallFilterController : ControllerTarget
    {
        public ActionCallFilterModel DifferentMethod()
        {
            return null;
        }
    }
}