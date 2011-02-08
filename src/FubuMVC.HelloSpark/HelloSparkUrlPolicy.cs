using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using Spark.Web.FubuMVC.Extensions;

namespace FubuMVC.HelloSpark
{
	public class HelloSparkUrlPolicy : IUrlPolicy
	{
		public bool Matches(ActionCall call, IConfigurationObserver log)
		{
			return call.HandlerType.Name.EndsWith("Controller");
		}

		public IRouteDefinition Build(ActionCall call)
		{
			var route = call.ToRouteDefinition();
			route.Append(call.HandlerType.Name.RemoveSuffix("Controller"));
			route.Append(call.Method.Name);
			return route;
		}
	}
}