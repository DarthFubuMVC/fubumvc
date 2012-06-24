namespace FubuMVC.Core.Registration.Conventions
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