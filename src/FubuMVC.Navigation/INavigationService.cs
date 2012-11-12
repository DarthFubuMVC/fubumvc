using System.Collections.Generic;
using FubuLocalization;

namespace FubuMVC.Navigation
{
    public interface INavigationService
    {
        IEnumerable<MenuItemToken> MenuFor(StringToken key);
    }
}