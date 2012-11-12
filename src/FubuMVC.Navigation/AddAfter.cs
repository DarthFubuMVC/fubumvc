using FubuLocalization;
using FubuCore;

namespace FubuMVC.Navigation
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