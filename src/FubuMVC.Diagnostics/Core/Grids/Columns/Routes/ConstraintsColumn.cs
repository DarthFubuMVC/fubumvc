using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Core.Infrastructure;

namespace FubuMVC.Diagnostics.Core.Grids.Columns.Routes
{
	public class ConstraintsColumn : GridColumnBase<BehaviorChain>
	{
		private readonly IHttpConstraintResolver _resolver;
		
		public ConstraintsColumn(IHttpConstraintResolver resolver)
			: base("Constraints")
		{
			_resolver = resolver;
		}

		public override int Rank()
		{
			return 4;
		}

		public override string ValueFor(BehaviorChain chain)
		{
			return _resolver.Resolve(chain);
		}
	}
}