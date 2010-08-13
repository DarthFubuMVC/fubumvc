using System;
using System.Collections.Generic;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuMVC.UI.Configuration;
using System.Linq;

namespace FubuMVC.UI.Security
{
    public interface IFieldAccessRule
    {
        AccessRight RightsFor(ElementRequest request);
        bool Matches(Accessor accessor);
    }


    public class FieldAccessRights
    {
        private readonly IEnumerable<IFieldAccessRule> _securityRules;
        private readonly IEnumerable<IFieldAccessRule> _logicRules;

        public FieldAccessRights(IEnumerable<IFieldAccessRule> securityRules, IEnumerable<IFieldAccessRule> logicRules)
        {
            _securityRules = securityRules;
            _logicRules = logicRules;
        }

        public AccessRight RightsFor(ElementRequest request)
        {
            var securityRights = getSecurityRights(request);
            var logicRights = getLogicRights(request);

            return AccessRight.Least(logicRights, securityRights);
        }

        private AccessRight getLogicRights(ElementRequest request)
        {
            if (!_logicRules.Any()) return AccessRight.All;

            return _logicRules.Select(x => x.RightsFor(request)).Least();
        }

        private AccessRight getSecurityRights(ElementRequest request)
        {
            if (!_securityRules.Any()) return AccessRight.All;

            return _securityRules.Select(x => x.RightsFor(request)).Most();
        }
    }

    public static class AccessRightExtensions
    {
        public static AccessRight Most(this IEnumerable<AccessRight> rights)
        {
            return AccessRight.Most(rights.ToArray());
        }

        public static AccessRight Least(this IEnumerable<AccessRight> rights)
        {
            return AccessRight.Least(rights.ToArray());
        }
    }

    public class FieldAccessRegistry
    {
        private readonly List<IFieldAccessRule> _securityRules = new List<IFieldAccessRule>();
        private readonly List<IFieldAccessRule> _logicRules = new List<IFieldAccessRule>();
        

    }
}