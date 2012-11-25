using FubuMVC.Core.Caching;

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