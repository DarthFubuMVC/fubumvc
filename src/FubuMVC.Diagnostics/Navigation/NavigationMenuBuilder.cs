using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Diagnostics.Core.Infrastructure;

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
            return _actions
                .OrderBy(a => a.Rank)
                .ThenBy(a => a.Text())
                .Select(a =>
                {
                    throw new NotImplementedException();

                    return new NavigationMenuItem();
                    //var url = a.Url().ToAbsoluteUrl();
                    //if (url.EndsWith("/"))
                    //{
                    //    url = url.TrimEnd('/');
                    //}

                    //return new NavigationMenuItem{
                    //    Text = a.Text(),
                    //    Url = a.Url(),
                    //    IsActive = url.Equals(_request.CurrentUrl())
                    //};
                });
        }
    }
}