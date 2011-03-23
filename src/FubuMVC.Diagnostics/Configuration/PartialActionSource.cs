using System.Collections.Generic;
using System.Reflection;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Configuration.Partials;
using FubuMVC.Diagnostics.Navigation;

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
}