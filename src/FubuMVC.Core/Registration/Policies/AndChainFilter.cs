using System.Collections.Generic;
using System.Linq;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Policies
{
    public class AndChainFilter : IChainFilter, DescribesItself
    {
        private readonly IList<IChainFilter> _filters = new List<IChainFilter>();

        public AndChainFilter(params IChainFilter[] filters)
        {
            _filters.AddRange(filters);
        }

        public bool Matches(BehaviorChain chain)
        {
            return _filters.All(x => x.Matches(chain));
        }

        public void Add(IChainFilter filter)
        {
            _filters.Add(filter);
        }

        public void Describe(Description description)
        {
            description.Title = "All";
            description.AddList("Filters", _filters);
        }
    }
}