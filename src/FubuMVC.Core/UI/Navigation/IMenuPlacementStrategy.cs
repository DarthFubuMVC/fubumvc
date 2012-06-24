using FubuLocalization;

namespace FubuMVC.Core.UI.Navigation
{
    public interface IMenuPlacementStrategy
    {
        string FormatDescription(string matcherDescription, StringToken nodeKey);
        void Apply(IMenuNode dependency, MenuNode node);
    }
}