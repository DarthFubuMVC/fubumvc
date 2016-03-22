using FubuCore;
using FubuMVC.Core.Localization;

namespace FubuMVC.Core.Navigation
{
    public class AddAfter : IMenuPlacementStrategy
    {
        public string FormatDescription(string matcherDescription, StringToken nodeKey)
        {
            return "Insert '{0}' after '{1}'".ToFormat(nodeKey.ToLocalizationKey(), matcherDescription);
        }

        public void Apply(IMenuNode dependency, MenuNode node)
        {
            dependency.AddAfter(node);
        }
    }
}