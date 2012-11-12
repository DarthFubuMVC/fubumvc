using FubuLocalization;

namespace FubuMVC.Navigation
{
    public interface IMenuPlacementStrategy
    {
        string FormatDescription(string matcherDescription, StringToken nodeKey);
        void Apply(IMenuNode dependency, MenuNode node);
    }
}