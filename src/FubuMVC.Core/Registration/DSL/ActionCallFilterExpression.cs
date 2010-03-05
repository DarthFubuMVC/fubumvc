using System;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.DSL
{
    public class ActionCallFilterExpression
    {
        private readonly CompositeFilter<ActionCall> _filter;

        public ActionCallFilterExpression(CompositeFilter<ActionCall> filter)
        {
            _filter = filter;
        }

        public ActionCallFilterExpression WhenTheOutputModelIs<T>()
        {
            return WhenCallMatches(call => call.OutputType().CanBeCastTo<T>());
        }

        public ActionCallFilterExpression WhenCallMatches(Expression<Func<ActionCall, bool>> func)
        {
            _filter.Includes += func;

            return this;
        }
    }
}