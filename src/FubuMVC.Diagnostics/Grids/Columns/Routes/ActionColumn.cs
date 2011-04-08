using System.Linq;
using FubuMVC.Core.Registration.Nodes;
using System.Collections.Generic;

namespace FubuMVC.Diagnostics.Grids.Columns.Routes
{
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