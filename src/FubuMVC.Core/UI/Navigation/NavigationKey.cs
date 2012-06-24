using FubuLocalization;

namespace FubuMVC.Core.UI.Navigation
{
    public class NavigationKey : StringToken
    {
        public NavigationKey(string text) : base(text, text, namespaceByType:true)
        {
        }
    }
}