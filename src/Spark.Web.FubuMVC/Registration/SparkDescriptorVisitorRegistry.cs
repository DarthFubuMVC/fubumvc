using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;

namespace Spark.Web.FubuMVC.Registration
{
	public class SparkDescriptorVisitorRegistry : ISparkDescriptorVisitorRegistry
	{
		private readonly List<ISparkDescriptorVisitor> _visitors;

		public SparkDescriptorVisitorRegistry(List<ISparkDescriptorVisitor> visitors)
		{
			_visitors = visitors;
		}

		public IEnumerable<ISparkDescriptorVisitor> VisitorsFor(ActionCall call)
		{
			return _visitors.Where(visitor => visitor.AppliesTo(call));
		}
	}
}