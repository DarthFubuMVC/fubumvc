using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.Http.Compression
{
    [Description("Applies gzip and deflate compression on all chains without the [DoNotCompress] attribute")]
    public class ApplyCompression : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Chains
                .Where(chain => chain.Calls.Any(x => !x.HasAttribute<DoNotCompressAttribute>()))
                .Each(chain => chain.ApplyCompression());
        }
    }
}