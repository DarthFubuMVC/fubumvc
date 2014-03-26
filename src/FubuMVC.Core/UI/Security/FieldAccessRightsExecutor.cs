using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.UI.Elements;

namespace FubuMVC.Core.UI.Security
{
    public interface IFieldAccessRightsExecutor
    {
        AccessRight RightsFor(ElementRequest request, IEnumerable<IFieldAccessRule> securityRules, IEnumerable<IFieldAccessRule> logicRules);
    }

    public class FieldAccessRightsExecutor : IFieldAccessRightsExecutor
    {
        public virtual AccessRight RightsFor(ElementRequest request, IEnumerable<IFieldAccessRule> securityRules, IEnumerable<IFieldAccessRule> logicRules)
        {
            var securityRights = getSecurityRights(request, securityRules);
            var logicRights = getLogicRights(request, logicRules);

            return AccessRight.Least(logicRights, securityRights);
        }

        protected AccessRight getLogicRights(ElementRequest request, IEnumerable<IFieldAccessRule> logicRules)
        {
            if (!logicRules.Any()) return AccessRight.All;

            return logicRules.Select(x => x.RightsFor(request)).Least();
        }

        protected AccessRight getSecurityRights(ElementRequest request, IEnumerable<IFieldAccessRule> securityRules)
        {
            if (!securityRules.Any()) return AccessRight.All;

            return securityRules.Select(x => x.RightsFor(request)).Most();
        }
    }
}