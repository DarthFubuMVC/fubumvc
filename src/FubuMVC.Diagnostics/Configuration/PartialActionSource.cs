using System.Collections.Generic;
using System.Reflection;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Navigation;
using FubuMVC.Diagnostics.Notifications;
using FubuMVC.Diagnostics.Partials;

namespace FubuMVC.Diagnostics.Configuration
{
    public class PartialActionSource : IActionSource
    {
        public IEnumerable<ActionCall> FindActions(TypePool types)
        {
            // TODO -- this should be from the container
            var actionType = typeof (PartialAction<>).MakeGenericType(typeof (NavigationMenu));
            yield return new ActionCall(actionType, actionType.GetMethod("Execute", BindingFlags.Public | BindingFlags.Instance));
        }
    }

	public class NotificationActionSource : IActionSource
	{
		public IEnumerable<ActionCall> FindActions(TypePool types)
		{
			// TODO -- This should ceom from the container
			var actionType = typeof(NotificationAction<>).MakeGenericType(typeof(NoOutputsNotification));
			yield return new ActionCall(actionType, actionType.GetExecuteMethod());
		}
	}
}