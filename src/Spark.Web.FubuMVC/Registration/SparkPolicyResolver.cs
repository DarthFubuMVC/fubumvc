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

        public string ResolveViewName(ActionCall call)
        {
            return getPolicy(call).BuildViewName(call);
        }

        public string ResolveViewLocator(ActionCall call)
        {
            return getPolicy(call).BuildViewLocator(call);
        }

        private ISparkPolicy getPolicy(ActionCall call)
        {
            var policy = _policies.FirstOrDefault(p => p.Matches(call));
            if (policy == null)
            {
                throw new SparkFubuException(1001, "No policies found for {0}", call.ToString());
            }

            return policy;
        }
    }
}