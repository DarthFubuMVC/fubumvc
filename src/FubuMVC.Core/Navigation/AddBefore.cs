using FubuCore;
using FubuMVC.Core.Localization;

namespace FubuMVC.Core.Navigation
{
    public class AddBefore : IMenuPlacementStrategy
    {
        public string FormatDescription(string matcherDescription, StringToken nodeKey)
        {
            return "Insert '{0}' before '{1}'".ToFormat(nodeKey.ToLocalizationKey(), matcherDescription);
        }

        public void Apply(IMenuNode dependency, MenuNode node)
        {
            dependency.AddBefore(node);
        }
    }
}