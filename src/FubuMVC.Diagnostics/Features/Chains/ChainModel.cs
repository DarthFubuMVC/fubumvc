using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Core.Infrastructure;

namespace FubuMVC.Diagnostics.Features.Chains
{
	public class ChainModel
	{
	    public ChainModel()
	    {
	        Behaviors = new List<BehaviorModel>();
	    }

        public string Constraints { get; set; }
		public BehaviorChain Chain { get; set; }
        public IEnumerable<BehaviorModel> Behaviors { get; set; }

        public bool HasConstraints()
        {
            return !HttpConstraintResolver
                    .NoConstraints
                    .Equals(Constraints);
        }
	}
}