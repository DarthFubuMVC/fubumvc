using System.Collections.Generic;
using System.Linq;
using FubuMVC.UI.Configuration;

namespace FubuMVC.UI.Security
{
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
}