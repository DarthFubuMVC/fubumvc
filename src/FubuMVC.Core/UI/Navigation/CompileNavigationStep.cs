using FubuMVC.Core.Registration;

namespace FubuMVC.Core.UI.Navigation
{
    // Depending on integration tests for this one.
    public class CompileNavigationStep : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Navigation.Compile();
        }
    }
}