using System.Collections.Generic;
using System.Linq;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Policies
{
    public class OrChainFilter : IChainFilter, DescribesItself
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

        public void Describe(Description description)
        {
            description.Title = "Matches any of these filters";
            description.AddList("Filters", _filters);
        }
    }
}