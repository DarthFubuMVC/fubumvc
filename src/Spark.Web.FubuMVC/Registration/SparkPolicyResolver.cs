using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;

namespace Spark.Web.FubuMVC.Registration
{
    public class SparkPolicyResolver : ISparkPolicyResolver
    {
        private readonly List<ISparkPolicy> _policies;

        public SparkPolicyResolver(List<ISparkPolicy> policies)
        {
            _policies = policies;
        }

        public bool HasMatchFor(ActionCall call)
        {
            return getPolicy(call) != null;
        }

        public string ResolveViewName(ActionCall call)
        {
            return forPolicyMatching(call, policy => policy.BuildViewName(call));
        }

        public string ResolveViewLocator(ActionCall call)
        {
            return forPolicyMatching(call, policy => policy.BuildViewLocator(call));
        }

        private ISparkPolicy getPolicy(ActionCall call)
        {
            return _policies.FirstOrDefault(p => p.Matches(call));
        }

        private string forPolicyMatching(ActionCall call, Func<ISparkPolicy, string> action)
        {
            var policy = getPolicy(call);
            if(policy != null)
            {
                return action(policy);
            }

            return null;
        }
    }
}