using System.Collections.Generic;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuMVC.UI.Configuration;
using System.Linq;

namespace FubuMVC.UI.Security
{
    public class FieldAccessRegistry
    {
        private readonly List<IFieldAccessRule> _securityRules = new List<IFieldAccessRule>();
        private readonly List<IFieldAccessRule> _logicRules = new List<IFieldAccessRule>();
        private readonly Cache<Accessor, FieldAccessRights> _rights;

        public FieldAccessRegistry()
        {
            _rights = new Cache<Accessor, FieldAccessRights>(accessor =>
            {
                var securityRules = _securityRules.Where(x => x.Matches(accessor));
                var logicRules = _logicRules.Where(x => x.Matches(accessor));
                return new FieldAccessRights(securityRules, logicRules);
            });
        }

        public void AddSecurityRule(IFieldAccessRule rule)
        {
            _securityRules.Add(rule);
        }

        public void AddLogicRule(IFieldAccessRule rule)
        {
            _logicRules.Add(rule);
        }

        public AccessRight RightsFor(ElementRequest request)
        {
            return _rights[request.Accessor].RightsFor(request);
        }
    }
}