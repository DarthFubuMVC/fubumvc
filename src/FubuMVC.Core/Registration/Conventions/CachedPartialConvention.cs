using FubuMVC.Core.Caching;
using FubuMVC.Core.Registration.Nodes;
using System.Linq;
using System.Collections.Generic;

namespace FubuMVC.Core.Registration.Conventions
{
    public class CachedPartialConvention : Policy
    {
        public CachedPartialConvention()
        {
            Where.AnyActionMatches(call => call.Method.Name.EndsWith("CachedPartial"));
            Add.NodeToEnd<OutputCachingNode>();
        }
    }
}