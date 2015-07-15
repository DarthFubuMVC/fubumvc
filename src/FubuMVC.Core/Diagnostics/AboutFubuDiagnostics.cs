using FubuMVC.Core.Registration;

namespace FubuMVC.Core.Diagnostics
{
    public class AboutFubuDiagnostics
    {
        private readonly AppReloaded _reloaded;
        private readonly BehaviorGraph _graph;

        public AboutFubuDiagnostics(AppReloaded reloaded, BehaviorGraph graph)
        {
            _reloaded = reloaded;
            _graph = graph;
        }

        public string get_about()
        {
            return FubuApplicationDescriber.WriteDescription(_graph.Diagnostics);
        }

        public string get_loaded()
        {
            return _reloaded.Timestamp.ToString();
        }
    }
}