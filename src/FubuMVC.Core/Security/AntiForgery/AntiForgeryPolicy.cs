using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Security.AntiForgery
{
    [Description("Adds anti-forgery protection to to matching chains")]
    public class AntiForgeryPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            var antiForgerySettings = graph.Settings.Get<AntiForgerySettings>();
            graph.Chains.OfType<RoutedChain>().Where(antiForgerySettings.Matches)
                .Each(x => x.Prepend(new AntiForgeryNode(x.InputType().FullName)));
        }
    }
}