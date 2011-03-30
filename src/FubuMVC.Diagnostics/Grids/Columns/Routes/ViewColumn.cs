using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Diagnostics.Grids.Columns.Routes
{
	public class ViewColumn : GridColumnBase<BehaviorChain>
	{
		public const string None = "None";

		public ViewColumn()
			: base("View")
		{
		}

		public override string ValueFor(BehaviorChain chain)
		{
			var outputs = chain.Outputs.Select(output => output.Description);
			if(!outputs.Any())
			{
				return None;
			}

			return outputs.Join(", ");
		}

		public override bool IsHidden(BehaviorChain chain)
		{
			return true;
		}
	}
}