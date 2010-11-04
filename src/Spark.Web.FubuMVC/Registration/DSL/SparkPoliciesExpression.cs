using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;

namespace Spark.Web.FubuMVC.Registration.DSL
{
    public class SparkPoliciesExpression
    {
        private readonly List<ISparkPolicy> _policies;

        public SparkPoliciesExpression(List<ISparkPolicy> policies)
        {
            _policies = policies;
        }

        public SparkPoliciesExpression Add<TPolicy>()
            where TPolicy : ISparkPolicy, new()
        {
            return Add(new TPolicy());
        }

        public SparkPoliciesExpression Add(ISparkPolicy policy)
        {
            _policies.Add(policy);
            return this;
        }

        public SparkPoliciesExpression AttachViewsBy(Func<ActionCall, bool> filter, Func<ActionCall, string> viewLocator, Func<ActionCall, string> viewName)
        {
            _policies.Add(new LambdaSparkPolicy(filter, viewLocator, viewName));
            return this;
        }
    }
}