using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuMVC.UI.Configuration;
using HtmlTags;

namespace FubuMVC.UI.Security
{
    public interface IFieldAccessService
    {
        AccessRight RightsFor(ElementRequest request);
        AccessRight RightsFor(object target, PropertyInfo property);
    }

    public class FieldAccessService : IFieldAccessService
    {
        private readonly List<IFieldAccessRule> _rules = new List<IFieldAccessRule>();

        public FieldAccessService(IEnumerable<IFieldAccessRule> rules)
        {
            _rules.AddRange(rules);
        }

        public AccessRight RightsFor(ElementRequest request)
        {
            var matchingRules = _rules.Where(x => x.Matches(request.Accessor));
            var authorizationRules = matchingRules.Where(x => x.Category == FieldAccessCategory.Authorization);
            var logicRules = matchingRules.Where(x => x.Category == FieldAccessCategory.LogicCondition);
            return new FieldAccessRights(authorizationRules, logicRules).RightsFor(request);
        }

        public AccessRight RightsFor(object target, PropertyInfo property)
        {
            var request = ElementRequest.For(target, property);
            return RightsFor(request);
        }
    }

    public class DegradingAccessElementBuilder : ElementBuilder
    {
        protected override bool matches(AccessorDef def)
        {
            return true;
        }

        public override HtmlTag Build(ElementRequest request)
        {
            var rights = request.AccessRights();
            if (rights.Write) return null;

            // The second span won't be rendered
            return rights.Read 
                ? request.Tags().DisplayFor(request) 
                : new HtmlTag("span").Authorized(false);
        }

        
    }
}