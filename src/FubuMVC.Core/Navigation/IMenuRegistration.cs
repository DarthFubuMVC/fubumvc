using FubuMVC.Core.Localization;

namespace FubuMVC.Core.Navigation
{
    public interface IMenuRegistration
    {
        bool DependsOn(StringToken token);
        string Description { get; }

        StringToken Key { get; }

        void Configure(NavigationGraph graph);
    }
}