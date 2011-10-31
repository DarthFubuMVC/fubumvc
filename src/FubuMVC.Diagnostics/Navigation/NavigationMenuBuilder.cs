using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Http;

namespace FubuMVC.Diagnostics.Navigation
{
    public class NavigationMenuBuilder : INavigationMenuBuilder
    {
        private readonly IEnumerable<INavigationItemAction> _actions;
        private readonly ICurrentHttpRequest _httpRequest;

        public NavigationMenuBuilder(IEnumerable<INavigationItemAction> actions, ICurrentHttpRequest httpRequest)
        {
            _actions = actions;
            _httpRequest = httpRequest;
        }

        public IEnumerable<NavigationMenuItem> MenuItems()
        {
            return _actions
                .OrderBy(a => a.Rank)
                .ThenBy(a => a.Text())
                .Select(a =>
                {
                    var url = _httpRequest.ToFullUrl(a.Url());
                    if (url.EndsWith("/"))
                    {
                        url = url.TrimEnd('/');
                    }

                    return new NavigationMenuItem{
                        Text = a.Text(),
                        Url = a.Url(),
                        IsActive = url.Equals(_httpRequest.RawUrl())
                    };
                });
        }
    }
}