using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Diagnostics.Core.Grids.Columns.Routes
{
	public class ViewColumn : GridColumnBase<BehaviorChain>
	{
		public const string None = "None";
	    public const string NotApplicable = "N/A";

		public ViewColumn()
			: base("View")
		{
		}

		public override string ValueFor(BehaviorChain chain)
		{
		    var lastCall = chain.LastCall();
            if(lastCall == null || lastCall.OutputType() == null || lastCall.OutputType() == typeof(FubuContinuation))
            {
                return NotApplicable;
            }

            var outputs = chain.Outputs.Select(output => output.Description);
            if (!outputs.Any())
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