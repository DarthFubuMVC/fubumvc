using System;
using System.Collections.Generic;
using FubuMVC.Core.Validation.Fields;
using FubuMVC.Core.Validation.Web.Remote;

namespace FubuMVC.Core.Validation.Web
{
    public class RemoteRuleExpression
    {
        private readonly IList<IRemoteRuleFilter> _filters = new List<IRemoteRuleFilter>();

        public RemoteRuleExpression(IList<IRemoteRuleFilter> filters)
        {
            _filters = filters;
        }

        public RemoteRuleExpression FindWith<T>() where T : IRemoteRuleFilter, new()
        {
            return FindWith(new T());
        }

        public RemoteRuleExpression Include<T>() where T : IFieldValidationRule
        {
            return IncludeIfType(type => type == typeof (T));
        }

        public RemoteRuleExpression IncludeIf(Func<IFieldValidationRule, bool> predicate)
        {
            return FindWith(new LambdaRemoteRuleFilter(predicate));
        }

        public RemoteRuleExpression IncludeIfType(Func<Type, bool> predicate)
        {
            return FindWith(new LambdaRemoteRuleFilter(rule => predicate(rule.GetType())));
        }

        public RemoteRuleExpression FindWith(IRemoteRuleFilter filter)
        {
            _filters.Fill(filter);
            return this;
        }
    }
}