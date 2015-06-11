using FubuMVC.Core.Security;

namespace FubuMVC.Tests.Docs.Topics.Authorization
{
    // SAMPLE: allowrole-attribute
    [AllowRole("CanI")]
    public class Controller
    {
        [AllowRole("MayI", "AllowMe")]
        public void RestrictedAction(RestrictedInputModel input)
        {
            //logic
        }
    }
    // ENDSAMPLE

    public class RestrictedInputModel
    {

    }
}