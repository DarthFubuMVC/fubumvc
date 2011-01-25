using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;

namespace Spark.Web.FubuMVC.Registration
{
	public interface ISparkDescriptorVisitorRegistry
	{
		IEnumerable<ISparkDescriptorVisitor> VisitorsFor(ActionCall call);
	}
}