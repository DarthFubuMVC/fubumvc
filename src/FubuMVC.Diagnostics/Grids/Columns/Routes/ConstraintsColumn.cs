using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Infrastructure;

namespace FubuMVC.Diagnostics.Grids.Columns.Routes
{
	public class ConstraintsColumn : GridColumnBase<BehaviorChain>
	{
		private readonly IHttpConstraintResolver _resolver;
		
		public ConstraintsColumn(IHttpConstraintResolver resolver)
			: base("Constraints")
		{
			_resolver = resolver;
		}

		public override string ValueFor(BehaviorChain chain)
		{
			return _resolver.Resolve(chain);
		}
	}
}