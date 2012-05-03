using System.Collections.Generic;
using FubuLocalization;

namespace FubuMVC.Core.UI.Navigation
{
    public interface INavigationService
    {
        IEnumerable<MenuItemToken> MenuFor(StringToken key);
    }
}