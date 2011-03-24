using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Diagnostics.Grids.Columns
{
	public class ViewColumn : BehaviorChainColumnBase
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

		public override bool IsIdentifier()
		{
			return false;
		}

		public override bool IsHidden(BehaviorChain chain)
		{
			return true;
		}

		public override bool HideFilter(BehaviorChain chain)
		{
			return false;
		}
	}
}