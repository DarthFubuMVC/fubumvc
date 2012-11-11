using System;
using System.Linq;
using System.Linq.Expressions;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Policies
{
    public class AnyActionMatch : IChainFilter, DescribesItself
    {
        private readonly Func<ActionCall, bool> _filter;
        private readonly string _description;

        public AnyActionMatch(Expression<Func<ActionCall, bool>> filter)
            : this(filter.Compile(), filter.ToString())
        {

        }

        public AnyActionMatch(Func<ActionCall, bool> filter, string description)
        {
            _filter = filter;
            _description = description;
        }

        public bool Matches(BehaviorChain chain)
        {
            return chain.Calls.Any(_filter);
        }

        public void Describe(Description description)
        {
            description.ShortDescription = _description;
        }
    }
}