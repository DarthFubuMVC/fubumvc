using FubuMVC.Core.Registration;

namespace FubuMVC.Core.Diagnostics
{
    public class AboutFubuDiagnostics
    {
        private readonly AppReloaded _reloaded;
        private readonly BehaviorGraph _graph;
        private readonly FubuRuntime _runtime;

        public AboutFubuDiagnostics(AppReloaded reloaded, BehaviorGraph graph, FubuRuntime runtime)
        {
            _reloaded = reloaded;
            _graph = graph;
            _runtime = runtime;
        }

        public string get_about()
        {
            return FubuApplicationDescriber.WriteDescription(_runtime.ActivationDiagnostics, _runtime);
        }

        public string get_loaded()
        {
            return _reloaded.Timestamp.ToString();
        }
    }
}