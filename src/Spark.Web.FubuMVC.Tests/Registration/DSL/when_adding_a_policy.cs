using System;
using System.Collections.Generic;
using FubuTestingSupport;
using NUnit.Framework;
using Spark.Web.FubuMVC.Registration;
using Spark.Web.FubuMVC.Registration.DSL;

namespace Spark.Web.FubuMVC.Tests.Registration.DSL
{
    [TestFixture]
    public class when_adding_a_spark_policy
    {
        private List<ISparkPolicy> _policies;
        private SparkPoliciesExpression _expression;

        [SetUp]
        public void SetUp()
        {
            _policies = new List<ISparkPolicy>();
            _expression = new SparkPoliciesExpression(_policies);
        }

        [Test]
        public void should_add_new_to_list()
        {
            addAndAssert<TestSparkPolicy>(expression => expression.Add<TestSparkPolicy>());
        }

        [Test]
        public void should_add_policy_to_list()
        {
            addAndAssert<TestSparkPolicy>(expression => expression.Add(new TestSparkPolicy()));
        }

        [Test]
        public void should_add_lambda_policy()
        {
            addAndAssert<LambdaSparkPolicy>(expression => expression.AttachViewsBy(call => true, call => "", call => ""));
        }

        private void addAndAssert<T>(Action<SparkPoliciesExpression> action)
            where T : ISparkPolicy
        {
            action(_expression);
            _policies
                .ShouldContain(policy => policy.GetType() == typeof(T));
        }
    }
}