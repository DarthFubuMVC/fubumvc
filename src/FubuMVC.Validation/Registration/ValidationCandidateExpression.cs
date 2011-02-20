using System;
using System.Linq.Expressions;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Validation.Registration
{
    public class ValidationCandidateExpression
    {
        private readonly ValidationCallMatcher _matcher;

        public ValidationCandidateExpression(ValidationCallMatcher matcher)
        {
            _matcher = matcher;
        }

        public ValidationCandidateExpression IncludeActionCalls(Expression<Func<ActionCall, bool>> filter)
        {
            _matcher.CallFilters.Includes += filter;
            return this;
        }

        public ValidationCandidateExpression ExcludeActionCalls(Expression<Func<ActionCall, bool>> filter)
        {
            _matcher.CallFilters.Excludes += filter;
            return this;
        }
    }
}