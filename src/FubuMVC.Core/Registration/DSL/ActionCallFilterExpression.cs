using System;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Util;

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
            return CallMatches(call => call.OutputType().CanBeCastTo<T>());
        }

        public ActionCallFilterExpression CallMatches(Func<ActionCall, bool> func)
        {
            _filter.Includes += func;

            return this;
        }
    }
}