using FubuLocalization;
using FubuCore;


namespace FubuMVC.Navigation
{
    public class AddChild : IMenuPlacementStrategy
    {
        public string FormatDescription(string parent, StringToken key)
        {
            return "Add '{0}' to menu '{1}'".ToFormat(key.ToLocalizationKey(), parent);
        }

        public void Apply(IMenuNode dependency, MenuNode node)
        {
            dependency.AddChild(node);
        }
    }
}