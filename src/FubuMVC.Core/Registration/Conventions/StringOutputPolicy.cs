using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Resources.Conneg.New;

namespace FubuMVC.Core.Registration.Conventions
{
    public class StringOutputPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Behaviors
                .Where(x => x.ResourceType() == typeof (string))
                .Each(x =>
                {
                    var shouldBeHtml = x.LastCall().Method.Name.EndsWith("Html", StringComparison.InvariantCultureIgnoreCase);

                    if (shouldBeHtml)
                    {
                        x.Output.AddHtml();
                    }
                    else
                    {
                        x.Output.Writers.AddToEnd(new WriteString());
                    }
                });

        }
    }
}