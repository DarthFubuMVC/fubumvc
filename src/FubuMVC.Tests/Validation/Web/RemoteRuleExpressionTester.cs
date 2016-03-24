using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Validation.Fields;
using FubuMVC.Core.Validation.Web;
using FubuMVC.Core.Validation.Web.Remote;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Validation.Web
{
    [TestFixture]
    public class RemoteRuleExpressionTester
    {
        private RemoteRuleExpression theExpression;
        private IList<IRemoteRuleFilter> theFilters;

        [SetUp]
        public void SetUp()
        {
            theFilters = new List<IRemoteRuleFilter>();
            theExpression = new RemoteRuleExpression(theFilters);
        }

        private bool matches(IFieldValidationRule rule)
        {
            return theFilter.Matches(rule);
        }

        private IRemoteRuleFilter theFilter
        {
            get { return theFilters.Single(); }
        }

        [Test]
        public void finds_with_generic()
        {
            theExpression.FindWith<RemoteRuleAttributeFilter>();
            theFilter.ShouldBeOfType<RemoteRuleAttributeFilter>();
        }

        [Test]
        public void finds_with_custom()
        {
            theExpression.FindWith(new RemoteFieldValidationRuleFilter());
            theFilter.ShouldBeOfType<RemoteFieldValidationRuleFilter>();
        }

        [Test]
        public void include_specific_rule_type()
        {
            theExpression.Include<RequiredFieldRule>();
            matches(new RequiredFieldRule()).ShouldBeTrue();
            matches(new GreaterThanZeroRule()).ShouldBeFalse();
        }

        [Test]
        public void include_rule_lambda()
        {
            theExpression.IncludeIf(rule => rule is GreaterThanZeroRule);
            matches(new GreaterThanZeroRule()).ShouldBeTrue();
            matches(new RequiredFieldRule()).ShouldBeFalse();
        }
    }
}