using System.Linq;
using FubuCore.Descriptions;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Policies;
using FubuMVC.Core.Security;
using FubuMVC.Core.Security.Authorization;

namespace FubuMVC.Authentication
{
    [Title("Any action with the [NotAuthenticated] attribute")]
    public class NotAuthenticatedFilter : IChainFilter
    {
        public bool Matches(BehaviorChain chain)
        {
            return chain.Calls.Any(ActionIsExempted);
        }

        public static bool ActionIsExempted(ActionCall call)
        {
            if (call.HasAttribute<NotAuthenticatedAttribute>()) return true;

            if (call.InputType() != null && call.InputType().HasAttribute<NotAuthenticatedAttribute>())
            {
                return true;
            }

            return false;
        }
    }
}