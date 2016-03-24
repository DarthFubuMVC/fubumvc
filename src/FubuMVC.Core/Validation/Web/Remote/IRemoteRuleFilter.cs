using System;
using FubuCore.Reflection;
using FubuMVC.Core.Validation.Fields;

namespace FubuMVC.Core.Validation.Web.Remote
{
    public interface IRemoteRuleFilter
    {
        bool Matches(IFieldValidationRule rule);
    }

    public class RemoteRuleAttributeFilter : IRemoteRuleFilter
    {
        public bool Matches(IFieldValidationRule rule)
        {
            return rule.GetType().HasAttribute<RemoteAttribute>();
        }
    }

    public class RemoteFieldValidationRuleFilter : IRemoteRuleFilter
    {
        public bool Matches(IFieldValidationRule rule)
        {
            return rule is IRemoteFieldValidationRule;
        }
    }

    public class LambdaRemoteRuleFilter : IRemoteRuleFilter
    {
        private readonly Func<IFieldValidationRule, bool> _filter;

        public LambdaRemoteRuleFilter(Func<IFieldValidationRule, bool> filter)
        {
            _filter = filter;
        }

        public bool Matches(IFieldValidationRule rule)
        {
            return _filter(rule);
        }
    }
}