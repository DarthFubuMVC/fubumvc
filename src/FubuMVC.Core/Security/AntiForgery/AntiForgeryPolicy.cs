using FubuMVC.Core.Registration;

namespace FubuMVC.Core.Security.AntiForgery
{
    public class AntiForgeryPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Services.SetServiceIfNone<IAntiForgeryValidator, AntiForgeryValidator>();
            graph.Services.SetServiceIfNone<IAntiForgeryEncoder, MachineKeyAntiForgeryEncoder>();
            graph.Services.SetServiceIfNone<IAntiForgerySerializer, BinaryAntiForgerySerializer>();
            graph.Services.SetServiceIfNone<IAntiForgeryTokenProvider, AntiForgeryTokenProvider>();
            graph.Services.SetServiceIfNone<IAntiForgeryService, AntiForgeryService>();
        }
    }
}