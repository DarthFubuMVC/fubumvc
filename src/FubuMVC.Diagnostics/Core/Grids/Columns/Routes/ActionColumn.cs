using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Diagnostics.Core.Grids.Columns.Routes
{
    [MarkedForTermination]
	public class ActionColumn : GridColumnBase<BehaviorChain>
	{
		public ActionColumn()
			: base("Action")
		{
		}

		public override int Rank()
		{
			return 3;
		}

		public override string ValueFor(BehaviorChain chain)
		{
            var descriptions = chain.Calls.Select(x => x.Description).ToArray();
            return descriptions.Length == 0 ? " -" : descriptions.Join(", ");
		}
	}
}