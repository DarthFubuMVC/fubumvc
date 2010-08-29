using System.Collections.Generic;
using System.Linq;
using FubuCore.Reflection;

namespace FubuMVC.Core.Registration.Conventions
{
    public class UrlForNewConvention : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Behaviors.Where(x => x.FirstCall() != null).Each(chain =>
            {
                chain.FirstCall().Method.ForAttribute<UrlForNewAttribute>(att =>
                {
                    chain.UrlCategory.Creates.Add(att.Type);
                });
            });
        }
    }
}