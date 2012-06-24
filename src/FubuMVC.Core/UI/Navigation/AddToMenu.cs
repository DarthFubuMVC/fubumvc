using FubuCore;
using FubuLocalization;

namespace FubuMVC.Core.UI.Navigation
{
    public class AddToMenu : IMenuPlacementStrategy
    {
        public string FormatDescription(string parent, StringToken key)
        {
            return "Add '{0}' to menu '{1}'".ToFormat(key.ToLocalizationKey(), parent);
        }

        public void Apply(IMenuNode dependency, MenuNode node)
        {
            dependency.AddAfter(node);
        }
    }
}