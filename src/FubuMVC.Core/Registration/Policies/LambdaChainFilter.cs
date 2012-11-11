using System;
using System.Linq.Expressions;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Policies
{
    public class LambdaChainFilter : IChainFilter, DescribesItself
    {
        private readonly Func<BehaviorChain, bool> _filter;
        private readonly string _description;

        public LambdaChainFilter(Expression<Func<BehaviorChain, bool>> expression) : this(expression.Compile(), expression.ToString())
        {
        }

        public LambdaChainFilter(Func<BehaviorChain, bool> filter, string description)
        {
            _filter = filter;
            _description = description;
        }

        public bool Matches(BehaviorChain chain)
        {
            return _filter(chain);
        }

        public void Describe(Description description)
        {
            description.ShortDescription = _description;
        }
    }
}