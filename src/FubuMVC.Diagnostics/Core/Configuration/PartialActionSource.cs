using System.Collections.Generic;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Navigation;
using FubuMVC.Diagnostics.Partials;

namespace FubuMVC.Diagnostics.Core.Configuration
{
    public class PartialActionSource : IActionSource
    {
        public IEnumerable<ActionCall> FindActions(TypePool types)
        {
        	yield return PartialActionFor<NavigationMenu>();
        }

		public ActionCall PartialActionFor<T>()
		{
			var actionType = typeof(PartialAction<>).MakeGenericType(typeof(T));
			return new ActionCall(actionType, actionType.GetExecuteMethod());
		}
    }
}