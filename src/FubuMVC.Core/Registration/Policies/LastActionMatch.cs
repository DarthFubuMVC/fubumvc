using System;
using System.Linq.Expressions;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Nodes;
using System.Linq;

namespace FubuMVC.Core.Registration.Policies
{
    public class LastActionMatch : IChainFilter, DescribesItself
    {
        private readonly Func<ActionCall, bool> _filter;
        private readonly string _description;

        public LastActionMatch(Expression<Func<ActionCall, bool>> filter) : this(filter.Compile(), filter.ToString())
        {
            
        }

        public LastActionMatch(Func<ActionCall, bool> filter, string description)
        {
            _filter = filter;
            _description = description;
        }

        public bool Matches(BehaviorChain chain)
        {
            var call = chain.Calls.LastOrDefault();
            if (call == null) return false;

            return _filter(call);
        }

        public void Describe(Description description)
        {
            description.ShortDescription = _description;
        }
    }
}