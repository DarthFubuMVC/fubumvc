using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Models.Routes;

namespace FubuMVC.Diagnostics.Models
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