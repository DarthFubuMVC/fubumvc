using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Diagnostics.Infrastructure;

namespace FubuMVC.Diagnostics.Navigation
{
    public class NavigationMenuBuilder : INavigationMenuBuilder
    {
        private readonly IEnumerable<INavigationItemAction> _actions;
        private readonly IHttpRequest _request;

        public NavigationMenuBuilder(IEnumerable<INavigationItemAction> actions, IHttpRequest request)
        {
            _actions = actions;
            _request = request;
        }

        public IEnumerable<NavigationMenuItem> MenuItems()
        {
            return _actions.Select(a =>
                                       {
                                           var url = a.Url().ToAbsoluteUrl();
                                           return new NavigationMenuItem
                                                          {
                                                              Text = a.Text(),
                                                              Url = url,
                                                              IsActive = url.Equals(_request.CurrentUrl())
                                                          };
                                       });
        }
    }
}