using FubuMVC.Core.Registration.Nodes;

namespace Spark.Web.FubuMVC.Registration
{
	public interface ISparkDescriptorVisitor
	{
		bool AppliesTo(ActionCall call);
		void Visit(SparkViewDescriptor matchedDescriptor, ActionCall call);
	}
}