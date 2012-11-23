using System.Collections.Generic;
using System.Linq;
using FubuCore.Descriptions;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Policies;

namespace FubuMVC.Core.Registration.Conventions
{
    [ConfigurationType(ConfigurationType.Attachment)]
    public class VaryByModification<T> : IChainModification, DescribesItself where T : IVaryBy
    {
        public void Modify(BehaviorChain chain)
        {
            chain.OfType<OutputCachingNode>().Each(x => x.Apply<T>());
        }

        public void Describe(Description description)
        {
            description.Title = "Caching varies by " + typeof (T).Name;
        }
    }
}