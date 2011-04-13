using FubuMVC.Core;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;

namespace FubuMVC.HelloFubuSpark.Controllers.Security
{
    public class SecretController
    {
        [AllowRole("user", "manager")]
        public JsonMessage ShowProfile(ShowProfileRequest input)
        {
            return null;
        }

        [AllowRole("user"), AuthorizedBy(typeof(SpecialCaseAccessPolicy))]
        public JsonMessage ChangePassword(ChangePasswordRequest input)
        {
            return null;
        }
    }

    public class ChangePasswordRequest {}

    public class ShowProfileRequest {}

    public class SpecialCaseAccessPolicy : IAuthorizationPolicy
    {
        public AuthorizationRight RightsFor(IFubuRequest request)
        {
            return AuthorizationRight.Allow;
        }
    }
}