using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Resources.Conneg;

namespace FubuMVC.Core.Registration.Conventions
{
    public class DictionaryOutputConvention : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph
                .Behaviors
                .Where(x => x.ResourceType().CanBeCastTo<IDictionary<string, object>>())
                .Each(x => x.MakeAsymmetricJson());
        }
    }
}