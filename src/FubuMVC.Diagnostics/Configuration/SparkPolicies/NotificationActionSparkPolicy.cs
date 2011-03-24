using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Notifications;
using Spark.Web.FubuMVC.Extensions;
using Spark.Web.FubuMVC.Registration;

namespace FubuMVC.Diagnostics.Configuration.SparkPolicies
{
	public class NotificationActionSparkPolicy : ISparkPolicy
	{
		public bool Matches(ActionCall call)
		{
			return call.HandlerType.Closes(typeof(NotificationAction<>));
		}

		public string BuildViewLocator(ActionCall call)
		{
			return "Notifications";
		}

		public string BuildViewName(ActionCall call)
		{
			// e.g. NoOutputsNotification => NoOutputs.spark
			return call.InputType().Name.RemoveSuffix("Notification").RemoveSuffix("Model");
		}
	}
}