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

        /// <summary>
        /// Applies a filter to the preceding declaration to only be applied on output models assignable to T
        /// </summary>
        public ActionCallFilterExpression WhenTheOutputModelIs<T>()
        {
            return WhenCallMatches(call => call.OutputType().CanBeCastTo<T>());
        }

        /// <summary>
        /// Applies a generic filter to the preceding declaration
        /// </summary>
        public ActionCallFilterExpression WhenCallMatches(Expression<Func<ActionCall, bool>> func)
        {
            _filter.Includes += func;

            return this;
        }
    }
}