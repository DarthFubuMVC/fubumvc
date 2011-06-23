using System.Collections.Generic;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

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
            graph.Behaviors.Each(b =>
                {
                    ActionCall call = b.FirstCall();
                    if(call == null)
                    {
                        return;
                    }

                    call.ForAttributes<AntiForgeryTokenAttribute>(a => call.AddBefore(new AntiForgeryNode(a.Salt)));
                });
        }
    }
}