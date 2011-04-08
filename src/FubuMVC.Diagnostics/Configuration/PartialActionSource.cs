using System.Collections.Generic;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Diagnostics.Tracing;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.UI.Diagnostics;
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
            // TODO -- this should be from the container or use headless views
        	yield return PartialActionFor<NavigationMenu>();
			yield return PartialActionFor<AuthorizationReport>();
			yield return PartialActionFor<BehaviorStart>();
			yield return PartialActionFor<ExceptionReport>();
			yield return PartialActionFor<FieldAccessReport>();
			yield return PartialActionFor<FileOutputReport>();
			yield return PartialActionFor<ModelBindingReport>();
			yield return PartialActionFor<OutputReport>();
			yield return PartialActionFor<RedirectReport>();
			yield return PartialActionFor<SetValueReport>();
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
			// TODO -- This should come from the container or use headless views
			var actionType = typeof(NotificationAction<>).MakeGenericType(typeof(NoOutputsNotification));
			yield return new ActionCall(actionType, actionType.GetExecuteMethod());
		}
	}
}