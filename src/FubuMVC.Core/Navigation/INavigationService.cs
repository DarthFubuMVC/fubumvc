using System.Collections.Generic;
using FubuMVC.Core.Localization;

namespace FubuMVC.Core.Navigation
{
    public interface INavigationService
    {
        IEnumerable<MenuItemToken> MenuFor(StringToken key);
    }
}