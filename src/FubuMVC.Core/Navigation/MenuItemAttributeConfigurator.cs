using System.Collections.Generic;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Navigation
{
    public class MenuItemAttributeConfigurator : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            var navigationGraph = graph.Settings.Get<NavigationGraph>();
            graph.Actions().Each(
                action => action.ForAttributes<MenuItemAttribute>(att => Configure(action, att, navigationGraph)));
        }

        public void Configure(ActionCall action, MenuItemAttribute att, NavigationGraph graph)
        {
            var registrations = att.ToMenuRegistrations(action.ParentChain());
            graph.AddRegistrations(registrations);
        }
    }
}