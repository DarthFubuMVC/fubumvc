using FubuCore;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.Registration.Conventions
{
    [MarkedForTermination("this just shouldn't be necessary")]
    public class RegisterAllSettings : ServiceRegistry
    {
        public RegisterAllSettings(BehaviorGraph graph)
        {
            graph.Settings.Register(graph.Services);
        }
    }


}