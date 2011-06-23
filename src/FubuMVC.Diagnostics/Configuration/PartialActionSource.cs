using System.Collections.Generic;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Models.Requests;
using FubuMVC.Diagnostics.Navigation;
using FubuMVC.Diagnostics.Notifications;
using FubuMVC.Diagnostics.Partials;

namespace FubuMVC.Diagnostics.Configuration
{
    public class PartialActionSource : IActionSource
    {
        public IEnumerable<ActionCall> FindActions(TypePool types)
        {
            // TODO -- This should come from the container somehow
        	yield return PartialActionFor<NavigationMenu>();
            yield return PartialActionFor<BehaviorDetailsModel>();
        }

		public ActionCall PartialActionFor<T>()
		{
			var actionType = typeof(PartialAction<>).MakeGenericType(typeof(T));
			return new ActionCall(actionType, actionType.GetExecuteMethod());
		}
    }

	public class NotificationActionSource : IActionSource
	{
		public IEnumerable<ActionCall> FindActions(TypePool types)
		{
			// TODO -- This should come from the container somehow
			var actionType = typeof(NotificationAction<>).MakeGenericType(typeof(NoOutputsNotification));
			yield return new ActionCall(actionType, actionType.GetExecuteMethod());
		}
	}
}