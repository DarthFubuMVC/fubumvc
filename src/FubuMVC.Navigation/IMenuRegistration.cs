using FubuLocalization;

namespace FubuMVC.Navigation
{
    public interface IMenuRegistration
    {
        bool DependsOn(StringToken token);
        string Description { get; }

        StringToken Key { get; }

        void Configure(NavigationGraph graph);
    }
}