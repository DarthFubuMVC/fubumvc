using FubuMVC.Core.Registration.Nodes;
using Spark.Web.FubuMVC.Extensions;
using Spark.Web.FubuMVC.Registration;

namespace FubuMVC.HelloSpark
{
	public class HelloSparkJavaScriptViewPolicy : ISparkPolicy
	{
		public bool Matches(ActionCall call)
		{
			return call.Method.Name.EndsWith("View");
		}

		public string BuildViewLocator(ActionCall call)
		{
			return call.HandlerType.Name.RemoveSuffix("Controller");
		}

		public string BuildViewName(ActionCall call)
		{
			return call.Method.Name.RemoveSuffix("View");
		}
	}
}