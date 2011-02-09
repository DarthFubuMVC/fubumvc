using FubuMVC.Core.Registration.Nodes;
using Spark.Web.FubuMVC.Extensions;
using Spark.Web.FubuMVC.Registration;

namespace Spark.Web.FubuMVC.Policies
{
	public class ControllerSparkPolicy : ISparkPolicy
	{
		public bool Matches(ActionCall call)
		{
			return call.HandlerType.Name.EndsWith("Controller");
		}

		public string BuildViewLocator(ActionCall call)
		{
			return call.HandlerType.Name.RemoveSuffix("Controller");
		}

		public string BuildViewName(ActionCall call)
		{
			return call.Method.Name;
		}
	}
}