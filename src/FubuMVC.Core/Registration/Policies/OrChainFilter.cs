using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Policies
{
    public class OrChainFilter : IChainFilter
    {
        private readonly IList<IChainFilter> _filters = new List<IChainFilter>();

        public bool Matches(BehaviorChain chain)
        {
            return _filters.Any(x => x.Matches(chain));
        }

        public void Add(IChainFilter filter)
        {
            _filters.Add(filter);
        }
    }
}