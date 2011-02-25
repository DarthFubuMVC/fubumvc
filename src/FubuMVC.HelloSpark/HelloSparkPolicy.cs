using FubuMVC.Core.Registration.Nodes;
using FubuMVC.HelloSpark.Controllers;
using Spark.Web.FubuMVC.Extensions;
using Spark.Web.FubuMVC.Registration;

namespace FubuMVC.HelloSpark
{
	public class HelloSparkPolicy : ISparkPolicy
	{
		public bool Matches(ActionCall call)
		{
			return call.HandlerType.Name.EndsWith("Controller") && (!call.HasOutput || !call.OutputType().Equals(typeof(JsonResponse)));
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